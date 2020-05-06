using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsDevice
    {
        private readonly string _connectionString;

        public RpsDevice(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsDevice()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region  Common

        public List<IdentityJobSeekerDevice> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_GetAllDevicesByPage";
            List<IdentityJobSeekerDevice> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerDeviceData(reader);
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

        public static IdentityJobSeekerDevice ExtractJobSeekerDeviceData(IDataReader reader)
        {
            var record = new IdentityJobSeekerDevice();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.device_name = reader["device_name"].ToString();            
            record.device_id = reader["device_id"].ToString();            
            record.registration_id = reader["registration_id"].ToString();
            record.device_type = Utils.ConvertToInt32(reader["device_type"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.last_connected = reader["last_connected"] == DBNull.Value ? null : (DateTime?)reader["last_connected"];
            record.language_code = reader["language_code"].ToString();

            return record;
        }
        
        public int JobSeekerUpdate(IdentityJobSeekerDevice identity)
        {
            var deviceId = 0;
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateDevice";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", identity.job_seeker_id},
                {"@device_name", identity.device_name},
                {"@device_id", identity.device_id},
                {"@registration_id", identity.registration_id },
                {"@device_type", identity.device_type },
                {"@language_code", identity.language_code }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    deviceId = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return deviceId;
        }


        public List<IdentityJobSeekerDevice> JobSeekerGetAllDevices(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_GetAllDevices";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityJobSeekerDevice> listData = new List<IdentityJobSeekerDevice>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerDeviceData(reader);

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

        #endregion
    }
}
