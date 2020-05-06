using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StoreProjectCategory : IStoreProjectCategory
    {
        private readonly string _connectionString;
        private RpsProjectCategory myRepository;

        public StoreProjectCategory()
            : this("DefaultConnection")
        {

        }

        public StoreProjectCategory(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsProjectCategory(_connectionString);
        }

        #region --- ProjectCategory ---
        public List<IdentityProjectCategory> GetByPage(IdentityProjectCategory filter)
        {
            return myRepository.GetByPage(filter);
        }

        public IdentityProjectCategory GetById(int id, string langCode)
        {
            return myRepository.GetById(id, langCode);
        }

        public int Insert(IdentityProjectCategory identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityProjectCategory identity)
        {
            return myRepository.Update(identity);
        }

        public int Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityProjectCategory F_ProjectCategory_GetDetailById(int id)
        {
            return myRepository.F_ProjectCategory_GetDetailById(id);
        }

        public List<IdentityProjectCategory> F_ProjectCategory_GetRelated(IdentityProjectCategory filter)
        {
            return myRepository.F_ProjectCategory_GetRelated(filter);
        }

        public List<IdentityProjectCategory> GetList()
        {
            return myRepository.GetList();
        }

        #endregion

        #region Images

        //public bool AddNewImage(IdentityProjectCategoryImage identity)
        //{
        //    return myRepository.AddNewImage(identity);
        //}

        //public bool RemoveImage(string projectId)
        //{
        //    return myRepository.RemoveImage(projectId);
        //}

        //public List<IdentityProjectCategoryImage> GetListImage(int projectId)
        //{
        //    return myRepository.GetListImage(projectId);
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
