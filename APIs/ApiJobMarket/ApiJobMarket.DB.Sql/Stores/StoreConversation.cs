//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Repositories;
//using System.Collections.Generic;
//using System.Configuration;

//namespace ApiJobMarket.DB.Sql.Stores
//{
//    public class StoreConversation : IStoreConversation
//    {
//        private readonly string _connectionString;
//        private RpsConversation myRepository;

//        public StoreConversation()
//            : this("JobMarketDB")
//        {

//        }

//        public StoreConversation(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            myRepository = new RpsConversation(_connectionString);
//        }

//        public List<IdentityConversation> GetByPage(dynamic filter)
//        {
//            return myRepository.GetByPage(filter);
//        }

//        public int GetCurrentConversation(IdentityConversation identity)
//        {
//            return myRepository.GetCurrentConversation(identity);
//        }

//        public int Delete(IdentityConversation identity)
//        {
//            return myRepository.Delete(identity);
//        }
//    }
//    public interface IStoreConversation
//    {
//        List<IdentityConversation> GetByPage(dynamic filter);
//        int GetCurrentConversation(IdentityConversation identity);
//        int Delete(IdentityConversation identity);
//    }
//}
