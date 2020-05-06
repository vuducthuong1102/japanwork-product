using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreInvitation
    {
        List<IdentityInvitation> GetByPage(IdentityInvitation filter, int currentPage, int pageSize);
        int Insert(IdentityInvitation identity);
        bool Update(IdentityInvitation identity);
        IdentityInvitation GetById(int id);
        bool Delete(int id);

        //Actions
        int Cancel(dynamic applyInfo);
        int Ignore(dynamic applyInfo);
        int Accept(dynamic applyInfo);
        int Interview(int id);

        List<IdentityInvitation> GetList();
        IdentityInvitation CheckJobApplied(int job_id, int job_seeker_id);
        //List<IdentityInvitation> GetListByJobSeeker(int job_seeker_id);
        
        List<IdentityInvitation> GetListByJob(IdentityInvitation filter, int currentPage, int pageSize);

        List<IdentityInvitation> GetListByAgency(dynamic filter, int currentPage, int pageSize);

        List<IdentityInvitation> GetReceivers(dynamic filter, int currentPage, int pageSize);

        int Invite(IdentityInvitation identity);
        int InviteMultiJobs(IdentityInvitation identity);
        dynamic InvitationChecking(dynamic filter);

        #region JobSeeker

        List<IdentityInvitation> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize);

        #endregion
    }

    public class StoreInvitation : IStoreInvitation
    {
        private readonly string _connectionString;
        private RpsInvitation myRepository;

        public StoreInvitation() : this("JobMarketDB")
        {

        }

        public StoreInvitation(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsInvitation(_connectionString);
        }

        #region  Common

        public List<IdentityInvitation> GetByPage(IdentityInvitation filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityInvitation identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityInvitation identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityInvitation GetById(int id)
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

        public int Cancel(dynamic applyInfo)
        {
            return myRepository.Cancel(applyInfo);
        }

        public int Interview(int id)
        {
            return myRepository.Interview(id);
        }

        public List<IdentityInvitation> GetList()
        {
            return myRepository.GetList();
        }

        public IdentityInvitation CheckJobApplied(int job_id, int job_seeker_id)
        {
            return myRepository.CheckJobApplied(job_id, job_seeker_id);
        }

        //public List<IdentityInvitation> GetListByJobSeeker(int job_seeker_id)
        //{
        //    return myRepository.GetListByJobSeeker(job_seeker_id);
        //}

        public List<IdentityInvitation> GetListByJob(IdentityInvitation filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJob(filter, currentPage, pageSize);
        }

        public List<IdentityInvitation> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByAgency(filter, currentPage, pageSize);
        }

        public List<IdentityInvitation> GetReceivers(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetReceivers(filter, currentPage, pageSize);
        }

        public int Invite(IdentityInvitation identity)
        {
            return myRepository.Invite(identity);
        }

        public int InviteMultiJobs(IdentityInvitation identity)
        {
            return myRepository.InviteMultiJobs(identity);
        }

        public dynamic InvitationChecking(dynamic filter)
        {
            return myRepository.InvitationChecking(filter);
        }

        //public List<IdentityInvitation> GetListByJob(int job_id)
        //{
        //    return myRepository.GetListByJob(job_id);
        //}
        #endregion

        #region JobSeeker

        public List<IdentityInvitation> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeeker(filter, currentPage, pageSize);
        }

        #endregion
    }
}
