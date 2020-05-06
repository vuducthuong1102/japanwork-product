using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreSearchPlace
    {
        List<IdentitySearchPlace> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize);       
    }

    public class StoreSearchPlace : IStoreSearchPlace
    {
        private readonly string _connectionString;
        private RpsSearchPlace myRepository;

        public StoreSearchPlace() : this("JobMarketDB")
        {

        }

        public StoreSearchPlace(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSearchPlace(_connectionString);
        }

        #region  Common

        public List<IdentitySearchPlace> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsByPage(filter, currentPage, pageSize);
        }
        
        #endregion
    }
}
