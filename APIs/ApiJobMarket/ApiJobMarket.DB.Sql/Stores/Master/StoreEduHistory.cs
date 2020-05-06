using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreEduHistory
    {
        List<IdentityEduHistory> GetByPage(IdentityEduHistory filter, int currentPage, int pageSize);
        int Insert(IdentityEduHistory identity);
        bool Update(IdentityEduHistory identity);
        IdentityEduHistory GetById(int id);
        bool Delete(int id);
        List<IdentityEduHistory> GetList();

        #region Job seeker

        IdentityJobSeekerEduHistory JobSeekerGetDetail(int id, int job_seeker_id);
        List<IdentityJobSeekerEduHistory> GetListJobSeekerEduHistory(int job_seeker_id);
        bool JobSeekerUpdate(IdentityJobSeekerEduHistory identity);
        bool JobSeekerDelete(IdentityJobSeekerEduHistory identity);

        #endregion

        #region Agency

        IdentityJobSeekerEduHistory A_JobSeekerGetDetail(int id, int job_seeker_id);
        List<IdentityJobSeekerEduHistory> A_GetListJobSeekerEduHistory(int job_seeker_id);
        bool A_JobSeekerUpdate(IdentityJobSeekerEduHistory identity);
        bool A_JobSeekerDelete(IdentityJobSeekerEduHistory identity);

        #endregion

        #region Cv

        IdentityCvEduHistory CvGetDetail(int id, int cv_id);
        List<IdentityCvEduHistory> GetListCvEduHistory(int cv_id);
        int CvUpdate(IdentityCvEduHistory identity);
        bool CvDelete(IdentityCvEduHistory identity);

        #endregion

        #region Cs

        IdentityCsEduHistory CsGetDetail(int id, int cv_id);
        List<IdentityCsEduHistory> GetListCsEduHistory(int cv_id);
        int CsUpdate(IdentityCsEduHistory identity);
        bool CsDelete(IdentityCsEduHistory identity);

        #endregion
    }

    public class StoreEduHistory : IStoreEduHistory
    {
        private readonly string _connectionString;
        private RpsEduHistory myRepository;

        public StoreEduHistory() : this("JobMarketDB")
        {

        }

        public StoreEduHistory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsEduHistory(_connectionString);
        }

        #region  Common

        public List<IdentityEduHistory> GetByPage(IdentityEduHistory filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityEduHistory identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityEduHistory identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityEduHistory GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityEduHistory> GetList()
        {
            return myRepository.GetList();
        }

        #region Job seeker

        public IdentityJobSeekerEduHistory JobSeekerGetDetail(int id, int job_seeker_id)
        {
            return myRepository.JobSeekerGetDetail(id, job_seeker_id);
        }

        public List<IdentityJobSeekerEduHistory> GetListJobSeekerEduHistory(int job_seeker_id)
        {
            return myRepository.GetListJobSeekerEduHistory(job_seeker_id);
        }

        public bool JobSeekerUpdate(IdentityJobSeekerEduHistory identity)
        {
            return myRepository.JobSeekerUpdate(identity);
        }

        public bool JobSeekerDelete(IdentityJobSeekerEduHistory identity)
        {
            return myRepository.JobSeekerDelete(identity);
        }

        #endregion

        #region Agency

        public IdentityJobSeekerEduHistory A_JobSeekerGetDetail(int id, int job_seeker_id)
        {
            return myRepository.A_JobSeekerGetDetail(id, job_seeker_id);
        }

        public List<IdentityJobSeekerEduHistory> A_GetListJobSeekerEduHistory(int job_seeker_id)
        {
            return myRepository.A_GetListJobSeekerEduHistory(job_seeker_id);
        }

        public bool A_JobSeekerUpdate(IdentityJobSeekerEduHistory identity)
        {
            return myRepository.A_JobSeekerUpdate(identity);
        }

        public bool A_JobSeekerDelete(IdentityJobSeekerEduHistory identity)
        {
            return myRepository.A_JobSeekerDelete(identity);
        }

        #endregion

        #region Cv

        public IdentityCvEduHistory CvGetDetail(int id, int cv_id)
        {
            return myRepository.CvGetDetail(id, cv_id);
        }

        public List<IdentityCvEduHistory> GetListCvEduHistory(int cv_id)
        {
            return myRepository.GetListCvEduHistory(cv_id);
        }

        public int CvUpdate(IdentityCvEduHistory identity)
        {
            return myRepository.CvUpdate(identity);
        }

        public bool CvDelete(IdentityCvEduHistory identity)
        {
            return myRepository.CvDelete(identity);
        }

        #endregion

        #region Cs

        public IdentityCsEduHistory CsGetDetail(int id, int cs_id_id)
        {
            return myRepository.CsGetDetail(id, cs_id_id);
        }

        public List<IdentityCsEduHistory> GetListCsEduHistory(int cs_id_id)
        {
            return myRepository.GetListCsEduHistory(cs_id_id);
        }

        public int CsUpdate(IdentityCsEduHistory identity)
        {
            return myRepository.CsUpdate(identity);
        }

        public bool CsDelete(IdentityCsEduHistory identity)
        {
            return myRepository.CsDelete(identity);
        }

        #endregion

        #endregion
    }
}
