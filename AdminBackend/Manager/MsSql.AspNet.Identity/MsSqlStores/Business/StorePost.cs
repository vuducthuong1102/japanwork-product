using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StorePost : IStorePost
    {
        private readonly string _connectionString;
        private RpsPost myRepository;

        public StorePost()
            : this("DefaultConnection")
        {

        }

        public StorePost(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPost(_connectionString);
        }

        #region --- Post ---
        public List<IdentityPost> GetByPage(IdentityPost filter)
        {
            return myRepository.GetByPage(filter);
        }

        public IdentityPost GetById(int id, string langCode)
        {
            return myRepository.GetById(id, langCode);
        }

        public int Insert(IdentityPost identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityPost identity)
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

        public IdentityPost F_GetDetailById(int id)
        {
            return myRepository.F_GetDetailById(id);
        }

        public List<IdentityPost> F_GetRelated(IdentityPost filter)
        {
            return myRepository.F_GetRelated(filter);
        }

        public List<IdentityPost> F_GetByCategory(IdentityPost filter)
        {
            return myRepository.F_GetByCategory(filter);
        }

        public List<IdentityPost> F_SearchByPage(IdentityPost filter)
        {
            return myRepository.F_SearchByPage(filter);
        }

        #endregion
    }

    public interface IStorePost
    {
        List<IdentityPost> GetByPage(IdentityPost identity);

        IdentityPost GetById(int id, string langCode);

        int Insert(IdentityPost identity);

        int Update(IdentityPost identity);

        int Delete(int id);

        bool AssignCategory(int postId, int catId);

        IdentityPost F_GetDetailById(int id);

        List<IdentityPost> F_GetRelated(IdentityPost filter);

        List<IdentityPost> F_GetByCategory(IdentityPost filter);

        List<IdentityPost> F_SearchByPage(IdentityPost filter);
    }
}
