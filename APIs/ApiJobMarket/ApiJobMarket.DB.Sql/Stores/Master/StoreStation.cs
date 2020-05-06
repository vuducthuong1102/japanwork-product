using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreStation
    {
        List<IdentityStation> GetByPage(IdentityStation filter, int currentPage, int pageSize);
        List<IdentityStation> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityStation identity);
        bool Update(IdentityStation identity);
        IdentityStation GetById(int id);
        bool Delete(int id);
        List<IdentityStation> GetList();
        List<IdentityStation> GetListByPosition(int line_id);
        List<IdentityStation> GetListByCityId(int city_id);
    }

    public class StoreStation : IStoreStation
    {
        private readonly string _connectionString;
        private RpsStation myRepository;

        public StoreStation() : this("JobMarketDB")
        {

        }

        public StoreStation(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsStation(_connectionString);
        }

        #region  Common

        public List<IdentityStation> GetByPage(IdentityStation filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityStation> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityStation identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityStation identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityStation GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityStation> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityStation> GetListByPosition(int line_id)
        {
            return myRepository.GetListByPosition(line_id);
        }

        public List<IdentityStation> GetListByCityId(int city_id)
        {
            return myRepository.GetListByCityId(city_id);
        }
        #endregion
    }
}
