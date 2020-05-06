//using System;
//using System.Linq;
//using System.Configuration;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using ApiJobMarket.ShareLibs.Exceptions;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.ShareLibs;
//using Newtonsoft.Json;

//namespace ApiJobMarket.DB.Sql.Repositories
//{
//    public class RpsPost
//    {
//        private readonly string _connectionString;

//        public RpsPost(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public RpsPost()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
//        }

//        #region --- Post ----
//        public List<IdentityPost> GetByPage(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Keyword", identity.Keyword},
//                {"@UserId", identity.UserId},
//                {"@TokenKey", identity.TokenKey},
//                {"@Offset", offset},
//                {"@PageSize", identity.PageSize},
//                {"@LangCode", identity.LangCode}
//            };

//            var sqlCmd = @"Post_GetByPage";

//            List<IdentityPost> myList = null;
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {

//                        if (reader.Read())
//                        {
//                            returnCode = Utils.ConvertToInt32(reader[0]);
//                        }

//                        if (reader.NextResult())
//                        {
//                            myList = ParsingPostData(reader, identity.UserId);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetByPage. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> GetByCategory(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@Keyword", identity.Keyword},
//                    {"@CategoryId", identity.CategoryId},
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Offset", offset},
//                    {"@PageSize", identity.PageSize},
//                    {"@LangCode", identity.LangCode}
//            };

//            var sqlCmd = @"Post_GetByCategory";

//            List<IdentityPost> myList = null;
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        myList = ParsingPostData(reader, identity.UserId);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetByCategory. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> GetListByDestination(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@Keyword", identity.Keyword},
//                    {"@DistrictId", identity.DistrictId},
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Offset", offset},
//                    {"@PageSize", identity.PageSize},
//                    {"@LangCode", identity.LangCode}
//            };

//            var sqlCmd = @"Post_GetListByDestination";

//            List<IdentityPost> myList = null;
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        myList = ParsingPostData(reader, identity.UserId);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetListByDestination. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> GetListPosted(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                {"@OwnerId", identity.OwnerId},
//                {"@Keyword", identity.Keyword},
//                {"@UserId", identity.UserId},
//                {"@TokenKey", identity.TokenKey},
//                {"@Offset", offset},
//                {"@PageSize", identity.PageSize},
//                {"@LangCode", identity.LangCode}
//            };

//            var sqlCmd = @"User_GetListPosted";

//            List<IdentityPost> myList = null;
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            returnCode = Utils.ConvertToInt32(reader[0]);
//                        }

//                        if (reader.NextResult())
//                        {
//                            myList = ParsingPostData(reader, identity.UserId);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetListPosted. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> GetTopByPlace(IdentityFilter identity, int placeId)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                {"@PlaceId", placeId},
//                {"@PageSize", identity.PageSize},
//                {"@UserId", identity.UserId},
//                {"@TokenKey", identity.TokenKey}
//            };

//            var sqlCmd = @"Post_GetTopByPlace";

//            List<IdentityPost> myList = new List<IdentityPost>();

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            //Get base post info
//                            var info = ExtractPostItem(reader);

//                            myList.Add(info);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetTopByPlace. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> SearchPosts(IdentityFilter identity)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Keyword", identity.Keyword},
//                {"@UserId", identity.UserId},
//                {"@TokenKey", identity.TokenKey},
//                {"@Offset", offset},
//                {"@PageSize", identity.PageSize},
//                {"@LangCode", identity.LangCode}
//            };

//            var sqlCmd = @"Search_Post";

//            List<IdentityPost> myList = new List<IdentityPost>();

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        myList = ParsingPostData(reader, identity.UserId);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Search_Post. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityImage> ParseImagesData(string imagesStr)
//        {
//            var myList = new List<IdentityImage>();

//            if (!string.IsNullOrEmpty(imagesStr))
//            {
//                myList = JsonConvert.DeserializeObject<List<IdentityImage>>(imagesStr);

//            }

//            return myList;
//        }

//        public List<IdentityImage> GetUploadedImages(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                {"@OwnerId", identity.OwnerId},
//                {"@UserId", identity.UserId},
//                {"@TokenKey", identity.TokenKey},
//                {"@Offset", offset},
//                {"@PageSize", identity.PageSize}
//            };

//            var sqlCmd = @"User_GetUploadedImages";

//            List<IdentityImage> myList = new List<IdentityImage>();
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            returnCode = Utils.ConvertToInt32(reader[0]);
//                        }

//                        if (reader.NextResult())
//                        {
//                            while (reader.Read())
//                            {
//                                var newList = ParseImagesData(reader["Images"].ToString());

//                                myList.AddRange(newList);
//                            }
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetUploadedImages. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> GetListLiked(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                {"@OwnerId", identity.OwnerId},
//                {"@Keyword", identity.Keyword},
//                {"@UserId", identity.UserId},
//                {"@TokenKey", identity.TokenKey},
//                {"@Offset", offset},
//                {"@PageSize", identity.PageSize}
//            };

//            var sqlCmd = @"User_GetListLiked";

//            List<IdentityPost> myList = null;
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            returnCode = Utils.ConvertToInt32(reader[0]);
//                        }

//                        if (reader.NextResult())
//                        {
//                            myList = ParsingPostData(reader, identity.UserId);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetListLiked. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public IdentityPostDetail GetDetail(int postId, int userId, int PageIndex, int PageSize)
//        {
//            int offset = (PageIndex - 1) * PageSize;
//            var parameters = new Dictionary<string, object>
//            {
//                    {"@Id", postId},
//                    {"@UserId",userId },
//                    {"@Offset", offset},
//                    {"@PageSize", PageSize}
//            };

//            var sqlCmd = @"Post_GetDetail";

//            IdentityPostDetail postDetail = new IdentityPostDetail();
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        // Post
//                        if (reader.Read())
//                        {
//                            postDetail.Post = ExtractPostItem(reader);
//                            postDetail.Post.IsLike = Utils.ConvertToBoolean(reader["IsLike"], false);
//                            postDetail.Post.UserRating = Utils.ConvertToDecimal(reader["UserRating"]);

//                            postDetail.Post.CategoryName = reader["CategoryName"].ToString();
//                        }
//                        // Post Data
//                        if (reader.NextResult())
//                        {
//                            if (reader.Read())
//                                postDetail.PostData = ExtractPostDataItem(reader);
//                        }

//                        // Post comment
//                        if (reader.NextResult())
//                        {
//                            postDetail.PostComment = new RpsComment().ParsingListData(reader, userId);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetDetail. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return postDetail;
//        }

//        public IdentityPostDetail GetFullInfo(int postId)
//        {
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Id", postId}
//            };

//            var sqlCmd = @"Post_GetFullInfo";

//            IdentityPostDetail postDetail = new IdentityPostDetail();
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        // Post
//                        if (reader.Read())
//                        {
//                            postDetail.Post = ExtractPostItem(reader);
//                        }
//                        // Post Data (Image,...)
//                        if (reader.NextResult())
//                        {
//                            if (reader.Read())
//                                postDetail.PostData = ExtractPostDataItem(reader);
//                        }

//                        // Post location
//                        if (reader.NextResult())
//                        {
//                            postDetail.PostLocations = ExtractPostLocation(reader);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetFullInfo. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return postDetail;
//        }

//        private List<IdentityPost> ParsingPostData(IDataReader reader, int userId)
//        {
//            List<IdentityPost> listData = new List<IdentityPost>();
//            while (reader.Read())
//            {
//                //Get post info
//                var info = ExtractPostItem(reader);
//                if (reader["CategoryName"] != null)
//                {
//                    info.CategoryName = reader["CategoryName"].ToString();
//                }

//                if (reader["CategoryFriendlyUrl"] != null)
//                {
//                    info.CategoryFriendlyUrl = reader["CategoryFriendlyUrl"].ToString();
//                }

//                //Get extends 
//                if (listData.Count == 0)
//                {
//                    info.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);
//                }
//                //Get post data

//                listData.Add(info);
//            }

//            if (listData != null && listData.Count > 0)
//            {
//                listData = SetDataToPost(listData, userId);
//            }

//            return listData;
//        }
//        private List<IdentityPost> SetDataToPost(List<IdentityPost> listPosts, int userId)
//        {
//            var myListIds = new List<string>();
//            foreach (var item in listPosts)
//            {
//                myListIds.Add(item.Id.ToString());
//            }
//            var strlist = String.Join(",", myListIds);

//            var ListPostLike = new List<int>();
//            var listPostData = GetPostDataByPostId(strlist, userId, ref ListPostLike);

//            foreach (var item in listPosts)
//            {
//                if (listPostData != null && listPostData.Count > 0)
//                {
//                    foreach (var itemdata in listPostData)
//                    {
//                        if (item.Id == itemdata.PostId)
//                        {
//                            item.Images = itemdata.Images;
//                            item.TotalLocations = itemdata.TotalLocations;
//                            item.ListLocations = itemdata.ListLocations;
//                            break;
//                        }
//                    }
//                }
//                if (ListPostLike != null && ListPostLike.Count > 0)
//                {
//                    foreach (var itemlike in ListPostLike)
//                    {
//                        if (item.Id == itemlike)
//                        {
//                            item.IsLike = true;
//                            break;
//                        }
//                    }
//                }
//            }

//            return listPosts;
//        }

//        private IdentityPost ExtractPostItem(IDataReader reader)
//        {
//            var entity = new IdentityPost();

//            entity.Id = Utils.ConvertToInt32(reader["Id"]);
//            entity.UserId = Utils.ConvertToInt32(reader["UserId"]);
//            entity.Days = Utils.ConvertToInt32(reader["Days"]);
//            entity.Title = reader["Title"].ToString();
//            entity.Description = reader["Description"].ToString();
//            entity.Like_Count = Utils.ConvertToInt32(reader["Like_Count"]);
//            entity.Share_Count = Utils.ConvertToInt32(reader["Share_Count"]);
//            entity.Comment_Count = Utils.ConvertToInt32(reader["Comment_Count"]);
//            entity.Report_Count = Utils.ConvertToInt32(reader["Report_Count"]);
//            entity.Saved_Count = Utils.ConvertToInt32(reader["Saved_Count"]);
//            entity.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);
//            entity.CreatedDate = (DateTime)reader["CreatedDate"];
//            entity.Rating_Count = Utils.ConvertToInt32(reader["Rating_Count"]);
//            entity.RatingScore = Utils.ConvertToDecimal(reader["RatingScore"]);

//            return entity;
//        }

//        public IdentityPost GetBaseInfo(int id)
//        {
//            var sqlCmd = @"Post_GetBaseInfo";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Id", id}
//            };

//            IdentityPost info = null;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            info = ExtractPostItem(reader);

//                            //Extends
//                            if (info != null && info.Id > 0)
//                            {
//                                var ListPostId = new List<int>();
//                                var postdata = GetPostDataByPostId(info.Id.ToString(), 0, ref ListPostId);
//                                if (postdata != null && postdata.Count > 0)
//                                {
//                                    info.Images = postdata[0].Images;
//                                    //info.Locations = JsonConvert.SerializeObject(postdata[0].Locations);
//                                }
//                                if (ListPostId != null && ListPostId.Count > 0)
//                                {
//                                    info.IsLike = true;
//                                }
//                            }

//                            if (reader.NextResult())
//                            {
//                                var listPlace = ExtractPlaceData(reader);
//                                info.ListLocations = listPlace;                                
//                            }
//                        }

//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetBaseInfo. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return info;
//        }

//        public int Insert(IdentityPost identity, ref int code)
//        {
//            var sqlCmd = @"Post_Insert";
//            var newId = 0;
//            var queryLocation = "";
//            if (identity.Locations != null && identity.Locations.Count > 0)
//            {
//                queryLocation = string.Join(",", identity.Locations);
//            }
//            var listLocation = new List<IdentityPostPlace>();
//            var listImages = JsonConvert.SerializeObject(identity.Images);
//            var parameters = new Dictionary<string, object>
//            {
//                    {"@UserId", identity.UserId},
//                    {"@Days", identity.Days},
//                    {"@Title", identity.Title},
//                    {"@ShortDescription", identity.ShortDescription},
//                    {"@Description", identity.Description},
//                    {"@Images", listImages},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@TotalImages", identity.TotalImages},
//                    {"@TotalLocations", identity.TotalLocations},
//                    {"@Locations", queryLocation},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
//                    if (reader.Read())
//                    {
//                        code = Convert.ToInt32(reader[0]);
//                    }

//                    if (reader.NextResult())
//                    {
//                        if (reader.Read())
//                        {
//                            newId = Utils.ConvertToInt32(reader[0]);
//                        }
//                    }
//                    if (reader.NextResult())
//                    {
//                        listLocation = ExtractPostPlace(reader);
//                    }
//                    if (newId > 0 && listLocation != null && listLocation.Count > 0)
//                    {
//                        //Insert bulk
//                        PlacesInsertBulk(listLocation, newId);

//                        UpdateCountPostByPlace(listLocation);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_Insert. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return newId;
//        }

//        public void UpdateCountPostByPlace(List<IdentityPostPlace> listPostPlaces)
//        {
//            var listProvince = listPostPlaces.GroupBy(s => s.ProvinceId).Select(gr => gr.First()).Select(s => s.ProvinceId).ToList();

//            var listDistrict = listPostPlaces.Where(s => s.DistrictId != s.ProvinceId).GroupBy(s => s.DistrictId).Select(gr => gr.First()).Select(s => s.DistrictId).ToList();

//            if (listProvince != null && listProvince.Count > 0)
//            {
//                foreach (var item in listProvince)
//                {
//                    UpdateCountPostByProvince(item);
//                }
//            }

//            if (listDistrict != null && listDistrict.Count > 0)
//            {
//                foreach (var item in listDistrict)
//                {
//                    UpdateCountPostByDistrict(item);
//                }
//            }
//        }

//        public void UpdateCountPostByProvince(int provinceId)
//        {
//            var sqlCmd = @"Place_UpdateCountProvince";
//            var parameters = new Dictionary<string, object>
//            {
//                    {"@ProvinceId", provinceId},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var reader = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Place_UpdateCountProvince. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public void UpdateCountPostByDistrict(int districtId)
//        {
//            var sqlCmd = @"Place_UpdateCountDistrict";
//            var parameters = new Dictionary<string, object>
//            {
//                    {"@DistrictId", districtId},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var reader = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Place_UpdateCountDistrict. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public List<IdentityPostPlace> ExtractPostPlace(IDataReader reader)
//        {
//            List<IdentityPostPlace> listResult = new List<IdentityPostPlace>();
//            while (reader.Read())
//            {
//                var record = new IdentityPostPlace();
//                record.PlaceId = Utils.ConvertToInt32(reader["PlaceId"]);
//                record.ProvinceId = Utils.ConvertToInt32(reader["ProvinceId"]);
//                record.DistrictId = Utils.ConvertToInt32(reader["DistrictId"]);

//                listResult.Add(record);
//            }
//            return listResult;
//        }

//        public List<IdentityPostPlace> ExtractPostPlaceFromList(List<IdentityPlace> places)
//        {
//            List<IdentityPostPlace> listResult = new List<IdentityPostPlace>();
//            if (places != null && places.Count > 0)
//            {
//                foreach (var place in places)
//                {
//                    var record = new IdentityPostPlace();
//                    record.PlaceId = Utils.ConvertToInt32(place.Id);
//                    record.ProvinceId = place.ProvinceId;
//                    record.DistrictId = place.DistrictId;

//                    listResult.Add(record);
//                }
//            }

//            return listResult;
//        }

//        public int PlacesInsertBulk(List<IdentityPostPlace> places, int postId)
//        {
//            var result = -1;
//            using (SqlConnection connection =
//                   new SqlConnection(_connectionString))
//            {
//                connection.Open();



//                // Create the SqlBulkCopy object. 
//                // Note that the column positions in the source DataTable 
//                // match the column positions in the destination table so 
//                // there is no need to map columns. 
//                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
//                {
//                    bulkCopy.DestinationTableName = "dbo.tbl_post_place";

//                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("PostId", "PostId"));
//                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("PlaceId", "PlaceId"));
//                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("DistrictId", "DistrictId"));
//                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping("ProvinceId", "ProvinceId"));
//                    try
//                    {
//                        // Create a table with some rows. 
//                        DataTable newProducts = PlacesTempTable(places, postId);

//                        // Write from the source to the destination.
//                        bulkCopy.WriteToServer(newProducts);

//                        result = 1;
//                    }
//                    catch (Exception ex)
//                    {
//                        var strError = "Failed to call PlacesInsertBulk. Error: " + ex.Message;
//                        throw new CustomSQLException(strError);
//                    }
//                    finally
//                    {
//                        connection.Dispose();
//                    }
//                }
//            }

//            return result;
//        }

//        private static DataTable PlacesTempTable(List<IdentityPostPlace> places, int postId)
//        {
//            DataTable newTable = new DataTable("places");

//            // Add three column objects to the table. 
//            DataColumn dtPostId = new DataColumn();
//            dtPostId.DataType = System.Type.GetType("System.Int64");
//            dtPostId.ColumnName = "PostId";
//            newTable.Columns.Add(dtPostId);

//            DataColumn placeId = new DataColumn();
//            placeId.DataType = System.Type.GetType("System.Int64");
//            placeId.ColumnName = "PlaceId";
//            newTable.Columns.Add(placeId);

//            DataColumn districtId = new DataColumn();
//            districtId.DataType = System.Type.GetType("System.Int64");
//            districtId.ColumnName = "DistrictId";
//            newTable.Columns.Add(districtId);

//            DataColumn provinceId = new DataColumn();
//            provinceId.DataType = System.Type.GetType("System.Int64");
//            provinceId.ColumnName = "ProvinceId";
//            newTable.Columns.Add(provinceId);

//            // Create an array for DataColumn objects.
//            //DataColumn[] keys = new DataColumn[1];
//            //keys[0] = productID;
//            //newTable.PrimaryKey = keys;
//            if (places != null && places.Count > 0)
//            {
//                // Add some new rows to the collection. 
//                DataRow row = null;
//                for (int i = 0; i < places.Count; i++)
//                {
//                    var item = places[i];
//                    row = newTable.NewRow();
//                    row["PostId"] = postId;
//                    row["PlaceId"] = item.PlaceId;
//                    if (item.DistrictId == 0)
//                    {
//                        if (item.ProvinceId == 0)
//                        {
//                            item.ProvinceId = item.PlaceId;
//                        }
//                        item.DistrictId = item.ProvinceId;

//                    }
//                    row["DistrictId"] = item.DistrictId;
//                    row["ProvinceId"] = item.ProvinceId;
//                    newTable.Rows.Add(row);
//                }
//            }

//            newTable.AcceptChanges();
//            return newTable;
//        }

//        public bool Update(IdentityPost identity, List<IdentityPlace> places)
//        {
//            var sqlCmd = @"Post_Update";
//            var queryLocation = "";
//            var code = 0;
//            if (identity.Locations != null && identity.Locations.Count > 0)
//            {
//                queryLocation = string.Join(",", identity.Locations);
//            }

//            var postPlaces = new List<IdentityPostPlace>();
//            var listImages = JsonConvert.SerializeObject(identity.Images);
//            var parameters = new Dictionary<string, object>
//            {
//                {"@PostId", identity.Id},
//                {"@UserId", identity.UserId},
//                {"@Days", identity.Days},
//                {"@Title", identity.Title},
//                {"@Description", identity.Description},
//                {"@Images", listImages},
//                {"@TokenKey", identity.TokenKey},
//                {"@TotalImages", identity.TotalImages},
//                {"@TotalLocations", identity.TotalLocations},
//                {"@Locations", queryLocation}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    if (reader.Read())
//                    {
//                        code = Utils.ConvertToInt32(reader[0]);
//                    }

//                    postPlaces = ExtractPostPlaceFromList(places);

//                    if (code == 1)
//                    {
//                        if (postPlaces != null && postPlaces.Count > 0)
//                        {
//                            //Insert bulk
//                            PlacesInsertBulk(postPlaces, identity.Id);

//                            UpdateCountPostByPlace(postPlaces);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_Update. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return true;
//        }

//        public int Delete(IdentityPost identity)
//        {
//            var sqlCmd = @"Post_Delete";

//            var parameters = new Dictionary<string, object>
//            {
//                    {"@Id", identity.Id},
//                    {"@UserId", identity.UserId},
//                    {"@Status", identity.Status},
//                    {"@TokenKey", identity.TokenKey}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_Delete. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public List<IdentityPost> GetRecent(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@Keyword", identity.Keyword},
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Offset", offset},
//                    {"@PageSize", identity.PageSize}
//            };

//            var sqlCmd = @"Post_GetRecent";

//            List<IdentityPost> myList = null;
//            returnCode = 1;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            returnCode = Utils.ConvertToInt32(reader[0]);
//                        }

//                        if (reader.NextResult())
//                        {
//                            myList = new List<IdentityPost>();
//                            while (reader.Read())
//                            {
//                                var entity = ExtractPostItem(reader);

//                                myList.Add(entity);
//                            }

//                            if (myList != null && myList.Count > 0)
//                            {
//                                myList = SetDataToPost(myList, identity.UserId);
//                            }
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_GetRecent. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }

//        public List<IdentityPost> GetSaved(IdentityFilter identity)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@OwnerId", identity.OwnerId},
//                    {"@Keyword", identity.Keyword},
//                    {"@Offset", offset},
//                    {"@PageSize", identity.PageSize},
//                    {"@LangCode", identity.LangCode}
//            };

//            var sqlCmd = @"User_GetListSaved";

//            List<IdentityPost> myList = null;

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        myList = new List<IdentityPost>();
//                        while (reader.Read())
//                        {
//                            var entity = ExtractPostItem(reader);

//                            if (reader["CategoryName"] != null)
//                            {
//                                entity.CategoryName = reader["CategoryName"].ToString();
//                            }

//                            if (reader["CategoryFriendlyUrl"] != null)
//                            {
//                                entity.CategoryFriendlyUrl = reader["CategoryFriendlyUrl"].ToString();
//                            }

//                            myList.Add(entity);
//                        }

//                        if (myList != null && myList.Count > 0)
//                        {
//                            myList = SetDataToPost(myList, identity.UserId);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetListSaved. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }
//        #endregion

//        #region --- Post Action ---

//        public int AddActionLike(IdentityPostAction identity)
//        {
//            var sqlCmd = @"Post_AddActionLike";

//            var parameters = new Dictionary<string, object>
//            {
//                    {"@PostId", identity.PostId},
//                    {"@ActionType", identity.ActionType},
//                    {"@RatingScore", identity.RatingScore},
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_AddActionLike. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public int AddActionReport(IdentityPostAction identity)
//        {
//            var sqlCmd = @"Post_AddActionReport";

//            var parameters = new Dictionary<string, object>
//            {
//                    {"@PostId", identity.PostId},
//                    {"@ActionType", identity.ActionType},
//                    {"@RatingScore", identity.RatingScore},
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_AddActionReport. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public int AddActionShare(IdentityPostAction identity)
//        {
//            var sqlCmd = @"Post_AddActionShare";

//            var parameters = new Dictionary<string, object>
//            {
//                   {"@PostId", identity.PostId},
//                   {"@ActionType", identity.ActionType},
//                   {"@RatingScore", identity.RatingScore},
//                   {"@UserId", identity.UserId},
//                   {"@TokenKey", identity.TokenKey}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_AddActionShare. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public int AddActionRatingScore(IdentityPostAction identity)
//        {
//            var sqlCmd = @"Post_AddActionRatingScore";

//            var parameters = new Dictionary<string, object>
//            {
//                   {"@PostId", identity.PostId},
//                   {"@ActionType", identity.ActionType},
//                   {"@RatingScore", identity.RatingScore},
//                   {"@UserId", identity.UserId},
//                   {"@TokenKey", identity.TokenKey}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_AddActionRatingScore. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public int SavePost(IdentityPostAction identity)
//        {
//            var sqlCmd = @"Post_Save";
//            var returnId = 0;
//            var parameters = new Dictionary<string, object>
//            {
//                    {"@UserId", identity.UserId},
//                    {"@PostId", identity.PostId}
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    returnId = Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Post_Save. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return returnId;
//        }

//        #endregion

//        public List<IdentityPostData> GetPostDataByPostId(string listPostId, int UserId, ref List<int> ListPostId)
//        {
//            var sqlCmd = @"PostData_GetByListPostId";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@ListPostId", listPostId},
//                {"@UserId", UserId }
//            };
//            var listData = new List<IdentityPostData>();
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            var record = ExtractPostDataItem(reader);
//                            listData.Add(record);
//                        }
//                        if (reader.NextResult())
//                        {
//                            while (reader.Read())
//                            {
//                                ListPostId.Add(Utils.ConvertToInt32(reader["PostId"]));
//                            }
//                        }
//                        if (reader.NextResult())
//                        {
//                            var listPlace = ExtractPlaceData(reader);
//                            if (listData != null && listData.Count > 0)
//                            {
//                                if (listPlace != null && listPlace.Count > 0)
//                                {
//                                    foreach (var item in listData)
//                                    {
//                                          item.ListLocations= listPlace.Where(s => s.PostId == item.PostId).ToList();  
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to PostData_GetByListPostId. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }

//        private List<IdentityPlace> ExtractPlaceData(IDataReader reader)
//        {
//            var listItem = new List<IdentityPlace>();
//            while (reader.Read())
//            {
//                var record = new IdentityPlace();

//                //Seperate properties
//                record.Id = Utils.ConvertToInt32(reader["Id"]);
//                record.GName = reader["GName"].ToString();
//                record.GFullName = reader["GFullName"].ToString();
//                record.GType = reader["GType"].ToString();
//                record.GPlaceId = reader["GPlaceId"].ToString();
//                record.Province = reader["Province"].ToString();
//                record.District = reader["District"].ToString();
//                record.RawData = reader["RawData"].ToString();
//                record.PostId = Utils.ConvertToInt32(reader["PostId"]);
//                listItem.Add(record);
//            }

//            return listItem;
//        }

//        private IdentityPostData ExtractPostDataItem(IDataReader reader)
//        {
//            var record = new IdentityPostData();
//            record.Images = JsonConvert.DeserializeObject<List<IdentityImage>>(reader["Images"].ToString());
//            //if (record.Images != null && record.Images.Count > 0)
//            //{
//            //    foreach (var item in record.Images)
//            //    {
//            //        item.Url = ShareLibCdnHelper.GetFullImgPath(item.Url);
//            //    }
//            //}
//            record.TotalLocations = Utils.ConvertToInt32(reader["TotalLocations"]);

//            record.PostId = Utils.ConvertToInt32(reader["PostId"]);
//            return record;
//        }

//        private List<IdentityPlace> ExtractPostLocation(IDataReader reader)
//        {
//            List<IdentityPlace> locations = new List<IdentityPlace>();
//            while (reader.Read())
//            {
//                var record = new IdentityPlace();

//                record.Id = Utils.ConvertToInt32(reader["Id"]);
//                record.GName = reader["GName"].ToString();
//                record.GFullName = reader["GFullName"].ToString();
//                record.GId = reader["GId"].ToString();
//                record.GLat = reader["GLat"].ToString();
//                record.GLong = reader["GLong"].ToString();
//                record.GUrl = reader["GUrl"].ToString();
//                record.Icon = reader["Icon"].ToString();

//                locations.Add(record);
//            }

//            return locations;
//        }

//    }
//}
