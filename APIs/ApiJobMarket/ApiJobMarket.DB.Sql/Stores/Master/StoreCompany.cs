using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCompany
    {
        List<IdentityCompany> GetByPage(IdentityCompany filter, int currentPage, int pageSize);
        int Insert(IdentityCompany identity);
        int Update(IdentityCompany identity);
        IdentityCompany GetById(int id);

        IdentityCompany CountJobById(int id);
        bool Delete(string ids, int staff_id);
        List<IdentityCompany> GetList();


        List<IdentityCompany> GetListByAgency(IdentityCompany filter, int currentPage, int pageSize);
        //List<IdentityCompany> GetPageByAgency(dynamic filter);
        IdentityCompanyCounter GetCounterForDeletion(int id);
        List<int> DeleteAllJobs(int id);
    }

    public class StoreCompany : IStoreCompany
    {
        private readonly string _connectionString;
        private RpsCompany myRepository;

        public StoreCompany() : this("JobMarketDB")
        {

        }

        public StoreCompany(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCompany(_connectionString);
        }

        #region  Common

        public List<IdentityCompany> GetByPage(IdentityCompany filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCompany identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityCompany identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCompany GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityCompany CountJobById(int id)
        {
            return myRepository.CountJobById(id);
        }

        public bool Delete(string ids, int staff_id)
        {
            return myRepository.Delete(ids, staff_id);
        }

        public List<IdentityCompany> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityCompany> GetListByAgency(IdentityCompany filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByAgency(filter, currentPage, pageSize);
        }

        //public List<IdentityCompany> GetPageByAgency(dynamic filter)
        //{
        //    return myRepository.GetPageByAgency(filter);
        //}

        public IdentityCompanyCounter GetCounterForDeletion(int id)
        {
            return myRepository.GetCounterForDeletion(id);
        }

        public List<int> DeleteAllJobs(int id)
        {
            return myRepository.DeleteAllJobs(id);
        }
        #endregion
    }
}
