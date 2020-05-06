using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.WebApp.Models
{
    public class StatisticsUsersOnlineModel
    {
        public List<Connector> ListUser { get; set; }

        public StatisticsUsersOnlineModel()
        {
            ListUser = new List<Connector>();
        }
    }

    public class DashBoardStatisticsModel
    {
        public StatisticsDataInYearModel InYearData { get; set; }

        public string UpdatedTime { get; set; }
        public string NextUpdate { get; set; }

        public DashBoardStatisticsModel()
        {
            InYearData = new StatisticsDataInYearModel();
        }
    }

    public class StatisticsDataInYearModel
    {
        public int[] processingData { get; set; }
        public int[] successData { get; set; }
        public int[] failedData { get; set; }

        public StatisticsDataInYearModel()
        {
            processingData = new int[12];
            successData = new int[12];
            failedData = new int[12];
        }
    }

    public class DashBoardStatisticsByWeekModel
    {
        public StatisticsDataInWeekModel InWeekData { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string UpdatedTime { get; set; }
        public string NextUpdate { get; set; }

        public DashBoardStatisticsByWeekModel()
        {
            InWeekData = new StatisticsDataInWeekModel();
        }
    }

    public class DashBoardStatisticsAppByWeekModel
    {
        public StatisticsApplicationDataInWeekModel InWeekData { get; set; }
        public string FromDateStr { get; set; }
        public string ToDateStr { get; set; }
        public string UpdatedTime { get; set; }
        public string NextUpdate { get; set; }

        public DashBoardStatisticsAppByWeekModel()
        {
            InWeekData = new StatisticsApplicationDataInWeekModel();
        }
    }

    public class StatisticsDataInWeekModel
    {
        public int[] processingData { get; set; }
        public int[] successData { get; set; }
        public int[] failedData { get; set; }

        public int processingTotal { get; set; }
        public int successTotal { get; set; }
        public int failedTotal { get; set; }

        public StatisticsDataInWeekModel()
        {
            processingData = new int[7];
            successData = new int[7];
            failedData = new int[7];
        }
    }

    public class StatisticsApplicationDataInWeekModel
    {
        public int[] waitingData { get; set; }
        public int[] approvedData { get; set; }
        public int[] ignoredData { get; set; }

        public int waitingTotal { get; set; }
        public int approvedTotal { get; set; }
        public int ignoredTotal { get; set; }

        public StatisticsApplicationDataInWeekModel()
        {
            waitingData = new int[7];
            approvedData = new int[7];
            ignoredData = new int[7];
        }
    }
}