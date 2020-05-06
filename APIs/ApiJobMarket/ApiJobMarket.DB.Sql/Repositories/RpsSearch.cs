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
//    public class RpsSearch
//    {
//        private readonly string _connectionString;

//        public RpsSearch(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public RpsSearch()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
//        }
//        #region == Post ==
//        public List<IdentityPost> GetPostByPage(IdentityFilter identity, out int returnCode)
//        {
//            int offset = (identity.PageIndex - 1) * identity.PageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@Keyword", identity.Keyword},
//                    {"@Offset", offset},
//                    {"@PageSize", identity.PageSize}
//            };

//            var sqlCmd = @"Search_Post";

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

//        private List<IdentityPost> ParsingPostData(IDataReader reader, int userId)
//        {
//            List<IdentityPost> listData = new List<IdentityPost>();
//            while (reader.Read())
//            {
//                //Get post info
//                var info = ExtractPostItem(reader);

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

//        #endregion

//        #region == Category ==

//        public List<IdentityPlace> GetCategoryByPage(IdentityPlace filter, int currentPage, int pageSize)
//        {
//            //Common syntax           
//            var sqlCmd = @"Search_Category";
//            List<IdentityPlace> listData = null;

//            //For paging 
//            int offset = (currentPage - 1) * pageSize;

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Keyword", filter.Keyword },
//                {"@Offset", offset},
//                {"@PageSize", pageSize},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        listData = new List<IdentityPlace>();

//                        while (reader.Read())
//                        {
//                            var item = ExtractPlaceData(reader);
//                            listData.Add(item);
//                        }
//                        if (listData != null && listData.Count > 0)
//                        {
//                            listData = GetStatisticsDetail(listData);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Place_GetBestdestination. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }
//        #endregion

//        #region == Destination And Place ==

//        public List<IdentityPlace> GetDestinationByPage(IdentityPlace filter, int currentPage, int pageSize)
//        {
//            //Common syntax           
//            var sqlCmd = @"Search_Destination";
//            List<IdentityPlace> listData = null;

//            //For paging 
//            int offset = (currentPage - 1) * pageSize;

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Keyword", filter.Keyword },
//                {"@Offset", offset},
//                {"@PageSize", pageSize},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        listData = new List<IdentityPlace>();

//                        while (reader.Read())
//                        {
//                            var item = ExtractPlaceData(reader);
//                            listData.Add(item);
//                        }
//                        if (listData != null && listData.Count > 0)
//                        {
//                            listData = GetStatisticsDetail(listData);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Search_Destination. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }

//        public List<IdentityPlace> GetPlaceByPage(IdentityPlace filter, int currentPage, int pageSize)
//        {
//            //Common syntax           
//            var sqlCmd = @"Search_Place";
//            List<IdentityPlace> listData = null;

//            //For paging 
//            int offset = (currentPage - 1) * pageSize;

//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Keyword", filter.Keyword },
//                {"@Offset", offset},
//                {"@PageSize", pageSize},
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        listData = new List<IdentityPlace>();

//                        while (reader.Read())
//                        {
//                            var item = ExtractPlaceData(reader);
//                            listData.Add(item);
//                        }
//                        if (listData != null && listData.Count > 0)
//                        {
//                            listData = GetStatisticsDetail(listData);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Search_Place. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return listData;
//        }
//        private IdentityPlace ExtractPlaceData(IDataReader reader, bool getShort = false)
//        {
//            var record = new IdentityPlace();

//            //Seperate properties
//            record.Id = Utils.ConvertToInt32(reader["Id"]);
//            record.GName = reader["GName"].ToString();
//            record.GFullName = reader["GFullName"].ToString();
//            record.GId = reader["GId"].ToString();
//            record.GLat = reader["GLat"].ToString();
//            record.GLong = reader["GLong"].ToString();
//            record.GUrl = reader["GUrl"].ToString();
//            record.Icon = reader["Icon"].ToString();
//            record.ProvinceId = Utils.ConvertToInt32(reader["ProvinceId"]);
//            record.DistrictId = Utils.ConvertToInt32(reader["DistrictId"]);

//            if (!getShort)
//            {
//                record.GType = reader["GType"].ToString();
//                record.GPlaceId = reader["GPlaceId"].ToString();
//                record.Province = reader["Province"].ToString();
//                record.District = reader["District"].ToString();
//                record.RawData = reader["RawData"].ToString();
//            }

//            record.Status = Utils.ConvertToInt32(reader["Status"]);
//            record.PostCount = Utils.ConvertToInt32(reader["PostCount"]);
//            record.PostCountProvince = Utils.ConvertToInt32(reader["PostCountProvince"]);

//            record.Cover = reader["Cover"].ToString();

//            return record;
//        }

//        public List<IdentityPlace> GetStatisticsDetail(List<IdentityPlace> myList, bool IsProvince = false)
//        {
//            //Common syntax           
//            var sqlCmd = @"Place_GetStatistics";
//            var listProvince = myList.Where(s => s.GType == "administrative_area_level_1").Select(x => x.Id);
//            var listDistrict = myList.Where(s => s.GType == "administrative_area_level_2").Select(x => x.Id);

//            var strListProvince = string.Join(",", listProvince);
//            var strListDistrict = string.Join(",", listDistrict);
//            if (strListProvince == "") strListProvince = "0";
//            if (strListDistrict == "") strListDistrict = "0";



//            List<IdentityPlace> returnList = new List<IdentityPlace>();
//            //For parameters
//            var parameters = new Dictionary<string, object>
//            {
//                {"@ListProvinceId", strListProvince },
//                {"@ListDistrictId", strListDistrict },
//            };

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            var item = new IdentityPlace();
//                            item.Id = Utils.ConvertToInt32(reader["Id"]);
//                            item.SubPlacesCount = Utils.ConvertToInt32(reader["SubPlacesCount"]);
//                            returnList.Add(item);
//                        }
//                        if (reader.NextResult())
//                        {
//                            while (reader.Read())
//                            {
//                                var item = new IdentityPlace();
//                                item.Id = Utils.ConvertToInt32(reader["Id"]);
//                                item.SubPlacesCount = Utils.ConvertToInt32(reader["SubPlacesCount"]);
//                                returnList.Add(item);
//                            }
//                        }

//                        if (returnList != null && returnList.Count > 0)
//                        {
//                            foreach (var data in myList)
//                            {
//                                foreach (var item in returnList)
//                                {
//                                    if (item.Id == data.Id)
//                                    {
//                                        data.SubPlacesCount = item.SubPlacesCount;
//                                        break;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to execute Place_GetStatistics. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return myList;
//        }
//        #endregion

//    }
//}
