using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using ApiJobMarket.DB.Sql.Entities;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreEmailBusiness
    {
        List<IdentityEmailBusiness> GetByPage(dynamic filter);
        int Insert(IdentityEmailBusiness identity);
        int BatchInsert(IdentityEmailBusiness identity);
        int Delete(IdentityEmailBusiness identity);
        IdentityEmailBusiness GetDetailById(int id);
    }

    public class StoreEmailBusiness : IStoreEmailBusiness
    {
        private readonly string _connectionString;
        private RpsEmailBusiness r;

        public StoreEmailBusiness()
            : this("DefaultConnection")
        {

        }

        public StoreEmailBusiness(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsEmailBusiness(_connectionString);
        }

        #region --- Common ---

        public List<IdentityEmailBusiness> GetByPage(dynamic filter)
        {
            return r.GetByPage(filter);
        }
        
        public int Insert(IdentityEmailBusiness identity)
        {
            return r.Insert(identity);
        }

        public int BatchInsert(IdentityEmailBusiness identity)
        {
            return r.BatchInsert(identity);
        }

        public int Delete(IdentityEmailBusiness identity)
        {
            return r.Delete(identity);
        }

        public IdentityEmailBusiness GetDetailById(int id)
        {
            return r.GetDetailById(id);
        }

        #endregion
    }   
}
