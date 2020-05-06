
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsPageTemplate
    {
        private readonly string _connectionString;

        public RpsPageTemplate(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsPageTemplate()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityPageTemplate> GetByPage(IdentityPageTemplate filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"Page_GetByPage";
            List<IdentityPageTemplate> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using(var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListPageFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Page_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityPageTemplate> ParsingListPageFromReader(IDataReader reader)
        {
            List<IdentityPageTemplate> listData = listData = new List<IdentityPageTemplate>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractPageTemplateData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        public IdentityPageTemplate ExtractPageTemplateData(IDataReader reader)
        {
            var record = new IdentityPageTemplate();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Widgets = reader["Widgets"].ToString();
            record.IsDefault = Convert.ToBoolean(reader["IsDefault"]);
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            //record.CreatedBy = reader["CreatedBy"].ToString();
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            //record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();

            return record;
        }

        public int Insert(IdentityPageTemplate identity)
        {
            //Common syntax           
            var sqlCmd = @"Page_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Controller", identity.Controller},
                //{"@Action", identity.Action }
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

        public int RegisterNewPage(IdentityPageTemplate identity, out bool isNew)
        {
            //Common syntax           
            var sqlCmd = @"Page_RegisterNewPage";
            var newId = 0;
            var hasInserted = 0;
            isNew = false;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Controller", identity.Controller},
                //{"@Action", identity.Action }
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
        public bool Update(IdentityPageTemplate identity)
        {
            //Common syntax
            var sqlCmd = @"Page_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                //{"@Controller", identity.Controller},
                //{"@Action", identity.Action },
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

        public IdentityPageTemplate GetById(int Id)
        {
            var info = new IdentityPageTemplate();         
            var sqlCmd = @"Page_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            info = ExtractPageTemplateData(reader);
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

        public List<IdentityPageTemplate> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Page_GetList";

            List<IdentityPageTemplate> listData = new List<IdentityPageTemplate>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractPageTemplateData(reader);

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

        #endregion
    }
}
