using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLibs;
using MsSql.AspNet.Identity.Entities;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsEmailServer
    {
        private readonly string _connectionString;

        public RpsEmailServer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsEmailServer()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- EmailServer ----

        public List<IdentityEmailServer> GetByPage(dynamic filter)
        {
            int offset = (filter.PageIndex - 1) * filter.PageSize;
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", filter.Keyword},
                {"@AgencyId", filter.AgencyId},
                {"@Status", filter.Status },
                {"@Offset", offset},
                {"@PageSize", filter.PageSize}
            };

            var sqlCmd = @"EmailServer_GetByPage";

            List<IdentityEmailServer> myList = new List<IdentityEmailServer>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractEmailServerItem(reader);

                            if (reader.HasColumn("TotalCount"))
                                entity.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                            myList.Add(entity);
                        }                 
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to EmailServer_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public List<IdentityEmailServer> GetListByAgency(int agencyId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", agencyId}
            };

            var sqlCmd = @"EmailServer_GetListByAgency";

            List<IdentityEmailServer> myList = new List<IdentityEmailServer>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractEmailServerItem(reader);

                            myList.Add(entity);
                        }                 
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityEmailServer GetById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id},
            };

            var sqlCmd = @"EmailServer_GetById";

            IdentityEmailServer info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractEmailServerItem(reader);
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

        public IdentityEmailServer GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"EmailServer_GetDetailById";

            IdentityEmailServer info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractEmailServerItem(reader);
                        }

                        //if (reader.NextResult())
                        //{
                        //    if (info != null)
                        //    {
                        //        while (reader.Read())
                        //        {
                        //            var itemLang = ExtractEmailServerLangItem(reader);
                        //            info.MyLanguages.Add(itemLang);
                        //        }
                        //    }
                        //}

                        ////Images
                        //if (reader.NextResult())
                        //{
                        //    info.Images = ExtractEmailServerImageData(reader);
                        //}
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

        private IdentityEmailServer ExtractEmailServerItem(IDataReader reader)
        {
            var record = new IdentityEmailServer();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);
            record.StaffId = Utils.ConvertToInt32(reader["StaffId"]);
            record.Name = reader["Name"].ToString();
            record.SMTPConfig = reader["SMTPConfig"].ToString();
            record.POPConfig = reader["POPConfig"].ToString();
            record.TestingSuccessed = Utils.ConvertToBoolean(reader["TestingSuccessed"]);
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
            record.Status = Utils.ConvertToInt32(reader["Status"]);

            return record;
        }

        private IdentityEmailSetting ExtractEmailSettingItem(IDataReader reader)
        {
            var record = new IdentityEmailSetting();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);
            record.StaffId = Utils.ConvertToInt32(reader["StaffId"]);
            record.Email = reader["Email"].ToString();
            record.EmailPasswordHash = reader["EmailPasswordHash"].ToString();
            record.EmailServerId = Utils.ConvertToInt32(reader["EmailServerId"]);
            record.EmailType = Utils.ConvertToInt32(reader["EmailType"]);
            record.TestingSuccessed = Utils.ConvertToBoolean(reader["TestingSuccessed"]);

            return record;
        }

        public int Insert(IdentityEmailServer identity)
        {
            var sqlCmd = @"EmailServer_Insert";
            var newId = 0;

            var metaData = string.Empty;

            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},
                {"@Name", identity.Name},
                {"@SMTPConfig", identity.SMTPConfig},
                {"@POPConfig", identity.POPConfig},
                {"@TestingSuccessed", identity.TestingSuccessed},
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
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int Update(IdentityEmailServer identity)
        {
            var sqlCmd = @"EmailServer_Update";
            var metaData = string.Empty;
            
            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@AgencyId", identity.AgencyId},
                {"@StaffId", identity.StaffId},
                {"@Name", identity.Name},
                {"@SMTPConfig", identity.SMTPConfig},
                {"@POPConfig", identity.POPConfig},
                {"@TestingSuccessed", identity.TestingSuccessed},
                {"@Status", identity.Status}
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
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(IdentityEmailServer identity)
        {
            var sqlCmd = @"EmailServer_Delete";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", identity.Id},
                {"@AgencyId", identity.AgencyId}
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
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
        }

        public List<IdentityEmailSetting> GetEmailSettingsByStaff(int agencyId, int staffId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@AgencyId", agencyId},
                {"@StaffId", staffId},
            };

            var sqlCmd = @"EmailServer_GetEmailSettingsByStaff";

            List<IdentityEmailSetting> listData = new List<IdentityEmailSetting>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var record = ExtractEmailSettingItem(reader);

                            listData.Add(record);
                        }
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

        public bool UpdateEmailSettings(List<IdentityEmailSetting> settings)
        {
            var sqlCmd = @"EmailServer_UpdateEmailSetting";
            bool result = false;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    if (settings.HasData())
                    {
                        foreach (var item in settings)
                        {
                            var parameters = new Dictionary<string, object>
                            {
                                {"@AgencyId", item.AgencyId},
                                {"@StaffId", item.StaffId},
                                {"@Email", item.Email},
                                {"@EmailPasswordHash", item.EmailPasswordHash},
                                {"@EmailServerId", item.EmailServerId},
                                {"@EmailType", item.EmailType},
                                {"@TestingSuccessed", item.TestingSuccessed},
                                {"@PasswordChanged", item.PasswordChanged},
                            };

                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                        }
                    }
                }

                result = true;
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
