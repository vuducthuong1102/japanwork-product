using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using ApiJobMarket.ShareLibs.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsNotification
    {
        private readonly string _connectionString;

        public RpsNotification(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsNotification()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public static IdentityNotification ExtractBaseNotificationData(IDataReader reader)
        {
            var record = new IdentityNotification();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.action_type = Utils.ConvertToInt32(reader["action_type"]);
            record.sender_id = Utils.ConvertToInt32(reader["sender_id"]);
            record.target_type = Utils.ConvertToInt32(reader["target_type"]);
            record.target_id = Utils.ConvertToInt32(reader["target_id"]);
            record.content = reader["content"].ToString();
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];

            return record;
        }

        public static IdentityNotification ExtractNotificationData(IDataReader reader)
        {
            var record = new IdentityNotification();

            //Seperate properties
            record.user_id = Utils.ConvertToInt32(reader["user_id"]);
            record.notification_id = Utils.ConvertToInt32(reader["notification_id"]);
            record.is_viewed = Utils.ConvertToBoolean(reader["is_viewed"]);
            record.is_read = Utils.ConvertToBoolean(reader["is_read"]);
            record.read_at = reader["read_at"] == DBNull.Value ? null : (DateTime?)reader["read_at"];

            return record;
        }

        public int Insert(IdentityNotification identity)
        {
            //Common syntax           
            var sqlCmd = @"Notification_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@cv_id", identity.cv_id},
                //{"@job_id", identity.job_id },
                //{"@job_seeker_id", identity.job_seeker_id }
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

        public int SinglePush(IdentityNotification identity)
        {
            //Common syntax           
            var sqlCmd = @"Notification_SinglePush";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@action_type", identity.action_type},
                {"@target_id", identity.target_id },
                {"@sender_id", identity.sender_id },
                {"@target_type", identity.target_type },
                {"@content", identity.content },
                {"@user_id", identity.user_id }
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

        public int MultiplePush(List<int> listIds, IdentityNotification identity)
        {
            //Common syntax           
            var sqlCmd = @"Notification_MultiplePush";
            var newId = 0;

            var listIdsStr = string.Join(",", listIds);

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@action_type", identity.action_type},
                {"@target_id", identity.target_id },
                {"@sender_id", identity.sender_id },
                {"@target_type", identity.target_type },
                {"@content", identity.content },
                {"@listIds", listIdsStr }
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

        public string MultiplePush(string listTargetIds, IdentityNotification identity,int job_seeker_id)
        {
            //Common syntax           
            var sqlCmd = @"Notification_MultiplePush_ToObject";
            var newId = string.Empty;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@action_type", identity.action_type},
                {"@sender_id", identity.sender_id },
                {"@target_type", identity.target_type },
                {"@content", identity.content },
                {"@listTargetIds", listTargetIds },
                {"@job_seeker_id", job_seeker_id },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = returnObj.ToString();
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentityNotification identity)
        {
            //Common syntax
            var sqlCmd = @"Notification_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {

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

        public IdentityNotification GetById(int id)
        {
            IdentityNotification info = null;
            var sqlCmd = @"Notification_GetById";

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
                        //Detail
                        if (reader.Read())
                        {
                            info = ExtractNotificationData(reader);
                        }

                        //Base data
                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                var baseNotif = ExtractBaseNotificationData(reader);
                                info.action_type = baseNotif.action_type;
                                info.sender_id = baseNotif.sender_id;
                                info.target_type = baseNotif.target_type;
                                info.target_id = baseNotif.target_id;
                                info.content = baseNotif.content;
                                info.created_at = baseNotif.created_at;
                                info.notification_id = baseNotif.id;
                            }
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

        public bool Delete(int id)
        {
            //Common syntax            
            var sqlCmd = @"Notification_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
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

        public List<IdentityNotification> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax            
            var sqlCmd = @"Notification_GetListByJobSeeker";

            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", filter.job_seeker_id},
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            List<IdentityNotification> listData = new List<IdentityNotification>();
            List<IdentityNotificationBase> listBase = new List<IdentityNotificationBase>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        //Detail
                        while (reader.Read())
                        {                            
                            var record = ExtractNotificationData(reader);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);
                            
                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            //Base data
                            while (reader.Read())
                            {
                                var baseNotif = ExtractBaseNotificationData(reader);
                                listBase.Add(baseNotif);
                            }
                        }

                        if(listData.HasData() && listBase.HasData())
                        {
                            foreach (var item in listData)
                            {
                                var matchedMaster = listBase.Where(x=>x.id == item.notification_id).FirstOrDefault();
                                if (matchedMaster != null)
                                {
                                    item.action_type = matchedMaster.action_type;
                                    item.sender_id = matchedMaster.sender_id;
                                    item.target_type = matchedMaster.target_type;
                                    item.target_id = matchedMaster.target_id;
                                    item.content = matchedMaster.content;
                                    item.created_at = matchedMaster.created_at;
                                    item.notification_id = matchedMaster.id;
                                }
                            }
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

        public List<IdentityNotification> GetListByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Notification_GetListByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityNotification> listData = new List<IdentityNotification>();
            List<IdentityNotificationBase> listBase = new List<IdentityNotificationBase>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        //Detail
                        while (reader.Read())
                        {
                            var record = ExtractNotificationData(reader);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            //Base data
                            while (reader.Read())
                            {
                                var baseNotif = ExtractBaseNotificationData(reader);
                                listBase.Add(baseNotif);
                            }
                        }

                        if (listData.HasData() && listBase.HasData())
                        {
                            foreach (var item in listData)
                            {
                                var matchedMaster = listBase.Where(x => x.id == item.notification_id).FirstOrDefault();
                                if (matchedMaster != null)
                                {
                                    item.action_type = matchedMaster.action_type;
                                    item.sender_id = matchedMaster.sender_id;
                                    item.target_type = matchedMaster.target_type;
                                    item.target_id = matchedMaster.target_id;
                                    item.content = matchedMaster.content;
                                    item.created_at = matchedMaster.created_at;
                                    item.notification_id = matchedMaster.id;
                                }
                            }
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

        public int CountUnread(dynamic identity)
        {
            //Common syntax           
            var sqlCmd = @"Notification_CountUnread";
            var totalCount = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@user_id", identity.user_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    totalCount = Convert.ToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return totalCount;
        }

        #endregion
    }
}
