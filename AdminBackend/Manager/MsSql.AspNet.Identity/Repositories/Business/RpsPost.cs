using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using Manager.SharedLibs;

namespace MsSql.AspNet.Identity.Repositories
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
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- Post ----

        public List<IdentityPost> GetByPage(IdentityPost filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@CategoryId", filter.CategoryId},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                //{"@SortField", filter.SortField },
                //{"@SortType", filter.SortType },
                {"@Status", filter.Status },
                {"@LangCode", filter.LangCode },
            };

            var sqlCmd = @"Post_GetByPage";

            List<IdentityPost> myList = new List<IdentityPost>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractPostItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if(myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPostLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.PostId)
                                            item.MyLanguages.Add(itemLang);
                                    }
                                }
                            }
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

        public IdentityPost GetById(int id, string langCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id},
                {"@LangCode", langCode}
            };

            var sqlCmd = @"Post_GetById";

            IdentityPost info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractPostItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPostLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }                            
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityPost F_GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"F_Post_GetDetailById";

            IdentityPost info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractPostItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPostLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_Post_GetDetailById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public List<IdentityPost> F_GetRelated(IdentityPost filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@Id", filter.Id},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@LangCode", filter.LangCode },
            };

            var sqlCmd = @"F_Post_GetRelated";

            List<IdentityPost> myList = new List<IdentityPost>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractPostItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPostLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.PostId)
                                            item.MyLanguages.Add(itemLang);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_Post_GetRelated. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<IdentityPost> F_GetByCategory(IdentityPost filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@CategoryId", filter.CategoryId},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@LangCode", filter.LangCode },
            };

            var sqlCmd = @"F_Post_GetByCategory";

            List<IdentityPost> myList = new List<IdentityPost>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractPostItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPostLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.PostId)
                                            item.MyLanguages.Add(itemLang);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_Post_GetByCategory. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<IdentityPost> F_SearchByPage(IdentityPost filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@LangCode", filter.LangCode }
            };

            var sqlCmd = @"F_Post_SearchByPage";

            List<IdentityPost> myList = new List<IdentityPost>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractPostItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPostLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.PostId)
                                            item.MyLanguages.Add(itemLang);
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_Post_SearchByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        private IdentityPostData ExtractPostData(IDataReader reader)
        {
            var record = new IdentityPostData();
            record.Images = JsonConvert.DeserializeObject<List<IdentityImage>>(reader["Images"].ToString());
            record.Locations = JsonConvert.DeserializeObject<List<IdentityLocation>>(reader["Locations"].ToString());

            return record;
        }

        private List<IdentityPost> ParsingPostData(IDataReader reader)
        {
            List<IdentityPost> listData = new List<IdentityPost>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractPostItem(reader);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityPost ExtractPostItem(IDataReader reader)
        {
            var entity = new IdentityPost();

            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.Title = reader["Title"].ToString();
            entity.IsHighlights = Utils.ConvertToBoolean(reader["IsHighlights"]);
            entity.Cover = reader["Cover"].ToString();
            entity.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);
            entity.CreatedBy = reader["CreatedBy"].ToString();
            entity.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            entity.Status = Utils.ConvertToInt32(reader["Status"]);

            if (reader.HasColumn("TotalCount"))
                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return entity;
        }

        private IdentityPostLang ExtractPostLangItem(IDataReader reader)
        {
            var entity = new IdentityPostLang();
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.PostId = Utils.ConvertToInt32(reader["PostId"]);
            entity.Title = reader["Title"].ToString();
            entity.Description = reader["Description"].ToString();
            entity.BodyContent = reader["BodyContent"].ToString();
            entity.LangCode = reader["LangCode"].ToString();
            entity.UrlFriendly = reader["UrlFriendly"].ToString();

            return entity;
        }

        public int Insert(IdentityPost identity)
        {
            var sqlCmd = @"Post_Insert";
            var newId = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@IsHighlights", identity.IsHighlights},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@CategoryId", identity.CategoryId},
                {"@LangCode", identity.LangCode},
                {"@CreatedBy", identity.CreatedBy},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        newId = Utils.ConvertToInt32(reader[0]);
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
            
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@IsHighlights", identity.IsHighlights},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@CategoryId", identity.CategoryId},
                {"@Status", identity.Status},
                {"@LangCode", identity.LangCode}
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

        public int Delete(int id)
        {
            var sqlCmd = @"Post_Delete";

            var parameters = new Dictionary<string, object>
            {
                    {"@Id", id}
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

        #endregion

        public bool AssignCategory(int postId, int catId)
        {
            //Common syntax
            var sqlCmd = @"Post_AssignCategory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@PostId", postId},
                {"@CatId", catId}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Post_AssignCategory. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }
    }
}
