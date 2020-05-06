using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreCurrency
    {
        List<IdentityCurrency> GetByPage(IdentityCurrency filter, int currentPage, int pageSize);
        int Insert(IdentityCurrency identity);
        bool Update(IdentityCurrency identity);
        IdentityCurrency GetById(int id);
        bool Delete(int id);
        List<IdentityCurrency> GetList();
    }

    public class StoreCurrency : IStoreCurrency
    {
        private readonly string _connectionString;
        private RpsCurrency r;

        public StoreCurrency(): this("DefaultConnection")
        {

        }

        public StoreCurrency(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsCurrency(_connectionString);
        }

        #region  Common

        public List<IdentityCurrency> GetByPage(IdentityCurrency filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityCurrency identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityCurrency identity)
        {
            return r.Update(identity);
        }

        public IdentityCurrency GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentityCurrency> GetList()
        {
            return r.GetList();
        }

        #endregion
    }
}
