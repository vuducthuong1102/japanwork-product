using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreEmailServer
    {
        List<IdentityEmailServer> GetByPage(dynamic filter);

        List<IdentityEmailServer> GetListByAgency(int agencyId);

        IdentityEmailServer GetById(int id);

        int Insert(IdentityEmailServer identity);

        int Update(IdentityEmailServer identity);

        int Delete(IdentityEmailServer identity);

        IdentityEmailServer GetDetailById(int id);

        List<IdentityEmailSetting> GetEmailSettingsByStaff(int agencyId, int staffId);

        bool UpdateEmailSettings(List<IdentityEmailSetting> settings);
    }

    public class StoreEmailServer : IStoreEmailServer
    {
        private readonly string _connectionString;
        private RpsEmailServer r;

        public StoreEmailServer()
            : this("DefaultConnection")
        {

        }

        public StoreEmailServer(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsEmailServer(_connectionString);
        }

        #region --- EmailServer ---
        public List<IdentityEmailServer> GetByPage(dynamic filter)
        {
            return r.GetByPage(filter);
        }

        public List<IdentityEmailServer> GetListByAgency(int agencyId)
        {
            return r.GetListByAgency(agencyId);
        }

        public IdentityEmailServer GetById(int id)
        {
            return r.GetById(id);
        }

        public int Insert(IdentityEmailServer identity)
        {
            return r.Insert(identity);
        }

        public int Update(IdentityEmailServer identity)
        {
            return r.Update(identity);
        }

        public int Delete(IdentityEmailServer identity)
        {
            return r.Delete(identity);
        }

        public IdentityEmailServer GetDetailById(int id)
        {
            return r.GetDetailById(id);
        }

        public List<IdentityEmailSetting> GetEmailSettingsByStaff(int agencyId, int staffId)
        {
            return r.GetEmailSettingsByStaff(agencyId, staffId);
        }

        public bool UpdateEmailSettings(List<IdentityEmailSetting> settings)
        {
            return r.UpdateEmailSettings(settings);
        }

        #endregion
    }   
}
