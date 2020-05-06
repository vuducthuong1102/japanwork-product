
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsWidget
    {
        private readonly string _connectionString;

        public RpsWidget(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsWidget()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityWidget> GetByPage(IdentityWidget filter, int currentPage, int pageSize)
        {           
            //Common syntax           
            var sqlCmd = @"Widget_GetByPage";
            List<IdentityWidget> listData = null;

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
                        listData = ParsingListWidgetFromReader(reader);
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Widget_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityWidget> ParsingListWidgetFromReader(IDataReader reader)
        {
            List<IdentityWidget> listData = listData = new List<IdentityWidget>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractWidgetData(reader);

                //Extends information
                if(reader.HasColumn("TotalCount"))
                    record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityWidget ExtractWidgetData(IDataReader reader)
        {
            var record = new IdentityWidget();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Controller = reader["Controller"].ToString();
            record.Action = reader["Action"].ToString();

            //record.CreatedBy = reader["CreatedBy"].ToString();
            //record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            //record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();

            return record;
        }

        public int Insert(IdentityWidget identity)
        {
            //Common syntax           
            var sqlCmd = @"Widget_Insert";
            var newId = 0;

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
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Widget_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int RegisterNewWidget(IdentityWidget identity, out bool isNew)
        {
            //Common syntax           
            var sqlCmd = @"Widget_RegisterNewWidget";
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
                var strError = "Failed to execute RegisterNewWidget. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }
        public bool Update(IdentityWidget identity)
        {
            //Common syntax
            var sqlCmd = @"Widget_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Controller", identity.Controller},
                {"@Action", identity.Action },
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
                var strError = "Failed to execute Widget_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityWidget GetById(int Id)
        {
            var info = new IdentityWidget();         
            var sqlCmd = @"Widget_GetById";

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
                            info = ExtractWidgetData(reader);
                        }
                    }
                }                            
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Widget_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Widget_Delete";

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
                var strError = "Failed to execute Widget_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityWidget> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Widget_GetList";

            List<IdentityWidget> listData = new List<IdentityWidget>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractWidgetData(reader);

                            listData.Add(record);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Widget_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
