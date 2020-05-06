using System.Configuration;
using SingleSignOn.DB.Sql.Repositories;
using SingleSignOn.DB.Sql.Entities;
using System.Collections.Generic;

namespace SingleSignOn.DB.Sql.Stores
{
    public class StorePost : IPostStore
    {
        private readonly string _connectionString;
        private RpsPost myRepository;

        public StorePost()
            : this("SingleSignOnDB")
        {

        }

        public StorePost(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            myRepository = new RpsPost(_connectionString);
        }

        #region --- Post ---
        public List<IdentityPost> GetByPage(IdentityFilter identity, out int returnCode)
        {
            return myRepository.GetByPage(identity, out returnCode);
        }

        public IdentityPostDetail GetDetail(int id, int userId, int PageIndex, int PageSize)
        {
            return myRepository.GetDetail(id, userId, PageIndex, PageSize);
        }

        public IdentityPost GetBaseInfo(int id)
        {
            return myRepository.GetBaseInfo(id);
        }

        public int Insert(IdentityPost identity, ref int code)
        {
            return myRepository.Insert(identity, ref code);
        }

        public int Update(IdentityPost identity)
        {
            return myRepository.Update(identity);
        }

        public int Delete(IdentityPost identity)
        {
            return myRepository.Delete(identity);
        }

        public List<IdentityPost> GetRecent(IdentityFilter identity, out int returnCode)
        {
            return myRepository.GetRecent(identity, out returnCode);
        }

        public List<IdentityPost> GetListPosted(IdentityFilter identity, out int returnCode)
        {
            return myRepository.GetListPosted(identity, out returnCode);
        }

        public List<IdentityImage> GetUploadedImages(IdentityFilter identity, out int returnCode)
        {
            return myRepository.GetUploadedImages(identity, out returnCode);
        }

        public List<IdentityPost> GetListLiked(IdentityFilter identity, out int returnCode)
        {
            return myRepository.GetListLiked(identity, out returnCode);
        }

        #endregion

        #region --- Post Action ---
        public int AddActionLike(IdentityPostAction identity)
        {
            return myRepository.AddActionLike(identity);
        }

        public int AddActionReport(IdentityPostAction identity)
        {
            return myRepository.AddActionReport(identity);
        }

        public int AddActionShare(IdentityPostAction identity)
        {
            return myRepository.AddActionShare(identity);
        }

        public int AddActionRatingScore(IdentityPostAction identity)
        {
            return myRepository.AddActionRatingScore(identity);
        }
        #endregion

        #region --- Post Comment ---
        public List<IdentityComment> GetCommentByPage(IdentityCommentFilter identity)
        {
            return myRepository.GetCommentByPage(identity);
        }

        public IdentityComment AddComment(IdentityComment identity, ref int code)
        {
            return myRepository.AddComment(identity, ref code);
        }

        public int UpdateComment(IdentityComment identity)
        {
            return myRepository.UpdateComment(identity);
        }

        public int DeleteComment(IdentityComment identity)
        {
            return myRepository.DeleteComment(identity);
        }
        #endregion

        #region --- Post Comment Reply ---

        public List<IdentityCommentReply> GetCommentReplyByPage(IdentityCommentReplyFilter identity)
        {
            return myRepository.GetCommentReplyByPage(identity);
        }

        public IdentityCommentReply AddCommentReply(IdentityCommentReply identity, ref int code)
        {
            return myRepository.AddCommentReply(identity, ref code);
        }

        public int UpdateCommentReply(IdentityCommentReply identity)
        {
            return myRepository.UpdateCommentReply(identity);
        }

        public int DeleteCommentReply(IdentityCommentReply identity)
        {
            return myRepository.DeleteCommentReply(identity);
        }
        #endregion
    }

    public interface IPostStore
    {
        List<IdentityPost> GetByPage(IdentityFilter identity, out int returnCode);

        IdentityPostDetail GetDetail(int id, int userId, int PageIndex, int PageSize);

        IdentityPost GetBaseInfo(int id);

        int Insert(IdentityPost identity, ref int code);

        int Update(IdentityPost identity);

        int Delete(IdentityPost identity);

        int AddActionLike(IdentityPostAction identity);

        int AddActionReport(IdentityPostAction identity);

        int AddActionShare(IdentityPostAction identity);

        int AddActionRatingScore(IdentityPostAction identity);

        List<IdentityComment> GetCommentByPage(IdentityCommentFilter identity);

        IdentityComment AddComment(IdentityComment identity, ref int code);

        int UpdateComment(IdentityComment identity);

        int DeleteComment(IdentityComment identity);

        List<IdentityCommentReply> GetCommentReplyByPage(IdentityCommentReplyFilter identity);

        IdentityCommentReply AddCommentReply(IdentityCommentReply identity, ref int code);

        int UpdateCommentReply(IdentityCommentReply identity);

        int DeleteCommentReply(IdentityCommentReply identity);

        List<IdentityPost> GetRecent(IdentityFilter identity, out int returnCode);

        List<IdentityPost> GetListPosted(IdentityFilter identity, out int returnCode);

        List<IdentityImage> GetUploadedImages(IdentityFilter identity, out int returnCode);

        List<IdentityPost> GetListLiked(IdentityFilter identity, out int returnCode);
    }
}
