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
    public class RpsTypeSuggest
    {
        private readonly string _connectionString;

        public RpsTypeSuggest(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsTypeSuggest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityTypeSuggest> GetAll()
        {
            //Common syntax           
            var sqlCmd = @"TypeSuggest_GetAll";
            List<IdentityTypeSuggest> listData = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListTypeSuggestFromReader(reader);

                        if (reader.NextResult())
                        {
                            var listLang = ParsingListTypeSuggestLangFromReader(reader);
                            if (listData != null && listData.Count > 0)
                            {
                                if (listLang != null && listLang.Count > 0)
                                {
                                    foreach (var item in listData)
                                    {
                                        var listLangItem = listLang.Where(s => s.type_suggest_id == item.id);
                                        if (listLangItem != null && listLangItem.Count() > 0)
                                        {
                                            item.ListLang = listLangItem.ToList();
                                        }
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

        private List<IdentityTypeSuggest> ParsingListTypeSuggestFromReader(IDataReader reader)
        {
            List<IdentityTypeSuggest> listData = listData = new List<IdentityTypeSuggest>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractTypeSuggestData(reader);


                listData.Add(record);
            }

            return listData;
        }

        private List<IdentityTypeSuggestLang> ParsingListTypeSuggestLangFromReader(IDataReader reader)
        {
            List<IdentityTypeSuggestLang> listData  = new List<IdentityTypeSuggestLang>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractTypeSuggestLangData(reader);


                listData.Add(record);
            }

            return listData;
        }

        public static IdentityTypeSuggest ExtractTypeSuggestData(IDataReader reader)
        {
            var record = new IdentityTypeSuggest();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.type = reader["type"].ToString();
            record.description = reader["description"].ToString();
            record.form_id = Utils.ConvertToInt32(reader["form_id"]);
            record.icon = reader["icon"].ToString();
            return record;
        }
        public static IdentityTypeSuggestLang ExtractTypeSuggestLangData(IDataReader reader)
        {
            var record = new IdentityTypeSuggestLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.type = reader["type"].ToString();
            record.description = reader["description"].ToString();
            record.language_code = reader["language_code"].ToString();
            record.type_suggest_id = Utils.ConvertToInt32(reader["type_suggest_id"]);
            return record;
        }
        public int Insert(IdentityTypeSuggest identity)
        {
            //Common syntax           
            var sqlCmd = @"TypeSuggest_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@type", identity.type},
                {"@form_id", identity.form_id },
                {"@language_code", identity.language_code },
                {"@description", identity.description },
                {"@icon", identity.icon }
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

        public bool Update(IdentityTypeSuggest identity)
        {
            //Common syntax
            var sqlCmd = @"TypeSuggest_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@type", identity.type},
                {"@form_id", identity.form_id },
                {"@language_code", identity.language_code },
                {"@description", identity.description },
                {"@icon", identity.icon }
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

        public IdentityTypeSuggest GetById(int id)
        {
            IdentityTypeSuggest info = null;
            var sqlCmd = @"TypeSuggest_GetById";

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
                            info = ExtractTypeSuggestData(reader);
                        }
                        if (reader.NextResult())
                        {
                            info.ListLang = ParsingListTypeSuggestLangFromReader(reader);
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
            var sqlCmd = @"TypeSuggest_Delete";

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
        #endregion
    }
}
