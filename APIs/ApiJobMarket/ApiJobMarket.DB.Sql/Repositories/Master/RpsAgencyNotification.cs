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
    public class RpsAgencyNotification
    {
        private readonly string _connectionString;

        public RpsAgencyNotification(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsAgencyNotification()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public static IdentityAgencyNotification ExtractBaseAgencyNotificationData(IDataReader reader)
        {
            var record = new IdentityAgencyNotification();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.action_type = Utils.ConvertToInt32(reader["action_type"]);
            record.sender_id = Utils.ConvertToInt32(reader["sender_id"]);
            record.target_type = Utils.ConvertToInt32(reader["target_type"]);
            record.target_id = Utils.ConvertToInt32(reader["target_id"]);
            record.content = reader["content"].ToString();
            if (reader.HasColumn("action_id"))
            {
                record.action_id = Utils.ConvertToInt32(reader["action_id"]);
            }
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];

            return record;
        }

        public static IdentityAgencyNotification ExtractAgencyNotificationData(IDataReader reader)
        {
            var record = new IdentityAgencyNotification();

            //Seperate properties
            record.notification_id = Utils.ConvertToInt32(reader["notification_id"]);
            record.is_viewed = Utils.ConvertToBoolean(reader["is_viewed"]);
            record.is_read = Utils.ConvertToBoolean(reader["is_read"]);
            record.is_sent = Utils.ConvertToBoolean(reader["is_sent"]);
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
            if (reader.HasColumn("action_id"))
            {
                record.action_id = Utils.ConvertToInt32(reader["action_id"]);
            }
            record.read_at = reader["read_at"] == DBNull.Value ? null : (DateTime?)reader["read_at"];

            return record;
        }

        public int SinglePush(IdentityAgencyNotification identity)
        {
            //Common syntax           
            var sqlCmd = @"A_Notification_SinglePush";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@action_type", identity.action_type},
                {"@target_id", identity.target_id },
                {"@sender_id", identity.sender_id },
                {"@target_type", identity.target_type },
                {"@content", identity.content },
                {"@staff_id", identity.staff_id }
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

        public int MultiplePush(IdentityAgencyNotification identity, List<int> listIds)
        {
            //Common syntax           
            var sqlCmd = @"A_Notification_MultiplePush";
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
                {"@agency_id", identity.agency_id },
                {"@company_id", identity.company_id },
                {"@action_id", identity.action_id },
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

        public IdentityAgencyNotification GetById(int id)
        {
            IdentityAgencyNotification info = null;
            var sqlCmd = @"A_Notification_GetById";

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
                            info = ExtractAgencyNotificationData(reader);
                        }

                        //Base data
                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                var baseNotif = ExtractBaseAgencyNotificationData(reader);
                                info.action_type = baseNotif.action_type;
                                info.sender_id = baseNotif.sender_id;
                                info.target_type = baseNotif.target_type;
                                info.target_id = baseNotif.target_id;
                                info.content = baseNotif.content;
                                info.created_at = baseNotif.created_at;
                                info.notification_id = baseNotif.id;
                                info.action_id = baseNotif.action_id;
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
            var sqlCmd = @"A_Notification_Delete";

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

        public List<IdentityAgencyNotification> GetListByStaff(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax            
            var sqlCmd = @"A_Notification_GetListByStaff";

            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", filter.agency_id},
                {"@staff_id", filter.staff_id},
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@is_update_view", filter.is_update_view},
            };

            List<IdentityAgencyNotification> listData = new List<IdentityAgencyNotification>();
            List<IdentityAgencyNotificationBase> listBase = new List<IdentityAgencyNotificationBase>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        //Detail
                        while (reader.Read())
                        {                            
                            var record = ExtractAgencyNotificationData(reader);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);
                            
                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            //Base data
                            while (reader.Read())
                            {
                                var baseNotif = ExtractBaseAgencyNotificationData(reader);
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

        public int CountUnread(dynamic identity)
        {
            //Common syntax           
            var sqlCmd = @"A_Notification_CountUnread";
            var totalCount = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@staff_id", identity.staff_id }
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

        public int MarkIsRead(IdentityAgencyNotification info)
        {
            //Common syntax            
            var sqlCmd = @"A_Notification_MarkIsRead";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", info.id},
                {"@staff_id", info.staff_id},
                {"@is_read", info.is_read}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    status = Utils.ConvertToInt32(statusObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status;
        }

        #endregion
    }
}
