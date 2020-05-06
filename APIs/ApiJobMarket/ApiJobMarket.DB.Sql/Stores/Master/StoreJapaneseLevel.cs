using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreJapaneseLevel
    {
        List<IdentityJapaneseLevel> GetByPage(IdentityJapaneseLevel filter, int currentPage, int pageSize);
        int Insert(IdentityJapaneseLevel identity);
        bool Update(IdentityJapaneseLevel identity);
        IdentityJapaneseLevel GetById(int id);
        bool Delete(int id);
        List<IdentityJapaneseLevel> GetList();
    }

    public class StoreJapaneseLevel : IStoreJapaneseLevel
    {
        private readonly string _connectionString;
        private RpsJapaneseLevel myRepository;

        public StoreJapaneseLevel() : this("JobMarketDB")
        {

        }

        public StoreJapaneseLevel(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsJapaneseLevel(_connectionString);
        }

        #region  Common

        public List<IdentityJapaneseLevel> GetByPage(IdentityJapaneseLevel filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityJapaneseLevel identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityJapaneseLevel identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityJapaneseLevel GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityJapaneseLevel> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
