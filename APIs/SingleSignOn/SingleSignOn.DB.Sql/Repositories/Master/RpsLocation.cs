using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.ShareLibs;
using SingleSignOn.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SingleSignOn.DB.Sql.Repositories
{
    public class RpsLocation
    {
        private readonly string _connectionString;

        public RpsLocation(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsLocation()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region  Common

        public List<IdentityLocation> GetAll(IdentityLocation filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Location_GetAll";
            List<IdentityLocation> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", filter.Name },
                {"@Code", filter.Code },
                {"@Status", filter.Status },
                {"@TuNgay", filter.FromDate },
                {"@DenNgay", filter.ToDate },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListLocationFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Location_GetAll. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentityLocation> ParsingListLocationFromReader(IDataReader reader)
        {
            List<IdentityLocation> listData = listData = new List<IdentityLocation>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractLocationData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentityLocation ExtractLocationData(IDataReader reader)
        {
            var record = new IdentityLocation();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Name = reader["Name"].ToString();
            record.Code = reader["Code"].ToString();
            record.Longitude = reader["Longitude"].ToString();
            record.Latitude = reader["Latitude"].ToString();
            record.CreatedBy = reader["CreatedBy"].ToString();
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : (DateTime?)reader["LastUpdated"];
            record.LastUpdatedBy = reader["LastUpdatedBy"].ToString();
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        public int Insert(IdentityLocation identity)
        {
            //Common syntax           
            var sqlCmd = @"Location_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Name", identity.Name},
                {"@Code", identity.Code },
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
                var strError = "Failed to execute Location_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityLocation identity)
        {
            //Common syntax
            var sqlCmd = @"Location_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@Name", identity.Name},
                {"@Code", identity.Code },
                {"@LastUpdatedBy", identity.LastUpdatedBy},
                {"@Status", identity.Status}
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
                var strError = "Failed to execute Location_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityLocation GetById(int Id)
        {
            var info = new IdentityLocation();
            var sqlCmd = @"Location_GetById";

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
                            info = ExtractLocationData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Location_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"Location_Delete";

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
                var strError = "Failed to execute Location_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentityLocation> GetList(string keyword)
        {
            //Common syntax            
            var sqlCmd = @"Location_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", keyword}
            };

            List<IdentityLocation> listData = new List<IdentityLocation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractLocationData(reader);

                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Location_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
