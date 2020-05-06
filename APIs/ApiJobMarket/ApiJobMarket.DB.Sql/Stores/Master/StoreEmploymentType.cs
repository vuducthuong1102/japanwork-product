using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreEmploymentType
    {
        List<IdentityEmploymentType> GetByPage(IdentityEmploymentType filter, int currentPage, int pageSize);
        int Insert(IdentityEmploymentType identity);
        bool Update(IdentityEmploymentType identity);
        IdentityEmploymentType GetById(int id);
        bool Delete(int id);
        List<IdentityEmploymentType> GetList();
    }

    public class StoreEmploymentType : IStoreEmploymentType
    {
        private readonly string _connectionString;
        private RpsEmploymentType myRepository;

        public StoreEmploymentType() : this("JobMarketDB")
        {

        }

        public StoreEmploymentType(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsEmploymentType(_connectionString);
        }

        #region  Common

        public List<IdentityEmploymentType> GetByPage(IdentityEmploymentType filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityEmploymentType identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityEmploymentType identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityEmploymentType GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityEmploymentType> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
