//using System.Configuration;
//using ApiJobMarket.DB.Sql.Repositories;
//using ApiJobMarket.DB.Sql.Entities;
//using System.Collections.Generic;

//namespace ApiJobMarket.DB.Sql.Stores
//{
//    public class StoreNotification : IStoreNotification
//    {
//        private readonly string _connectionString;
//        private RpsNotification myRepository;

//        public StoreNotification()
//            : this("JobMarketDB")
//        {

//        }

//        public StoreNotification(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            myRepository = new RpsNotification(_connectionString);
//        }

//        #region --- Notification ---
//        public List<IdentityNotification> GetByPage(IdentityFilter identity)
//        {
//            return myRepository.GetByPage(identity);
//        }

//        public int Insert(IdentityNotification identity)
//        {
//            return myRepository.Insert(identity);
//        }

//        public int Update(IdentityNotification identity)
//        {
//            return myRepository.Update(identity);
//        }

//        public IdentityNotification View(IdentityNotification identity)
//        {
//            return myRepository.View(identity);
//        }

//        #endregion
//    }

//    public interface IStoreNotification
//    {
//        List<IdentityNotification> GetByPage(IdentityFilter identity);       
//        int Insert(IdentityNotification identity);
//        int Update(IdentityNotification identity);
//        IdentityNotification View(IdentityNotification identity);
//    }
//}
