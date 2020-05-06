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
        private RpsWidget myRepository;

        public StoreWidget(): this("DefaultConnection")
        {

        }

        public StoreWidget(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsWidget(_connectionString);
        }

        #region  Common

        public List<IdentityWidget> GetByPage(IdentityWidget filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityWidget identity)
        {
            return myRepository.Insert(identity);
        }

        public int RegisterNewWidget(IdentityWidget identity, out bool isNew)
        {
            return myRepository.RegisterNewWidget(identity, out isNew);
        }

        public bool Update(IdentityWidget identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityWidget GetById(int Id)
        {
            return myRepository.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return myRepository.Delete(Id);
        }
       
        public List<IdentityWidget> GetList()
        {
            return myRepository.GetList();
        }

        #endregion
    }
}
