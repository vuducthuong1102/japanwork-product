using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCountry
    {
        List<IdentityCountry> GetByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityCountry identity);
        bool Update(IdentityCountry identity);
        IdentityCountry GetById(int id);
        bool Delete(int id);
        List<IdentityCountry> GetList();
    }

    public class StoreCountry : IStoreCountry
    {
        private readonly string _connectionString;
        private RpsCountry myRepository;

        public StoreCountry() : this("JobMarketDB")
        {

        }

        public StoreCountry(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCountry(_connectionString);
        }

        #region  Common

        public List<IdentityCountry> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCountry identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCountry identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCountry GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityCountry> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
