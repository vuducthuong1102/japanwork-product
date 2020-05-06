using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStorePropertyCategory
    {
        List<IdentityPropertyCategory> GetByPage(IdentityPropertyCategory filter, int currentPage, int pageSize);
        int Insert(IdentityPropertyCategory identity);
        bool Update(IdentityPropertyCategory identity);
        IdentityPropertyCategory GetById(int id);
        IdentityPropertyCategory GetDetail(int id);
        bool Delete(int id);
        List<IdentityPropertyCategory> GetList();
    }

    public class StorePropertyCategory : IStorePropertyCategory
    {
        private readonly string _connectionString;
        private RpsPropertyCategory r;

        public StorePropertyCategory(): this("DefaultConnection")
        {

        }

        public StorePropertyCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPropertyCategory(_connectionString);
        }

        #region  Common

        public List<IdentityPropertyCategory> GetByPage(IdentityPropertyCategory filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPropertyCategory identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityPropertyCategory identity)
        {
            return r.Update(identity);
        }

        public IdentityPropertyCategory GetById(int Id)
        {
            return r.GetById(Id);
        }

        public IdentityPropertyCategory GetDetail(int Id)
        {
            return r.GetDetail(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityPropertyCategory> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
