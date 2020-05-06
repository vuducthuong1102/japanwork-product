using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreWidget
    {
        List<IdentityWidget> GetByPage(IdentityWidget filter, int currentPage, int pageSize);
        int Insert(IdentityWidget identity);

        int RegisterNewWidget(IdentityWidget identity, out bool isNew);
        bool Update(IdentityWidget identity);
        IdentityWidget GetById(int id);
        bool Delete(int id);
        List<IdentityWidget> GetList();
    }

    public class StoreWidget : IStoreWidget
    {
        private readonly string _connectionString;
        private RpsWidget r;

        public StoreWidget(): this("DefaultConnection")
        {

        }

        public StoreWidget(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsWidget(_connectionString);
        }

        #region  Common

        public List<IdentityWidget> GetByPage(IdentityWidget filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityWidget identity)
        {
            return r.Insert(identity);
        }

        public int RegisterNewWidget(IdentityWidget identity, out bool isNew)
        {
            return r.RegisterNewWidget(identity, out isNew);
        }

        public bool Update(IdentityWidget identity)
        {
            return r.Update(identity);
        }

        public IdentityWidget GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityWidget> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
