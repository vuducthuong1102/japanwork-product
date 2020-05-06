using ApiJobMarket.DB.Sql.Entities;
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
    public class RpsSchedule
    {
        private readonly string _connectionString;

        public RpsSchedule(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSchedule()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentitySchedule> GetByPage(IdentitySchedule filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Schedule_GetAll";
            List<IdentitySchedule> listData = null;

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
                        listData = ParsingListScheduleFromReader(reader);
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

        public List<IdentitySchedule> GetListByJob(IdentitySchedule filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Schedule_GetListByJob";
            List<IdentitySchedule> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                //{"@job_id", filter.job_id },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListScheduleFromReader(reader);
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

        public List<IdentitySchedule> GetByStaff(dynamic filter)
        {
            //Common syntax           
            var sqlCmd = @"A_Schedule_GetByStaff";
            List<IdentitySchedule> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", filter.agency_id },
                {"@staff_id", filter.staff_id },
                {"@start_time", filter.start_time },
                {"@end_time", filter.end_time },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListScheduleFromReader(reader);
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

        public List<IdentitySchedule> GetTodayByStaff(dynamic filter)
        {
            //Common syntax           
            var sqlCmd = @"A_Schedule_GetTodayByStaff";
            List<IdentitySchedule> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", filter.agency_id },
                {"@staff_id", filter.staff_id },
                {"@start_time", filter.start_time },
                {"@end_time", filter.end_time },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListScheduleFromReader(reader);
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

        public List<IdentitySchedule> GetListByJobId(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Schedule_GetListByJobId";
            List<IdentitySchedule> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@job_id", filter.job_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListScheduleFromReader(reader);
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


        private List<IdentitySchedule> ParsingListScheduleFromReader(IDataReader reader)
        {
            List<IdentitySchedule> listData = listData = new List<IdentitySchedule>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractScheduleData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentitySchedule ExtractScheduleData(IDataReader reader)
        {
            var record = new IdentitySchedule();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
            record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);
            record.schedule_cat = Utils.ConvertToInt32(reader["schedule_cat"]);
            record.title = reader["title"].ToString();
            record.content = reader["content"].ToString();
            record.start_time = reader["start_time"] == DBNull.Value ? null : (DateTime?)reader["start_time"];
            record.end_time = reader["end_time"] == DBNull.Value ? null : (DateTime?)reader["end_time"];
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

            record.status = Utils.ConvertToInt32(reader["status"]);
            return record;
        }

        public int Insert(IdentitySchedule identity)
        {
            //Common syntax           
            var sqlCmd = @"A_Schedule_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@staff_id", identity.staff_id },
                {"@title", identity.title },
                {"@content", identity.content },
                {"@pic_id", identity.pic_id },
                {"@schedule_cat", identity.schedule_cat },
                {"@start_time", identity.start_time },
                {"@end_time", identity.end_time },
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

        public int Update(IdentitySchedule identity)
        {
            //Common syntax
            var sqlCmd = @"A_Schedule_Update";
            var returnId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@agency_id", identity.agency_id},
                {"@staff_id", identity.staff_id },
                {"@pic_id", identity.pic_id },
                {"@schedule_cat", identity.schedule_cat },
                {"@title", identity.title },
                {"@content", identity.content },
                {"@start_time", identity.start_time },
                {"@end_time", identity.end_time }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    returnId = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnId;
        }

        public int UpdateTime(IdentitySchedule identity)
        {
            //Common syntax
            var sqlCmd = @"A_Schedule_UpdateTime";
            var returnId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@agency_id", identity.agency_id},
                {"@staff_id", identity.staff_id },
                {"@start_time", identity.start_time },
                {"@end_time", identity.end_time }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    returnId = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnId;
        }

        public IdentitySchedule GetDetail(IdentitySchedule identity)
        {
            IdentitySchedule info = null;
            var sqlCmd = @"A_Schedule_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@agency_id", identity.agency_id},
                {"@staff_id", identity.staff_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractScheduleData(reader);
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

        public bool Delete(IdentitySchedule identity)
        {
            //Common syntax            
            var sqlCmd = @"A_Schedule_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@agency_id", identity.agency_id},
                {"@staff_id", identity.staff_id}
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

        public List<IdentitySchedule> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Schedule_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentitySchedule> listData = new List<IdentitySchedule>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractScheduleData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityScheduleLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityScheduleLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.ScheduleId = Utils.ConvertToInt32(reader["ScheduleId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.ScheduleId == item.Id).ToList();
                        //    }
                        //}
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

        public List<IdentitySchedule> GetListByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Schedule_GetListByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentitySchedule> listData = new List<IdentitySchedule>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractScheduleData(reader);

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

        public List<IdentitySchedule> GetListByJob(int job_id)
        {
            //Common syntax            
            var sqlCmd = @"Schedule_GetListByJob";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
            };

            List<IdentitySchedule> listData = new List<IdentitySchedule>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractScheduleData(reader);

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
