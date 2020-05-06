using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;


namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsProjectCategory
    {
        private readonly string _connectionString;

        public RpsProjectCategory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProjectCategory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- ProjectCategory ----

        public List<IdentityProjectCategory> GetByPage(IdentityProjectCategory filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@Status", filter.Status },
                {"@LangCode", filter.LangCode },
            };

            var sqlCmd = @"ProjectCategory_GetByPage";

            List<IdentityProjectCategory> myList = new List<IdentityProjectCategory>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractProjectCategoryItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if(myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectCategoryLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.ProjectCategoryId)
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
                var strError = "Failed to ProjectCategory_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityProjectCategory GetById(int id, string langCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id},
                {"@LangCode", langCode}
            };

            var sqlCmd = @"ProjectCategory_GetById";

            IdentityProjectCategory info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractProjectCategoryItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectCategoryLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }                            
                        }

                        ////Images
                        //if (reader.NextResult())
                        //{
                        //    info.Images = ExtractProjectCategoryImageData(reader);
                        //}
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

        public IdentityProjectCategory F_ProjectCategory_GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"F_ProjectCategory_GetDetailById";

            IdentityProjectCategory info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractProjectCategoryItem(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectCategoryLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to F_ProjectCategory_GetDetailById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public List<IdentityProjectCategory> F_ProjectCategory_GetRelated(IdentityProjectCategory filter)
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

            var sqlCmd = @"F_ProjectCategory_GetRelated";

            List<IdentityProjectCategory> myList = new List<IdentityProjectCategory>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractProjectCategoryItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectCategoryLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.ProjectCategoryId)
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
                var strError = "Failed to F_ProjectCategory_GetRelated. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        private List<IdentityProjectCategory> ParsingProjectCategoryData(IDataReader reader)
        {
            List<IdentityProjectCategory> listData = new List<IdentityProjectCategory>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractProjectCategoryItem(reader);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityProjectCategory ExtractProjectCategoryItem(IDataReader reader)
        {
            var entity = new IdentityProjectCategory();

            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.Name = reader["Name"].ToString();
            entity.Cover = reader["Cover"].ToString();
            entity.ParentId = Utils.ConvertToInt32(reader["ParentId"]);
            entity.CreatedBy = reader["CreatedBy"].ToString();
            entity.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            entity.Status = Utils.ConvertToInt32(reader["Status"]);

            if (reader.HasColumn("TotalCount"))
                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return entity;
        }

        private IdentityProjectCategoryLang ExtractProjectCategoryLangItem(IDataReader reader)
        {
            var entity = new IdentityProjectCategoryLang();
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.ProjectCategoryId = Utils.ConvertToInt32(reader["ProjectCategoryId"]);
            entity.Name = reader["Name"].ToString();
            entity.Description = reader["Description"].ToString();
            entity.LangCode = reader["LangCode"].ToString();
            entity.UrlFriendly = reader["UrlFriendly"].ToString();

            return entity;
        }

        //private List<IdentityProjectCategoryImage> ExtractProjectCategoryImageData(IDataReader reader)
        //{
        //    var myList = new List<IdentityProjectCategoryImage>();
        //    while (reader.Read())
        //    {
        //        var record = new IdentityProjectCategoryImage();

        //        //Seperate properties
        //        record.Id = reader["Id"].ToString();
        //        record.ProjectCategoryId = Utils.ConvertToInt32(reader["ProjectCategoryId"]);
        //        record.Name = reader["Name"].ToString();
        //        record.Url = reader["Url"].ToString();
        //        record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

        //        myList.Add(record);
        //    }

        //    return myList;
        //}

        public int Insert(IdentityProjectCategory identity)
        {
            var sqlCmd = @"ProjectCategory_Insert";
            var newId = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Description", identity.Description},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@ParentId", identity.ParentId},
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
                var strError = "Failed to ProjectCategory_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityProjectCategory identity)
        {
            var sqlCmd = @"ProjectCategory_Update";
            
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Description", identity.Description},
                {"@Cover", identity.Cover},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@ParentId", identity.ParentId},
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
                var strError = "Failed to ProjectCategory_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(int id)
        {
            var sqlCmd = @"ProjectCategory_Delete";

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
                var strError = "Failed to ProjectCategory_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public List<IdentityProjectCategory> GetList()
        {
            var parameters = new Dictionary<string, object>
            {
                
            };

            var sqlCmd = @"ProjectCategory_GetList";

            List<IdentityProjectCategory> myList = new List<IdentityProjectCategory>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractProjectCategoryItem(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractProjectCategoryLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.ProjectCategoryId)
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
                var strError = "Failed to ProjectCategory_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        #endregion

        #region Images

        //public bool AddNewImage(IdentityProjectCategoryImage identity)
        //{
        //    //Common syntax
        //    var sqlCmd = @"ProjectCategory_AddNewImage";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@Id", identity.Id},
        //        {"@ProjectCategoryId", identity.ProjectCategoryId},
        //        {"@Name", identity.Name},
        //        {"@Url", identity.Url}
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute ProjectCategory_AddNewImage. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return true;
        //}

        //public bool RemoveImage(string Id)
        //{
        //    //Common syntax
        //    var sqlCmd = @"ProjectCategory_RemoveImage";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@Id", Id},
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute ProjectCategory_RemoveImage. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return true;
        //}

        //public List<IdentityProjectCategoryImage> GetListImage(int Id)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"ProjectCategory_GetListImage";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@ProjectCategoryId", Id},
        //    };

        //    List<IdentityProjectCategoryImage> listData = new List<IdentityProjectCategoryImage>();
        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //            {
        //                listData = ExtractProjectCategoryImageData(reader);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute ProjectCategory_GetListImage. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return listData;
        //}

        #endregion

    }
}
