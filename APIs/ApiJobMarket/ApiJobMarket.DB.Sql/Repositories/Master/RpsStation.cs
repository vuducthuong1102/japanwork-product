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
    public class RpsStation
    {
        private readonly string _connectionString;

        public RpsStation(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsStation()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityStation> GetByPage(IdentityStation filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Station_GetAll";
            List<IdentityStation> listData = null;

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
                        listData = ParsingListStationFromReader(reader);
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

        public List<IdentityStation> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Station_GetSuggestionsByPage";
            List<IdentityStation> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@place_id", filter.place_id },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListStationFromReader(reader);
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

        private List<IdentityStation> ParsingListStationFromReader(IDataReader reader)
        {
            List<IdentityStation> listData = listData = new List<IdentityStation>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractStationData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityStation ExtractStationData(IDataReader reader)
        {
            var record = new IdentityStation();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.station = reader["station"].ToString();
            record.furigana = reader["furigana"].ToString();
            record.address = reader["address"].ToString();
            record.postal_code = reader["postal_code"].ToString();
            record.region_id = Utils.ConvertToInt32(reader["region_id"]);
            record.prefecture_id = Utils.ConvertToInt32(reader["prefecture_id"]);
            record.city_id = Utils.ConvertToInt32(reader["city_id"]);

            return record;
        }

        public int Insert(IdentityStation identity)
        {
            //Common syntax           
            var sqlCmd = @"Station_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@station", identity.station},
                {"@prefecture_id", identity.prefecture_id },
                {"@furigana", identity.furigana }
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

        public bool Update(IdentityStation identity)
        {
            //Common syntax
            var sqlCmd = @"Station_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@station", identity.station},
                {"@prefecture_id", identity.prefecture_id },
                {"@furigana", identity.furigana }
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

        public IdentityStation GetById(int id)
        {
            IdentityStation info = null;
            var sqlCmd = @"Station_GetById";

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
                        while (reader.Read())
                        {
                            info = ExtractStationData(reader);
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
            var sqlCmd = @"Station_Delete";

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

        public List<IdentityStation> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Station_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentityStation> listData = new List<IdentityStation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractStationData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityStationLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityStationLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.StationId = Utils.ConvertToInt32(reader["StationId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.StationId == item.Id).ToList();
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

        public List<IdentityStation> GetListByPosition(int line_id)
        {
            //Common syntax            
            var sqlCmd = @"Station_GetListByPosition";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@line_id",line_id }
            };

            List<IdentityStation> listData = new List<IdentityStation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractStationData(reader);

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
        public List<IdentityStation> GetListByCityId(int city_id)
        {
            //Common syntax            
            var sqlCmd = @"Station_GetListByCityId";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@city_id",city_id }
            };

            List<IdentityStation> listData = new List<IdentityStation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractStationData(reader);

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

        public List<IdentityStation> GetListByPrefecture(int prefecture_id)
        {
            //Common syntax            
            var sqlCmd = @"Station_GetListByPrefecture";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@prefecture_id", prefecture_id},
            };

            List<IdentityStation> listData = new List<IdentityStation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractStationData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityStationLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityStationLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.StationId = Utils.ConvertToInt32(reader["StationId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.StationId == item.Id).ToList();
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
