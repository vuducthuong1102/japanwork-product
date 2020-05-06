using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public class StorePost : IStorePost
    {
        private readonly string _connectionString;
        private RpsPost r;

        public StorePost()
            : this("DefaultConnection")
        {

        }

        public StorePost(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsPost(_connectionString);
        }

        #region --- Post ---
        public List<IdentityPost> GetByPage(IdentityPost filter)
        {
            return r.GetByPage(filter);
        }

        public IdentityPost GetById(int id, string langCode)
        {
            return r.GetById(id, langCode);
        }

        public int Insert(IdentityPost identity)
        {
            return r.Insert(identity);
        }

        public int Update(IdentityPost identity)
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

        public IdentityPost F_GetDetailById(int id)
        {
            return r.F_GetDetailById(id);
        }

        public List<IdentityPost> F_GetRelated(IdentityPost filter)
        {
            return r.F_GetRelated(filter);
        }

        public List<IdentityPost> F_GetByCategory(IdentityPost filter)
        {
            return r.F_GetByCategory(filter);
        }

        public List<IdentityPost> F_SearchByPage(IdentityPost filter)
        {
            return r.F_SearchByPage(filter);
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
