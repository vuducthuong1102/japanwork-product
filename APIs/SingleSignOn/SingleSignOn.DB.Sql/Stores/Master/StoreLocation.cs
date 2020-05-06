using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace SingleSignOn.DB.Sql.Stores
{
    public interface IStoreLocation
    {
        List<IdentityLocation> GetAll(IdentityLocation filter, int currentPage, int pageSize);
        int Insert(IdentityLocation identity);
        bool Update(IdentityLocation identity);
        IdentityLocation GetById(int Id);
        bool Delete(int Id);
        List<IdentityLocation> GetList(string keyword);
    }

    public class StoreLocation : IStoreLocation
    {
        private readonly string _connectionString;
        private RpsLocation myRepository;

        public StoreLocation() : this("SingleSignOnDB")
        {

        }

        public StoreLocation(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsLocation(_connectionString);
        }

        #region  Common

        public List<IdentityLocation> GetAll(IdentityLocation filter, int currentPage, int pageSize)
        {
            return myRepository.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityLocation identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityLocation identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityLocation GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }

        public List<IdentityLocation> GetList(string keyword)
        {
            return myRepository.GetList(keyword);
        }

        #endregion
    }
}
