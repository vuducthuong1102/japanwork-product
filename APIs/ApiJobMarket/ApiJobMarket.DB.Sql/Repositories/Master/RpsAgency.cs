using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsAgency
    {
        private readonly string _connectionString;

        public RpsAgency(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsAgency()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityAgency> GetByPage(IdentityAgency filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Agency_GetAll";
            List<IdentityAgency> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListAgencyFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return listData;
        }
       
        private List<IdentityAgency> ParsingListAgencyFromReader(IDataReader reader)
        {
            List<IdentityAgency> listData = listData = new List<IdentityAgency>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractAgencyData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityAgency ExtractAgencyData(IDataReader reader)
        {
            var record = new IdentityAgency();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.agency = reader["agency"].ToString();
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.company_name = reader["company_name"].ToString();
            record.logo_path = reader["logo_path"].ToString();
            record.email = reader["email"].ToString();
            record.phone = reader["phone"].ToString();
            record.website = reader["website"].ToString();
            record.address = reader["address"].ToString();
            record.constract_id = Utils.ConvertToInt32(reader["constract_id"]);         
            record.status = Utils.ConvertToInt32(reader["status"]);         

            return record;
        }

        public int Insert(IdentityAgency identity)
        {
            //Common syntax           
            var sqlCmd = @"Agency_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@constract_id", identity.constract_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int CreateProfile(IdentityAgency identity)
        {
            //Common syntax           
            var sqlCmd = @"Agency_CreateProfile";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@agency", identity.agency },
                {"@company_name", identity.company_name },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@website", identity.website },
                {"@address", identity.address }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool UpdateProfile(IdentityAgency identity)
        {
            //Common syntax           
            var sqlCmd = @"Agency_UpdateProfile";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@agency", identity.agency },
                {"@company_name", identity.company_name },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@website", identity.website },
                {"@address", identity.address },
                {"@logo_path", identity.logo_path },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool Update(IdentityAgency identity)
        {
            //Common syntax
            var sqlCmd = @"Agency_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@agency_id", identity.agency_id},
                {"@constract_id", identity.constract_id },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityAgency GetById(int id)
        {
            IdentityAgency info = null;
            var sqlCmd = @"Agency_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractAgencyData(reader);
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

        public IdentityAgency GetBaseInfo(int id)
        {
            IdentityAgency info = null;
            var sqlCmd = @"Agency_GetBaseInfo";

            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractAgencyData(reader);
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

        public bool Delete(int id)
        {
            //Common syntax            
            var sqlCmd = @"Agency_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityAgency> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Agency_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityAgency> listData = new List<IdentityAgency>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractAgencyData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityAgencyLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityAgencyLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.AgencyId == item.Id).ToList();
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

            return listData;
        }

        public bool UpdateLogo(IdentityAgency identity)
        {
            //Common syntax
            var sqlCmd = @"Agency_UpdateLogo";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@logo_path", identity.logo_path }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }

        #endregion
    }
}
