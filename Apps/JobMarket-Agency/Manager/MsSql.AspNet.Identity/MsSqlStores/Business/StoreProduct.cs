using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreProduct
    {
        List<IdentityProduct> GetByPage(IdentityProduct filter, int currentPage, int pageSize);

        List<IdentityProduct> GetActiveForChoosen(IdentityProduct filter, int currentPage, int pageSize);

        List<IdentityProduct> GetListNeedReflectByPage(IdentityProduct filter, int currentPage, int pageSize);

        List<IdentityProduct> GetByListIds(List<int> listIds);

        int Insert(IdentityProduct identity);
        bool Update(IdentityProduct identity);
        IdentityProduct GetById(int id);

        IdentityProduct GetDetail(int id);
        IdentityProduct GetByCode(string code);
        bool Delete(int id);
    }

    public class StoreProduct : IStoreProduct
    {
        private readonly string _connectionString;
        private RpsProduct r;

        public StoreProduct() : this("DefaultConnection")
        {
        }

        public StoreProduct(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsProduct(_connectionString);
        }

        #region  Common

        public List<IdentityProduct> GetByPage(IdentityProduct filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityProduct> GetByListIds(List<int> listIds)
        {
            return r.GetByListIds(listIds);
        }

        public int Insert(IdentityProduct identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentityProduct identity)
        {
            return r.Update(identity);
        }

        public List<IdentityProduct> GetActiveForChoosen(IdentityProduct filter, int currentPage, int pageSize)
        {
            return r.GetActiveForChoosen(filter, currentPage, pageSize);
        }

        public List<IdentityProduct> GetListNeedReflectByPage(IdentityProduct filter, int currentPage, int pageSize)
        {
            return r.GetListNeedReflectByPage(filter, currentPage, pageSize);
        }

        public IdentityProduct GetById(int id)
        {
            return r.GetById(id);
        }

        public IdentityProduct GetDetail(int id)
        {
            return r.GetDetail(id);
        }

        public IdentityProduct GetByCode(string code)
        {
            return r.GetByCode(code);
        }

        public bool Delete(int id)
        {
            return r.Delete(id);
        }

        #endregion
    }

}
