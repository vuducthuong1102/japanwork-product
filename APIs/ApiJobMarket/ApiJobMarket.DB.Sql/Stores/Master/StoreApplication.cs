using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreApplication
    {
        List<IdentityApplication> GetByPage(IdentityApplication filter, int currentPage, int pageSize);
        int Insert(IdentityApplication identity);
        bool Update(IdentityApplication identity);
        IdentityApplication GetById(int id);
        bool Delete(int id);

        //Actions
        int Cancel(dynamic applyInfo);
        int Ignore(dynamic applyInfo);
        int Accept(dynamic applyInfo);

        int SendCv(dynamic applyInfo);
        int Interview(int id);

        int Deletes(string ids,int agency_id, int staff_id);

        List<IdentityApplication> GetList();
        IdentityApplication CheckJobApplied(int job_id, int job_seeker_id);
        //List<IdentityApplication> GetListByJobSeeker(int job_seeker_id);

        List<IdentityApplication> GetListByJobSeeker(IdentityApplication filter, int currentPage, int pageSize);
        List<IdentityApplication> GetListByJob(IdentityApplication filter, int currentPage, int pageSize);

        List<IdentityApplication> GetListByAgency(dynamic filter, int currentPage, int pageSize);

        List<IdentityApplication> GetListByPage(dynamic filter, int currentPage, int pageSize);


        List<IdentityApplication> GetListInvited(dynamic filter, int currentPage, int pageSize);

        List<IdentityApplication> GetListOffline(dynamic filter, int currentPage, int pageSize);

        List<IdentityApplication> GetListRecruited(dynamic filter, int currentPage, int pageSize);

        //List<IdentityApplication> GetListByJob(int job_id);

        bool UpdatePic(int job_seeker_id, int agency_id, int pic_id);
    }

    public class StoreApplication : IStoreApplication
    {
        private readonly string _connectionString;
        private RpsApplication myRepository;

        public StoreApplication() : this("JobMarketDB")
        {

        }

        public StoreApplication(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsApplication(_connectionString);
        }

        #region  Common

        public List<IdentityApplication> GetByPage(IdentityApplication filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityApplication identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityApplication identity)
        {
            return myRepository.Update(identity);
        }
        public int Deletes(string ids, int agency_id, int staff_id)
        {
            return myRepository.Deletes(ids, agency_id, staff_id);
        }

        public IdentityApplication GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        //Actions
        public int Ignore(dynamic applyInfo)
        {
            return myRepository.Ignore(applyInfo);
        }

        public int Accept(dynamic applyInfo)
        {
            return myRepository.Accept(applyInfo);
        }
        public int SendCv(dynamic applyInfo)
        {
            return myRepository.SendCv(applyInfo);
        }

        public int Cancel(dynamic applyInfo)
        {
            return myRepository.Cancel(applyInfo);
        }

        public int Interview(int id)
        {
            return myRepository.Interview(id);
        }

        public List<IdentityApplication> GetList()
        {
            return myRepository.GetList();
        }

        public IdentityApplication CheckJobApplied(int job_id, int job_seeker_id)
        {
            return myRepository.CheckJobApplied(job_id, job_seeker_id);
        }

        //public List<IdentityApplication> GetListByJobSeeker(int job_seeker_id)
        //{
        //    return myRepository.GetListByJobSeeker(job_seeker_id);
        //}

        public List<IdentityApplication> GetListByJobSeeker(IdentityApplication filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeeker(filter, currentPage, pageSize);
        }

        public List<IdentityApplication> GetListByJob(IdentityApplication filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJob(filter, currentPage, pageSize);
        }

        public List<IdentityApplication> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByAgency(filter, currentPage, pageSize);
        }

        public List<IdentityApplication> GetListByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByPage(filter, currentPage, pageSize);
        }

        public List<IdentityApplication> GetListInvited(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListInvited(filter, currentPage, pageSize);
        }
        public List<IdentityApplication> GetListOffline(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListOffline(filter, currentPage, pageSize);
        }

        public List<IdentityApplication> GetListRecruited(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListRecruited(filter, currentPage, pageSize);
        }
        //public List<IdentityApplication> GetListByJob(int job_id)
        //{
        //    return myRepository.GetListByJob(job_id);
        //}
        public bool UpdatePic(int job_seeker_id, int agency_id, int pic_id)
        {
            return myRepository.UpdatePic(job_seeker_id, agency_id, pic_id);
        }
        #endregion
    }
}
