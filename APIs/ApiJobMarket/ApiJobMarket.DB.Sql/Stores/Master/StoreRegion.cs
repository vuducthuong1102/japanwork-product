using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreRegion
    {
        List<IdentityRegion> GetByPage(IdentityRegion filter, int currentPage, int pageSize);
        int Insert(IdentityRegion identity);
        bool Update(IdentityRegion identity);
        IdentityRegion GetById(int id);
        bool Delete(int id);
        List<IdentityRegion> GetList(int country_id);
    }

    public class StoreRegion : IStoreRegion
    {
        private readonly string _connectionString;
        private RpsRegion myRepository;

        public StoreRegion() : this("JobMarketDB")
        {

        }

        public StoreRegion(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsRegion(_connectionString);
        }

        #region  Common

        public List<IdentityRegion> GetByPage(IdentityRegion filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityRegion identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityRegion identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityRegion GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityRegion> GetList(int country_id)
        {
            return myRepository.GetList(country_id);
        }
        #endregion
    }
}
