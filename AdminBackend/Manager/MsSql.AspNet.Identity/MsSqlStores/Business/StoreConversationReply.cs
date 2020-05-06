using System.Collections.Generic;
using System.Configuration;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreConversationReply : IStoreConversationReply
    {
        private readonly string _connectionString;
        private RpsConversationReply r;

        public StoreConversationReply()
            : this("MessengerConnection")
        {

        }

        public StoreConversationReply(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsConversationReply(_connectionString);
        }

        public int Insert(IdentityConversationReply identity)
        {
            return r.Insert(identity);
        }

        public List<IdentityConversationReply> GetFirstByListId(List<int> listConversations)
        {
            return r.GetFirstByListId(listConversations);
        }

        public List<IdentityConversationReply> GetByPage(dynamic filter)
        {
            return r.GetByPage(filter);
        }

        public IdentityConversationReply GetDetailByConversation(int converSationId, int id)
        {
            return r.GetDetailByConversation(converSationId, id);
        }

        public int Delete(IdentityConversationReply identity)
        {
            return r.Delete(identity);
        }
    }
    public interface IStoreConversationReply
    {
        int Insert(IdentityConversationReply identity);
        List<IdentityConversationReply> GetByPage(dynamic filter);
        IdentityConversationReply GetDetailByConversation(int converSationId, int id);
        int Delete(IdentityConversationReply identity);
        List<IdentityConversationReply> GetFirstByListId(List<int> listConversations);
    }
}
