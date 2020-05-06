//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.DB.Sql.Repositories;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ApiJobMarket.DB.Sql.Stores
//{
//    public class StorePlace : IStorePlace
//    {
//        private readonly string _connectionString;
//        private RpsPlace myRepository;

//        public StorePlace()
//            : this("JobMarketDB")
//        {

//        }

//        public StorePlace(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            myRepository = new RpsPlace(_connectionString);
//        }


//        public int Insert(IdentityPlace identity, string language)
//        {
//            return myRepository.Insert(identity, language);
//        }

//        public List<IdentityPlace> GetPlanTripPage(IdentityPlace filter, int currentPage, int pageSize)
//        {
//            return myRepository.GetPlanTripPage(filter, currentPage, pageSize);
//        }

//        public List<IdentityPlace> GetBestDestination(IdentityFilter filter)
//        {
//            return myRepository.GetBestDestination(filter);
//        }

//        public List<IdentityPlace> GetByPage(IdentityFilter filter)
//        {
//            return myRepository.GetByPage(filter);
//        }

//        public List<IdentityPlace> GetByProvince(int provinceId)
//        {
//            return myRepository.GetByProvince(provinceId);
//        }

//        public IdentityPlace GetPlaceById(int Id, bool getShort = false)
//        {
//            return myRepository.GetPlaceById(Id, getShort);
//        }

//        public List<IdentityPlace> GetFromList(string placesList)
//        {
//            return myRepository.GetFromList(placesList);
//        }
//    }
//    public interface IStorePlace
//    {
//        int Insert(IdentityPlace identity, string language);

//        List<IdentityPlace> GetPlanTripPage(IdentityPlace filter, int currentPage, int pageSize);
//        List<IdentityPlace> GetBestDestination(IdentityFilter filter);
//        List<IdentityPlace> GetByPage(IdentityFilter filter);
//        List<IdentityPlace> GetByProvince(int provinceId);
//        IdentityPlace GetPlaceById(int Id, bool getShort = false);
//        List<IdentityPlace> GetFromList(string placesList);
//    }
//}
