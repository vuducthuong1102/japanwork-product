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
    public class RpsSuggest
    {
        private readonly string _connectionString;

        public RpsSuggest(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSuggest()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentitySuggest> GetByPage(IdentitySuggest filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Suggest_GetByPage";
            List<IdentitySuggest> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
                //{"@language_code", filter.language_code},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListSuggestFromReader(reader);
                        //if (reader.NextResult())
                        //{
                        //    var listLang = ParsingListSuggestLangFromReader(reader);
                        //    if (listData.HasData())
                        //    {
                        //        foreach (var item in listData)
                        //        {
                        //            item.LangList = listLang.Where(s => s.suggest_id == item.id).ToList();
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

            return listData;
        }

        private List<IdentitySuggest> ParsingListSuggestFromReader(IDataReader reader)
        {
            List<IdentitySuggest> listData = new List<IdentitySuggest>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSuggestData(reader);

                if (listData != null&&listData.Count==0)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        private List<IdentitySuggestLang> ParsingListSuggestLangFromReader(IDataReader reader)
        {
            List<IdentitySuggestLang> listData = listData = new List<IdentitySuggestLang>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSuggestLangData(reader);
                listData.Add(record);
            }

            return listData;
        }

        public static IdentitySuggest ExtractSuggestData(IDataReader reader)
        {
            var record = new IdentitySuggest();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.form = Utils.ConvertToInt32(reader["form"]);
            record.type = Utils.ConvertToInt32(reader["type"]);
            record.title = reader["title"].ToString();
            record.content = reader["content"].ToString();
            record.isDescription = Utils.ConvertToBoolean(reader["isdescription"]);

            return record;
        }

        public static IdentitySuggestLang ExtractSuggestLangData(IDataReader reader)
        {
            var record = new IdentitySuggestLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.suggest_id = Utils.ConvertToInt32(reader["suggest_id"]);
            record.title = reader["title"].ToString();
            record.content = reader["content"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentitySuggest identity)
        {
            //Common syntax           
            var sqlCmd = @"Suggest_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@title", identity.title},
                {"@content", identity.content},
                {"@form", identity.form},
                {"@type", identity.type}
                //{"@language_code", identity.language_code}
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

        public bool Update(IdentitySuggest identity)
        {
            //Common syntax
            var sqlCmd = @"Suggest_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@title", identity.title},
                {"@content", identity.content},
                {"@form", identity.form},
                {"@type", identity.type}
                //{"@language_code", identity.language_code}
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

        public IdentitySuggest GetById(int id)
        {
            IdentitySuggest info = null;
            var sqlCmd = @"Suggest_GetById";

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
                            info = ExtractSuggestData(reader);
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
            var sqlCmd = @"Suggest_Delete";

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

        public List<IdentitySuggest> GetList(IdentitySuggest filter)
        {
            //Common syntax            
            var sqlCmd = @"Suggest_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                 {"@form", filter.form},
                 {"@type", filter.type}
            };

            List<IdentitySuggest> listData = new List<IdentitySuggest>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractSuggestData(reader);

                            listData.Add(record);
                        }

                        //Lang
                        //if (reader.NextResult())
                        //{
                        //    listLang = new List<IdentitySuggestLang>();
                        //    while (reader.Read())
                        //    {
                        //        listLang.Add(ExtractSuggestLangData(reader));
                        //    }
                        //}

                        //if (listData.HasData())
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = listLang.Where(x => x.suggest_id == item.id).ToList();
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
        #endregion
    }
}
