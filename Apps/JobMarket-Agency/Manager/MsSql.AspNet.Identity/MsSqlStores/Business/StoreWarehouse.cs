using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreWarehouse
    {
        List<IdentityWarehouse> GetByPage(IdentityWarehouse filter, int currentPage, int pageSize);
        List<IdentityWarehouse> GetProductStockOutByPage(IdentityWarehouse filter, int currentPage, int pageSize);
        int GoodsReceipt(IdentityWarehouse identity, IdentityWarehouseActivity activity);
        int GoodsIssue(IdentityWarehouse identity, IdentityWarehouseActivity activity);
        double ReflectStockTake(IdentityWarehouse identity, IdentityWarehouseActivity activity);
        bool Update(IdentityWarehouse identity);
        IdentityWarehouse GetById(int id);
        bool Delete(int id);
        List<IdentityWarehouse> GetList();
        List<IdentityWarehouseActivity> GetHistoryByPage(IdentityWarehouseActivity filter, int currentPage, int pageSize);

        bool RemoveReflection(int productId);
    }

    public class StoreWarehouse : IStoreWarehouse
    {
        private readonly string _connectionString;
        private RpsWarehouse r;

        public StoreWarehouse() : this("DefaultConnection")
        {

        }

        public StoreWarehouse(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsWarehouse(_connectionString);
        }

        #region  Common

        public List<IdentityWarehouse> GetByPage(IdentityWarehouse filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityWarehouse> GetProductStockOutByPage(IdentityWarehouse filter, int currentPage, int pageSize)
        {
            return r.GetProductStockOutByPage(filter, currentPage, pageSize);
        }

        public int GoodsReceipt(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            return r.GoodsReceipt(identity, activity);
        }

        public int GoodsIssue(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            return r.GoodsIssue(identity, activity);
        }

        public double ReflectStockTake(IdentityWarehouse identity, IdentityWarehouseActivity activity)
        {
            return r.ReflectStockTake(identity, activity);
        }

        public bool Update(IdentityWarehouse identity)
        {
            return r.Update(identity);
        }

        public IdentityWarehouse GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }

        public bool RemoveReflection(int productId)
        {
            return r.RemoveReflection(productId);
        }

        public List<IdentityWarehouse> GetList()
        {
            return r.GetList();
        }

        public List<IdentityWarehouseActivity> GetHistoryByPage(IdentityWarehouseActivity filter, int currentPage, int pageSize)
        {
            return r.GetHistoryByPage(filter, currentPage, pageSize);
        }

        #endregion
    }
}
