
using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStorePrefecture
    {
        List<IdentityPrefecture> GetByPage(IdentityPrefecture filter, int currentPage, int pageSize);
        int Insert(IdentityPrefecture identity);
        bool Update(IdentityPrefecture identity);
        IdentityPrefecture GetById(int id);
        bool Delete(int id);
        List<IdentityPrefecture> GetList();
        List<IdentityPrefecture> GetListByRegion(int region_id);
    }

    public class StorePrefecture : IStorePrefecture
    {
        private readonly string _connectionString;
        private RpsPrefecture myRepository;

        public StorePrefecture() : this("JobMarketDB")
        {

        }

        public StorePrefecture(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPrefecture(_connectionString);
        }

        #region  Common

        public List<IdentityPrefecture> GetByPage(IdentityPrefecture filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPrefecture identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityPrefecture identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityPrefecture GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityPrefecture> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityPrefecture> GetListByRegion(int region_id)
        {
            return myRepository.GetListByRegion(region_id);
        }

        #endregion
    }
}
