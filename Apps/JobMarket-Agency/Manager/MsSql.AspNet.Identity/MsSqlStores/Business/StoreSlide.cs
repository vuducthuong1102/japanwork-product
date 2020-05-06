using System;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Entities;
using MsSql.AspNet.Identity.Repositories;
using System.Configuration;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreSlide
    {
        List<IdentitySlide> GetByPage(IdentitySlide filter, int currentPage, int pageSize);
        List<IdentitySlide> GetAll(IdentitySlide filter, int currentPage, int pageSize);
        List<IdentitySlideItem> GetAllSlideItemBySlide(int slideId);
        int Insert(IdentitySlide identity);
        bool Update(IdentitySlide identity);
        IdentitySlide GetById(int id);
        bool Delete(int id);
        List<IdentitySlide> GetList();

        #region Slide Item

        int InsertSlideItem(IdentitySlideItem identity);
        bool UpdateSlideItem(IdentitySlideItem identity);
        IdentitySlideItem GetSlideItemById(int id);
        bool DeleteSlideItem(int id);

        #endregion
    }

    public class StoreSlide : IStoreSlide
    {
        private readonly string _connectionString;
        private RpsSlide r;

        public StoreSlide(): this("PfoodDBConnection")
        {

        }

        public StoreSlide(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsSlide(_connectionString);
        }

        #region  Common

        public List<IdentitySlide> GetByPage(IdentitySlide filter, int currentPage, int pageSize)
        {
            return r.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentitySlide> GetAll(IdentitySlide filter, int currentPage, int pageSize)
        {
            return r.GetAll(filter, currentPage, pageSize);
        }

        public List<IdentitySlideItem> GetAllSlideItemBySlide(int slideId)
        {
            return r.GetAllSlideItemBySlide(slideId);
        }

        public int Insert(IdentitySlide identity)
        {
            return r.Insert(identity);
        }

        public bool Update(IdentitySlide identity)
        {
            return r.Update(identity);
        }

        public IdentitySlide GetById(int Id)
        {
            return r.GetById(Id);
        }

        public bool Delete(int Id)
        {
            return r.Delete(Id);
        }
       
        public List<IdentitySlide> GetList()
        {
            return r.GetList();
        }

        #endregion

        #region Slide Item

        public int InsertSlideItem(IdentitySlideItem identity)
        {
            return r.InsertSlideItem(identity);
        }

        public bool UpdateSlideItem(IdentitySlideItem identity)
        {
            return r.UpdateSlideItem(identity);
        }

        public IdentitySlideItem GetSlideItemById(int id)
        {
            return r.GetSlideItemById(id);
        }
        
        public bool DeleteSlideItem(int id)
        {
            return r.DeleteSlideItem(id);
        }

        #endregion
    }
}
