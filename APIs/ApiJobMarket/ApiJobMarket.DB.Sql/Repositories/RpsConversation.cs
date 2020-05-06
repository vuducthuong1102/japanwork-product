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
//    public class RpsConversation
//    {
//        private readonly string _connectionString;

//        public RpsConversation(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public RpsConversation()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
//        }

//        public int GetCurrentConversation(IdentityConversation identity)
//        {
//            var newId = 0;
//            var sqlCmd = @"Conversation_GetCurrentConversation";

//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserOne", identity.UserOne},
//                {"@UserTwo", identity.UserTwo},
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
//                var strError = "Failed to Conversation_GetCurrentConversation. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return newId;
//        }


//        public List<IdentityConversation> GetByPage(dynamic filter)
//        {
//            //Common syntax           
//            var sqlCmd = @"Conversation_GetByPage";
//            List<IdentityConversation> listData = null;

//            //For paging 
//            int offset = (filter.PageIndex - 1) * filter.PageSize;

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@OwnerId", filter.OwnerId },
//                {"@Offset", offset},
//                {"@PageSize", filter.PageSize},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        listData = new List<IdentityConversation>();

//                        while (reader.Read())
//                        {
//                            var item = ExtractConversationData(reader);
//                            listData.Add(item);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Conversation_GetByPage. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }

//        private List<IdentityConversation> ParsingListConversationFromReader(IDataReader reader)
//        {
//            List<IdentityConversation> listData = listData = new List<IdentityConversation>();
//            while (reader.Read())
//            {
//                //Get common information
//                var record = ExtractConversationData(reader);

//                //Extends information
//                if (listData == null)
//                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

//                listData.Add(record);
//            }

//            return listData;
//        }

//        private IdentityConversation ExtractConversationData(IDataReader reader)
//        {
//            var record = new IdentityConversation();

//            //Seperate properties
//            record.Id = Utils.ConvertToInt32(reader["Id"]);
//            record.OneDeleted = Utils.ConvertToBoolean(reader["OneDeleted"], false);
//            record.TwoDeleted = Utils.ConvertToBoolean(reader["TwoDeleted"], false);
//            record.OneRead = Utils.ConvertToBoolean(reader["OneRead"], false);
//            record.TwoRead = Utils.ConvertToBoolean(reader["TwoRead"], false);
//            record.UserOne = Utils.ConvertToInt32(reader["UserOne"]);
//            record.UserTwo = Utils.ConvertToInt32(reader["UserTwo"]);
//            record.OneViewed = Utils.ConvertToBoolean(reader["OneViewed"], false);
//            record.TwoViewed = Utils.ConvertToBoolean(reader["TwoViewed"], false);
//            record.Status = Utils.ConvertToInt32(reader["Status"]);
//            record.OneReadDate = reader["OneReadDate"] == DBNull.Value ? null : (DateTime?)reader["OneReadDate"];
//            record.TwoReadDate = reader["TwoReadDate"] == DBNull.Value ? null : (DateTime?)reader["TwoReadDate"];

//            record.ModifiedDate = reader["ModifiedDate"] == DBNull.Value ? null : (DateTime?)reader["ModifiedDate"];

//            return record;
//        }

//        public int Delete(IdentityConversation identity)
//        {
//            //Common syntax           
//            var sqlCmd = @"Conversation_Delete";
//            var result = 0;
//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Id", identity.Id },
//                {"@OwnerId", identity.OwnerId }
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
//                var strError = "Failed to execute Conversation_Delete. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return result;
//        }
//    }
//}
