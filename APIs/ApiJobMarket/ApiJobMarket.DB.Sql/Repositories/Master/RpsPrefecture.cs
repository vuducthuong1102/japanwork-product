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
    public class RpsPrefecture
    {
        private readonly string _connectionString;

        public RpsPrefecture(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPrefecture()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityPrefecture> GetByPage(IdentityPrefecture filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Prefecture_GetAll";
            List<IdentityPrefecture> listData = null;

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
                        listData = ParsingListPrefectureFromReader(reader);
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
       
        private List<IdentityPrefecture> ParsingListPrefectureFromReader(IDataReader reader)
        {
            List<IdentityPrefecture> listData = listData = new List<IdentityPrefecture>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPrefectureData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityPrefecture ExtractPrefectureData(IDataReader reader)
        {
            var record = new IdentityPrefecture();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.region_id = Utils.ConvertToInt32(reader["region_id"]);
            record.prefecture = reader["prefecture"].ToString();            
            record.furigana = reader["furigana"].ToString();

            return record;
        }

        public int Insert(IdentityPrefecture identity)
        {
            //Common syntax           
            var sqlCmd = @"Prefecture_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@prefecture", identity.prefecture},
                {"@region_id", identity.region_id },
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

        public bool Update(IdentityPrefecture identity)
        {
            //Common syntax
            var sqlCmd = @"Prefecture_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@prefecture", identity.prefecture},
                {"@region_id", identity.region_id },
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

        public IdentityPrefecture GetById(int id)
        {
            IdentityPrefecture info = null;
            var sqlCmd = @"Prefecture_GetById";

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
                            info = ExtractPrefectureData(reader);
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
            var sqlCmd = @"Prefecture_Delete";

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

        public List<IdentityPrefecture> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Prefecture_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityPrefecture> listData = new List<IdentityPrefecture>();
            List<IdentityCity> cities = new List<IdentityCity>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractPrefectureData(reader);

                            listData.Add(record);
                        }

                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        cities.Add(RpsCity.ExtractCityData(reader));
                        //    }
                        //}

                        //if (listData.HasData())
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.Cities = cities.Where(x =>x.prefecture_id  == item.id).ToList();                                
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

        public List<IdentityPrefecture> GetListByRegion(int region_id)
        {
            //Common syntax            
            var sqlCmd = @"Prefecture_GetListByRegion";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@region_id", region_id},
            };

            List<IdentityPrefecture> listData = new List<IdentityPrefecture>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractPrefectureData(reader);

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
