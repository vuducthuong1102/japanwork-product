using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreReport
    {
        List<IdentityReportByYear> GetStatisticsForDashBoard(IdentityReportFilter filter);
        List<IdentityReportByWeek> GetStatisticsByWeek(IdentityReportFilter filter);
        List<IdentityReportApplicationByWeek> GetStatisticsApplicationByWeek(IdentityReportFilter filter);
    }

    public class StoreReport : IStoreReport
    {
        private readonly string _connectionString;
        private RpsReport myRepository;

        public StoreReport() : this("JobMarketDB")
        {

        }

        public StoreReport(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsReport(_connectionString);
        }

        public List<IdentityReportByYear> GetStatisticsForDashBoard(IdentityReportFilter filter)
        {
            return myRepository.GetStatisticsForDashBoard(filter);
        }

        public List<IdentityReportByWeek> GetStatisticsByWeek(IdentityReportFilter filter)
        {
            return myRepository.GetStatisticsByWeek(filter);
        }
        
        public List<IdentityReportApplicationByWeek> GetStatisticsApplicationByWeek(IdentityReportFilter filter)
        {
            return myRepository.GetStatisticsApplicationByWeek(filter);
        }
    }
}
