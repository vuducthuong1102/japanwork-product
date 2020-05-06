using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreDevice
    {
        List<IdentityDevice> GetByPage(IdentityDevice filter, int currentPage, int pageSize);
        int Insert(IdentityDevice identity);

        int RegisterNewDevice(IdentityDevice identity, out bool isNew);
        bool Update(IdentityDevice identity);
        IdentityDevice GetById(int id);
        bool Delete(int id);
        List<IdentityDevice> GetList();
    }

    public class StoreDevice : IStoreDevice
    {
        private readonly string _connectionString;
        private RpsDevice r;

        public StoreDevice(): this("DefaultConnection")
        {

        }

        public StoreDevice(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsDevice(_connectionString);
        }

        #region  Common

        public List<IdentityDevice> GetByPage(IdentityDevice filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityDevice identity)
        {
            return r.Insert(identity);
        }

        public int RegisterNewDevice(IdentityDevice identity, out bool isNew)
        {
            return r.RegisterNewDevice(identity, out isNew);
        }

        public bool Update(IdentityDevice identity)
        {
            return r.Update(identity);
        }

        public IdentityDevice GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityDevice> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
