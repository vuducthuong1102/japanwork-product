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
    public class RpsSubIndustry
    {
        private readonly string _connectionString;

        public RpsSubIndustry(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSubIndustry()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentitySubIndustry> GetByPage(IdentitySubIndustry filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"SubIndustry_GetAll";
            List<IdentitySubIndustry> listData = null;

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
                        listData = ParsingListSubIndustryFromReader(reader);
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
       
        private List<IdentitySubIndustry> ParsingListSubIndustryFromReader(IDataReader reader)
        {
            List<IdentitySubIndustry> listData = listData = new List<IdentitySubIndustry>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSubIndustryData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentitySubIndustry ExtractSubIndustryData(IDataReader reader)
        {
            var record = new IdentitySubIndustry();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.industry_id = Utils.ConvertToInt32(reader["industry_id"]);
            record.sub_industry = reader["sub_industry"].ToString();            

            return record;
        }

        public static IdentitySubIndustryLang ExtractSubIndustryLangData(IDataReader reader)
        {
            var record = new IdentitySubIndustryLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.sub_industry_id = Utils.ConvertToInt32(reader["sub_industry_id"]);
            record.sub_industry = reader["sub_industry"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }


        public int Insert(IdentitySubIndustry identity)
        {
            //Common syntax           
            var sqlCmd = @"SubIndustry_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@industry_id", identity.industry_id},
                {"@sub_industry", identity.sub_industry},
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

        public bool Update(IdentitySubIndustry identity)
        {
            //Common syntax
            var sqlCmd = @"SubIndustry_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@industry_id", identity.industry_id},
                {"@sub_industry", identity.sub_industry},
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

        public IdentitySubIndustry GetById(int id)
        {
            IdentitySubIndustry info = null;
            var sqlCmd = @"SubIndustry_GetById";

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
                            info = ExtractSubIndustryData(reader);
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
            var sqlCmd = @"SubIndustry_Delete";

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

        public List<IdentitySubIndustry> GetList()
        {
            //Common syntax            
            var sqlCmd = @"SubIndustry_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentitySubIndustry> listData = new List<IdentitySubIndustry>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractSubIndustryData(reader);

                            listData.Add(record);
                        }                       
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
        #endregion
    }
}
