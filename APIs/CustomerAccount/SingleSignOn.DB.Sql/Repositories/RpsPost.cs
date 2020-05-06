using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SingleSignOn.ShareLibs.Exceptions;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.ShareLibs;
using Newtonsoft.Json;

namespace SingleSignOn.DB.Sql.Repositories
{
    public class RpsPost
    {
        private readonly string _connectionString;

        public RpsPost(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPost()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDB"].ConnectionString;
        }

        #region --- Post ----
        public List<IdentityPost> GetByPage(IdentityFilter identity, out int returnCode)
        {
            int offset = (identity.PageIndex - 1) * identity.PageSize;
            var parameters = new Dictionary<string, object>
                {
                    {"@Keyword", identity.Keyword},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey},
                    {"@Offset", offset},
                    {"@PageSize", identity.PageSize}
            };

            var sqlCmd = @"Post_GetByPage";

            List<IdentityPost> myList = null;
            returnCode = 1;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            returnCode = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            myList = ParsingPostData(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityPostDetail GetDetail(int postId, int userId, int PageIndex, int PageSize)
        {
            int offset = (PageIndex - 1) * PageSize;
            var parameters = new Dictionary<string, object>
            {
                    {"@Id", postId},
                    {"@UserId",userId },
                    {"@Offset", offset},
                    {"@PageSize", PageSize}
            };

            var sqlCmd = @"Post_GetDetail";

            IdentityPostDetail postDetail = new IdentityPostDetail();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        // Post
                        if (reader.Read())
                        {
                            postDetail.Post = ExtractPostItem(reader);
                            postDetail.Post.IsLike = Utils.ConvertToBoolean(reader["IsLike"], false);
                            postDetail.Post.UserRating = Utils.ConvertToDecimal(reader["UserRating"]);
                            postDetail.Post.UserName = reader["UserName"].ToString();
                            postDetail.Post.FullName = reader["FullName"].ToString();
                            postDetail.Post.DisplayName = reader["DisplayName"].ToString();
                            postDetail.Post.Avatar = reader["Avatar"].ToString();
                            postDetail.Post.CategoryName = reader["CategoryName"].ToString();
                        }
                        // Post Data
                        if (reader.NextResult())
                        {
                            if (reader.Read())
                                postDetail.PostData = ExtractPostData(reader);
                        }

                        // Post comment
                        if (reader.NextResult())
                        {

                            postDetail.PostComment = ParsingListPostComment(reader);
                        }

                        //// Post comment
                        //if (reader.NextResult())
                        //{
                        //    postDetail.CommentReply = ParsingListCommentReply(reader);
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return postDetail;
        }
        private IdentityPostData ExtractPostData(IDataReader reader)
        {
            var record = new IdentityPostData();
            record.Images = JsonConvert.DeserializeObject<List<IdentityImage>>(reader["Images"].ToString());
            record.Locations = JsonConvert.DeserializeObject<List<IdentityLocation>>(reader["Locations"].ToString());

            return record;
        }
        private List<IdentityComment> ParsingListPostComment(IDataReader reader)
        {
            List<IdentityComment> listData = new List<IdentityComment>();
            while (reader.Read())
            {
                var record = ExtractPostComment(reader);
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }
            return listData;
        }

        private List<IdentityCommentReply> ParsingListCommentReply(IDataReader reader)
        {
            List<IdentityCommentReply> listData = new List<IdentityCommentReply>();
            while (reader.Read())
            {
                var record = ExtractCommentReply(reader);
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }
            return listData;
        }

        private IdentityComment ExtractPostComment(IDataReader reader)
        {
            var record = new IdentityComment();
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.UserId = Utils.ConvertToInt32(reader["UserId"]);
            record.Content = reader["Content"].ToString();
            record.Like_Count = Utils.ConvertToInt32(reader["Like_Count"]);
            record.Comment_Count = Utils.ConvertToInt32(reader["Comment_Count"]);
            record.CreatedDate = (DateTime)reader["CreatedDate"];
            record.Avatar = reader["Avatar"].ToString();
            record.DisplayName = reader["DisplayName"].ToString();
            record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return record;
        }

        private IdentityCommentReply ExtractCommentReply(IDataReader reader)
        {
            var record = new IdentityCommentReply();

            record.PostCommentId = Utils.ConvertToInt32(reader["PostCommentId"]);
            record.UserId = Utils.ConvertToInt32(reader["UserId"]);
            record.Content = reader["Content"].ToString();
            record.Like_Count = Utils.ConvertToInt32(reader["Like_Count"]);
            record.CreatedDate = (DateTime)reader["CreatedDate"];
            record.Avatar = reader["Avatar"].ToString();
            record.DisplayName = reader["DisplayName"].ToString();
            record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return record;
        }

        private List<IdentityPost> ParsingPostData(IDataReader reader)
        {
            List<IdentityPost> listData = new List<IdentityPost>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractPostItem(reader);

                //Get extends 
                entity.Locations = reader["Locations"].ToString();
                entity.Images = reader["Images"].ToString();
                entity.UserName = reader["UserName"].ToString();
                entity.FullName = reader["FullName"].ToString();
                entity.DisplayName = reader["DisplayName"].ToString();
                entity.Avatar = reader["Avatar"].ToString();
                entity.CategoryName = reader["CategoryName"].ToString();

                entity.IsLike = Utils.ConvertToBoolean(reader["IsLike"], false);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityPost ExtractPostItem(IDataReader reader)
        {
            var entity = new IdentityPost();

            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.UserId = Utils.ConvertToInt32(reader["UserId"]);
            entity.Days = Utils.ConvertToInt32(reader["Days"]);
            entity.Title = reader["Title"].ToString();
            entity.ShortDescription = reader["ShortDescription"].ToString();
            entity.Description = reader["Description"].ToString();
            entity.Like_Count = Utils.ConvertToInt32(reader["Like_Count"]);
            entity.Share_Count = Utils.ConvertToInt32(reader["Share_Count"]);
            entity.Comment_Count = Utils.ConvertToInt32(reader["Comment_Count"]);
            entity.Report_Count = Utils.ConvertToInt32(reader["Report_Count"]);
            entity.Saved_Count = Utils.ConvertToInt32(reader["Saved_Count"]);
            entity.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);
            entity.CreatedDate = (DateTime)reader["CreatedDate"];
            entity.Status = Utils.ConvertToInt32(reader["Status"]);
            entity.Avatar = reader["Avatar"].ToString();
            entity.Rating_Count = Utils.ConvertToInt32(reader["Rating_Count"]);
            entity.RatingScore = Utils.ConvertToDecimal(reader["RatingScore"]);

            return entity;
        }

        public IdentityPost GetBaseInfo(int id)
        {
            var sqlCmd = @"Post_GetBaseInfo";
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            IdentityPost info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractPostItem(reader);

                            //Extends
                            info.UserName = reader["UserName"].ToString();
                            info.FullName = reader["FullName"].ToString();
                            info.DisplayName = reader["DisplayName"].ToString();
                            info.Images = reader["Images"].ToString();
                            info.Locations = reader["Locations"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_GetBaseInfo. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public int Insert(IdentityPost identity, ref int code)
        {
            var sqlCmd = @"Post_Insert";
            var newId = 0;

            var parameters = new Dictionary<string, object>
            {
                    {"@UserId", identity.UserId},
                    {"@Days", identity.Days},
                    {"@Title", identity.Title},
                    {"@ShortDescription", identity.ShortDescription},
                    {"@Description", identity.Description},
                    {"@CategoryId", identity.CategoryId},
                    {"@Locations", identity.Locations},
                    {"@Images", identity.Images},
                    {"@TokenKey", identity.TokenKey},
                    {"@TotalImages", identity.TotalImages},
                    {"@TotalLocations", identity.TotalLocations}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        code = Convert.ToInt32(reader[0]);
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            newId = Utils.ConvertToInt32(reader[0]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityPost identity)
        {
            var sqlCmd = @"Post_Update";
            string locations = JsonConvert.SerializeObject(identity.Locations);
            string images = JsonConvert.SerializeObject(identity.Images);

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", identity.Id},
                    {"@UserId", identity.UserId},
                    {"@Days", identity.Days},
                    {"@Title", identity.Title},
                    {"@ShortDescription", identity.ShortDescription},
                    {"@Description", identity.Description},
                    {"@Locations", locations},
                    {"@Images", images},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(IdentityPost identity)
        {
            var sqlCmd = @"Post_Delete";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", identity.Id},
                    {"@UserId", identity.UserId},
                    {"@Status", identity.Status},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public List<IdentityPost> GetRecent(IdentityFilter identity, out int returnCode)
        {
            int offset = (identity.PageIndex - 1) * identity.PageSize;
            var parameters = new Dictionary<string, object>
                {
                    {"@Keyword", identity.Keyword},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey},
                    {"@Offset", offset},
                    {"@PageSize", identity.PageSize}
            };

            var sqlCmd = @"Post_GetRecent";

            List<IdentityPost> myList = null;
            returnCode = 1;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            returnCode = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            myList = new List<IdentityPost>();
                            while (reader.Read())
                            {
                                var entity = ExtractPostItem(reader);

                                //Extends
                                entity.Avatar = reader["Avatar"].ToString();
                                entity.Days = Utils.ConvertToInt32(reader["Days"]);
                                entity.Locations = reader["Locations"].ToString();

                                myList.Add(entity);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_GetRecent. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        #endregion

        #region --- Post Action ---
        public int AddActionLike(IdentityPostAction identity)
        {
            var sqlCmd = @"Post_AddActionLike";

            var parameters = new Dictionary<string, object>
            {
                    {"@PostId", identity.PostId},
                    {"@ActionType", identity.ActionType},
                    {"@RatingScore", identity.RatingScore},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_AddActionLike. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int AddActionReport(IdentityPostAction identity)
        {
            var sqlCmd = @"Post_AddActionReport";

            var parameters = new Dictionary<string, object>
            {
                    {"@PostId", identity.PostId},
                    {"@ActionType", identity.ActionType},
                    {"@RatingScore", identity.RatingScore},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_AddActionReport. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int AddActionShare(IdentityPostAction identity)
        {
            var sqlCmd = @"Post_AddActionShare";

            var parameters = new Dictionary<string, object>
            {
                   {"@PostId", identity.PostId},
                   {"@ActionType", identity.ActionType},
                   {"@RatingScore", identity.RatingScore},
                   {"@UserId", identity.UserId},
                   {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_AddActionShare. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int AddActionRatingScore(IdentityPostAction identity)
        {
            var sqlCmd = @"Post_AddActionRatingScore";

            var parameters = new Dictionary<string, object>
            {
                   {"@PostId", identity.PostId},
                   {"@ActionType", identity.ActionType},
                   {"@RatingScore", identity.RatingScore},
                   {"@UserId", identity.UserId},
                   {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_AddActionRatingScore. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }
        #endregion

        #region --- Post Comment ---
        public List<IdentityComment> GetCommentByPage(IdentityCommentFilter identity)
        {
            var sqlCmd = @"Comment_GetByPage";
            var myList = new List<IdentityComment>();
            var offset = (identity.PageIndex - 1) * identity.PageSize;

            var parameters = new Dictionary<string, object>
            {
                    {"@PostId", identity.PostId},
                    {"@UserId", identity.UserId},
                    {"@Offset", offset},
                    {"@PageSize", identity.PageSize}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    myList = ParsingListPostComment(reader);

                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to get comment by page. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return myList;
        }

        public IdentityComment AddComment(IdentityComment identity, ref int code)
        {
            var sqlCmd = @"Post_AddComment";
            var record = new IdentityComment();
            var parameters = new Dictionary<string, object>
            {
                    {"@PostId", identity.PostId},
                    {"@Content", identity.Content},
                    //{"@Images", identity.Images},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        code = Convert.ToInt32(reader[0]);
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            record = ExtractPostComment(reader);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_AddComment. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return record;
        }

        public int UpdateComment(IdentityComment identity)
        {
            var sqlCmd = @"Post_UpdateComment";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", identity.Id},
                    {"@Content", identity.Content},
                    //{"@Images", identity.Images},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_UpdateComment. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int DeleteComment(IdentityComment identity)
        {
            var sqlCmd = @"Post_DeleteComment";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", identity.Id},
                    {"@Status", identity.Status},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_DeleteComment. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }
        #endregion

        #region  --- Post Reply Comment ---

        public List<IdentityCommentReply> GetCommentReplyByPage(IdentityCommentReplyFilter identity)
        {
            var sqlCmd = @"CommentReply_GetByPage";
            var myList = new List<IdentityCommentReply>();
            var offset = (identity.PageIndex - 1) * identity.PageSize;

            var parameters = new Dictionary<string, object>
            {
                    {"@CommentId", identity.CommnentId},
                    {"@UserId", identity.UserId},
                    {"@Offset", offset},
                    {"@PageSize", identity.PageSize}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    myList = ParsingListCommentReply(reader);

                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to get comment reply by page. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return myList;
        }

        public IdentityCommentReply AddCommentReply(IdentityCommentReply identity, ref int code)
        {
            var sqlCmd = @"Post_AddCommentReply";
            var record = new IdentityCommentReply();
            var parameters = new Dictionary<string, object>
            {
                    {"@PostCommentId", identity.PostCommentId},
                    {"@Content", identity.Content},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        code = Convert.ToInt32(reader[0]);
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            record = ExtractCommentReply(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_AddCommentReply. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return record;
        }

        public int UpdateCommentReply(IdentityCommentReply identity)
        {
            var sqlCmd = @"Post_UpdateCommentReply";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", identity.Id},
                    {"@Content", identity.Content},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_UpdateCommentReply. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int DeleteCommentReply(IdentityCommentReply identity)
        {
            var sqlCmd = @"Post_DeleteCommentReply";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", identity.Id},
                    {"@Status", identity.Status},
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Post_DeleteCommentReply. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public List<IdentityPost> GetListPosted(IdentityFilter identity, out int returnCode)
        {
            int offset = (identity.PageIndex - 1) * identity.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@OwnerId", identity.OwnerId},
                {"@Keyword", identity.Keyword},
                {"@UserId", identity.UserId},
                {"@TokenKey", identity.TokenKey},
                {"@Offset", offset},
                {"@PageSize", identity.PageSize}
            };

            var sqlCmd = @"User_GetListPosted";

            List<IdentityPost> myList = null;
            returnCode = 1;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            returnCode = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            myList = ParsingPostData(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetListPosted. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<IdentityImage> ParseImagesData(string imagesStr)
        {
            var myList = new List<IdentityImage>();

            if (!string.IsNullOrEmpty(imagesStr))
            {
                myList = JsonConvert.DeserializeObject<List<IdentityImage>>(imagesStr);

            }

            return myList;
        }

        public List<IdentityImage> GetUploadedImages(IdentityFilter identity, out int returnCode)
        {
            int offset = (identity.PageIndex - 1) * identity.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@OwnerId", identity.OwnerId},
                {"@UserId", identity.UserId},
                {"@TokenKey", identity.TokenKey},
                {"@Offset", offset},
                {"@PageSize", identity.PageSize}
            };

            var sqlCmd = @"User_GetUploadedImages";

            List<IdentityImage> myList = new List<IdentityImage>();
            returnCode = 1;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            returnCode = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var newList = ParseImagesData(reader["Images"].ToString());

                                myList.AddRange(newList);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetUploadedImages. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<IdentityPost> GetListLiked(IdentityFilter identity, out int returnCode)
        {
            int offset = (identity.PageIndex - 1) * identity.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@OwnerId", identity.OwnerId},
                {"@Keyword", identity.Keyword},
                {"@UserId", identity.UserId},
                {"@TokenKey", identity.TokenKey},
                {"@Offset", offset},
                {"@PageSize", identity.PageSize}
            };

            var sqlCmd = @"User_GetListLiked";

            List<IdentityPost> myList = null;
            returnCode = 1;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            returnCode = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            myList = ParsingPostData(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetListLiked. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        #endregion
    }
}
