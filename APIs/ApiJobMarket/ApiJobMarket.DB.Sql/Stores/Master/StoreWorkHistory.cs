using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreWorkHistory
    {
        List<IdentityWorkHistory> GetByPage(IdentityWorkHistory filter, int currentPage, int pageSize);
        int Insert(IdentityWorkHistory identity);
        bool Update(IdentityWorkHistory identity);
        IdentityWorkHistory GetById(int id);
        bool Delete(int id);
        List<IdentityWorkHistory> GetList();

        #region Job seeker

        IdentityJobSeekerWorkHistory JobSeekerGetDetail(int id, int job_seeker_id);
        List<IdentityJobSeekerWorkHistory> GetListJobSeekerWorkHistory(int job_seeker_id);
        bool JobSeekerUpdate(IdentityJobSeekerWorkHistory identity);
        bool JobSeekerDelete(IdentityJobSeekerWorkHistory identity);

        #endregion

        #region Agency

        IdentityJobSeekerWorkHistory A_JobSeekerGetDetail(int id, int job_seeker_id);
        List<IdentityJobSeekerWorkHistory> A_GetListJobSeekerWorkHistory(int job_seeker_id);
        bool A_JobSeekerUpdate(IdentityJobSeekerWorkHistory identity);
        bool A_JobSeekerDelete(IdentityJobSeekerWorkHistory identity);

        #endregion

        #region Cv

        IdentityCvWorkHistory CvGetDetail(int id, int job_seeker_id);
        List<IdentityCvWorkHistory> GetListCvWorkHistory(int job_seeker_id);
        int CvUpdate(IdentityCvWorkHistory identity);
        bool CvDelete(IdentityCvWorkHistory identity);

        #endregion

        #region Cs

        IdentityCsWorkHistory CsGetDetail(int id, int job_seeker_id);
        List<IdentityCsWorkHistory> GetListCsWorkHistory(int job_seeker_id);
        int CsUpdate(IdentityCsWorkHistory identity);
        bool CsDelete(IdentityCsWorkHistory identity);

        #endregion
    }

    public class StoreWorkHistory : IStoreWorkHistory
    {
        private readonly string _connectionString;
        private RpsWorkHistory myRepository;

        public StoreWorkHistory() : this("JobMarketDB")
        {

        }

        public StoreWorkHistory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsWorkHistory(_connectionString);
        }

        #region  Common

        public List<IdentityWorkHistory> GetByPage(IdentityWorkHistory filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityWorkHistory identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityWorkHistory identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityWorkHistory GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityWorkHistory> GetList()
        {
            return myRepository.GetList();
        }

        #region Job seeker

        public IdentityJobSeekerWorkHistory JobSeekerGetDetail(int id, int job_seeker_id)
        {
            return myRepository.JobSeekerGetDetail(id, job_seeker_id);
        }

        public List<IdentityJobSeekerWorkHistory> GetListJobSeekerWorkHistory(int job_seeker_id)
        {
            return myRepository.GetListJobSeekerWorkHistory(job_seeker_id);
        }

        public bool JobSeekerUpdate(IdentityJobSeekerWorkHistory identity)
        {
            return myRepository.JobSeekerUpdate(identity);
        }

        public bool JobSeekerDelete(IdentityJobSeekerWorkHistory identity)
        {
            return myRepository.JobSeekerDelete(identity);
        }

        #endregion

        #region Agency

        public IdentityJobSeekerWorkHistory A_JobSeekerGetDetail(int id, int job_seeker_id)
        {
            return myRepository.A_JobSeekerGetDetail(id, job_seeker_id);
        }

        public List<IdentityJobSeekerWorkHistory> A_GetListJobSeekerWorkHistory(int job_seeker_id)
        {
            return myRepository.A_GetListJobSeekerWorkHistory(job_seeker_id);
        }

        public bool A_JobSeekerUpdate(IdentityJobSeekerWorkHistory identity)
        {
            return myRepository.A_JobSeekerUpdate(identity);
        }

        public bool A_JobSeekerDelete(IdentityJobSeekerWorkHistory identity)
        {
            return myRepository.A_JobSeekerDelete(identity);
        }

        #endregion

        #region Cv

        public IdentityCvWorkHistory CvGetDetail(int id, int cv_id)
        {
            return myRepository.CvGetDetail(id, cv_id);
        }

        public List<IdentityCvWorkHistory> GetListCvWorkHistory(int cv_id)
        {
            return myRepository.GetListCvWorkHistory(cv_id);
        }

        public int CvUpdate(IdentityCvWorkHistory identity)
        {
            return myRepository.CvUpdate(identity);
        }

        public bool CvDelete(IdentityCvWorkHistory identity)
        {
            return myRepository.CvDelete(identity);
        }

        #endregion

        #region Cs

        public IdentityCsWorkHistory CsGetDetail(int id, int cs_id)
        {
            return myRepository.CsGetDetail(id, cs_id);
        }

        public List<IdentityCsWorkHistory> GetListCsWorkHistory(int cs_id)
        {
            return myRepository.GetListCsWorkHistory(cs_id);
        }

        public int CsUpdate(IdentityCsWorkHistory identity)
        {
            return myRepository.CsUpdate(identity);
        }

        public bool CsDelete(IdentityCsWorkHistory identity)
        {
            return myRepository.CsDelete(identity);
        }

        #endregion

        #endregion
    }
}
