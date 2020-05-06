using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreJobSeekerNote
    {
        List<IdentityJobSeekerNote> GetByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityJobSeekerNote identity);
        int Update(IdentityJobSeekerNote identity);
        IdentityJobSeekerNote GetById(int id);
        bool Delete(int id);
    }

    public class StoreJobSeekerNote : IStoreJobSeekerNote
    {
        private readonly string _connectionString;
        private RpsJobSeekerNote myRepository;

        public StoreJobSeekerNote() : this("JobMarketDB")
        {

        }

        public StoreJobSeekerNote(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsJobSeekerNote(_connectionString);
        }

        #region  Common

        public List<IdentityJobSeekerNote> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityJobSeekerNote identity)
        {
            return myRepository.Insert(identity);
        }

        public int Update(IdentityJobSeekerNote identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityJobSeekerNote GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        #endregion
    }
}
