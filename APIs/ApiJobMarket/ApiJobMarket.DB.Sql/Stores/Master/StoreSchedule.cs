using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreSchedule
    {
        List<IdentitySchedule> GetByPage(IdentitySchedule filter, int currentPage, int pageSize);
        int Insert(IdentitySchedule identity);
        int Update(IdentitySchedule identity);
        int UpdateTime(IdentitySchedule identity);
        IdentitySchedule GetDetail(IdentitySchedule identity);
        bool Delete(IdentitySchedule identity);
       
        List<IdentitySchedule> GetList();        
        List<IdentitySchedule> GetListByJobSeeker(int job_seeker_id);
        List<IdentitySchedule> GetListByJob(IdentitySchedule filter, int currentPage, int pageSize);
        List<IdentitySchedule> GetByStaff(dynamic filter);
        List<IdentitySchedule> GetTodayByStaff(dynamic filter);
        List<IdentitySchedule> GetListByJobId(dynamic filter, int currentPage, int pageSize);
    }

    public class StoreSchedule : IStoreSchedule
    {
        private readonly string _connectionString;
        private RpsSchedule myRepository;

        public StoreSchedule() : this("JobMarketDB")
        {

        }

        public StoreSchedule(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSchedule(_connectionString);
        }

        #region  Common

        public List<IdentitySchedule> GetByPage(IdentitySchedule filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentitySchedule identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentitySchedule identity)
        {
            return myRepository.Update(identity);
        }

        public int UpdateTime(IdentitySchedule identity)
        {
            return myRepository.UpdateTime(identity);
        }

        public IdentitySchedule GetDetail(IdentitySchedule identity)
        {
            return myRepository.GetDetail(identity);
        }

        public bool Delete(IdentitySchedule identity)
        {
            return myRepository.Delete(identity);
        }        

        public List<IdentitySchedule> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentitySchedule> GetListByJobSeeker(int job_seeker_id)
        {
            return myRepository.GetListByJobSeeker(job_seeker_id);
        }

        public List<IdentitySchedule> GetListByJob(IdentitySchedule filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJob(filter, currentPage, pageSize);
        }

        public List<IdentitySchedule> GetByStaff(dynamic filter)
        {
            return myRepository.GetByStaff(filter);
        }

        public List<IdentitySchedule> GetTodayByStaff(dynamic filter)
        {
            return myRepository.GetTodayByStaff(filter);
        }

        public List<IdentitySchedule> GetListByJobId(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobId(filter, currentPage, pageSize);
        }
        #endregion
    }
}
