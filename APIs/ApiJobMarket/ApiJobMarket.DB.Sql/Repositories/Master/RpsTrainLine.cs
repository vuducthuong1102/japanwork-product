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
    public class RpsTrainLine
    {
        private readonly string _connectionString;

        public RpsTrainLine(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsTrainLine()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityTrainLine> GetByPage(IdentityTrainLine filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"TrainLine_GetAll";
            List<IdentityTrainLine> listData = null;

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
                        listData = ParsingListTrainLineFromReader(reader);
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

        public List<IdentityTrainLine> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"TrainLine_GetSuggestionsByPage";
            List<IdentityTrainLine> listData = null;

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
                        listData = ParsingListTrainLineFromReader(reader);
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

        private List<IdentityTrainLine> ParsingListTrainLineFromReader(IDataReader reader)
        {
            List<IdentityTrainLine> listData = listData = new List<IdentityTrainLine>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractTrainLineData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityTrainLine ExtractTrainLineData(IDataReader reader)
        {
            var record = new IdentityTrainLine();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.translation_id = Utils.ConvertToInt32(reader["translation_id"]);
            record.train_line = reader["train_line"].ToString();            
            record.furigana = reader["furigana"].ToString();

            return record;
        }

        public int Insert(IdentityTrainLine identity)
        {
            //Common syntax           
            var sqlCmd = @"TrainLine_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {

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

        public bool Update(IdentityTrainLine identity)
        {
            //Common syntax
            var sqlCmd = @"TrainLine_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@id", identity.id},
                //{"@city", identity.city},
                //{"@prefecture_id", identity.prefecture_id },
                //{"@furigana", identity.furigana }
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

        public IdentityTrainLine GetById(int id)
        {
            IdentityTrainLine info = null;
            var sqlCmd = @"TrainLine_GetById";

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
                            info = ExtractTrainLineData(reader);
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
            var sqlCmd = @"TrainLine_Delete";

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

        public List<IdentityTrainLine> GetList()
        {
            //Common syntax            
            var sqlCmd = @"TrainLine_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityTrainLine> listData = new List<IdentityTrainLine>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractTrainLineData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityTrainLineLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityTrainLineLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.TrainLineId = Utils.ConvertToInt32(reader["TrainLineId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.TrainLineId == item.Id).ToList();
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

        public List<IdentityTrainLine> GetListByPrefecture(int prefecture_id)
        {
            //Common syntax            
            var sqlCmd = @"TrainLine_GetListByPrefecture";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@prefecture_id", prefecture_id},
            };

            List<IdentityTrainLine> listData = new List<IdentityTrainLine>();
            List<IdentityStation> listStations = new List<IdentityStation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractTrainLineData(reader);

                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var station = RpsStation.ExtractStationData(reader);
                                station.train_line_id = Utils.ConvertToInt32(reader["train_line_id"]);

                                listStations.Add(station);
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.Stations = listStations.Where(x => x.train_line_id == item.id).ToList();
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
        public List<IdentityTrainLine> GetListByCityId(int city_id,string keyword)
        {
            //Common syntax            
            var sqlCmd = @"TrainLine_GetListByCityId";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@city_id", city_id},
                {"@keyword", keyword}
            };

            List<IdentityTrainLine> listData = new List<IdentityTrainLine>();
            List<IdentityStation> listStations = new List<IdentityStation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractTrainLineData(reader);
                            record.city_id = city_id;
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
