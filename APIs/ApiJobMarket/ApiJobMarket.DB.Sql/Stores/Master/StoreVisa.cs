using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreVisa
    {
        List<IdentityVisa> GetByPage(IdentityVisa filter, int currentPage, int pageSize);
        int Insert(IdentityVisa identity);
        bool Update(IdentityVisa identity);
        IdentityVisa GetById(int id);
        bool Delete(int id);
        List<IdentityVisa> GetList();
    }

    public class StoreVisa : IStoreVisa
    {
        private readonly string _connectionString;
        private RpsVisa myRepository;

        public StoreVisa() : this("JobMarketDB")
        {

        }

        public StoreVisa(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsVisa(_connectionString);
        }

        #region  Common

        public List<IdentityVisa> GetByPage(IdentityVisa filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityVisa identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityVisa identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityVisa GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityVisa> GetList()
        {
            return myRepository.GetList();
        }
        #endregion
    }
}
