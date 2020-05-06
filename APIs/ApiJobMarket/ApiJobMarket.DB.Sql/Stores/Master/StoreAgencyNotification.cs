using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreAgencyNotification
    {
        IdentityAgencyNotification GetById(int id);
        bool Delete(int id);
        int MarkIsRead(IdentityAgencyNotification identity);

        int SinglePush(IdentityAgencyNotification identity);
        int MultiplePush(IdentityAgencyNotification identity, List<int> listIds);

        List<IdentityAgencyNotification> GetListByStaff(dynamic filter, int currentPage, int pageSize);
        int CountUnread(dynamic filter);
    }

    public class StoreAgencyNotification : IStoreAgencyNotification
    {
        private readonly string _connectionString;
        private RpsAgencyNotification myRepository;

        public StoreAgencyNotification() : this("JobMarketDB")
        {

        }

        public StoreAgencyNotification(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsAgencyNotification(_connectionString);
        }

        #region  Common

        public IdentityAgencyNotification GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public int SinglePush(IdentityAgencyNotification identity)
        {
            return myRepository.SinglePush(identity);
        }

        public int MultiplePush(IdentityAgencyNotification identity, List<int> listIds)
        {
            return myRepository.MultiplePush(identity, listIds);
        }

        public List<IdentityAgencyNotification> GetListByStaff(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetListByStaff(filter, currentPage, pageSize);
        }

        public int CountUnread(dynamic identity)
        {
            return myRepository.CountUnread(identity);
        }

        public int MarkIsRead(IdentityAgencyNotification identity)
        {
            return myRepository.MarkIsRead(identity);
        }

        #endregion
    }
}
