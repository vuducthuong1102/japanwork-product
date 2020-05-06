using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreJobSeekerWish
    {
        bool Update(IdentityJobSeekerWish identity);
        IdentityJobSeekerWish GetById(int id);
        bool A_Update(IdentityJobSeekerWish identity);
        bool A_Delete(IdentityJobSeekerWish identity);
        List<IdentityJobSeekerWish> A_GetById(int id);
    }

    public class StoreJobSeekerWish : IStoreJobSeekerWish
    {
        private readonly string _connectionString;
        private RpsJobSeekerWish myRepository;

        public StoreJobSeekerWish() : this("JobMarketDB")
        {

        }

        public StoreJobSeekerWish(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsJobSeekerWish(_connectionString);
        }

        #region  JobSeekerWish

        public bool Update(IdentityJobSeekerWish identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityJobSeekerWish GetById(int id)
        {
            return myRepository.GetById(id);
        }
        #endregion

        #region  A_JobSeekerWish

        public bool A_Update(IdentityJobSeekerWish identity)
        {
            return myRepository.A_Update(identity);
        }

        public List<IdentityJobSeekerWish> A_GetById(int id)
        {
            return myRepository.A_GetById(id);
        }

        public bool A_Delete(IdentityJobSeekerWish identity)
        {
            return myRepository.A_Delete(identity);
        }

        #endregion
    }
}
