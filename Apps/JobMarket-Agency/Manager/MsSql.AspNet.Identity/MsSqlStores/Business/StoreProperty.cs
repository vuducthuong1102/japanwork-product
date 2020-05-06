using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public class StoreProperty : IStoreProperty
    {
        private readonly string _connectionString;
        private RpsProperty r;

        public StoreProperty()
            : this("DefaultConnection")
        {

        }

        public StoreProperty(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsProperty(_connectionString);
        }

        public List<IdentityProperty> GetByPage(IdentityProperty filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityProperty identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityProperty identity)
        {
            return r.Update(identity);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityProperty GetById(int id)
        {
            return r.GetById(id);
        }

        public IdentityProperty GetDetail(int id)
        {
            return r.GetDetail(id);
        }

        public List<IdentityPropertyLang> GetLangDetail(int id)
        {
            return r.GetLangDetail(id);
        }

        public int InsertLang(IdentityPropertyLang identity)
        {
            return r.InsertLang(identity);
        }

        public int UpdateLang(IdentityPropertyLang identity)
        {
            return r.UpdateLang(identity);
        }

        public bool DeleteLang(int id)
        {
            return r.DeleteLang(id);
        }

        public List<IdentityProperty> GetAll()
        {
            return r.GetAll();
        }

        public List<IdentityProperty> GetByCategory(int propertyCategoryId)
        {
            return r.GetByCategory(propertyCategoryId);
        }
    }

    public interface IStoreProperty
    {
        List<IdentityProperty> GetByPage(IdentityProperty filter, int currentPage, int pageSize);
        List<IdentityProperty> GetByCategory(int propertyCategoryId);

        int Insert(IdentityProperty identity);
        bool Update(IdentityProperty identity);
        bool Delete(int id);

        IdentityProperty GetById(int id);
        IdentityProperty GetDetail(int id);

        List<IdentityPropertyLang> GetLangDetail(int id);
        int InsertLang(IdentityPropertyLang identity);
        int UpdateLang(IdentityPropertyLang identity);
        bool DeleteLang(int id);
        List<IdentityProperty> GetAll();
    }
}
