//using MsSql.AspNet.Identity.Entities;
//using MsSql.AspNet.Identity.Repositories;
//using System.Collections.Generic;
//using System.Configuration;

//namespace MsSql.AspNet.Identity.MsSqlStores
//{
//    public class StoreConversation : IStoreConversation
//    {
//        private readonly string _connectionString;
//        private RpsConversation r;

//        public StoreConversation()
//            : this("MessengerConnection")
//        {

//        }

//        public StoreConversation(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            r = new RpsConversation(_connectionString);
//        }

//        public List<IdentityConversation> GetByPage(dynamic filter)
//        {
//            return r.GetByPage(filter);
//        }

//        public int GetCurrentConversation(IdentityConversation identity)
//        {
//            return r.GetCurrentConversation(identity);
//        }

//        public int JoinToConversation(IdentityConversationObject from, IdentityConversationObject to)
//        {
//            return r.JoinToConversation(from, to);
//        }

//        public int Delete(IdentityConversation identity)
//        {
//            return r.Delete(identity);
//        }
//    }
//    public interface IStoreConversation
//    {
//        List<IdentityConversation> GetByPage(dynamic filter);
//        int GetCurrentConversation(IdentityConversation identity);
//        int JoinToConversation(IdentityConversationObject from, IdentityConversationObject to);
//        int Delete(IdentityConversation identity);
//    }
//}
