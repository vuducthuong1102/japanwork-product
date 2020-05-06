using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCompanyNote
    {
        List<IdentityCompanyNote> GetByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityCompanyNote identity);
        int Update(IdentityCompanyNote identity);
        IdentityCompanyNote GetById(int id);
        bool Delete(int id);
    }

    public class StoreCompanyNote : IStoreCompanyNote
    {
        private readonly string _connectionString;
        private RpsCompanyNote myRepository;

        public StoreCompanyNote() : this("JobMarketDB")
        {

        }

        public StoreCompanyNote(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCompanyNote(_connectionString);
        }

        #region  Common

        public List<IdentityCompanyNote> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCompanyNote identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityCompanyNote identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCompanyNote GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        #endregion
    }
}
