using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreHTDefaultSetting
    {
        List<IdentityHTDefaultSetting> GetByPage(IdentityHTDefaultSetting filter, int currentPage, int pageSize);
        int Insert(IdentityHTDefaultSetting identity);
        int Update(IdentityHTDefaultSetting identity);
        IdentityHTDefaultSetting GetById(int id);
        bool Delete(int id);
        List<IdentityHTDefaultSetting> GetList();
    }

    public class StoreHTDefaultSetting : IStoreHTDefaultSetting
    {
        private readonly string _connectionString;
        private RpsHTDefaultSetting r;

        public StoreHTDefaultSetting(): this("DefaultConnection")
        {

        }

        public StoreHTDefaultSetting(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsHTDefaultSetting(_connectionString);
        }

        #region  Common

        public List<IdentityHTDefaultSetting> GetByPage(IdentityHTDefaultSetting filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityHTDefaultSetting identity)
        {
            return r.Insert(identity);
        }

        public int Update(IdentityHTDefaultSetting identity)
        {
            return r.Update(identity);
        }

        public IdentityHTDefaultSetting GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityHTDefaultSetting> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
