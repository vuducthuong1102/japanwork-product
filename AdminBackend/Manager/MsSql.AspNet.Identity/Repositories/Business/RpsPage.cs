using Manager.SharedLibs;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPage
    {
        private readonly string _connectionString;

        public RpsPage(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPage()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityPage> GetByPage(IdentityPage filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"Page_GetByPage";
            List<IdentityPage> myList = new List<IdentityPage>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@Offset", offset},
                {"@PageSize", filter.PageSize},
                {"@Status", filter.Status },
                {"@LangCode", filter.LangCode },
            };

            try
            {
                using(var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractPageData(reader);

                            myList.Add(entity);
                        }

                        if (reader.NextResult())
                        {
                            if (myList.Count > 0)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPageLangItem(reader);
                                    foreach (var item in myList)
                                    {
                                        if (item.Id == itemLang.PageId)
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
                var strError = "Failed to execute Page_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        private List<IdentityPage> ParsingListPageFromReader(IDataReader reader)
        {
            List<IdentityPage> listData = listData = new List<IdentityPage>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPageData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityPage ExtractPageData(IDataReader reader)
        {
            var record = new IdentityPage();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Title = reader["Title"].ToString();
            record.Controller = reader["Controller"].ToString();
            record.Action = reader["Action"].ToString();
            record.SortOrder = Utils.ConvertToInt32(reader["SortOrder"]);

            record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();

            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        private IdentityPageLang ExtractPageLangItem(IDataReader reader)
        {
            var entity = new IdentityPageLang();
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.PageId = Utils.ConvertToInt32(reader["PageId"]);
            entity.Title = reader["Title"].ToString();
            entity.Description = reader["Description"].ToString();
            entity.BodyContent = reader["BodyContent"].ToString();
            entity.UrlFriendly = reader["UrlFriendly"].ToString();

            return entity;
        }

        public int Insert(IdentityPage identity)
        {
            //Common syntax           
            var sqlCmd = @"Page_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@LangCode", identity.LangCode},
                {"@CreatedBy", identity.CreatedBy},
                {"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Page_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int RegisterNewPage(IdentityPage identity, out bool isNew)
        {
            //Common syntax           
            var sqlCmd = @"Page_RegisterNewPage";
            var newId = 0;
            var hasInserted = 0;
            isNew = false;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Controller", identity.Controller},
                {"@Action", identity.Action }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);                    
                    if (reader.Read())
                    {
                        newId = Convert.ToInt32(reader[0]);
                    }

                    if (reader.NextResult())
                    {
                        if (reader.Read())
                        {
                            hasInserted = Utils.ConvertToInt32(reader[0]);
                            if (hasInserted == 1)
                                isNew = true;
                            else
                                isNew = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute RegisterNewPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }
        public bool Update(IdentityPage identity)
        {
            //Common syntax
            var sqlCmd = @"Page_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Title", identity.Title},
                {"@Description", identity.Description},
                {"@BodyContent", identity.BodyContent},
                {"@UrlFriendly", identity.UrlFriendly},
                {"@Status", identity.Status},
                {"@LangCode", identity.LangCode}
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
                var strError = "Failed to execute Page_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityPage GetById(int Id, string langCode)
        {
            var info = new IdentityPage();         
            var sqlCmd = @"Page_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
                {"@LangCode", langCode},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if(reader.Read())
                        {
                            info = ExtractPageData(reader);
                        }

                        if (reader.NextResult())
                        {
                            if (info != null)
                            {
                                while (reader.Read())
                                {
                                    var itemLang = ExtractPageLangItem(reader);
                                    info.MyLanguages.Add(itemLang);
                                }
                            }
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Page_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Page_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
            };

            try
            {
                using(var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Page_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityPage> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Page_GetList";

            List<IdentityPage> listData = new List<IdentityPage>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractPageData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Page_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public IdentityPage F_Page_GetPageByOperation(string controller, string action)
        {
            var info = new IdentityPage();
            var sqlCmd = @"F_Page_GetPageByOperation";

            var parameters = new Dictionary<string, object>
            {
                {"@Controller", controller},
                {"@Action", action}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractPageData(reader);
                        }

                        if(info != null)
                        {
                            if (reader.NextResult())
                            {
                                if (reader.Read())
                                {
                                    info.TemplateInfo = new RpsPageTemplate().ExtractPageTemplateData(reader);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute F_Page_GetPageByOperation. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        #endregion
    }
}
