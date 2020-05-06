using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using ApiJobMarket.SharedLibs.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreProcessStatus
    {
        List<IdentityProcessStatus> GetList(int agency_id);
        int Insert(IdentityProcessStatus identity);
        bool Update(IdentityProcessStatus identity);

        bool UpdateSorting(List<SortingElement> elements);
        IdentityProcessStatus GetById(int id);
        int Delete(int id);
    }
    public class StoreProcessStatus : IStoreProcessStatus
    {
        private readonly string _connectionString;
        private RpsProcessStatus myRepository;

        public StoreProcessStatus() : this("JobMarketDB")
        {

        }

        public StoreProcessStatus(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsProcessStatus(_connectionString);
        }

        #region  Common

        public List<IdentityProcessStatus> GetList(int agency_id)
        {
            return myRepository.GetList(agency_id);
        }

        public int Insert(IdentityProcessStatus identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityProcessStatus identity)
        {
            return myRepository.Update(identity);
        }
        public bool UpdateSorting(List<SortingElement> elements)
        {
            return myRepository.UpdateSorting(elements);
        }
        public IdentityProcessStatus GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public int Delete(int id)
        {
            return myRepository.Delete(id);
        }
        #endregion
    }
}
