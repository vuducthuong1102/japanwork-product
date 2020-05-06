using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreCategory : IStoreCategory
    {
        private readonly string _connectionString;
        private RpsCategory r;

        public StoreCategory(): this("PfoodDBConnection")
        {

        }

        public StoreCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsCategory(_connectionString);
        }

        #region  Common

        public List<IdentityCategory> GetAll(IdentityCategory filter, int currentPage, int pageSize)
        {
            return r.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCategory identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityCategory identity)
        {
            return r.Update(identity);
        }

        public IdentityCategory GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityCategory> GetList()
        {
            return r.GetList();
        }

        public IdentityCategory GetDetail(int id)
        {
            return r.GetDetail(id);
        }

        public IdentityCategoryLang GetLangDetail(int id)
        {
            return r.GetLangDetail(id);
        }

        public int AddNewLang(IdentityCategoryLang identity)
        {
            return r.AddNewLang(identity);
        }

        public int UpdateLang(IdentityCategoryLang identity)
        {
            return r.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return r.DeleteLang(id);
        }

        #endregion
    }

    public interface IStoreCategory
    {
        List<IdentityCategory> GetAll(IdentityCategory filter, int currentPage, int pageSize);
        int Insert(IdentityCategory identity);
        bool Update(IdentityCategory identity);
        IdentityCategory GetById(int id);
        bool Delete(int id);
        List<IdentityCategory> GetList();
        IdentityCategory GetDetail(int id);

        int AddNewLang(IdentityCategoryLang identity);
        int UpdateLang(IdentityCategoryLang identity);
        IdentityCategoryLang GetLangDetail(int id);
        bool DeleteLang(int id);
    }
}
