using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreQualification
    {
        List<IdentityQualification> GetByPage(IdentityQualification filter, int currentPage, int pageSize);
        int Insert(IdentityQualification identity);
        bool Update(IdentityQualification identity);
        IdentityQualification GetById(int id);
        bool Delete(int id);
        List<IdentityQualification> GetList();
    }

    public class StoreQualification : IStoreQualification
    {
        private readonly string _connectionString;
        private RpsQualification myRepository;

        public StoreQualification() : this("JobMarketDB")
        {

        }

        public StoreQualification(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsQualification(_connectionString);
        }

        #region  Common

        public List<IdentityQualification> GetByPage(IdentityQualification filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityQualification identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityQualification identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityQualification GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityQualification> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
