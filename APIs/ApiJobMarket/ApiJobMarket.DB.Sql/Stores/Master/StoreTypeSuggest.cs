using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreTypeSuggest
    {
        int Insert(IdentityTypeSuggest identity);
        bool Update(IdentityTypeSuggest identity);
        IdentityTypeSuggest GetById(int id);
        bool Delete(int id);
        List<IdentityTypeSuggest> GetAll();
    }

    public class StoreTypeSuggest : IStoreTypeSuggest
    {
        private readonly string _connectionString;
        private RpsTypeSuggest myRepository;

        public StoreTypeSuggest() : this("JobMarketDB")
        {

        }

        public StoreTypeSuggest(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsTypeSuggest(_connectionString);
        }

        #region  Common

        public List<IdentityTypeSuggest> GetAll()
        {
            return myRepository.GetAll();
        }

        public int Insert(IdentityTypeSuggest identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityTypeSuggest identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityTypeSuggest GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }
        #endregion
    }
}
