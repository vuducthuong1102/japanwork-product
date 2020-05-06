//using System.Configuration;
//using ApiJobMarket.DB.Sql.Repositories;
//using ApiJobMarket.DB.Sql.Entities;
//using System.Collections.Generic;

//namespace ApiJobMarket.DB.Sql.Stores
//{
//    public class StorePost : IStorePost
//    {
//        private readonly string _connectionString;
//        private RpsPost myRepository;

//        public StorePost()
//            : this("JobMarketDB")
//        {

//        }

//        public StorePost(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
//            myRepository = new RpsPost(_connectionString);
//        }

//        #region --- Post ---
//        public List<IdentityPost> GetByPage(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetByPage(identity, out returnCode);
//        }

//        public List<IdentityPost> GetByCategory(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetByCategory(identity, out returnCode);
//        }

//        public List<IdentityPost> GetListByDestination(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetListByDestination(identity, out returnCode);
//        }

//        public List<IdentityPost> GetRecent(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetRecent(identity, out returnCode);
//        }

//        public List<IdentityPost> GetListPosted(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetListPosted(identity, out returnCode);
//        }

//        public List<IdentityImage> GetUploadedImages(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetUploadedImages(identity, out returnCode);
//        }

//        public List<IdentityPost> GetListLiked(IdentityFilter identity, out int returnCode)
//        {
//            return myRepository.GetListLiked(identity, out returnCode);
//        }

//        public List<IdentityPost> GetSaved(IdentityFilter identity)
//        {
//            return myRepository.GetSaved(identity);
//        }

//        public List<IdentityPost> GetTopByPlace(IdentityFilter identity, int placeId)
//        {
//            return myRepository.GetTopByPlace(identity, placeId);
//        }

//        public List<IdentityPost> SearchPosts(IdentityFilter identity)
//        {
//            return myRepository.SearchPosts(identity);
//        }

//        public IdentityPostDetail GetDetail(int id, int userId, int PageIndex, int PageSize)
//        {
//            return myRepository.GetDetail(id, userId, PageIndex, PageSize);
//        }

//        public IdentityPostDetail GetFullInfo(int id)
//        {
//            return myRepository.GetFullInfo(id);
//        }

//        public IdentityPost GetBaseInfo(int id)
//        {
//            return myRepository.GetBaseInfo(id);
//        }

//        public int Insert(IdentityPost identity, ref int code)
//        {
//            return myRepository.Insert(identity, ref code);
//        }

//        public bool Update(IdentityPost identity, List<IdentityPlace> places)
//        {
//            return myRepository.Update(identity, places);
//        }

//        public int Delete(IdentityPost identity)
//        {
//            return myRepository.Delete(identity);
//        }

//        #endregion

//        #region --- Post Action ---

//        public int AddActionLike(IdentityPostAction identity)
//        {
//            return myRepository.AddActionLike(identity);
//        }

//        public int AddActionReport(IdentityPostAction identity)
//        {
//            return myRepository.AddActionReport(identity);
//        }

//        public int AddActionShare(IdentityPostAction identity)
//        {
//            return myRepository.AddActionShare(identity);
//        }

//        public int AddActionRatingScore(IdentityPostAction identity)
//        {
//            return myRepository.AddActionRatingScore(identity);
//        }

//        public int SavePost(IdentityPostAction identity)
//        {
//            return myRepository.SavePost(identity);
//        }

//        #endregion


//    }

//    public interface IStorePost
//    {
//        List<IdentityPost> GetByPage(IdentityFilter identity, out int returnCode);

//        List<IdentityPost> GetByCategory(IdentityFilter identity, out int returnCode);

//        List<IdentityPost> GetListByDestination(IdentityFilter identity, out int returnCode);

//        List<IdentityPost> GetRecent(IdentityFilter identity, out int returnCode);

//        List<IdentityPost> GetListPosted(IdentityFilter identity, out int returnCode);

//        List<IdentityImage> GetUploadedImages(IdentityFilter identity, out int returnCode);

//        List<IdentityPost> GetListLiked(IdentityFilter identity, out int returnCode);

//        List<IdentityPost> GetSaved(IdentityFilter identity);

//        List<IdentityPost> GetTopByPlace(IdentityFilter identity, int placeId);

//        List<IdentityPost> SearchPosts(IdentityFilter identity);

//        IdentityPostDetail GetDetail(int id, int userId, int PageIndex, int PageSize);

//        IdentityPostDetail GetFullInfo(int id);

//        IdentityPost GetBaseInfo(int id);

//        int Insert(IdentityPost identity, ref int code);

//        bool Update(IdentityPost identity, List<IdentityPlace> places);

//        int Delete(IdentityPost identity);

//        int SavePost(IdentityPostAction identity);

//        int AddActionLike(IdentityPostAction identity);

//        int AddActionReport(IdentityPostAction identity);

//        int AddActionShare(IdentityPostAction identity);

//        int AddActionRatingScore(IdentityPostAction identity);
//    }
//}
