using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsEmailMessage
    {
        private readonly string _connectionString;

        public RpsEmailMessage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsEmailMessage()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- Common ----

        public List<IdentityEmailMessage> GetByPage(dynamic filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@AgencyId", filter.AgencyId},
                {"@StaffId", filter.StaffId},
                {"@SortField", filter.SortField},
                {"@SortType", filter.SortType},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize}
            };

            var sqlCmd = @"EmailMessage_GetByPage";

            List<IdentityEmailMessage> myList = new List<IdentityEmailMessage>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractEmailMessageItem(reader);

                            entity.TotalChilds = Utils.ConvertToInt32(reader["TotalChilds"]);

                            myList.Add(entity);
                        }            
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to EmailMessage_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<string> GetSynchronizedIds(int agencyId, int staffId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", agencyId},
                {"@StaffId", staffId}
            };

            var sqlCmd = @"EmailMessage_GetSynchronizedIds";

            List<string> myList = new List<string>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var messageId = reader["MessageId"].ToString();
                            myList.Add(messageId);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var messageId = reader["MessageId"].ToString();
                                myList.Add(messageId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityEmailMessage GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"EmailMessage_GetDetailById";

            IdentityEmailMessage info = null;
            List<IdentityEmailMessage> parts = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractEmailMessageItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            parts = new List<IdentityEmailMessage>();
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var part = ExtractEmailMessagePartItem(reader);
                                    info.MessageParts.Add(part);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityEmailMessage GetPartDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"EmailMessage_GetPartDetailById";

            IdentityEmailMessage info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractEmailMessagePartItem(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public List<IdentityEmailMessage> GetMessageParts(dynamic filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@ParentMessageIdHash", filter.ParentMessageIdHash},
                {"@AgencyId", filter.AgencyId},
                {"@StaffId", filter.StaffId},
                {"@SortField", filter.SortField},
                {"@SortType", filter.SortType},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize}
            };

            var sqlCmd = @"EmailMessage_GetParts";

            List<IdentityEmailMessage> myList = new List<IdentityEmailMessage>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractEmailMessageItem(reader);

                            myList.Add(entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to EmailMessage_GetParts. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        private List<IdentityEmailMessage> ParsingEmailMessageData(IDataReader reader)
        {
            List<IdentityEmailMessage> listData = new List<IdentityEmailMessage>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractEmailMessageItem(reader);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityEmailMessage ExtractEmailMessageItem(IDataReader reader)
        {
            var record = new IdentityEmailMessage();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);
            record.MessageId = reader["MessageId"].ToString();
            record.MessageIdHash = reader["MessageIdHash"].ToString();
            record.Sender = reader["Sender"].ToString();
            record.Receiver = reader["Receiver"].ToString();
            record.Cc = reader["Cc"].ToString();
            record.Bcc = reader["Bcc"].ToString();
            record.Subject = reader["Subject"].ToString();
            record.ShortMessage = reader["ShortMessage"].ToString();
            record.Message = reader["Message"].ToString();
            record.Attachments = reader["Attachments"].ToString();
            record.IsRead = Utils.ConvertToBoolean(reader["IsRead"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.FetchedDate = reader["FetchedDate"] == DBNull.Value ? null : (DateTime?)reader["FetchedDate"];

            if (reader.HasColumn("TotalCount"))
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return record;
        }

        private IdentityEmailMessage ExtractEmailMessagePartItem(IDataReader reader)
        {
            var record = ExtractEmailMessageItem(reader);

            //Seperate properties
            record.ParentMessageIdHash = reader["ParentMessageIdHash"].ToString();
            record.ParentMessageId = reader["ParentMessageId"].ToString();

            return record;
        }

        public int Insert(IdentityEmailMessage identity)
        {
            var sqlCmd = @"EmailMessage_Insert";
            var newId = 0;

            var metaData = string.Empty;

            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},
                {"@MessageId", identity.MessageId},
                {"@MessageIdHash", identity.MessageIdHash},
                {"@Sender", identity.Sender},
                {"@Receiver", identity.Receiver},
                {"@Cc", identity.Cc},
                {"@Bcc", identity.Bcc},
                {"@Subject", identity.Subject},
                {"@ShortMessage", identity.ShortMessage},
                {"@Message", identity.Message},
                {"@Attachments", identity.Attachments},
                //{"@IsRead", identity.IsRead},
                {"@CreatedDate", identity.CreatedDate}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        newId = Utils.ConvertToInt32(reader[0]);
                    }

                    //var insertPartCmd = @"EmailMessage_Insert_Part";
                    //if (identity.MessageParts != null)
                    //{
                    //    var totalParts = identity.MessageParts.Count;
                    //    if(totalParts > 1)
                    //    {
                    //        for (int i = 1; i <= totalParts; i++)
                    //        {
                    //            var part = identity.MessageParts[i];
                    //            var myPartParams = new Dictionary<string, object>
                    //            {
                    //                {"@EmailMessageId", newId},
                    //                {"@MessageId", part.MessageId},
                    //                {"@ParentMessageId", part.ParentMessageId},
                    //                {"@Subject", part.Subject},
                    //                {"@BodyContent", part.BodyContent},
                    //                {"@Attachment", part.Attachment},
                    //                {"@CreatedDate", part.CreatedDate}                                    
                    //            };

                    //            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, insertPartCmd, myPartParams);
                    //        }
                    //    }                        
                    //}
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int InsertPart(IdentityEmailMessage identity)
        {
            var sqlCmd = @"EmailMessage_Insert_Part";
            var newId = 0;

            var metaData = string.Empty;

            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},
                {"@MessageId", identity.MessageId},
                {"@MessageIdHash", identity.MessageIdHash},
                {"@ParentMessageId", identity.ParentMessageId},
                {"@ParentMessageIdHash", identity.ParentMessageIdHash},
                {"@Sender", identity.Sender},
                {"@Receiver", identity.Receiver},
                {"@Cc", identity.Cc},
                {"@Bcc", identity.Bcc},
                {"@Subject", identity.Subject},
                {"@ShortMessage", identity.ShortMessage},
                {"@Message", identity.Message},
                {"@Attachments", identity.Attachments},
                //{"@IsRead", identity.IsRead},
                {"@CreatedDate", identity.CreatedDate}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        newId = Utils.ConvertToInt32(reader[0]);
                    }                    
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityEmailMessage identity)
        {
            var sqlCmd = @"EmailMessage_Update";
            var metaData = string.Empty;
            
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},
                {"@MessageId", identity.MessageId},
                {"@Sender", identity.Sender},
                {"@Receiver", identity.Receiver},
                {"@Subject", identity.Subject},
                {"@Message", identity.Message},
                //{"@IsRead", identity.IsRead},
                {"@CreatedDate", identity.CreatedDate}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(int id)
        {
            var sqlCmd = @"EmailMessage_Delete";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
        }

        #endregion        
    }
}
