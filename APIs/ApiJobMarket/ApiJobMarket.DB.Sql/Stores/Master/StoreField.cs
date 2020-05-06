using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreField
    {
        List<IdentityField> GetByPage(IdentityField filter, int currentPage, int pageSize);
        int Insert(IdentityField identity);
        bool Update(IdentityField identity);
        IdentityField GetById(int id);
        bool Delete(int id);
        List<IdentityField> GetList();
        List<IdentityField> GetListFieldsByEmploymentType(int employment_type);
        List<IdentityField> F_GetListCount();
    }

    public class StoreField : IStoreField
    {
        private readonly string _connectionString;
        private RpsField myRepository;

        public StoreField() : this("JobMarketDB")
        {

        }

        public StoreField(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsField(_connectionString);
        }

        #region  Common

        public List<IdentityField> GetByPage(IdentityField filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityField identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityField identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityField GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityField> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityField> GetListFieldsByEmploymentType(int employment_type)
        {
            return myRepository.GetListFieldsByEmploymentType(employment_type);
        }
        
        public List<IdentityField> F_GetListCount()
        {
            return myRepository.F_GetListCount();
        }

        #endregion
    }
}
