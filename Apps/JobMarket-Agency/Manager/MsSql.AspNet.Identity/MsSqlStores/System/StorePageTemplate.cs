using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStorePageTemplate
    {
        List<IdentityPageTemplate> GetByPage(IdentityPageTemplate filter, int currentPage, int pageSize);
        int Insert(IdentityPageTemplate identity);

        int RegisterNewPageTemplate(IdentityPageTemplate identity, out bool isNew);
        bool Update(IdentityPageTemplate identity);
        IdentityPageTemplate GetById(int id);
        bool Delete(int id);
        List<IdentityPageTemplate> GetList();
    }

    public class StorePageTemplate : IStorePageTemplate
    {
        private readonly string _connectionString;
        private RpsPageTemplate r;

        public StorePageTemplate(): this("DefaultConnection")
        {

        }

        public StorePageTemplate(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPageTemplate(_connectionString);
        }

        #region  Common

        public List<IdentityPageTemplate> GetByPage(IdentityPageTemplate filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPageTemplate identity)
        {
            return r.Insert(identity);
        }

        public int RegisterNewPageTemplate(IdentityPageTemplate identity, out bool isNew)
        {
            return r.RegisterNewPage(identity, out isNew);
        }

        public bool Update(IdentityPageTemplate identity)
        {
            return r.Update(identity);
        }

        public IdentityPageTemplate GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityPageTemplate> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
