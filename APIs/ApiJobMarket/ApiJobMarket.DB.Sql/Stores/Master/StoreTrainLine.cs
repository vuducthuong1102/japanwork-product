using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.DB.Sql.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ApiJobMarket.DB.Sql.Stores
{
    public interface IStoreTrainLine
    {
        List<IdentityTrainLine> GetByPage(IdentityTrainLine filter, int currentPage, int pageSize);
        List<IdentityTrainLine> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize);
        int Insert(IdentityTrainLine identity);
        bool Update(IdentityTrainLine identity);
        IdentityTrainLine GetById(int id);
        bool Delete(int id);
        List<IdentityTrainLine> GetList();

        List<IdentityTrainLine> GetListByPrefecture(int prefecture_id);
        List<IdentityTrainLine> GetListByCityId(int city_id, string keyword);
    }

    public class StoreTrainLine : IStoreTrainLine
    {
        private readonly string _connectionString;
        private RpsTrainLine myRepository;

        public StoreTrainLine() : this("JobMarketDB")
        {

        }

        public StoreTrainLine(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsTrainLine(_connectionString);
        }

        #region  Common

        public List<IdentityTrainLine> GetByPage(IdentityTrainLine filter, int currentPage, int pageSize)
        {
            return myRepository.GetByPage(filter, currentPage, pageSize);
        }

        public List<IdentityTrainLine> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            return myRepository.GetSuggestionsByPage(filter, currentPage, pageSize);
        }

        public int Insert(IdentityTrainLine identity)
        {
            return myRepository.Insert(identity);
        }

        public bool Update(IdentityTrainLine identity)
        {
            return myRepository.Update(identity);
        }

        public IdentityTrainLine GetById(int id)
        {
            return myRepository.GetById(id);
        }

        public bool Delete(int id)
        {
            return myRepository.Delete(id);
        }

        public List<IdentityTrainLine> GetList()
        {
            return myRepository.GetList();
        }

        public List<IdentityTrainLine> GetListByPrefecture(int prefecture_id)
        {
            return myRepository.GetListByPrefecture(prefecture_id);
        }
        public List<IdentityTrainLine> GetListByCityId(int city_id, string keyword)
        {
            return myRepository.GetListByCityId(city_id, keyword);
        }
        #endregion
    }
}
