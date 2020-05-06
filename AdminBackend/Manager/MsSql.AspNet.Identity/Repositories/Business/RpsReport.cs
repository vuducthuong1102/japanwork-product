using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
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
            _connectionString = ConfigurationManager.ConnectionStrings["BookingConnection"].ConnectionString;
        }

        //public List<IdentityReport> GetManually(ReportFilter filter)
        //{
        //    //Common syntax
        //    var conn = new SqlConnection(_connectionString);
        //    var sqlCmd = @"Report_GetReportManually";
        //    List<IdentityReport> listData = null;

        //    //For paging 
        //    //int offset = (currentPage - 1) * pageSize;

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@p_PhuTrachId", filter.PhuTrachId },
        //        {"@p_DaiLyId", filter.DaiLyId },
        //        {"@p_FromDate", filter.FromDate },
        //        {"@p_ToDate", filter.ToDate }
        //    };

        //    try
        //    {
        //        using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //        {
        //            listData = ParsingListReportFromReader(reader);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute Report_GetReportManually. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return listData;
        //}

        //private IdentityReport ExtractReportData(IDataReader reader)
        //{
        //    var record = new IdentityReport();

        //    //Seperate properties
        //    record.DatVeId = Utils.ConvertToInt32(reader["DatVeId"]);
        //    record.TenDaiLy = reader["TenDaiLy"].ToString();
        //    record.NguoiPhuTrach = reader["NguoiPhuTrach"].ToString();
        //    record.PNR = reader["PNR"].ToString();
        //    record.TongTien = Utils.ConvertToInt32(reader["TongTien"]);
        //    record.TongPhiDV = Utils.ConvertToInt32(reader["TongPhiDV"]);
        //    record.ChenhLech = Utils.ConvertToInt32(reader["ChenhLech"]);
        //    record.PhiKhac = Utils.ConvertToInt32(reader["PhiKhac"]);
        //    record.ChenhLechHoanVe = Utils.ConvertToInt32(reader["ChenhLechHoanVe"]);
        //    record.ChenhLechTicket = Utils.ConvertToInt32(reader["ChenhLechTicket"]);
        //    record.PhiDoiVe = Utils.ConvertToInt32(reader["PhiDoiVe"]);
        //    record.ChenhLechDoiVe = Utils.ConvertToInt32(reader["ChenhLechDoiVe"]);
        //    record.NgayXuat = reader["NgayXuat"] == DBNull.Value ? null : (DateTime?)reader["NgayXuat"];
           
        //    record.FromDate = reader["FromDate"] == DBNull.Value ? null : (DateTime?)reader["FromDate"];
        //    record.ToDate = reader["ToDate"] == DBNull.Value ? null : (DateTime?)reader["ToDate"];

        //    record.NguoiLienHe = reader["NguoiLienHe"].ToString();

        //    return record;
        //}

        //private List<IdentityReport> ParsingListReportFromReader(IDataReader reader)
        //{
        //    List<IdentityReport> listData = listData = new List<IdentityReport>();
        //    while (reader.Read())
        //    {
        //        //Get common information
        //        var record = ExtractReportData(reader);

        //        listData.Add(record);
        //    }

        //    return listData;
        //}

        private List<IdentityReportByYear> ParsingReportByYearFromReader(IDataReader reader)
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

        private List<IdentityReportInWeek> ParsingReportInWeekFromReader(IDataReader reader)
        {
            List<IdentityReportInWeek> listData = listData = new List<IdentityReportInWeek>();
            while (reader.Read())
            {
                //Get common information
                var record = new IdentityReportInWeek();

                record.Idx = Utils.ConvertToInt32(reader["Idx"]);
                record.CurrentDate = Utils.ConvertToInt32(reader["CurrentDate"]);
                record.PublicCount = Utils.ConvertToInt32(reader["PublicCount"]);
                record.ProcessingCount = Utils.ConvertToInt32(reader["ProcessingCount"]);               
                record.UnProcessedCount = Utils.ConvertToInt32(reader["UnProcessedCount"]);               

                listData.Add(record);
            }

            return listData;
        }

        public List<IdentityReportByYear> GetStatisticsForDashBoard(ReportFilter filter)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Report_GetStatisticsForDashBoard";
            List<IdentityReportByYear> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@FromDate", filter.FromDate },
                {"@ToDate", filter.ToDate }
            };

            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingReportByYearFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Report_GetStatisticsForDashBoard. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
        
        public List<IdentityReportInWeek> GetStatisticsInWeek(ReportFilter filter)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Report_GetStatisticsInWeek";
            List<IdentityReportInWeek> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@FromDate", filter.FromDate },
                {"@ToDate", filter.ToDate },
                {"@ListDays", filter.ListDays }
            };

            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingReportInWeekFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Report_GetStatisticsInWeek. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
    }
}
