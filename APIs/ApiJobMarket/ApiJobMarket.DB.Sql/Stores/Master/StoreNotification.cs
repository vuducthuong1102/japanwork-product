using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreNotification
    {
        int Insert(IdentityNotification identity);
        bool Update(IdentityNotification identity);
        IdentityNotification GetById(int id);
        bool Delete(int id);

        int SinglePush(IdentityNotification identity);
        int MultiplePush(List<int> listIds, IdentityNotification identity);

        string MultiplePush(string job_ids, IdentityNotification identity, int job_seeker_id);


        List<IdentityNotification> GetListByJobSeeker(int job_seeker_id);
        List<IdentityNotification> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize);
        int CountUnread(dynamic filter);
    }

    public class StoreNotification : IStoreNotification
    {
        private readonly string _connectionString;
        private RpsNotification myRepository;

        public StoreNotification() : this("JobMarketDB")
        {

        }

        public StoreNotification(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsNotification(_connectionString);
        }

        #region  Common

        public int Insert(IdentityNotification identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityNotification identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityNotification GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public int SinglePush(IdentityNotification identity)
        {
            return myRepository.SinglePush(identity);
        }

        public int MultiplePush(List<int> listIds, IdentityNotification identity)
        {
            return myRepository.MultiplePush(listIds, identity);
        }
        public string MultiplePush(string job_ids, IdentityNotification identity, int job_seeker_id)
        {
            return myRepository.MultiplePush(job_ids, identity, job_seeker_id);
        }

        public List<IdentityNotification> GetListByJobSeeker(int job_seeker_id)
        {
            return myRepository.GetListByJobSeeker(job_seeker_id);
        }

        public List<IdentityNotification> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByJobSeeker(filter, currentPage, pageSize);
        }

        public int CountUnread(dynamic identity)
        {
            return myRepository.CountUnread(identity);
        }

        #endregion
    }
}
