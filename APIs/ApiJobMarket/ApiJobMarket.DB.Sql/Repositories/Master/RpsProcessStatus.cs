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
    public class RpsProcessStatus
    {
        private readonly string _connectionString;

        public RpsProcessStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsProcessStatus()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityProcessStatus> GetList(int agency_id)
        {
            //Common syntax           
            var sqlCmd = @"ProcessStatus_GetList";
            List<IdentityProcessStatus> listData = null;
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", agency_id }
            };
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListProcessStatusFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityProcessStatus> ParsingListProcessStatusFromReader(IDataReader reader)
        {
            List<IdentityProcessStatus> listData = listData = new List<IdentityProcessStatus>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractProcessStatusData(reader);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityProcessStatus ExtractProcessStatusData(IDataReader reader)
        {
            var record = new IdentityProcessStatus();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.status_name = reader["status_name"].ToString();
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.order = Utils.ConvertToInt32(reader["order"]);
            record.description = reader["description"].ToString();
            return record;
        }

        public int Insert(IdentityProcessStatus identity)
        {
            //Common syntax           
            var sqlCmd = @"ProcessStatus_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@status_name", identity.status_name},
                {"@status", identity.status },
                {"@description", identity.description },
                {"@agency_id", identity.agency_id },
                {"@order", identity.order }
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
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityProcessStatus identity)
        {
            //Common syntax
            var sqlCmd = @"ProcessStatus_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@status_name", identity.status_name },
                {"@status", identity.status },
                {"@description", identity.description },
                {"@order", identity.order }
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
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
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
                var strError = "Failed to execute UpdateSorting Menu. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public string BuildUpdateSortingCmd(List<SortingElement> sortList)
        {
            var myCmd = new StringBuilder();
            var itemCmdFormat = @"UPDATE process_status SET [Order] = {0} WHERE 1=1 AND Id = {1}; ";
            if (sortList != null && sortList.Count > 0)
            {
                foreach (var item in sortList)
                {
                    var itemCmd = string.Format(itemCmdFormat, item.SortOrder, item.id);
                    myCmd.Append(itemCmd);
                }
            }

            return myCmd.ToString();
        }
        public IdentityProcessStatus GetById(int id)
        {
            IdentityProcessStatus info = null;
            var sqlCmd = @"ProcessStatus_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractProcessStatusData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int Delete(int id)
        {
            //Common syntax            
            var sqlCmd = @"ProcessStatus_Delete";
            var result = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            result = Utils.ConvertToInt32(reader[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }
        #endregion
    }
}
