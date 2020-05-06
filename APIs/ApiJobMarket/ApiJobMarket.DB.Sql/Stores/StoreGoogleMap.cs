using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Stores
{
    public class StoreGoogleMap :IStoreGoogleMap
    {
        private readonly string _connectionString;
        private RpsGoogleMap myRepository;

        public StoreGoogleMap()
            : this("JobMarketDB")
        {

        }

        public StoreGoogleMap(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsGoogleMap(_connectionString);
        }


        public int Insert(IdentityGoogleMap identity)
        {
            return myRepository.Insert(identity);
        }
    }
    public interface IStoreGoogleMap
    {
        int Insert(IdentityGoogleMap identity);
    }
}
