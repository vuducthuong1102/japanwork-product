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
    public class RpsSalaryFilter
    {
        private readonly string _connectionString;

        public RpsSalaryFilter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSalaryFilter()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentitySalaryFilter> GetAll()
        {
            //Common syntax           
            var sqlCmd = @"SalaryFilter_GetAll";
            List<IdentitySalaryFilter> listData = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        listData = ParsingListSalaryFilterFromReader(reader);
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

        private List<IdentitySalaryFilter> ParsingListSalaryFilterFromReader(IDataReader reader)
        {
            List<IdentitySalaryFilter> listData = new List<IdentitySalaryFilter>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSalaryFilterData(reader);

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentitySalaryFilter ExtractSalaryFilterData(IDataReader reader)
        {
            var record = new IdentitySalaryFilter();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.type = Utils.ConvertToInt32(reader["type"].ToString());
            record.min = Utils.ConvertToInt32(reader["min"]);
            record.max = Utils.ConvertToInt32(reader["max"]);
            record.order = Utils.ConvertToInt32(reader["order"]);
            return record;
        }

        public int Insert(IdentitySalaryFilter identity)
        {
            //Common syntax           
            var sqlCmd = @"SalaryFilter_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@type", identity.type},
                {"@min", identity.min },
                {"@max", identity.max },
                {"@order", identity.order }
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

        public bool Update(IdentitySalaryFilter identity)
        {
            //Common syntax
            var sqlCmd = @"SalaryFilter_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@type", identity.type},
                {"@min", identity.min },
                {"@max", identity.max },
                {"@order", identity.order }
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

        public IdentitySalaryFilter GetById(int id)
        {
            IdentitySalaryFilter info = null;
            var sqlCmd = @"SalaryFilter_GetById";

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
                            info = ExtractSalaryFilterData(reader);
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
            var sqlCmd = @"SalaryFilter_Delete";

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
