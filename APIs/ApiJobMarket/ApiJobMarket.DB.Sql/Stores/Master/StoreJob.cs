using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreJob
    {
        List<IdentityJob> GetByPage(IdentityJob filter, int currentPage, int pageSize);
        List<IdentityJob> SearchByPage(dynamic filter, int currentPage, int pageSize);
        List<IdentityJob> GetRecent(dynamic filter, int currentPage, int pageSize);
        List<IdentityJob> GetListAssignWorkByCompanyId(dynamic filter);
        int Insert(IdentityJob identity);
        int Update(IdentityJob identity);
        IdentityJob GetById(int id);
        bool Delete(int id, int agency_id);

        IdentityJob GetDetail(int id);

        int CheckInvite(dynamic model);

        IdentityJob GetBaseInfo(int id, string laguage_code);
        IdentityJob GetMetaData(dynamic filter);
        List<IdentityJob> GetList();
        //List<IdentityJob> GetListByCompany(int company_id);
        List<IdentityJob> GetListByCompany(dynamic filter, int currentPage, int pageSize);

        List<IdentityJob> GetListForDelete(dynamic filter, int currentPage, int pageSize);

        List<IdentityJob> GetListProcess(dynamic filter, int currentPage, int pageSize);

        List<IdentityApplication> GetListHot(int job_seeker_id, int page_size);

        //Actions
        bool Close(int id);

        bool UpdateStatus(int id, int status);
        int Apply(dynamic applyInfo);
        int Save(dynamic info);

        IdentityJob CheckJobSaved(int job_id, int job_seeker_id);
        //List<IdentityJob> GetListSavedByJobSeeker(int job_seeker_id);
        List<IdentityJob> GetListSavedByJobSeeker(IdentityJob filter, int currentPage, int pageSize);

        bool DeleteJobSaved(int job_id, int job_seeker_id);

        List<IdentityJob> M_GetListByPage(dynamic filter, int currentPage, int pageSize);

        bool UpdateTranslation(IdentityJobTranslation identity);

        IdentityJobSeekerCounter GetCounterForDeletion(int id);
    }

    public class StoreJob : IStoreJob
    {
        private readonly string _connectionString;
        private RpsJob myRepository;

        public StoreJob() : this("JobMarketDB")
        {

        }

        public StoreJob(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsJob(_connectionString);
        }

        #region  Common

        public List<IdentityJob> GetByPage(IdentityJob filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityJob> SearchByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.SearchByPage(filter, currentPage, pageSize);
        }

        public List<IdentityJob> GetRecent(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetRecent(filter, currentPage, pageSize);
        }
        public List<IdentityJob> GetListAssignWorkByCompanyId(dynamic filter)
        {
            return myRepository.GetListAssignWorkByCompanyId(filter);
        }
        public int Insert(IdentityJob identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityJob identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityJob GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityJob GetDetail(int id)
        {
            return myRepository.GetDetail(id);
        }
        public int CheckInvite(dynamic model)
        {
            return myRepository.CheckInvite(model);
        }

        public IdentityJob GetBaseInfo(int id, string language_code)
        {
            return myRepository.GetBaseInfo(id, language_code);
        }

        public IdentityJob GetMetaData(dynamic filter)
        {
            return myRepository.GetMetaData(filter);
        }

        public bool Delete(int id, int agency_id)
        {
            return myRepository.Delete(id, agency_id);
        }

        //Actions
        public bool Close(int id)
        {
            return myRepository.Close(id);
        }
        public bool UpdateStatus(int id, int status)
        {
            return myRepository.UpdateStatus(id, status);
        }
        public int Apply(dynamic applyInfo)
        {
            return myRepository.Apply(applyInfo);
        }

        public int Save(dynamic info)
        {
            return myRepository.Save(info);
        }

        public List<IdentityJob> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityApplication> GetListHot(int job_seeker_id, int page_size)
        {
            return myRepository.GetListHot(job_seeker_id, page_size);
        }
        public List<IdentityJob> GetListProcess(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListProcess(filter, currentPage, pageSize);
        }

        public List<IdentityJob> GetListByCompany(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByCompany(filter, currentPage, pageSize);
        }

        public List<IdentityJob> GetListForDelete(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListForDelete(filter, currentPage, pageSize);
        }

        public IdentityJob CheckJobSaved(int job_id, int job_seeker_id)
        {
            return myRepository.CheckJobSaved(job_id, job_seeker_id);
        }

        public List<IdentityJob> GetListSavedByJobSeeker(int job_id)
        {
            return myRepository.GetListSavedByJobSeeker(job_id);
        }

        public List<IdentityJob> GetListSavedByJobSeeker(IdentityJob filter, int currentPage, int pageSize)
        {
            return myRepository.GetListSavedByJobSeeker(filter, currentPage, pageSize);
        }

        public bool DeleteJobSaved(int job_id, int job_seeker_id)
        {
            return myRepository.DeleteJobSaved(job_id, job_seeker_id);
        }
        public List<IdentityJob> M_GetListByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.M_GetListByPage(filter, currentPage, pageSize);
        }

        public bool UpdateTranslation(IdentityJobTranslation identity)
        {
            return myRepository.UpdateTranslation(identity);
        }
        
        public IdentityJobSeekerCounter GetCounterForDeletion(int id)
        {
            return myRepository.GetCounterForDeletion(id);
        }
        #endregion
    }
}
