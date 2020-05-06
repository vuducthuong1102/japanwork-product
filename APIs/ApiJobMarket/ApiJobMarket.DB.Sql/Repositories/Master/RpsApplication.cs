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
    public class RpsApplication
    {
        private readonly string _connectionString;

        public RpsApplication(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsApplication()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityApplication> GetByPage(IdentityApplication filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetAll";
            List<IdentityApplication> listData = null;

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
                        listData = ParsingListApplicationFromReader(reader);
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

        public List<IdentityApplication> GetListByJob(IdentityApplication filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListByJob";
            List<IdentityApplication> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@job_id", filter.job_id },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListApplicationFromReader(reader);
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

        public List<IdentityApplication> GetListByJobSeeker(IdentityApplication filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListByJobSeeker";
            List<IdentityApplication> listData = null;
            //List<IdentityJob> jobs = jobs = new List<IdentityJob>();
            //List<IdentityJobTranslation> translations = new List<IdentityJobTranslation>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@job_seeker_id", filter.job_seeker_id },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListApplicationFromReader(reader);                       

                        //if (listData.HasData())
                        //{
                        //    //Jobs
                        //    if (reader.NextResult())
                        //    {                                
                        //        while (reader.Read())
                        //        {
                        //            var job = RpsJob.ExtractJobData(reader);

                        //            jobs.Add(job);
                        //        }
                        //    }

                        //    //Translations
                        //    if (reader.NextResult())
                        //    {                                
                        //        while (reader.Read())
                        //        {
                        //            var tran = RpsJob.ExtractJobTranslationData(reader);

                        //            translations.Add(tran);
                        //        }
                        //    }

                        //    foreach (var item in listData)
                        //    {
                        //        item.job_info = jobs.Where(x => x.id == item.job_id).FirstOrDefault();

                        //        if(item.job_info != null)
                        //        {
                        //            item.job_info.Job_translations = translations.Where(x => x.job_id == item.job_info.id).ToList();
                        //        }
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

        public List<IdentityApplication> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListByAgency";
            List<IdentityApplication> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@company_id", filter.company_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@staff_id",filter.staff_id },
                {"@cv_id",filter.cv_id },
                {"@job_id",filter.job_id },
                {"@language_code",filter.language_code }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListApplicationFromReader(reader);
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

        public List<IdentityApplication> GetListByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListByPage";
            List<IdentityApplication> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@language_code",filter.language_code },
                {"@type_job_seeker",filter.type_job_seeker },
                {"@staff_id",filter.staff_id },
                {"@gender",filter.gender },
                {"@major_id",filter.major_id },
                {"@age_min",filter.age_min },
                {"@age_max",filter.age_max },
                {"@has_process",filter.has_process },
                {"@japanese_level_number",filter.japanese_level_number },
                {"@qualification_id",filter.qualification_id },

                {"@sub_field_id",filter.sub_field_id },
                {"@prefecture_id",filter.prefecture_id },
                {"@employment_type_id",filter.employment_type_id },
                {"@visa_id",filter.visa_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListApplicationFromReader(reader);
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

        public List<IdentityApplication> GetListInvited(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListInvited";
            List<IdentityApplication> listData = new List<IdentityApplication>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@company_id", filter.company_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@staff_id",filter.staff_id },
                {"@cv_id",filter.cv_id },
                {"@job_id",filter.job_id },
                {"@language_code",filter.language_code }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityApplication();
                            record.fullname = reader["fullname"].ToString();

                            if (reader.HasColumn("major"))
                                record.major = reader["major"].ToString();

                            if (reader.HasColumn("qualification"))
                                record.qualification = reader["qualification"].ToString();

                            if (reader.HasColumn("birthday"))
                                record.birthday = reader["birthday"] == DBNull.Value ? null : (DateTime?)reader["birthday"];

                            if (reader.HasColumn("created_at"))
                                record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];

                            if (reader.HasColumn("updated_at"))
                                record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                            if (reader.HasColumn("gender"))
                                record.gender = Utils.ConvertToInt32(reader["gender"]);

                            if (reader.HasColumn("japanese_level_number"))
                                record.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);

                            if (reader.HasColumn("qualification"))
                                record.qualification = reader["qualification"].ToString();

                            if (reader.HasColumn("cv_id"))
                                record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);

                            if (reader.HasColumn("pic_id"))
                                record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);

                            if (reader.HasColumn("job_seeker_id"))
                                record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("code"))
                                record.code = reader["code"].ToString();
                            record.status = Utils.ConvertToInt32(reader["status"]);


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

        public List<IdentityApplication> GetListOffline(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListOffline";
            List<IdentityApplication> listData = new List<IdentityApplication>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@company_id", filter.company_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@staff_id",filter.staff_id },
                {"@cv_id",filter.cv_id },
                {"@job_id",filter.job_id },
                {"@language_code",filter.language_code }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityApplication();
                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            record.fullname = reader["fullname"].ToString();

                            if (reader.HasColumn("qualification"))
                                record.qualification = reader["qualification"].ToString();

                            if (reader.HasColumn("birthday"))
                                record.birthday = reader["birthday"] == DBNull.Value ? null : (DateTime?)reader["birthday"];

                            if (reader.HasColumn("created_at"))
                                record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];

                            if (reader.HasColumn("gender"))
                                record.gender = Utils.ConvertToInt32(reader["gender"]);

                            if (reader.HasColumn("japanese_level_number"))
                                record.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);

                            if (reader.HasColumn("job_seeker_id"))
                                record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);

                            if (reader.HasColumn("cv_id"))
                                record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);

                            if (reader.HasColumn("pic_id"))
                                record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);

                            if (reader.HasColumn("code"))
                                record.code = reader["code"].ToString();

                            record.status = Utils.ConvertToInt32(reader["status"]);
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

        public List<IdentityApplication> GetListRecruited(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Application_GetListRecruited";
            List<IdentityApplication> listData = new List<IdentityApplication>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@company_id", filter.company_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@staff_id",filter.staff_id },
                {"@cv_id",filter.cv_id },
                {"@job_id",filter.job_id },
                {"@language_code",filter.language_code }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityApplication();
                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("major"))
                                record.major = reader["major"].ToString();

                            if (reader.HasColumn("qualification"))
                                record.qualification = reader["qualification"].ToString();

                            if (reader.HasColumn("code"))
                                record.code = reader["code"].ToString();

                            if (reader.HasColumn("job_seeker_id"))
                                record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);

                            if (reader.HasColumn("cv_id"))
                                record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);

                            if (reader.HasColumn("type"))
                                record.type = Utils.ConvertToInt32(reader["type"]);

                            if (reader.HasColumn("updated_at"))
                                record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                            if (reader.HasColumn("is_show_info"))
                                record.is_show_info = Utils.ConvertToBoolean(reader["is_show_info"]);

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

        public bool UpdatePic(int job_seeker_id, int agency_id, int pic_id)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_UpdatePic";
        
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id },
                {"@agency_id", agency_id },
                {"@pic_id", pic_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }

        private List<IdentityApplication> ParsingListApplicationFromReader(IDataReader reader)
        {
            List<IdentityApplication> listData = new List<IdentityApplication>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractApplicationData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                if (reader.HasColumn("pic_id"))
                    record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);

                if (reader.HasColumn("job_pic_id"))
                    record.job_pic_id = Utils.ConvertToInt32(reader["job_pic_id"]);

                if (reader.HasColumn("major"))
                    record.major = reader["major"].ToString();

                if (reader.HasColumn("email"))
                    record.email = reader["email"].ToString();

                if (reader.HasColumn("qualification"))
                    record.qualification = reader["qualification"].ToString();

                if (reader.HasColumn("birthday"))
                    record.birthday = reader["birthday"] == DBNull.Value ? null : (DateTime?)reader["birthday"];

                if (reader.HasColumn("gender"))
                    record.gender = Utils.ConvertToInt32(reader["gender"]);

                if (reader.HasColumn("japanese_level_number"))
                    record.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);

                if (reader.HasColumn("qualification"))
                    record.qualification = reader["qualification"].ToString();

                if (reader.HasColumn("cv_id"))
                    record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);

                if (reader.HasColumn("is_show_info"))
                {
                    record.is_show_info = Utils.ConvertToBoolean(reader["is_show_info"]);
                }

                if (reader.HasColumn("code"))
                {
                    record.code = reader["code"].ToString();
                }

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityApplication ExtractApplicationData(IDataReader reader)
        {
            var record = new IdentityApplication();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);            
            record.interview_accept_time = reader["interview_accept_time"] == DBNull.Value ? null : (DateTime?)reader["interview_accept_time"];
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            if (reader.HasColumn("fullname"))
            {
                record.fullname = reader["fullname"].ToString();
            }

            record.status = Utils.ConvertToInt32(reader["status"]);
            
            

            return record;
        }

        public int Insert(IdentityApplication identity)
        {
            //Common syntax           
            var sqlCmd = @"Application_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@cv_id", identity.cv_id},
                {"@job_id", identity.job_id },
                {"@job_seeker_id", identity.job_seeker_id }
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

        public bool Update(IdentityApplication identity)
        {
            //Common syntax
            var sqlCmd = @"Application_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@cv_id", identity.cv_id},
                {"@job_id", identity.job_id },
                {"@job_seeker_id", identity.job_seeker_id }
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
        public int Deletes(string ids, int agency_id, int staff_id)
        {
            //Common syntax
            var sqlCmd = @"Application_Deletes";
            int result = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ids", ids},
                {"@agency_id", agency_id},
                {"@staff_id", staff_id }
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
        public IdentityApplication GetById(int id)
        {
            IdentityApplication info = null;
            var sqlCmd = @"Application_GetById";

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
                            info = ExtractApplicationData(reader);
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
            var sqlCmd = @"Application_Delete";

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

        public int Cancel(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"Application_Cancel";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", applyInfo.job_id},
                {"@job_seeker_id", applyInfo.job_seeker_id},
                {"@cv_id", applyInfo.cv_id }
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

        public int Ignore(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"Application_Ignore";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", applyInfo.id},
                {"@agency_id", applyInfo.agency_id},
                {"@cv_id", applyInfo.cv_id},
                {"@staff_id", applyInfo.staff_id},
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

        public int Accept(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"Application_Accept";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", applyInfo.id},
                {"@agency_id", applyInfo.agency_id},
                {"@cv_id", applyInfo.cv_id},
                {"@staff_id", applyInfo.staff_id},
                {"@pic_id", applyInfo.pic_id},
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

        public int SendCv(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"Application_SendCv";

            var pic_id = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", applyInfo.job_id},
                {"@job_seeker_id", applyInfo.job_seeker_id},
                {"@cv_id", applyInfo.cv_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    pic_id = Utils.ConvertToInt32(statusObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return pic_id;
        }

        public int Interview(int id)
        {
            //Common syntax            
            var sqlCmd = @"Application_Interview";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
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

        public List<IdentityApplication> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Application_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityApplication> listData = new List<IdentityApplication>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractApplicationData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityApplicationLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityApplicationLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.ApplicationId = Utils.ConvertToInt32(reader["ApplicationId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.ApplicationId == item.Id).ToList();
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

        public List<IdentityApplication> GetListByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Application_GetListByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityApplication> listData = new List<IdentityApplication>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractApplicationData(reader);

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

        public List<IdentityApplication> GetListByJob(int job_id)
        {
            //Common syntax            
            var sqlCmd = @"Application_GetListByJob";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
            };

            List<IdentityApplication> listData = new List<IdentityApplication>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractApplicationData(reader);

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

        public IdentityApplication CheckJobApplied(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Application_CheckJobApplied";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
            };

            IdentityApplication info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractApplicationData(reader);
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

#endregion
    }
}
