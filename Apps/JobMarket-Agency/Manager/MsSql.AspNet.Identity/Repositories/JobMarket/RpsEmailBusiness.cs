using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;
using MsSql.AspNet.Identity.Entities;
using Newtonsoft.Json;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsEmailBusiness
    {
        private readonly string _connectionString;

        public RpsEmailBusiness(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsEmailBusiness()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- Common ----

        public List<IdentityEmailBusiness> GetByPage(dynamic filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@AgencyId", filter.AgencyId},
                {"@StaffId", filter.StaffId},
                {"@JobSeekerId", filter.JobSeekerId},
                {"@CompanyId", filter.CompanyId},                
                {"@IsOnlineUser", filter.IsOnlineUser},                
                {"@Offset", offset},
                {"@PageSize", filter.PageSize}
            };

            var sqlCmd = @"EmailBusiness_GetByPage";

            List<IdentityEmailBusiness> myList = new List<IdentityEmailBusiness>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractEmailBusinessItem(reader);
                            if (reader.HasColumn("TotalCount"))
                                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                            myList.Add(entity);
                        }            
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to EmailBusiness_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityEmailBusiness GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"EmailBusiness_GetDetailById";

            IdentityEmailBusiness info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractEmailBusinessItem(reader);
                        }

                        //if (reader.NextResult())
                        //{
                        //    parts = new List<IdentityEmailBusiness>();
                        //    if (info != null)
                        //    {
                        //        while (reader.Read())
                        //        {
                        //            var part = ExtractEmailBusinessPartItem(reader);
                        //            info.MessageParts.Add(part);
                        //        }
                        //    }
                        //}
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

        public List<IdentityEmailBusiness> GetMessageParts(dynamic filter)
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

            var sqlCmd = @"EmailBusiness_GetParts";

            List<IdentityEmailBusiness> myList = new List<IdentityEmailBusiness>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractEmailBusinessItem(reader);

                            myList.Add(entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to EmailBusiness_GetParts. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        private List<IdentityEmailBusiness> ParsingEmailBusinessData(IDataReader reader)
        {
            List<IdentityEmailBusiness> listData = new List<IdentityEmailBusiness>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractEmailBusinessItem(reader);

                if (reader.HasColumn("TotalCount"))
                    entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityEmailBusiness ExtractEmailBusinessItem(IDataReader reader)
        {
            var record = new IdentityEmailBusiness();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);           
            record.StaffId = Utils.ConvertToInt32(reader["StaffId"]);           
            record.JobSeekerId = Utils.ConvertToInt32(reader["JobSeekerId"]);
            record.CompanyId = Utils.ConvertToInt32(reader["CompanyId"]);
            record.Sender = reader["Sender"].ToString();
            record.Receiver = reader["Receiver"].ToString();
            record.Cc = reader["Cc"].ToString();
            record.Bcc = reader["Bcc"].ToString();
            record.Subject = reader["Subject"].ToString();
            record.Message = reader["Message"].ToString();
            record.Attachments = reader["Attachments"].ToString();
            record.IsOnlineUser = Utils.ConvertToBoolean(reader["IsOnlineUser"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityEmailBusiness identity)
        {
            var sqlCmd = @"EmailBusiness_Insert";
            var newId = 0;

            var metaData = string.Empty;

            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},
                {"@JobSeekerId", identity.JobSeekerId},
                {"@CompanyId", identity.CompanyId},
                {"@Sender", identity.Sender},
                {"@Receiver", identity.Receiver},
                {"@Cc", identity.Cc},
                {"@Bcc", identity.Bcc},
                {"@Subject", identity.Subject},
                {"@Message", identity.Message},
                {"@Attachments", identity.Attachments},
                {"@IsOnlineUser", identity.IsOnlineUser},
                {"@Status", identity.Status}
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

        public int BatchInsert(IdentityEmailBusiness identity)
        {
            var sqlCmd = @"EmailBusiness_BatchInsert";
            var status = 0;

            var receivers = JsonConvert.SerializeObject(identity.Receivers);

            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},                
                {"@Sender", identity.Sender},               
                {"@Cc", identity.Cc},
                {"@Bcc", identity.Bcc},
                {"@Subject", identity.Subject},
                {"@Message", identity.Message},
                {"@JsonData", receivers},
                {"@IsOnlineUser", identity.IsOnlineUser},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        status = Utils.ConvertToInt32(reader[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status;
        }

        public int Delete(IdentityEmailBusiness identity)
        {
            var sqlCmd = @"EmailBusiness_Delete";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId}
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
