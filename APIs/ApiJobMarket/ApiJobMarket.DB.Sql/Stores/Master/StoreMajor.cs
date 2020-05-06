using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreMajor
    {
        List<IdentityMajor> GetByPage(IdentityMajor filter, int currentPage, int pageSize);
        int Insert(IdentityMajor identity);
        bool Update(IdentityMajor identity);
        IdentityMajor GetById(int id);
        bool Delete(int id);
        List<IdentityMajor> GetList();
    }

    public class StoreMajor : IStoreMajor
    {
        private readonly string _connectionString;
        private RpsMajor myRepository;

        public StoreMajor() : this("JobMarketDB")
        {

        }

        public StoreMajor(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsMajor(_connectionString);
        }

        #region  Common

        public List<IdentityMajor> GetByPage(IdentityMajor filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityMajor identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityMajor identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityMajor GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityMajor> GetList()
        {
            return myRepository.GetList();
        }
        #endregion
    }
}
