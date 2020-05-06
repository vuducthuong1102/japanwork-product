using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreSuggest
    {
        List<IdentitySuggest> GetByPage(IdentitySuggest filter, int currentPage, int pageSize);
        int Insert(IdentitySuggest identity);
        bool Update(IdentitySuggest identity);
        IdentitySuggest GetById(int id);
        bool Delete(int id);

        List<IdentitySuggest> GetList(IdentitySuggest filter);
    }

    public class StoreSuggest : IStoreSuggest
    {
        private readonly string _connectionString;
        private RpsSuggest myRepository;

        public StoreSuggest() : this("JobMarketDB")
        {

        }

        public StoreSuggest(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSuggest(_connectionString);
        }

        #region  Common

        public List<IdentitySuggest> GetByPage(IdentitySuggest filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentitySuggest identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentitySuggest identity)
        {
            return myRepository.Update(identity);
        }

        public IdentitySuggest GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentitySuggest> GetList(IdentitySuggest filter)
        {
            return myRepository.GetList(filter);
        }
        #endregion
    }
}
