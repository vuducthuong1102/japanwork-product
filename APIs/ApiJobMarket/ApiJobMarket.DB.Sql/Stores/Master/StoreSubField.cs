using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreSubField
    {
        List<IdentitySubField> GetList();
    }

    public class StoreSubField : IStoreSubField
    {
        private readonly string _connectionString;
        private RpsSubField myRepository;

        public StoreSubField() : this("JobMarketDB")
        {

        }

        public StoreSubField(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsSubField(_connectionString);
        }

        #region  Common

        public List<IdentitySubField> GetList()
        {
            return myRepository.GetList();
        }
        #endregion
    }
}
