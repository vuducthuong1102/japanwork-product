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
    public class RpsSearchPlace
    {
        private readonly string _connectionString;

        public RpsSearchPlace(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSearchPlace()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentitySearchPlace> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"SearchPlace_GetSuggestionsByPage";
            List<IdentitySearchPlace> listData = new List<IdentitySearchPlace>();

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
                        //listData = ParsingListSearchPlaceFromReader(reader);
                        while (reader.Read())
                        {
                            //Regions
                            var region = RpsRegion.ExtractRegionData(reader);
                            if(region != null)
                            {
                                var record = new IdentitySearchPlace();
                                record.id = region.id;
                                record.name = region.region;
                                record.type = "region";
                                record.furigana = region.furigana;

                                listData.Add(record);
                            }
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                //Prefectures
                                var prefecture = RpsPrefecture.ExtractPrefectureData(reader);
                                if (prefecture != null)
                                {
                                    var record = new IdentitySearchPlace();
                                    record.id = prefecture.id;
                                    record.parent_id = prefecture.region_id;
                                    record.name = prefecture.prefecture;
                                    record.type = "prefecture";
                                    record.furigana = prefecture.furigana;

                                    listData.Add(record);
                                }
                            }
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                //Cities
                                var city = RpsCity.ExtractCityData(reader);
                                if (city != null)
                                {
                                    var record = new IdentitySearchPlace();
                                    record.id = city.id;
                                    record.parent_id = city.prefecture_id;
                                    record.name = city.city;
                                    record.type = "city";
                                    record.furigana = city.furigana;

                                    listData.Add(record);
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
        
        #endregion
    }
}
