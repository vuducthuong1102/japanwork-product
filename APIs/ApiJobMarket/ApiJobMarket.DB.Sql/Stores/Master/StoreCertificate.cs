using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreCertificate
    {
        List<IdentityCertificate> GetByPage(IdentityCertificate filter, int currentPage, int pageSize);
        int Insert(IdentityCertificate identity);
        bool Update(IdentityCertificate identity);
        IdentityCertificate GetById(int id);
        bool Delete(int id);
        List<IdentityCertificate> GetList();

        #region Job seeker

        IdentityJobSeekerCertificate JobSeekerGetDetail(int id, int job_seeker_id);
        List<IdentityJobSeekerCertificate> GetListJobSeekerCertificate(int job_seeker_id);
        bool JobSeekerUpdate(IdentityJobSeekerCertificate identity);
        bool JobSeekerDelete(IdentityJobSeekerCertificate identity);

        #endregion

        #region Agency

        IdentityJobSeekerCertificate A_JobSeekerGetDetail(int id, int job_seeker_id);
        List<IdentityJobSeekerCertificate> A_GetListJobSeekerCertificate(int job_seeker_id);
        bool A_JobSeekerUpdate(IdentityJobSeekerCertificate identity);
        bool A_JobSeekerDelete(IdentityJobSeekerCertificate identity);

        #endregion

        #region Cv

        IdentityCvCertificate CvGetDetail(int id, int job_seeker_id);
        List<IdentityCvCertificate> GetListCvCertificate(int job_seeker_id);
        int CvUpdate(IdentityCvCertificate identity);
        bool CvDelete(IdentityCvCertificate identity);

        #endregion

        #region Cs

        IdentityCsCertificate CsGetDetail(int id, int job_seeker_id);
        List<IdentityCsCertificate> GetListCsCertificate(int job_seeker_id);
        int CsUpdate(IdentityCsCertificate identity);
        bool CsDelete(IdentityCsCertificate identity);

        #endregion
    }

    public class StoreCertificate : IStoreCertificate
    {
        private readonly string _connectionString;
        private RpsCertificate myRepository;

        public StoreCertificate() : this("JobMarketDB")
        {

        }

        public StoreCertificate(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsCertificate(_connectionString);
        }

        #region  Common

        public List<IdentityCertificate> GetByPage(IdentityCertificate filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCertificate identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityCertificate identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityCertificate GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityCertificate> GetList()
        {
            return myRepository.GetList();
        }

        #region Job seeker

        public IdentityJobSeekerCertificate JobSeekerGetDetail(int id, int job_seeker_id)
        {
            return myRepository.JobSeekerGetDetail(id, job_seeker_id);
        }

        public List<IdentityJobSeekerCertificate> GetListJobSeekerCertificate(int job_seeker_id)
        {
            return myRepository.GetListJobSeekerCertificate(job_seeker_id);
        }

        public bool JobSeekerUpdate(IdentityJobSeekerCertificate identity)
        {
            return myRepository.JobSeekerUpdate(identity);
        }

        public bool JobSeekerDelete(IdentityJobSeekerCertificate identity)
        {
            return myRepository.JobSeekerDelete(identity);
        }

        #endregion

        #region Agency

        public IdentityJobSeekerCertificate A_JobSeekerGetDetail(int id, int job_seeker_id)
        {
            return myRepository.A_JobSeekerGetDetail(id, job_seeker_id);
        }

        public List<IdentityJobSeekerCertificate> A_GetListJobSeekerCertificate(int job_seeker_id)
        {
            return myRepository.A_GetListJobSeekerCertificate(job_seeker_id);
        }

        public bool A_JobSeekerUpdate(IdentityJobSeekerCertificate identity)
        {
            return myRepository.A_JobSeekerUpdate(identity);
        }

        public bool A_JobSeekerDelete(IdentityJobSeekerCertificate identity)
        {
            return myRepository.A_JobSeekerDelete(identity);
        }

        #endregion

        #region Cv

        public IdentityCvCertificate CvGetDetail(int id, int cv_id)
        {
            return myRepository.CvGetDetail(id, cv_id);
        }

        public List<IdentityCvCertificate> GetListCvCertificate(int cv_id)
        {
            return myRepository.GetListCvCertificate(cv_id);
        }

        public int CvUpdate(IdentityCvCertificate identity)
        {
            return myRepository.CvUpdate(identity);
        }

        public bool CvDelete(IdentityCvCertificate identity)
        {
            return myRepository.CvDelete(identity);
        }

        #endregion

        #region Cs

        public IdentityCsCertificate CsGetDetail(int id, int cs_id)
        {
            return myRepository.CsGetDetail(id, cs_id);
        }

        public List<IdentityCsCertificate> GetListCsCertificate(int cs_id)
        {
            return myRepository.GetListCsCertificate(cs_id);
        }

        public int CsUpdate(IdentityCsCertificate identity)
        {
            return myRepository.CsUpdate(identity);
        }

        public bool CsDelete(IdentityCsCertificate identity)
        {
            return myRepository.CsDelete(identity);
        }

        #endregion

        #endregion
    }
}
