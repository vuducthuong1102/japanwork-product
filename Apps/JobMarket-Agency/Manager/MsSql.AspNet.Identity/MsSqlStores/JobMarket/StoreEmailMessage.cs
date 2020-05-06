using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreEmailMessage
    {
        List<IdentityEmailMessage> GetByPage(dynamic filter);
        List<string> GetSynchronizedIds(int agencyId, int staffId);
        List<IdentityEmailMessage> GetMessageParts(dynamic filter);
        int Insert(IdentityEmailMessage identity);
        int InsertPart(IdentityEmailMessage identity);
        int Update(IdentityEmailMessage identity);
        int Delete(int id);
        IdentityEmailMessage GetDetailById(int id);
        IdentityEmailMessage GetPartDetailById(int id);
    }

    public class StoreEmailMessage : IStoreEmailMessage
    {
        private readonly string _connectionString;
        private RpsEmailMessage r;

        public StoreEmailMessage()
            : this("DefaultConnection")
        {

        }

        public StoreEmailMessage(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsEmailMessage(_connectionString);
        }

        #region --- Common ---

        public List<IdentityEmailMessage> GetByPage(dynamic filter)
        {
            return r.GetByPage(filter);
        }

        public List<string> GetSynchronizedIds(int agencyId, int staffId)
        {
            return r.GetSynchronizedIds(agencyId, staffId);
        }

        public List<IdentityEmailMessage> GetMessageParts(dynamic filter)
        {
            return r.GetMessageParts(filter);
        }

        public int Insert(IdentityEmailMessage identity)
        {
            return r.Insert(identity);
        }

        public int InsertPart(IdentityEmailMessage identity)
        {
            return r.InsertPart(identity);
        }

        public int Update(IdentityEmailMessage identity)
        {
            return r.Update(identity);
        }

        public int Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityEmailMessage GetDetailById(int id)
        {
            return r.GetDetailById(id);
        }

        public IdentityEmailMessage GetPartDetailById(int id)
        {
            return r.GetPartDetailById(id);
        }

        #endregion
    }   
}
