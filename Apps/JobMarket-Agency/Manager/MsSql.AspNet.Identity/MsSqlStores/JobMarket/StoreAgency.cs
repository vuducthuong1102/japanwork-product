using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using ApiJobMarket.DB.Sql.Entities;

namespace MsSql.AspNet.Identity.MsSqlStores
{
    public interface IStoreAgency
    {
        List<IdentityAgency> GetByPage(dynamic filter);

        IdentityAgency GetById(int id, string langCode);

        int Insert(IdentityAgency identity);

        int Update(IdentityAgency identity);

        int Delete(int id);

        IdentityAgency GetDetailById(int id);

        int CreateAccount(IdentityUser identity);
        IdentityUser GetByUserName(string userName);

        //Active account
        int ResendEmailActive(IdentityActiveAccount info);
        int SendEmailRecover(dynamic info);
        int ActiveAccountByEmail(IdentityActiveAccount info);
        int ActiveAccountByOTP(IdentityActiveAccount info);

        int ChangePassword(IdentityUser info);
        //#region Images

        //List<IdentityAgencyImage> GetListImage(int id);
        //bool AddNewImage(IdentityAgencyImage identity);
        //bool RemoveImage(string Id);

        //#endregion
    }

    public class StoreAgency : IStoreAgency
    {
        private readonly string _connectionString;
        private RpsAgency r;

        public StoreAgency()
            : this("DefaultConnection")
        {

        }

        public StoreAgency(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            r = new RpsAgency(_connectionString);
        }

        #region --- Agency ---
        public List<IdentityAgency> GetByPage(dynamic filter)
        {
            return r.GetByPage(filter);
        }

        public IdentityAgency GetById(int id, string langCode)
        {
            return r.GetById(id, langCode);
        }

        public int Insert(IdentityAgency identity)
        {
            return r.Insert(identity);
        }

        public int Update(IdentityAgency identity)
        {
            return r.Update(identity);
        }

        public int Delete(int id)
        {
            return r.Delete(id);
        }

        public IdentityAgency GetDetailById(int id)
        {
            return r.GetDetailById(id);
        }

        public int CreateAccount(IdentityUser identity)
        {
            return r.CreateAccount(identity);
        }

        public IdentityUser GetByUserName(string userName)
        {
            return r.GetByUserName(userName);
        }

        //Active account
        public int ResendEmailActive(IdentityActiveAccount info)
        {
            return r.ResendEmailActive(info);
        }

        public int SendEmailRecover(dynamic info)
        {
            return r.SendEmailRecover(info);
        }

        public int ChangePassword(IdentityUser info)
        {
            return r.ChangePassword(info);
        }

        public int ActiveAccountByEmail(IdentityActiveAccount info)
        {
            return r.ActiveAccountByEmail(info);
        }

        public int ActiveAccountByOTP(IdentityActiveAccount info)
        {
            return r.ActiveAccountByOTP(info);
        }

        #endregion

        //#region Images

        //public bool AddNewImage(IdentityAgencyImage identity)
        //{
        //    return r.AddNewImage(identity);
        //}

        //public bool RemoveImage(string projectId)
        //{
        //    return r.RemoveImage(projectId);
        //}

        //public List<IdentityAgencyImage> GetListImage(int projectId)
        //{
        //    return r.GetListImage(projectId);
        //}

        //#endregion
    }   
}
