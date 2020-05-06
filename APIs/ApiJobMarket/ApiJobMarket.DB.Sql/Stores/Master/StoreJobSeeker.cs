using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreJobSeeker
    {
        List<IdentityJobSeeker> GetByPage(IdentityJobSeeker filter, int currentPage, int pageSize);
        int Insert(IdentityJobSeeker identity);
        bool Update(IdentityJobSeeker identity);
        IdentityJobSeeker GetById(int id);
        IdentityJobSeeker GetByEmail(string email);
        bool Delete(int id);
        List<IdentityJobSeeker> GetList();

        bool UpdateProfile(IdentityJobSeeker identity);
        bool UpdateConfig(IdentityJobSeekerConfig identity);
        bool UpdateVideoProfile(IdentityJobSeeker identity);

        IdentityJobSeeker GetBaseInfo(int id, int agency_id = 0);
        IdentityJobSeekerConfig GetConfig(int job_seeker_id);
        IdentityJobSeeker GetDetailForUpdate(int id);
        int SaveTokenFireBase(IdentityTokenFireBase info);
        int MarkIsReadNotification(IdentityNotification info);
        List<int> SetMainCv(IdentityCv info);

        List<IdentityJobSeeker> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize);
        IdentityJobSeekerCounter GetCounterForDeletion(int job_seeker_id, int agency_id);

        #region Agency

        List<IdentityJobSeeker> A_GetListByPage(dynamic filter, int currentPage, int pageSize);
        List<IdentityJobSeeker> A_GetListAssignmentWorkByPage(dynamic filter, int currentPage, int pageSize);
        int A_UpdateProfile(IdentityJobSeeker identity);
        int A_Delete(IdentityJobSeeker identity);

        int A_Deletes(string ids, int type);
        IdentityJobSeeker A_GetBaseInfo(int id, int agency_id);
        List<IdentityJobSeeker> GetByAgency(IdentityJobSeeker filter, int currentPage, int pageSize);

        IdentityJobSeekerCounter A_GetCounterForDeletion(int job_seeker_id, int agency_id);
        #endregion

        #region Admin
        List<IdentityJobSeeker> M_GetByPage(dynamic filter, int currentPage, int pageSize);
        #endregion
    }

    public class StoreJobSeeker : IStoreJobSeeker
    {
        private readonly string _connectionString;
        private RpsJobSeeker myRepository;

        public StoreJobSeeker() : this("JobMarketDB")
        {

        }

        public StoreJobSeeker(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsJobSeeker(_connectionString);
        }

        #region  Common

        public List<IdentityJobSeeker> GetByPage(IdentityJobSeeker filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityJobSeeker identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityJobSeeker identity)
        {
            return myRepository.Update(identity);
        }

        public bool UpdateConfig(IdentityJobSeekerConfig identity)
        {
            return myRepository.UpdateConfig(identity);
        }

        public bool UpdateVideoProfile(IdentityJobSeeker identity)
        {
            return myRepository.UpdateVideoProfile(identity);
        }

        public IdentityJobSeeker GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public IdentityJobSeekerConfig GetConfig(int job_seeker_id)
        {
            return myRepository.GetConfig(job_seeker_id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityJobSeeker> GetList()
        {
            return myRepository.GetList();
        }

        public bool UpdateProfile(IdentityJobSeeker identity)
        {
            return myRepository.UpdateProfile(identity);
        }

        public IdentityJobSeeker GetBaseInfo(int id, int agency_id = 0)
        {
            return myRepository.GetBaseInfo(id, agency_id);
        }

        public IdentityJobSeeker GetDetailForUpdate(int id)
        {
            return myRepository.GetDetailForUpdate(id);
        }

        public IdentityJobSeeker GetByEmail(string email)
        {
            return myRepository.GetByEmail(email);
        }

        public int SaveTokenFireBase(IdentityTokenFireBase info)
        {
            return myRepository.SaveTokenFireBase(info);
        }

        public int MarkIsReadNotification(IdentityNotification info)
        {
            return myRepository.MarkIsReadNotification(info);
        }

        public List<int> SetMainCv(IdentityCv info)
        {
            return myRepository.SetMainCv(info);
        }

        public List<IdentityJobSeeker> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsForInvitationByPage(filter, currentPage, pageSize);
        }

        public IdentityJobSeekerCounter GetCounterForDeletion(int job_seeker_id, int agency_id)
        {
          return myRepository.GetCounterForDeletion(job_seeker_id, agency_id);
        }
        #region Agency

        public IdentityJobSeeker A_GetBaseInfo(int id, int agency_id)
        {
            return myRepository.A_GetBaseInfo(id, agency_id);
        }

        public List<IdentityJobSeeker> A_GetListByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.A_GetListByPage(filter, currentPage, pageSize);
        }
        public List<IdentityJobSeeker> A_GetListAssignmentWorkByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.A_GetListAssignmentWorkByPage(filter, currentPage, pageSize);
        }

        public int A_UpdateProfile(IdentityJobSeeker identity)
        {
            return myRepository.A_UpdateProfile(identity);
        }

        public int A_Delete(IdentityJobSeeker identity)
        {
            return myRepository.A_Delete(identity);
        }

        public int A_Deletes(string Ids, int type)
        {
            return myRepository.A_Deletes(Ids, type);
        }

        public List<IdentityJobSeeker> GetByAgency(IdentityJobSeeker filter, int currentPage, int pageSize)
        {
            return myRepository.GetByAgency(filter, currentPage, pageSize);
        }

       public IdentityJobSeekerCounter A_GetCounterForDeletion(int job_seeker_id, int agency_id)
        {
            return myRepository.A_GetCounterForDeletion(job_seeker_id, agency_id);
        }
        #endregion

        #region Admin

        public List<IdentityJobSeeker> M_GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.M_GetByPage(filter, currentPage, pageSize);
        }

        #endregion

        #endregion
    }
}
