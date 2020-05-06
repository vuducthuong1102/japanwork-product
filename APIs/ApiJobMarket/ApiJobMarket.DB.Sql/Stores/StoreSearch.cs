//using System.Configuration;
//using ApiJobMarket.DB.Sql.Repositories;
//using ApiJobMarket.DB.Sql.Entities;
//using System.Collections.Generic;

//namespace ApiJobMarket.DB.Sql.Stores
//{
//    public class StoreSearch : IStoreSearch
//    {
//        private readonly string _connectionString;
//        private RpsSearch myRepository;

//        public StoreSearch()
//            : this("JobMarketDB")
//        {

//        }

//        public StoreSearch(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            myRepository = new RpsSearch(_connectionString);
//        }
      
//        public List<IdentityPost> GetPostByPage(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetPostByPage(identity, out returnCode);
//        }
//    }

//    public interface IStoreSearch
//    {
//        List<IdentityPost> GetPostByPage(IdentityFilter identity, out int returnCode);
//    }
//}
