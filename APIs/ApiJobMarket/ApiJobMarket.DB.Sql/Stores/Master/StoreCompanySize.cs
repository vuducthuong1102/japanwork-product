using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCompanySize
    {
        List<IdentityCompanySize> GetByPage(IdentityCompanySize filter, int currentPage, int pageSize);
        int Insert(IdentityCompanySize identity);
        bool Update(IdentityCompanySize identity);
        IdentityCompanySize GetById(int id);
        bool Delete(int id);
        List<IdentityCompanySize> GetList();
    }

    public class StoreCompanySize : IStoreCompanySize
    {
        private readonly string _connectionString;
        private RpsCompanySize myRepository;

        public StoreCompanySize() : this("JobMarketDB")
        {

        }

        public StoreCompanySize(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCompanySize(_connectionString);
        }

        #region  Common

        public List<IdentityCompanySize> GetByPage(IdentityCompanySize filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCompanySize identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCompanySize identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCompanySize GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityCompanySize> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
