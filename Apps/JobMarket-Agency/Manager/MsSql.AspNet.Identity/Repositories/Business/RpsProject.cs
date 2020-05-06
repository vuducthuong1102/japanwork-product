using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using System.Linq;


namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProject
    {
        private readonly string _connectionString;

        public RpsProject(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProject()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- Project ----

        public List<IdentityProject> GetByPage(IdentityProject filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@PropertyList", filter.PropertyList},
                {"@CategoryId", filter.CategoryId},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@Status", filter.Status },
                {"@LangCode", filter.LangCode },
            };

            var sqlCmd = @"Project_GetByPage";

            List<IdentityProject> myList = new List<IdentityProject>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractProjectItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if(myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.ProjectId)
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
                var strError = "Failed to Project_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityProject GetById(int id, string langCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id},
                {"@LangCode", langCode}
            };

            var sqlCmd = @"Project_GetById";

            IdentityProject info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractProjectItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }                            
                        }

                        //Images
                        if (reader.NextResult())
                        {
                            info.Images = ExtractProjectImageData(reader);
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

        public IdentityProject GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"Project_GetDetailById";

            IdentityProject info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractProjectItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }
                        }

                        //Images
                        if (reader.NextResult())
                        {
                            info.Images = ExtractProjectImageData(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Project_GetDetailById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public List<IdentityProject> F_Project_GetRelated(IdentityProject filter)
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

            var sqlCmd = @"F_Project_GetRelated";

            List<IdentityProject> myList = new List<IdentityProject>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractProjectItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.ProjectId)
                                            item.MyLanguages.Add(itemLang);
                                    }
                                }
                            }
                        }

                        if(myList != null && myList.Count > 0)
                        {
                            if (reader.NextResult())
                            {
                                var allImages = ExtractProjectImageData(reader);

                                if (allImages != null && allImages.Count > 0)
                                {
                                    foreach (var item in myList)
                                    {
                                        item.Images = allImages.Where(x => x.ProjectId == item.Id).ToList();
                                    }
                                }
                            }
                        }
                        
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_Project_GetRelated. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<IdentityProject> F_Project_GetNewest(IdentityProject filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Offset", offset},
                {"@PageSize", filter.PageSize}
            };

            var sqlCmd = @"F_Project_GetNewest";

            List<IdentityProject> myList = new List<IdentityProject>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = new IdentityProject();
                            entity.Id = Utils.ConvertToInt32(reader["Id"]);

                            myList.Add(entity);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_Project_GetNewest. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        private List<IdentityProject> ParsingProjectData(IDataReader reader)
        {
            List<IdentityProject> listData = new List<IdentityProject>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractProjectItem(reader);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityProject ExtractProjectItem(IDataReader reader)
        {
            var entity = new IdentityProject();

            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.Title = reader["Title"].ToString();
            entity.Cover = reader["Cover"].ToString();
            entity.CategoryId = Utils.ConvertToInt32(reader["CategoryId"]);
            entity.CreatedBy = reader["CreatedBy"].ToString();
            entity.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            entity.Status = Utils.ConvertToInt32(reader["Status"]);

            try
            {
                var metaData = reader["MetaData"].ToString();
                if (!string.IsNullOrEmpty(metaData))
                    entity.MetaData = JsonConvert.DeserializeObject<MetaDataProject>(reader["MetaData"].ToString());
            }
            catch
            {

            }

            if (reader.HasColumn("TotalCount"))
                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);


            return entity;
        }

        private IdentityProjectLang ExtractProjectLangItem(IDataReader reader)
        {
            var entity = new IdentityProjectLang();
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.ProjectId = Utils.ConvertToInt32(reader["ProjectId"]);
            entity.Title = reader["Title"].ToString();
            entity.Description = reader["Description"].ToString();
            entity.BodyContent = reader["BodyContent"].ToString();
            entity.LangCode = reader["LangCode"].ToString();
            entity.UrlFriendly = reader["UrlFriendly"].ToString();

            return entity;
        }

        private List<IdentityProjectImage> ExtractProjectImageData(IDataReader reader)
        {
            var myList = new List<IdentityProjectImage>();
            while (reader.Read())
            {
                var record = new IdentityProjectImage();

                //Seperate properties
                record.Id = reader["Id"].ToString();
                record.ProjectId = Utils.ConvertToInt32(reader["ProjectId"]);
                record.Name = reader["Name"].ToString();
                record.Url = reader["Url"].ToString();
                record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

                myList.Add(record);
            }

            return myList;
        }

        public int Insert(IdentityProject identity)
        {
            var sqlCmd = @"Project_Insert";
            var newId = 0;

            var metaData = string.Empty;

            if (identity.MetaData != null)
                metaData = JsonConvert.SerializeObject(identity.MetaData);

            var parameters = new Dictionary<string, object>
            {
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@CategoryId", identity.CategoryId},
                {"@LangCode", identity.LangCode},
                {"@MetaData", metaData},
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
                var strError = "Failed to Project_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityProject identity)
        {
            var sqlCmd = @"Project_Update";

            var metaData = string.Empty;

            if(identity.MetaData != null)
                metaData = JsonConvert.SerializeObject(identity.MetaData);
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@CategoryId", identity.CategoryId},
                {"@Status", identity.Status},
                {"@LangCode", identity.LangCode},
                {"@MetaData", metaData}
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
                var strError = "Failed to Project_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(int id)
        {
            var sqlCmd = @"Project_Delete";

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
                var strError = "Failed to Project_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        #endregion

        #region Images

        public bool AddNewImage(IdentityProjectImage identity)
        {
            //Common syntax
            var sqlCmd = @"Project_AddNewImage";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@ProjectId", identity.ProjectId},
                {"@Name", identity.Name},
                {"@Url", identity.Url}
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
                var strError = "Failed to execute Project_AddNewImage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public bool RemoveImage(string Id)
        {
            //Common syntax
            var sqlCmd = @"Project_RemoveImage";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
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
                var strError = "Failed to execute Project_RemoveImage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityProjectImage> GetListImage(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Project_GetListImage";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ProjectId", Id},
            };

            List<IdentityProjectImage> listData = new List<IdentityProjectImage>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ExtractProjectImageData(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Project_GetListImage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion

        public bool AssignCategory(int postId, int catId)
        {
            //Common syntax
            var sqlCmd = @"Project_AssignCategory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ProjectId", postId},
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
                var strError = "Failed to execute Project_AssignCategory. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }
    }
}
