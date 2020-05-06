using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using ApiJobMarket.ShareLibs.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsReport
    {
        private readonly string _connectionString;

        public RpsReport(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsReport()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        private List<IdentityReportByYear> ParsingReportByYear(IDataReader reader)
        {
            List<IdentityReportByYear> listData = listData = new List<IdentityReportByYear>();
            while (reader.Read())
            {
                //Get common information
                var record = new IdentityReportByYear();

                record.CurrentMonth = Utils.ConvertToInt32(reader["CurrentMonth"]);
                record.PublicCount = Utils.ConvertToInt32(reader["PublicCount"]);
                record.ProcessingCount = Utils.ConvertToInt32(reader["ProcessingCount"]);
                record.UnProcessedCount = Utils.ConvertToInt32(reader["UnProcessedCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private List<IdentityReportByWeek> ParsingReportByWeek(IDataReader reader)
        {
            List<IdentityReportByWeek> listData = listData = new List<IdentityReportByWeek>();
            while (reader.Read())
            {
                //Get common information
                var record = new IdentityReportByWeek();

                record.Idx = Utils.ConvertToInt32(reader["Idx"]);
                record.CurrentDate = Utils.ConvertToInt32(reader["CurrentDate"]);
                record.PublicCount = Utils.ConvertToInt32(reader["PublicCount"]);
                record.ProcessingCount = Utils.ConvertToInt32(reader["ProcessingCount"]);
                record.UnProcessedCount = Utils.ConvertToInt32(reader["UnProcessedCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private List<IdentityReportApplicationByWeek> ParsingReportApplicationByWeek(IDataReader reader)
        {
            List<IdentityReportApplicationByWeek> listData = listData = new List<IdentityReportApplicationByWeek>();
            while (reader.Read())
            {
                //Get common information
                var record = new IdentityReportApplicationByWeek();

                record.Idx = Utils.ConvertToInt32(reader["Idx"]);
                record.CurrentDate = Utils.ConvertToInt32(reader["CurrentDate"]);
                record.ApprovedCount = Utils.ConvertToInt32(reader["ApprovedCount"]);
                record.WaitingCount = Utils.ConvertToInt32(reader["WaitingCount"]);
                record.IgnoredCount = Utils.ConvertToInt32(reader["IgnoredCount"]);

                listData.Add(record);
            }

            return listData;
        }

        public List<IdentityReportByYear> GetStatisticsForDashBoard(IdentityReportFilter filter)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"A_Report_GetStatisticsForDashBoard";
            List<IdentityReportByYear> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", filter.AgencyId },
                {"@FromDate", filter.FromDate },
                {"@ToDate", filter.ToDate }
            };

            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingReportByYear(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityReportByWeek> GetStatisticsByWeek(IdentityReportFilter filter)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"A_Report_GetStatisticsByWeek";
            List<IdentityReportByWeek> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", filter.AgencyId },
                {"@FromDate", filter.FromDate },
                {"@ToDate", filter.ToDate },
                {"@ListDays", filter.ListDays }
            };

            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingReportByWeek(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return listData;
        }
        
        public List<IdentityReportApplicationByWeek> GetStatisticsApplicationByWeek(IdentityReportFilter filter)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"A_Report_GetStatisticsApplicationByWeek";
            List<IdentityReportApplicationByWeek> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", filter.AgencyId },
                {"@FromDate", filter.FromDate },
                {"@ToDate", filter.ToDate },
                {"@ListDays", filter.ListDays }
            };

            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingReportApplicationByWeek(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return listData;
        }
    }
}
