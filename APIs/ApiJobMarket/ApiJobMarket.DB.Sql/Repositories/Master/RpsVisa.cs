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
    public class RpsVisa
    {
        private readonly string _connectionString;

        public RpsVisa(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsVisa()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityVisa> GetByPage(IdentityVisa filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Visa_GetAll";
            List<IdentityVisa> listData = null;

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
                        listData = ParsingListVisaFromReader(reader);
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
       
        private List<IdentityVisa> ParsingListVisaFromReader(IDataReader reader)
        {
            List<IdentityVisa> listData = new List<IdentityVisa>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractVisaData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityVisa ExtractVisaData(IDataReader reader)
        {
            var record = new IdentityVisa();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.visa = reader["visa"].ToString();            

            return record;
        }

        public static IdentityVisaLang ExtractVisaLangData(IDataReader reader)
        {
            var record = new IdentityVisaLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.visa_id = Utils.ConvertToInt32(reader["visa_id"]);
            record.visa = reader["visa"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentityVisa identity)
        {
            //Common syntax           
            var sqlCmd = @"Visa_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@visa", identity.visa}
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

        public bool Update(IdentityVisa identity)
        {
            //Common syntax
            var sqlCmd = @"Visa_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@visa", identity.visa}
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

        public IdentityVisa GetById(int id)
        {
            IdentityVisa info = null;
            var sqlCmd = @"Visa_GetById";

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
                            info = ExtractVisaData(reader);
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
            var sqlCmd = @"Visa_Delete";

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

        public List<IdentityVisa> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Visa_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityVisa> listData = new List<IdentityVisa>();
            List<IdentityVisaLang> listLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractVisaData(reader);

                            listData.Add(record);
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listLang = new List<IdentityVisaLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ExtractVisaLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listLang.Where(x => x.visa_id == item.id).ToList();
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
