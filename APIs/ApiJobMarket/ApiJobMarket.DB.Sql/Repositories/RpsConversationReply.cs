//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.ShareLibs;
//using ApiJobMarket.ShareLibs.Exceptions;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;

//namespace ApiJobMarket.DB.Sql.Repositories
//{
//    public class RpsConversationReply
//    {
//        private readonly string _connectionString;

//        public RpsConversationReply(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public RpsConversationReply()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
//        }

//        public int Insert(IdentityConversationReply identity)
//        {
//            var newId = 0;
//            var sqlCmd = @"Conversation_Reply_Insert";

//            var parameters = new Dictionary<string, object>
//            {
//                {"@ConversationId", identity.ConversationId},
//                {"@Type", identity.Type},
//                {"@Content", identity.Content},
//                {"@UserId", identity.UserId},
//                {"@Ip", identity.Ip},
//                {"@Status", identity.Status}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
//                    if (reader.Read())
//                    {
//                        newId = Utils.ConvertToInt32(reader[0]);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to ConversationReply_Insert. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return newId;
//        }

//        public List<IdentityConversationReply> GetByPage(dynamic filter)
//        {
//            //Common syntax           
//            var sqlCmd = @"Conversation_Reply_GetByPage";
//            List<IdentityConversationReply> listData = null;

//            //For paging 
//            int offset = (filter.PageIndex - 1) * filter.PageSize;

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@OwnerId", filter.OwnerId },
//                {"@UserTwo", filter.UserTwo },
//                {"@Offset", offset},
//                {"@PageSize", filter.PageSize},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        listData = new List<IdentityConversationReply>();

//                        while (reader.Read())
//                        {
//                            var item = ExtractConversationReplyData(reader);
//                            listData.Add(item);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Conversation_Reply_GetByPage. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }

//        public List<IdentityConversationReply> GetFirstByListId(List<int> listConversations)
//        {
//            var strList = "";
//            if (listConversations != null & listConversations.Count > 0)
//            {
//                strList = string.Join(",", listConversations);
//            }

//            {
//                //Common syntax           
//                var sqlCmd = @"Conversation_Reply_GetFirstByListConversations";
//                List<IdentityConversationReply> listData = null;
//                var parameters = new Dictionary<string, object>
//            {
//                {"@ListConversationId", strList }
//            };

//                try
//                {
//                    using (var conn = new SqlConnection(_connectionString))
//                    {
//                        using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                        {
//                            listData = new List<IdentityConversationReply>();

//                            while (reader.Read())
//                            {
//                                var item = ExtractConversationReplyData(reader);
//                                listData.Add(item);
//                            }
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    var strError = "Failed to execute Conversation_Reply_GetByPage. Error: " + ex.Message;
//                    throw new CustomSQLException(strError);
//                }

//                return listData;
//            }
//        }

//        private List<IdentityConversationReply> ParsingListConversationReplyFromReader(IDataReader reader)
//        {
//            List<IdentityConversationReply> listData = listData = new List<IdentityConversationReply>();
//            while (reader.Read())
//            {
//                //Get common information
//                var record = ExtractConversationReplyData(reader);

//                listData.Add(record);
//            }

//            return listData;
//        }

//        private IdentityConversationReply ExtractConversationReplyData(IDataReader reader)
//        {
//            var record = new IdentityConversationReply();

//            //Seperate properties
//            record.Id = Utils.ConvertToInt32(reader["Id"]);
//            record.UserId = Utils.ConvertToInt32(reader["UserId"]);
//            record.Type = Utils.ConvertToInt32(reader["Type"]);
//            record.Content = reader["Content"].ToString();
//            record.Status = Utils.ConvertToInt32(reader["Status"]);
//            record.ConversationId = Utils.ConvertToInt32(reader["ConversationId"]);
//            record.CreatedDate = (DateTime)reader["CreatedDate"];

//            return record;
//        }

//        public int Delete(IdentityConversationReply identity)
//        {
//            //Common syntax           
//            var sqlCmd = @"Conversation_Reply_Delete";
//            var result = 0;
//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Id", identity.Id },
//                {"@UserId", identity.UserId }
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            result = Utils.ConvertToInt32(reader[0]);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Conversation_Reply_Delete. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return result;
//        }
//    }
//}
