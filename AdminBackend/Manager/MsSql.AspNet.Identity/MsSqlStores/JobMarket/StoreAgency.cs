using System.Configuration;
using System.Collections.Generic;
using MsSql.AspNet.Identity.Repositories;
using MsSql.AspNet.Identity.Entities;
using ApiJobMarket.DB.Sql.Entities;

namespace MsSql.AspNet.Identity.Stores
{
    public interface IStoreAgency
    {
        List<IdentityAgency> GetByPage(dynamic filter);

        IdentityAgency GetById(int id, string langCode);

        int Insert(IdentityAgency identity);

        int Update(IdentityAgency identity);

        int Delete(int id);

        IdentityAgency GetDetailById(int id);

        //#region Images

        //List<IdentityAgencyImage> GetListImage(int id);
        //bool AddNewImage(IdentityAgencyImage identity);
        //bool RemoveImage(string Id);

        //#endregion
    }

    public class StoreAgency : IStoreAgency
    {
        private readonly string _connectionString;
        private RpsAgency myRepository;

        public StoreAgency()
            : this("JobMarketConnection")
        {

        }

        public StoreAgency(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsAgency(_connectionString);
        }

        #region --- Agency ---
        public List<IdentityAgency> GetByPage(dynamic filter)
        {
            return myRepository.GetByPage(filter);
        }

        public IdentityAgency GetById(int id, string langCode)
        {
            return myRepository.GetById(id, langCode);
        }

        public int Insert(IdentityAgency identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityAgency identity)
        {
            return myRepository.Update(identity);
        }

        public int Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public IdentityAgency GetDetailById(int id)
        {
            return myRepository.GetDetailById(id);
        }

        #endregion

        //#region Images

        //public bool AddNewImage(IdentityAgencyImage identity)
        //{
        //    return myRepository.AddNewImage(identity);
        //}

        //public bool RemoveImage(string projectId)
        //{
        //    return myRepository.RemoveImage(projectId);
        //}

        //public List<IdentityAgencyImage> GetListImage(int projectId)
        //{
        //    return myRepository.GetListImage(projectId);
        //}

        //#endregion
    }   
}
