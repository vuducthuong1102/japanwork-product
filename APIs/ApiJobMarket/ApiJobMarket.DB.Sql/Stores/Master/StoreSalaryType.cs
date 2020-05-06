using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreSalaryType
    {
        List<IdentitySalaryType> GetByPage(IdentitySalaryType filter, int currentPage, int pageSize);
        int Insert(IdentitySalaryType identity);
        bool Update(IdentitySalaryType identity);
        IdentitySalaryType GetById(int id);
        bool Delete(int id);
        List<IdentitySalaryType> GetList();
    }

    public class StoreSalaryType : IStoreSalaryType
    {
        private readonly string _connectionString;
        private RpsSalaryType myRepository;

        public StoreSalaryType() : this("JobMarketDB")
        {

        }

        public StoreSalaryType(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSalaryType(_connectionString);
        }

        #region  Common

        public List<IdentitySalaryType> GetByPage(IdentitySalaryType filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentitySalaryType identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentitySalaryType identity)
        {
            return myRepository.Update(identity);
        }

        public IdentitySalaryType GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentitySalaryType> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
