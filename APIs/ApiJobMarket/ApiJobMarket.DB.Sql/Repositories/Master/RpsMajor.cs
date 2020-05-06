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
    public class RpsMajor
    {
        private readonly string _connectionString;

        public RpsMajor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsMajor()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityMajor> GetByPage(IdentityMajor filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Major_GetAll";
            List<IdentityMajor> listData = null;

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
                        listData = ParsingListMajorFromReader(reader);
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
       
        private List<IdentityMajor> ParsingListMajorFromReader(IDataReader reader)
        {
            List<IdentityMajor> listData = listData = new List<IdentityMajor>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractMajorData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityMajor ExtractMajorData(IDataReader reader)
        {
            var record = new IdentityMajor();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.major = reader["major"].ToString();            

            return record;
        }

        public static IdentityMajorLang ExtractMajorLangData(IDataReader reader)
        {
            var record = new IdentityMajorLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);
            record.major = reader["major"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentityMajor identity)
        {
            //Common syntax           
            var sqlCmd = @"Major_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@major", identity.major}
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

        public bool Update(IdentityMajor identity)
        {
            //Common syntax
            var sqlCmd = @"Major_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@major", identity.major}
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

        public IdentityMajor GetById(int id)
        {
            IdentityMajor info = null;
            var sqlCmd = @"Major_GetById";

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
                            info = ExtractMajorData(reader);
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
            var sqlCmd = @"Major_Delete";

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

        public List<IdentityMajor> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Major_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityMajor> listData = new List<IdentityMajor>();
            List<IdentityMajorLang> listLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractMajorData(reader);

                            listData.Add(record);
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listLang = new List<IdentityMajorLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ExtractMajorLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listLang.Where(x => x.major_id == item.id).ToList();
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

        public List<IdentityMajor> GetListByQualification(int qualification_id)
        {
            //Common syntax            
            var sqlCmd = @"Major_GetListByQualification";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@qualification_id", qualification_id}
            };

            List<IdentityMajor> listData = new List<IdentityMajor>();
            List<IdentityMajorLang> listLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractMajorData(reader);

                            listData.Add(record);
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listLang = new List<IdentityMajorLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ExtractMajorLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listLang.Where(x => x.major_id == item.id).ToList();
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
        #endregion
    }
}
