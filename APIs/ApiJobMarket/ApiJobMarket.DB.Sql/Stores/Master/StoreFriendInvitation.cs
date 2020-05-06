using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreFriendInvitation
    {
        List<IdentityFriendInvitation> GetByPage(IdentityFriendInvitation filter, int currentPage, int pageSize);
        int Insert(IdentityFriendInvitation identity);
        bool Update(IdentityFriendInvitation identity);
        IdentityFriendInvitation GetById(int id);
        bool Delete(int id);

        //Actions
        int Cancel(dynamic applyInfo);
        int Ignore(dynamic applyInfo);
        int Accept(dynamic applyInfo);
        int Interview(int id);

        List<IdentityFriendInvitation> GetList();
        IdentityFriendInvitation CheckJobApplied(int job_id, int job_seeker_id);
        //List<IdentityFriendInvitation> GetListByJobSeeker(int job_seeker_id);
        
        List<IdentityFriendInvitation> GetListByJob(IdentityFriendInvitation filter, int currentPage, int pageSize);

        List<IdentityFriendInvitation> GetListByAgency(dynamic filter, int currentPage, int pageSize);

        List<IdentityFriendInvitation> GetReceivers(dynamic filter, int currentPage, int pageSize);

        int Invite(IdentityFriendInvitation identity);

        dynamic FriendInvitationChecking(dynamic filter);

        #region JobSeeker

        List<IdentityFriendInvitation> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize);

        #endregion
    }

    public class StoreFriendInvitation : IStoreFriendInvitation
    {
        private readonly string _connectionString;
        private RpsFriendInvitation myRepository;

        public StoreFriendInvitation() : this("JobMarketDB")
        {

        }

        public StoreFriendInvitation(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsFriendInvitation(_connectionString);
        }

        #region  Common

        public List<IdentityFriendInvitation> GetByPage(IdentityFriendInvitation filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityFriendInvitation identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityFriendInvitation identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityFriendInvitation GetById(int id)
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

        public List<IdentityFriendInvitation> GetList()
        {
            return myRepository.GetList();
        }

        public IdentityFriendInvitation CheckJobApplied(int job_id, int job_seeker_id)
        {
            return myRepository.CheckJobApplied(job_id, job_seeker_id);
        }

        //public List<IdentityFriendInvitation> GetListByJobSeeker(int job_seeker_id)
        //{
        //    return myRepository.GetListByJobSeeker(job_seeker_id);
        //}

        public List<IdentityFriendInvitation> GetListByJob(IdentityFriendInvitation filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJob(filter, currentPage, pageSize);
        }

        public List<IdentityFriendInvitation> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByAgency(filter, currentPage, pageSize);
        }

        public List<IdentityFriendInvitation> GetReceivers(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetReceivers(filter, currentPage, pageSize);
        }

        public int Invite(IdentityFriendInvitation identity)
        {
            return myRepository.Invite(identity);
        }

        public dynamic FriendInvitationChecking(dynamic filter)
        {
            return myRepository.FriendInvitationChecking(filter);
        }

        //public List<IdentityFriendInvitation> GetListByJob(int job_id)
        //{
        //    return myRepository.GetListByJob(job_id);
        //}
        #endregion

        #region JobSeeker

        public List<IdentityFriendInvitation> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeeker(filter, currentPage, pageSize);
        }

        #endregion
    }
}
