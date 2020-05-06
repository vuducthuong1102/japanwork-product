using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCity
    {
        List<IdentityCity> GetByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityCity identity);
        bool Update(IdentityCity identity);
        IdentityCity GetById(int id);
        bool Delete(int id);
        List<IdentityCity> GetList();
        List<IdentityCity> GetListByPrefecture(int prefecture_id);
    }

    public class StoreCity : IStoreCity
    {
        private readonly string _connectionString;
        private RpsCity myRepository;

        public StoreCity() : this("JobMarketDB")
        {

        }

        public StoreCity(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCity(_connectionString);
        }

        #region  Common

        public List<IdentityCity> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCity identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCity identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCity GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityCity> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityCity> GetListByPrefecture(int prefecture_id)
        {
            return myRepository.GetListByPrefecture(prefecture_id);
        }

        #endregion
    }
}
