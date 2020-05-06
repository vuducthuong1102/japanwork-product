using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreTag
    {
        List<IdentityTag> GetByPage(IdentityTag filter, int currentPage, int pageSize);
        List<IdentityTag> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityTag identity);
        bool Update(IdentityTag identity);
        IdentityTag GetById(int id);
        bool Delete(int id);
        List<IdentityTag> GetList();
    }

    public class StoreTag : IStoreTag
    {
        private readonly string _connectionString;
        private RpsTag myRepository;

        public StoreTag() : this("JobMarketDB")
        {

        }

        public StoreTag(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsTag(_connectionString);
        }

        #region  Common

        public List<IdentityTag> GetByPage(IdentityTag filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityTag> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityTag identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityTag identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityTag GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityTag> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
