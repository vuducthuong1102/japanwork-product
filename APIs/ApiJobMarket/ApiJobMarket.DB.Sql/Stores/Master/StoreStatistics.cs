using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreStatistics
    {
        IdentityStatisticsJobByYear PublishedJobByYear(IdentityStatisticsFilter filter);       
    }

    public class StoreStatistics : IStoreStatistics
    {
        private readonly string _connectionString;
        private RpsStatistics rp;

        public StoreStatistics() : this("JobMarketDB")
        {

        }

        public StoreStatistics(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            rp = new RpsStatistics(_connectionString);
        }

        public IdentityStatisticsJobByYear PublishedJobByYear(IdentityStatisticsFilter filter)
        {
            return rp.PublishedJobByYear(filter);
        }       
    }
}
