using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreSalaryFilter
    {
        int Insert(IdentitySalaryFilter identity);
        bool Update(IdentitySalaryFilter identity);
        IdentitySalaryFilter GetById(int id);
        bool Delete(int id);
        List<IdentitySalaryFilter> GetAll();
    }

    public class StoreSalaryFilter : IStoreSalaryFilter
    {
        private readonly string _connectionString;
        private RpsSalaryFilter myRepository;

        public StoreSalaryFilter() : this("JobMarketDB")
        {

        }

        public StoreSalaryFilter(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSalaryFilter(_connectionString);
        }

        #region  Common

        public List<IdentitySalaryFilter> GetAll()
        {
            return myRepository.GetAll();
        }

        public int Insert(IdentitySalaryFilter identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentitySalaryFilter identity)
        {
            return myRepository.Update(identity);
        }

        public IdentitySalaryFilter GetById(int id)
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
