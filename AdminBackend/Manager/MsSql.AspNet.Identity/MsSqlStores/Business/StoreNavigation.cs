using System;
using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using Manager.SharedLibs;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreNavigation
    {
        List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize);
        List<IdentityNavigation> GetList();
        int Insert(IdentityNavigation identity);
        bool Update(IdentityNavigation identity);
        IdentityNavigation GetById(int id);
        bool Delete(int id);
        bool UpdateSorting(List<SortingElement> elements);

        int AddNewLang(IdentityNavigationLang identity);
        int UpdateLang(IdentityNavigationLang identity);
        bool DeleteLang(int Id);
        IdentityNavigation GetNavigationDetail(int id);
        IdentityNavigationLang GetLangDetail(int id);

        List<IdentityNavigation> F_GetList();
    }

    public class StoreNavigation : IStoreNavigation
    {
        private readonly string _connectionString;
        private RpsNavigation myRepository;

        public StoreNavigation() : this("DefaultConnection")
        {
        }

        public StoreNavigation(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsNavigation(_connectionString);
        }

        #region  Common

        public List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityNavigation> GetList()
        {
            return myRepository.GetList();
        }

        public int Insert(IdentityNavigation identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityNavigation identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityNavigation GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public bool UpdateSorting(List<SortingElement> elements)
        {
            return myRepository.UpdateSorting(elements);
        }

        public int AddNewLang(IdentityNavigationLang identity)
        {
            return myRepository.AddNewLang(identity);
        }

        public int UpdateLang(IdentityNavigationLang identity)
        {
            return myRepository.UpdateLang(identity);
        }

        public bool DeleteLang(int Id)
        {
            return myRepository.DeleteLang(Id);
        }

        public IdentityNavigation GetNavigationDetail(int id)
        {
            return myRepository.GetNavigationDetail(id);
        }

        public IdentityNavigationLang GetLangDetail(int id)
        {
            return myRepository.GetLangDetail(id);
        }

        public List<IdentityNavigation> F_GetList()
        {
            return myRepository.F_GetList();
        }

        #endregion
    }
}
