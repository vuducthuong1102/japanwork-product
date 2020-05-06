using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StoreProject : IStoreProject
    {
        private readonly string _connectionString;
        private RpsProject r;

        public StoreProject()
            : this("DefaultConnection")
        {

        }

        public StoreProject(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsProject(_connectionString);
        }

        #region --- Project ---
        public List<IdentityProject> GetByPage(IdentityProject filter)
        {
            return r.GetByPage(filter);
        }

        public IdentityProject GetById(int id, string langCode)
        {
            return r.GetById(id, langCode);
        }

        public int Insert(IdentityProject identity)
        {
            return r.Insert(identity);
        }

        public int Update(IdentityProject identity)
        {
            return r.Update(identity);
        }

        public int Delete(int id)
        {
            return r.Delete(id);
        }

        public bool AssignCategory(int postId, int catId)
        {
            return r.AssignCategory(postId, catId);
        }

        public IdentityProject GetDetailById(int id)
        {
            return r.GetDetailById(id);
        }

        public List<IdentityProject> F_Project_GetRelated(IdentityProject filter)
        {
            return r.F_Project_GetRelated(filter);
        }

        public List<IdentityProject> F_Project_GetNewest(IdentityProject filter)
        {
            return r.F_Project_GetNewest(filter);
        }

        #endregion

        #region Images

        public bool AddNewImage(IdentityProjectImage identity)
        {
            return r.AddNewImage(identity);
        }

        public bool RemoveImage(string projectId)
        {
            return r.RemoveImage(projectId);
        }

        public List<IdentityProjectImage> GetListImage(int projectId)
        {
            return r.GetListImage(projectId);
        }

        #endregion
    }

    public interface IStoreProject
    {
        List<IdentityProject> GetByPage(IdentityProject identity);

        IdentityProject GetById(int id, string langCode);

        int Insert(IdentityProject identity);

        int Update(IdentityProject identity);

        int Delete(int id);

        bool AssignCategory(int postId, int catId);

        IdentityProject GetDetailById(int id);

        List<IdentityProject> F_Project_GetRelated(IdentityProject filter);

        List<IdentityProject> F_Project_GetNewest(IdentityProject filter);

        #region Images

        List<IdentityProjectImage> GetListImage(int id);
        bool AddNewImage(IdentityProjectImage identity);
        bool RemoveImage(string Id);

        #endregion
    }
}
