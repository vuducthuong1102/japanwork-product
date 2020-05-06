using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.SharedLib.Extensions;
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
    public class RpsIndustry
    {
        private readonly string _connectionString;

        public RpsIndustry(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsIndustry()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityIndustry> GetByPage(IdentityIndustry filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Industry_GetAll";
            List<IdentityIndustry> listData = null;

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
                        listData = ParsingListIndustryFromReader(reader);
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
       
        private List<IdentityIndustry> ParsingListIndustryFromReader(IDataReader reader)
        {
            List<IdentityIndustry> listData = listData = new List<IdentityIndustry>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractIndustryData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityIndustry ExtractIndustryData(IDataReader reader)
        {
            var record = new IdentityIndustry();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.industry = reader["industry"].ToString();            

            return record;
        }

        public int Insert(IdentityIndustry identity)
        {
            //Common syntax           
            var sqlCmd = @"Industry_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@industry", identity.industry}
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

        public bool Update(IdentityIndustry identity)
        {
            //Common syntax
            var sqlCmd = @"Industry_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@industry", identity.industry}
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

        public IdentityIndustry GetById(int id)
        {
            IdentityIndustry info = null;
            var sqlCmd = @"Industry_GetById";

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
                            info = ExtractIndustryData(reader);
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
            var sqlCmd = @"Industry_Delete";

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

        public static IdentityIndustryLang ExtractIndustryLangData(IDataReader reader)
        {
            var record = new IdentityIndustryLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.industry_id = Utils.ConvertToInt32(reader["industry_id"]);
            record.industry = reader["industry"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public List<IdentityIndustry> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Industry_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityIndustry> listData = new List<IdentityIndustry>();            
            List<IdentitySubIndustry> subFileds = new List<IdentitySubIndustry>();

            List<IdentityIndustryLang> listIndustryLang = null;
            List<IdentitySubIndustryLang> listSubIndustryLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractIndustryData(reader);

                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                subFileds.Add(RpsSubIndustry.ExtractSubIndustryData(reader));
                            }
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listIndustryLang = new List<IdentityIndustryLang>();
                            while (reader.Read())
                            {
                                listIndustryLang.Add(ExtractIndustryLangData(reader));
                            }
                        }

                        //Sub Lang
                        if (reader.NextResult())
                        {
                            listSubIndustryLang = new List<IdentitySubIndustryLang>();
                            while (reader.Read())
                            {
                                listSubIndustryLang.Add(RpsSubIndustry.ExtractSubIndustryLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.Sub_industries = subFileds.Where(x => x.industry_id == item.id).ToList();
                                item.LangList = listIndustryLang.Where(x => x.industry_id == item.id).ToList();

                                if (item.Sub_industries.HasData())
                                {
                                    foreach (var sub in item.Sub_industries)
                                    {
                                        sub.LangList = listSubIndustryLang.Where(x => x.sub_industry_id == sub.id).ToList();
                                    }
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

            return listData;
        }

        public List<IdentitySubIndustry> GetListSubByIndustry(int industry_id)
        {
            //Common syntax            
            var sqlCmd = @"Industry_GetListSubByIndustry";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@industry_id", industry_id},
            };
         
            List<IdentitySubIndustry> subIndustries = new List<IdentitySubIndustry>();

            List<IdentitySubIndustryLang> listSubIndustryLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            subIndustries.Add(RpsSubIndustry.ExtractSubIndustryData(reader));
                        }

                        //Sub Lang
                        if (reader.NextResult())
                        {
                            listSubIndustryLang = new List<IdentitySubIndustryLang>();
                            while (reader.Read())
                            {
                                listSubIndustryLang.Add(RpsSubIndustry.ExtractSubIndustryLangData(reader));
                            }
                        }

                        if (subIndustries.HasData())
                        {
                            foreach (var item in subIndustries)
                            {
                                item.LangList = listSubIndustryLang.Where(x => x.sub_industry_id == item.id).ToList();
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

            return subIndustries;
        }

        public List<IdentitySubIndustry> GetListSub()
        {
            //Common syntax            
            var sqlCmd = @"Industry_GetListSub";

            //For parameters

            List<IdentitySubIndustry> subIndustries = new List<IdentitySubIndustry>();

            List<IdentitySubIndustryLang> listSubIndustryLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            subIndustries.Add(RpsSubIndustry.ExtractSubIndustryData(reader));
                        }

                        //Sub Lang
                        if (reader.NextResult())
                        {
                            listSubIndustryLang = new List<IdentitySubIndustryLang>();
                            while (reader.Read())
                            {
                                listSubIndustryLang.Add(RpsSubIndustry.ExtractSubIndustryLangData(reader));
                            }
                        }

                        if (subIndustries.HasData())
                        {
                            foreach (var item in subIndustries)
                            {
                                item.LangList = listSubIndustryLang.Where(x => x.sub_industry_id == item.id).ToList();
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

            return subIndustries;
        }

        #endregion
    }
}
