//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Repositories;
//using System.Collections.Generic;
//using System.Configuration;

//namespace ApiJobMarket.DB.Sql.Stores
//{
//    public class StoreConversationReply : IStoreConversationReply
//    {
//        private readonly string _connectionString;
//        private RpsConversationReply myRepository;

//        public StoreConversationReply()
//            : this("JobMarketDB")
//        {

//        }

//        public StoreConversationReply(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            myRepository = new RpsConversationReply(_connectionString);
//        }

//        public int Insert(IdentityConversationReply identity)
//        {
//            return myRepository.Insert(identity);
//        }

//        public List<IdentityConversationReply> GetFirstByListId(List<int> listConversations)
//        {
//            return myRepository.GetFirstByListId(listConversations);
//        }

//        public List<IdentityConversationReply> GetByPage(dynamic filter)
//        {
//            return myRepository.GetByPage(filter);
//        }

//        public int Delete(IdentityConversationReply identity)
//        {
//            return myRepository.Delete(identity);
//        }
//    }
//    public interface IStoreConversationReply
//    {
//        int Insert(IdentityConversationReply identity);
//        List<IdentityConversationReply> GetByPage(dynamic filter);
//        int Delete(IdentityConversationReply identity);
//        List<IdentityConversationReply> GetFirstByListId(List<int> listConversations);
//    }
//}
