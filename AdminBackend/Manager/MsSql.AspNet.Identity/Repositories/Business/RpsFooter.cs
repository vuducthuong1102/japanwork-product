//using Manager.SharedLibs;
//using MsSql.AspNet.Identity.Entities;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;

//namespace MsSql.AspNet.Identity.Repositories
//{
//    public class RpsFooter
//    {
//        private readonly string _connectionString;

//        public RpsFooter(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public RpsFooter()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
//        }

//        public List<IdentityFooter> GetAll()
//        {
//            //Common syntax           
//            var sqlCmd = @"Footer_GetAll";
//            List<IdentityFooter> listData = null;

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
//                    {
//                        listData = ParsingListFooterFromReader(reader);
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Footer_GetAll. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }

//        public IdentityFooter GetByLangCode(string langCode)
//        {
//            var info = new IdentityFooter();
//            var sqlCmd = @"Footer_GetByLangCode";

//            var parameters = new Dictionary<string, object>
//            {
//                {"@LangCode", langCode}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            info = ExtractFooterData(reader);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Footer_GetByLangCode. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            return info;
//        }

//        public bool Update(IdentityFooter identity)
//        {
//            //Common syntax
//            var sqlCmd = @"Footer_Update";

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@BodyContent", identity.BodyContent},
//                {"@LangCode", identity.LangCode}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Footer_Update. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return true;
//        }

//        private List<IdentityFooter> ParsingListFooterFromReader(IDataReader reader)
//        {
//            List<IdentityFooter> listData = listData = new List<IdentityFooter>();
//            while (reader.Read())
//            {
//                //Get common information
//                var record = ExtractFooterData(reader);

//                listData.Add(record);
//            }

//            return listData;
//        }

//        private IdentityFooter ExtractFooterData(IDataReader reader)
//        {
//            var record = new IdentityFooter();

//            //Seperate properties
//            record.BodyContent = reader["BodyContent"].ToString();
//            record.LangCode = reader["LangCode"].ToString();

//            return record;
//        }
//    }
//}
