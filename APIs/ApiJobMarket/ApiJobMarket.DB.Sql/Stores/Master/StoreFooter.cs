using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreFooter
    {
        List<IdentityFooter> GetAll();
        IdentityFooter GetByLangCode(string langCode);
        bool Update(IdentityFooter identity);       
    }

    public class StoreFooter : IStoreFooter
    {
        private readonly string _connectionString;
        private RpsFooter myRepository;

        public StoreFooter() : this("JobMarketDB")
        {
        }

        public StoreFooter(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsFooter(_connectionString);
        }

        #region  Common

        public List<IdentityFooter> GetAll()
        {
            return myRepository.GetAll();
        }
        
        public bool Update(IdentityFooter identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityFooter GetByLangCode(string langCode)
        {
            return myRepository.GetByLangCode(langCode);
        }
       
        #endregion
    }

}
