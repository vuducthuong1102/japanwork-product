using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StoreProjectCategory : IStoreProjectCategory
    {
        private readonly string _connectionString;
        private RpsProjectCategory r;

        public StoreProjectCategory()
            : this("DefaultConnection")
        {

        }

        public StoreProjectCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsProjectCategory(_connectionString);
        }

        #region --- ProjectCategory ---
        public List<IdentityProjectCategory> GetByPage(IdentityProjectCategory filter)
        {
            return r.GetByPage(filter);
        }

        public IdentityProjectCategory GetById(int id, string langCode)
        {
            return r.GetById(id, langCode);
        }

        public int Insert(IdentityProjectCategory identity)
        {
            return r.Insert(identity);
        }

        public int Update(IdentityProjectCategory identity)
        {
            return r.Update(identity);
        }

        public int Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityProjectCategory F_ProjectCategory_GetDetailById(int id)
        {
            return r.F_ProjectCategory_GetDetailById(id);
        }

        public List<IdentityProjectCategory> F_ProjectCategory_GetRelated(IdentityProjectCategory filter)
        {
            return r.F_ProjectCategory_GetRelated(filter);
        }

        public List<IdentityProjectCategory> GetList()
        {
            return r.GetList();
        }

        #endregion

        #region Images

        //public bool AddNewImage(IdentityProjectCategoryImage identity)
        //{
        //    return r.AddNewImage(identity);
        //}

        //public bool RemoveImage(string projectId)
        //{
        //    return r.RemoveImage(projectId);
        //}

        //public List<IdentityProjectCategoryImage> GetListImage(int projectId)
        //{
        //    return r.GetListImage(projectId);
        //}

        #endregion
    }

    public interface IStoreProjectCategory
    {
        List<IdentityProjectCategory> GetByPage(IdentityProjectCategory identity);

        IdentityProjectCategory GetById(int id, string langCode);

        int Insert(IdentityProjectCategory identity);

        int Update(IdentityProjectCategory identity);

        int Delete(int id);

        IdentityProjectCategory F_ProjectCategory_GetDetailById(int id);

        List<IdentityProjectCategory> F_ProjectCategory_GetRelated(IdentityProjectCategory filter);

        List<IdentityProjectCategory> GetList();
        #region Images

        //List<IdentityProjectCategoryImage> GetListImage(int id);
        //bool AddNewImage(IdentityProjectCategoryImage identity);
        //bool RemoveImage(string Id);

        #endregion
    }
}
