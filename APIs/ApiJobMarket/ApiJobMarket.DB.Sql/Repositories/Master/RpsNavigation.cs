using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.SharedLibs.Extensions;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsNavigation
    {
        private readonly string _connectionString;

        public RpsNavigation(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsNavigation()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }


        #region  Common

        public List<IdentityNavigation> GetByPage(IdentityNavigation filter, int currentPage, int pageSize)
        {
            //Common syntax
            var sqlCmd = @"Navigation_GetByPage";
            List<IdentityNavigation> listData = null;

            //For paging
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Status", filter.Active },
                    {"@Offset", offset },
                    {"@PageSize", pageSize }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Navigation_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityNavigation> GetList()
        {
            //Common syntax
            var sqlCmd = @"Navigation_GetList";
            List<IdentityNavigation> listData = new List<IdentityNavigation>();

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractNavigationData(reader);
                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Navigation_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public string BuildUpdateSortingCmd(List<SortingElement> sortList)
        {
            var myCmd = new StringBuilder();
            var itemCmdFormat = @"UPDATE tbl_navigation SET SortOrder = {0}, ParentId = {1} WHERE 1=1 AND Id = {2}; ";
            if (sortList != null && sortList.Count > 0)
            {
                foreach (var item in sortList)
                {
                    var itemCmd = string.Format(itemCmdFormat, item.SortOrder, item.ParentId, item.id);
                    myCmd.Append(itemCmd);

                    if (item.children != null && item.children.Count > 0)
                    {
                        myCmd.Append(BuildUpdateSortingCmd(item.children));
                    }
                }
            }

            return myCmd.ToString();
        }

        public bool UpdateSorting(List<SortingElement> elements)
        {
            //Common syntax
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = string.Empty;

            try
            {
                sqlCmd = BuildUpdateSortingCmd(elements);

                MsSqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlCmd, null);

                return true;
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute UpdateSorting Navigation. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int Insert(IdentityNavigation identity)
        {
            //Common syntax
            var sqlCmd = @"Navigation_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@ParentId", identity.ParentId },
                    {"@Area", identity.Area },
                    {"@Name", identity.Name },
                    {"@Title", identity.Title },
                    {"@Desc", identity.Desc },
                    {"@Action", identity.Action },
                    {"@Controller", identity.Controller },
                    {"@Visible", identity.Visible },
                    {"@Authenticate", identity.Authenticate },
                    {"@CssClass", identity.CssClass },
                    {"@SortOrder", identity.SortOrder },
                    {"@AbsoluteUri", identity.AbsoluteUri },
                    {"@Active", identity.Active },
                    {"@IconCss", identity.IconCss }
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
                var strError = "Failed to execute Navigation_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityNavigation identity)
        {
            //Common syntax
            var sqlCmd = @"Navigation_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", identity.Id },
                    {"@ParentId", identity.ParentId },
                    {"@Area", identity.Area },
                    {"@Name", identity.Name },
                    {"@Title", identity.Title },
                    {"@Desc", identity.Desc },
                    {"@Action", identity.Action },
                    {"@Controller", identity.Controller },
                    {"@Visible", identity.Visible },
                    {"@Authenticate", identity.Authenticate },
                    {"@CssClass", identity.CssClass },
                    {"@SortOrder", identity.SortOrder },
                    {"@AbsoluteUri", identity.AbsoluteUri },
                    {"@Active", identity.Active },
                    {"@IconCss", identity.IconCss }
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
                var strError = "Failed to execute Navigation_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityNavigation GetById(int id)
        {
            //Common syntax
            var sqlCmd = @"Navigation_GetDetail";
            IdentityNavigation info = null;

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
               };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractNavigationData(reader);
                        }

                        //All languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = ParsingNavigationLangFromReader(reader);
                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Navigation_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public bool Delete(int id)
        {
            //Common syntax
            var sqlCmd = @"Navigation_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
               {
                    {"@Id", id }
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
                var strError = "Failed to execute Navigation_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private IdentityNavigationLang ParsingNavigationLangFromReader(IDataReader reader)
        {
            var item = new IdentityNavigationLang();
            item.Id = Utils.ConvertToInt32(reader["Id"]);
            item.NavigationId = Utils.ConvertToInt32(reader["NavigationId"]);
            item.Title = reader["Title"].ToString();
            item.AbsoluteUri = reader["AbsoluteUri"].ToString();
            item.LangCode = reader["LangCode"].ToString();

            return item;
        }

        public IdentityNavigation GetNavigationDetail(int id)
        {
            var sqlCmd = @"Navigation_GetDetail";
            IdentityNavigation info = null;

            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractNavigationData(reader);
                        }

                        //All languages
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var langItem = ParsingNavigationLangFromReader(reader);
                                info.LangList.Add(langItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Navigation_GetDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public int AddNewLang(IdentityNavigationLang identity)
        {
            //Common syntax           
            var sqlCmd = @"Navigation_AddNewLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@NavigationId", identity.NavigationId},
                {"@Title", identity.Title},
                {"@AbsoluteUri", identity.AbsoluteUri},
                {"@LangCode", identity.LangCode }
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
                var strError = "Failed to execute Navigation_AddNewLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int UpdateLang(IdentityNavigationLang identity)
        {
            //Common syntax           
            var sqlCmd = @"Navigation_UpdateLang";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@NavigationId", identity.NavigationId},
                {"@Title", identity.Title},
                {"@AbsoluteUri", identity.AbsoluteUri},
                {"@LangCode", identity.LangCode }
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
                var strError = "Failed to execute Navigation_UpdateLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public IdentityNavigationLang GetLangDetail(int id)
        {
            var sqlCmd = @"Navigation_GetLangDetail";
            IdentityNavigationLang info = null;

            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ParsingNavigationLangFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Navigation_GetLangDetail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public bool DeleteLang(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Navigation_DeleteLang";

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
                var strError = "Failed to execute Navigation_DeleteLang. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityNavigation> F_GetList()
        {
            //Common syntax
            var sqlCmd = @"F_Navigation_GetList";
            List<IdentityNavigation> listData = new List<IdentityNavigation>();

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractNavigationData(reader);
                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            var listLang = new List<IdentityNavigationLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ParsingNavigationLangFromReader(reader));
                            }

                            if (listData.Count > 0 && listLang.Count > 0)
                            {
                                foreach (var item in listData)
                                {
                                    var itemLangs = listLang.Where(x => x.NavigationId == item.Id).ToList();
                                    if (itemLangs != null)
                                    {
                                        item.LangList = itemLangs;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute F_Navigation_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion

        #region  Helpers

        private List<IdentityNavigation> ParsingListFromReader(IDataReader reader)
        {
            List<IdentityNavigation> listData = listData = new List<IdentityNavigation>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractNavigationData(reader);

                //Extends information
                record.total_count = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }
            return listData;
        }

        private IdentityNavigation ExtractNavigationData(IDataReader reader)
        {
            List<IdentityNavigation> listData = new List<IdentityNavigation>();
            var record = new IdentityNavigation();

            //Seperate properties;
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.ParentId = Utils.ConvertToInt32(reader["ParentId"]);
            record.Area = reader["Area"].ToString();
            record.Name = reader["Name"].ToString();
            record.Title = reader["Title"].ToString();
            record.Desc = reader["Desc"].ToString();
            record.Action = reader["Action"].ToString();
            record.Controller = reader["Controller"].ToString();
            record.Visible = Utils.ConvertToBoolean(reader["Visible"]);
            record.Authenticate = Utils.ConvertToInt32(reader["Authenticate"]);
            record.CssClass = reader["CssClass"].ToString();
            record.SortOrder = Utils.ConvertToInt32(reader["SortOrder"]);
            record.AbsoluteUri = reader["AbsoluteUri"].ToString();
            record.Active = Utils.ConvertToInt32(reader["Active"]);
            record.IconCss = reader["IconCss"].ToString();

            return record;
        }

        #endregion
    }
}
