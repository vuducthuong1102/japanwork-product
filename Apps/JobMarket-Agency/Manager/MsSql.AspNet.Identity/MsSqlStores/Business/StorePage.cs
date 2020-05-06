using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStorePage
    {
        List<IdentityPage> GetByPage(IdentityPage filter, int currentPage, int pageSize);
        int Insert(IdentityPage identity);

        int RegisterNewPageLayout(IdentityPage identity, out bool isNew);
        bool Update(IdentityPage identity);
        IdentityPage GetById(int id, string langCode);
        bool Delete(int id);
        List<IdentityPage> GetList();

        IdentityPage F_Page_GetPageByOperation(string controller, string action);
    }

    public class StorePage : IStorePage
    {
        private readonly string _connectionString;
        private RpsPage r;

        public StorePage(): this("DefaultConnection")
        {

        }

        public StorePage(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPage(_connectionString);
        }

        #region  Common

        public List<IdentityPage> GetByPage(IdentityPage filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPage identity)
        {
            return r.Insert(identity);
        }

        public int RegisterNewPageLayout(IdentityPage identity, out bool isNew)
        {
            return r.RegisterNewPage(identity, out isNew);
        }

        public bool Update(IdentityPage identity)
        {
            return r.Update(identity);
        }

        public IdentityPage GetById(int id, string langCode)
        {
            return r.GetById(id, langCode);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }
       
        public List<IdentityPage> GetList()
        {
            return r.GetList();
        }

        public IdentityPage F_Page_GetPageByOperation(string controller, string action)
        {
            return r.F_Page_GetPageByOperation(controller, action);
        }

        #endregion
    }
}
