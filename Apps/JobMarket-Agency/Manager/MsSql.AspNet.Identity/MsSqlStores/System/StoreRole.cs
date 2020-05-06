using MsSql.AspNet.Identity.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreRole
    {
        int Insert(IdentityRole role);
        void Delete(string roleId);
        IdentityRole GetRoleById(string roleId);
        IdentityRole GetRoleByName(string roleName);
        IQueryable<IdentityRole> GetRoleByUserId(int Agency_Id);
        int Update(IdentityRole role);
        IQueryable<IdentityRole> GetAll();

    }

    public class StoreRole : IStoreRole
    {
        private readonly string _connectionString;
        private RoleRepository r;

        public StoreRole() : this("DefaultConnection")
        {

        }

        public StoreRole(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RoleRepository(_connectionString);
        }

        #region  Common

        public int Insert(IdentityRole role)
        {
           return r.Insert(role);
        }
        public void Delete(string roleId)
        {
            r.Delete(roleId);
        }

        public IdentityRole GetRoleById(string roleId)
        {
            return r.GetRoleById(roleId);
        }
        public IdentityRole GetRoleByName(string roleName)
        {
            return r.GetRoleByName(roleName);
        }

        public IQueryable<IdentityRole> GetRoleByUserId(int Agency_Id)
        {
            return r.GetRoleByUserId(Agency_Id);
        }
        public int Update(IdentityRole role)
        {
          return  r.Update(role);
        }

        public IQueryable<IdentityRole> GetAll()
        {
            return r.GetAll();
        }
        #endregion
    }
}
