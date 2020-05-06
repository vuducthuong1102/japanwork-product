using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreReport
    {
        List<IdentityReportByYear> GetStatisticsForDashBoard(ReportFilter filter);
        List<IdentityReportInWeek> GetStatisticsInWeek(ReportFilter filter);
    }

    public class StoreReport : IStoreReport
    {
        private readonly string _connectionString;
        private RpsReport myRepository;

        public StoreReport(): this("JobMarketConnection")
        {

        }

        public StoreReport(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsReport(_connectionString);
        }

        public List<IdentityReportByYear> GetStatisticsForDashBoard(ReportFilter filter)
        {
            return myRepository.GetStatisticsForDashBoard(filter);
        }

        public List<IdentityReportInWeek> GetStatisticsInWeek(ReportFilter filter)
        {
            return myRepository.GetStatisticsInWeek(filter);
        }
    }
}
