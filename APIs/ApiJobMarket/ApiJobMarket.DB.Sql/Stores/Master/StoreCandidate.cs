using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCandidate
    {
        List<IdentityCandidate> GetByPage(IdentityCandidate filter, int currentPage, int pageSize);
        int Insert(IdentityCandidate identity);
        int InsertMultiJobs(IdentityCandidate identity);
        bool Update(IdentityCandidate identity);
        IdentityCandidate GetById(int id);
        bool Delete(int job_id, int agency_id);

        //Actions
        int Cancel(dynamic applyInfo);
        int Ignore(int id);
        int Interview(int id);

        List<IdentityCandidate> GetList();
        IdentityCandidate CheckJobApplied(int job_id, int job_seeker_id);
        List<IdentityCandidate> GetListByJobSeeker(int job_seeker_id);

        List<IdentityCandidate> GetListByJob(IdentityCandidate filter, int currentPage, int pageSize);
        List<IdentityCandidate> GetListByCvId(IdentityCandidate filter, int currentPage, int pageSize);
        List<IdentityCandidate> GetListByAgency(dynamic filter, int currentPage, int pageSize);
        List<IdentityCandidate> GetListByJobId(dynamic filter, int currentPage, int pageSize);
        List<IdentityCandidate> GetListByJobSeekerId(IdentityCandidate filter, int currentPage, int pageSize);
    }

    public class StoreCandidate : IStoreCandidate
    {
        private readonly string _connectionString;
        private RpsCandidate myRepository;

        public StoreCandidate() : this("JobMarketDB")
        {

        }

        public StoreCandidate(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCandidate(_connectionString);
        }

        #region  Common

        public List<IdentityCandidate> GetByPage(IdentityCandidate filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCandidate identity)
        {
            return myRepository.Insert(identity);
        }

        public int InsertMultiJobs(IdentityCandidate identity)
        {
            return myRepository.InsertMultiJobs(identity);
        }

        public bool Update(IdentityCandidate identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCandidate GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int job_id, int agency_id)
        {
            return myRepository.Delete(job_id, agency_id);
        }

        //Actions
        public int Ignore(int id)
        {
            return myRepository.Ignore(id);
        }

        public int Cancel(dynamic applyInfo)
        {
            return myRepository.Cancel(applyInfo);
        }

        public int Interview(int id)
        {
            return myRepository.Interview(id);
        }

        public List<IdentityCandidate> GetList()
        {
            return myRepository.GetList();
        }

        public IdentityCandidate CheckJobApplied(int job_id, int job_seeker_id)
        {
            return myRepository.CheckJobApplied(job_id, job_seeker_id);
        }

        public List<IdentityCandidate> GetListByJobSeeker(int job_seeker_id)
        {
            return myRepository.GetListByJobSeeker(job_seeker_id);
        }

        public List<IdentityCandidate> GetListByJob(IdentityCandidate filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJob(filter, currentPage, pageSize);
        }

        public List<IdentityCandidate> GetListByCvId(IdentityCandidate filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByCvId(filter, currentPage, pageSize);
        }
        public List<IdentityCandidate> GetListByJobSeekerId(IdentityCandidate filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeekerId(filter, currentPage, pageSize);
        }

        public List<IdentityCandidate> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByAgency(filter, currentPage, pageSize);
        }

        public List<IdentityCandidate> GetListByJobId(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobId(filter, currentPage, pageSize);
        }
        #endregion
    }
}
