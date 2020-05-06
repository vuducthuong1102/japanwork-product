using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreLanguage
    {
        List<IdentityLanguage> GetAll(IdentityLanguage filter, int currentPage, int pageSize);
        int Insert(IdentityLanguage identity);
        bool Update(IdentityLanguage identity);
        IdentityLanguage GetById(int id);
        bool Delete(int id);
        List<IdentityLanguage> GetList();
    }

    public class StoreLanguage : IStoreLanguage
    {
        private readonly string _connectionString;
        private RpsLanguage r;

        public StoreLanguage(): this("PfoodDBConnection")
        {

        }

        public StoreLanguage(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsLanguage(_connectionString);
        }

        #region  Common

        public List<IdentityLanguage> GetAll(IdentityLanguage filter, int currentPage, int pageSize)
        {
            return r.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityLanguage identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityLanguage identity)
        {
            return r.Update(identity);
        }

        public IdentityLanguage GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityLanguage> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
