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
    public class RpsRegion
    {
        private readonly string _connectionString;

        public RpsRegion(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsRegion()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region  Common

        public List<IdentityRegion> GetByPage(IdentityRegion filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Region_GetAll";
            List<IdentityRegion> listData = null;

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
                        listData = ParsingListRegionFromReader(reader);
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
       
        private List<IdentityRegion> ParsingListRegionFromReader(IDataReader reader)
        {
            List<IdentityRegion> listData = listData = new List<IdentityRegion>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractRegionData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityRegion ExtractRegionData(IDataReader reader)
        {
            var record = new IdentityRegion();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.region = reader["region"].ToString();            
            record.furigana = reader["furigana"].ToString();

            return record;
        }

        public int Insert(IdentityRegion identity)
        {
            //Common syntax           
            var sqlCmd = @"Region_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@region", identity.region},
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

        public bool Update(IdentityRegion identity)
        {
            //Common syntax
            var sqlCmd = @"Region_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@region", identity.region},
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

        public IdentityRegion GetById(int id)
        {
            IdentityRegion info = null;
            var sqlCmd = @"Region_GetById";

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
                            info = ExtractRegionData(reader);
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
            var sqlCmd = @"Region_Delete";

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

        public List<IdentityRegion> GetList(int country_id)
        {
            //Common syntax            
            var sqlCmd = @"Region_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@country_id", country_id},
            };

            List<IdentityRegion> listData = new List<IdentityRegion>();
            //List<IdentityCity> cities = new List<IdentityCity>();
            List<IdentityPrefecture> prefectures = new List<IdentityPrefecture>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractRegionData(reader);

                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                prefectures.Add(RpsPrefecture.ExtractPrefectureData(reader));
                            }
                        }

                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        cities.Add(RpsCity.ExtractCityData(reader));
                        //    }
                        //}

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.Prefectures = prefectures.Where(x => x.region_id == item.id).ToList();
                            }
                        }

                        //var allLangList = new List<IdentityRegionLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityRegionLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.RegionId = Utils.ConvertToInt32(reader["RegionId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.RegionId == item.Id).ToList();
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
