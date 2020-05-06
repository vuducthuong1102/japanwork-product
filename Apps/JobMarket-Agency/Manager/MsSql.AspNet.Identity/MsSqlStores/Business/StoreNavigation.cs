//using System;
//using System.Configuration;
//using System.Collections.Generic;
//using MsSql.AspNet.Identity.Entities;
//using MsSql.AspNet.Identity.Repositories;
//using Manager.SharedLibs;

//namespace MsSql.AspNet.Identity.MsSqlStores
//{
//    public interface IStoreNavigation
//    {
//        List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize);
//        List<IdentityNavigation> GetList();
//        int Insert(IdentityNavigation identity);
//        bool Update(IdentityNavigation identity);
//        IdentityNavigation GetById(int id);
//        bool Delete(int id);
//        bool UpdateSorting(List<SortingElement> elements);

//        int AddNewLang(IdentityNavigationLang identity);
//        int UpdateLang(IdentityNavigationLang identity);
//        bool DeleteLang(int Id);
//        IdentityNavigation GetNavigationDetail(int id);
//        IdentityNavigationLang GetLangDetail(int id);

//        List<IdentityNavigation> F_GetList();
//    }

//    public class StoreNavigation : IStoreNavigation
//    {
//        private readonly string _connectionString;
//        private RpsNavigation r;

//        public StoreNavigation() : this("DefaultConnection")
//        {
//        }

//        public StoreNavigation(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            r = new RpsNavigation(_connectionString);
//        }

//        #region  Common

//        public List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize)
//        {
//            return r.GetByPage(filter, currentPage, pageSize);
//        }

//        public List<IdentityNavigation> GetList()
//        {
//            return r.GetList();
//        }

//        public int Insert(IdentityNavigation identity)
//        {
//            return r.Insert(identity);
//        }

//        public bool Update(IdentityNavigation identity)
//        {
//            return r.Update(identity);
//        }

//        public IdentityNavigation GetById(int id)
//        {
//            return r.GetById(id);
//        }

//        public bool Delete(int id)
//        {
//            return r.Delete(id);
//        }

//        public bool UpdateSorting(List<SortingElement> elements)
//        {
//            return r.UpdateSorting(elements);
//        }

//        public int AddNewLang(IdentityNavigationLang identity)
//        {
//            return r.AddNewLang(identity);
//        }

//        public int UpdateLang(IdentityNavigationLang identity)
//        {
//            return r.UpdateLang(identity);
//        }

//        public bool DeleteLang(int Id)
//        {
//            return r.DeleteLang(Id);
//        }

//        public IdentityNavigation GetNavigationDetail(int id)
//        {
//            return r.GetNavigationDetail(id);
//        }

//        public IdentityNavigationLang GetLangDetail(int id)
//        {
//            return r.GetLangDetail(id);
//        }

//        public List<IdentityNavigation> F_GetList()
//        {
//            return r.F_GetList();
//        }

//        #endregion
//    }
//}
