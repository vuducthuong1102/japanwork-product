using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStorePolicy
    {
        List<IdentityPolicy> GetAll(IdentityPolicy filter, int currentPage, int pageSize);
        int Insert(IdentityPolicy identity);
        bool Update(IdentityPolicy identity);
        IdentityPolicy GetById(int id);
        bool Delete(int id);
        List<IdentityPolicy> GetList();
    }

    public class StorePolicy : IStorePolicy
    {
        private readonly string _connectionString;
        private RpsPolicy r;

        public StorePolicy() : this("DefaultConnection")
        {

        }

        public StorePolicy(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPolicy(_connectionString);
        }

        #region  property

        public List<IdentityPolicy> GetAll(IdentityPolicy filter, int currentPage, int pageSize)
        {
            return r.GetAll(filter, currentPage, pageSize);
        }

        public int Insert(IdentityPolicy identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityPolicy identity)
        {
            return r.Update(identity);
        }

        public IdentityPolicy GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }

        public List<IdentityPolicy> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
