using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreProductCategory
    {
        List<IdentityProductCategory> GetByPage(IdentityProductCategory filter, int currentPage, int pageSize);
        int Insert(IdentityProductCategory identity);
        bool Update(IdentityProductCategory identity);
        IdentityProductCategory GetById(int id);
        bool Delete(int id);
        List<IdentityProductCategory> GetList();
    }

    public class StoreProductCategory : IStoreProductCategory
    {
        private readonly string _connectionString;
        private RpsProductCategory r;

        public StoreProductCategory(): this("DefaultConnection")
        {

        }

        public StoreProductCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsProductCategory(_connectionString);
        }

        #region  Common

        public List<IdentityProductCategory> GetByPage(IdentityProductCategory filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityProductCategory identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityProductCategory identity)
        {
            return r.Update(identity);
        }

        public IdentityProductCategory GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityProductCategory> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
