using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StoreProject : IStoreProject
    {
        private readonly string _connectionString;
        private RpsProject myRepository;

        public StoreProject()
            : this("DefaultConnection")
        {

        }

        public StoreProject(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsProject(_connectionString);
        }

        #region --- Project ---
        public List<IdentityProject> GetByPage(IdentityProject filter)
        {
            return myRepository.GetByPage(filter);
        }

        public IdentityProject GetById(int id, string langCode)
        {
            return myRepository.GetById(id, langCode);
        }

        public int Insert(IdentityProject identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityProject identity)
        {
            return myRepository.Update(identity);
        }

        public int Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public bool AssignCategory(int postId, int catId)
        {
            return myRepository.AssignCategory(postId, catId);
        }

        public IdentityProject GetDetailById(int id)
        {
            return myRepository.GetDetailById(id);
        }

        public List<IdentityProject> F_Project_GetRelated(IdentityProject filter)
        {
            return myRepository.F_Project_GetRelated(filter);
        }

        public List<IdentityProject> F_Project_GetNewest(IdentityProject filter)
        {
            return myRepository.F_Project_GetNewest(filter);
        }

        #endregion

        #region Images

        public bool AddNewImage(IdentityProjectImage identity)
        {
            return myRepository.AddNewImage(identity);
        }

        public bool RemoveImage(string projectId)
        {
            return myRepository.RemoveImage(projectId);
        }

        public List<IdentityProjectImage> GetListImage(int projectId)
        {
            return myRepository.GetListImage(projectId);
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
