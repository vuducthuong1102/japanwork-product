using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreAgency
    {
        List<IdentityAgency> GetByPage(IdentityAgency filter, int currentPage, int pageSize);
        int Insert(IdentityAgency identity);
        bool Update(IdentityAgency identity);
        IdentityAgency GetById(int id);
        IdentityAgency GetBaseInfo(int id);
        bool Delete(int id);
        List<IdentityAgency> GetList();

        int CreateProfile(IdentityAgency identity);
        bool UpdateProfile(IdentityAgency identity);
        bool UpdateLogo(IdentityAgency identity);
    }

    public class StoreAgency : IStoreAgency
    {
        private readonly string _connectionString;
        private RpsAgency myRepository;

        public StoreAgency() : this("JobMarketDB")
        {

        }

        public StoreAgency(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsAgency(_connectionString);
        }

        #region  Common

        public List<IdentityAgency> GetByPage(IdentityAgency filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityAgency identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityAgency identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityAgency GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityAgency GetBaseInfo(int id)
        {
            return myRepository.GetBaseInfo(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityAgency> GetList()
        {
            return myRepository.GetList();
        }

        public int CreateProfile(IdentityAgency identity)
        {
            return myRepository.CreateProfile(identity);
        }

        public bool UpdateProfile(IdentityAgency identity)
        {
            return myRepository.UpdateProfile(identity);
        }

        public bool UpdateLogo(IdentityAgency identity)
        {
            return myRepository.UpdateLogo(identity);
        }
        #endregion
    }
}
