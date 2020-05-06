using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreIndustry
    {
        List<IdentityIndustry> GetByPage(IdentityIndustry filter, int currentPage, int pageSize);
        int Insert(IdentityIndustry identity);
        bool Update(IdentityIndustry identity);
        IdentityIndustry GetById(int id);
        bool Delete(int id);
        List<IdentityIndustry> GetList();
        List<IdentitySubIndustry> GetListSub();
        List<IdentitySubIndustry> GetListSubByIndustry(int industry_id);
    }

    public class StoreIndustry : IStoreIndustry
    {
        private readonly string _connectionString;
        private RpsIndustry myRepository;

        public StoreIndustry() : this("JobMarketDB")
        {

        }

        public StoreIndustry(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsIndustry(_connectionString);
        }

        #region  Common

        public List<IdentityIndustry> GetByPage(IdentityIndustry filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityIndustry identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityIndustry identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityIndustry GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityIndustry> GetList()
        {
            return myRepository.GetList();
        }
        public List<IdentitySubIndustry> GetListSub()
        {
            return myRepository.GetListSub();
        }
        public List<IdentitySubIndustry> GetListSubByIndustry(int industry_id)
        {
            return myRepository.GetListSubByIndustry(industry_id);
        }

        #endregion
    }
}
