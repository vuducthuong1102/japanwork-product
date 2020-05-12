using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsStatistics
    {
        private readonly string _connectionString;

        public RpsStatistics(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsStatistics()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        private IdentityStatisticsJobByYear ParsingStatisticsByYear(IDataReader reader)
        {
            //Get common information
            var record = new IdentityStatisticsJobByYear();

            record.id = Utils.ConvertToInt32(reader["id"]);
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.year = Utils.ConvertToInt32(reader["year"]);
            record.month_1 = Utils.ConvertToInt32(reader["month_1"]);
            record.month_2 = Utils.ConvertToInt32(reader["month_2"]);
            record.month_3 = Utils.ConvertToInt32(reader["month_3"]);
            record.month_4 = Utils.ConvertToInt32(reader["month_4"]);
            record.month_5 = Utils.ConvertToInt32(reader["month_5"]);
            record.month_6 = Utils.ConvertToInt32(reader["month_6"]);
            record.month_7 = Utils.ConvertToInt32(reader["month_7"]);
            record.month_8 = Utils.ConvertToInt32(reader["month_8"]);
            record.month_9 = Utils.ConvertToInt32(reader["month_9"]);
            record.month_10 = Utils.ConvertToInt32(reader["month_10"]);
            record.month_11 = Utils.ConvertToInt32(reader["month_11"]);
            record.month_12 = Utils.ConvertToInt32(reader["month_12"]);

            record.last_cal_time = reader["last_cal_time"] == DBNull.Value ? null : (DateTime?)reader["last_cal_time"];

            return record;
        }
        
        public IdentityStatisticsJobByYear PublishedJobByYear(IdentityStatisticsFilter filter)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Statistics_PublishedJobByYear";

            IdentityStatisticsJobByYear data = new IdentityStatisticsJobByYear();

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", filter.agency_id },
                {"@year", filter.year }
            };

            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    if (reader.Read())
                    {
                        data = ParsingStatisticsByYear(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return data;
        }
    }
}
