using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreProvider
    {
        List<IdentityProvider> GetByPage(IdentityProvider filter, int currentPage, int pageSize);
        int Insert(IdentityProvider identity);
        bool Update(IdentityProvider identity);
        IdentityProvider GetById(int id);
        bool Delete(int id);
        List<IdentityProvider> GetList();
        List<IdentityProvider> GetListByUserId(string UserId);
    }

    public class StoreProvider : IStoreProvider
    {
        private readonly string _connectionString;
        private RpsProvider r;

        public StoreProvider(): this("DefaultConnection")
        {

        }

        public StoreProvider(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsProvider(_connectionString);
        }

        public List<IdentityProvider> GetByPage(IdentityProvider filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityProvider identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityProvider identity)
        {
            return r.Update(identity);
        }

        public IdentityProvider GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }

        public List<IdentityProvider> GetList()
        {
            return r.GetList();
        }

        public List<IdentityProvider> GetListByUserId( string UserId)
        {
            return r.GetListByUserId(UserId);
        }
    }
}
