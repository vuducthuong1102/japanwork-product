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
    public class RpsWorkHistory
    {
        private readonly string _connectionString;

        public RpsWorkHistory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsWorkHistory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityWorkHistory> GetByPage(IdentityWorkHistory filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"WorkHistory_GetAll";
            List<IdentityWorkHistory> listData = null;

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
                        listData = ParsingListWorkHistoryFromReader(reader);
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
       
        private List<IdentityWorkHistory> ParsingListWorkHistoryFromReader(IDataReader reader)
        {
            List<IdentityWorkHistory> listData = listData = new List<IdentityWorkHistory>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractWorkHistoryData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityWorkHistory ExtractWorkHistoryData(IDataReader reader)
        {
            var record = new IdentityWorkHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.company = reader["company"].ToString();
            record.content_work = reader["content_work"].ToString();
            record.form = Utils.ConvertToInt32(reader["form"]);
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();

            return record;
        }

        public static IdentityJobSeekerWorkHistory ExtractJobSeekerWorkHistoryData(IDataReader reader)
        {
            var record = new IdentityJobSeekerWorkHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.company = reader["company"].ToString();
            record.content_work = reader["content_work"].ToString();
            record.form = Utils.ConvertToInt32(reader["form"]);
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();

            return record;
        }

        public static IdentityCvWorkHistory ExtractCvWorkHistoryData(IDataReader reader)
        {
            var record = new IdentityCvWorkHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.company = reader["company"].ToString();
            record.content_work = reader["content_work"].ToString();
            record.form = Utils.ConvertToInt32(reader["form"]);
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();

            return record;
        }

        public static IdentityCsWorkHistory ExtractCsWorkHistoryData(IDataReader reader)
        {
            var record = new IdentityCsWorkHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cs_id = Utils.ConvertToInt32(reader["cs_id"]);
            record.company = reader["company"].ToString();
            record.sub_field_id = Utils.ConvertToInt32(reader["sub_field_id"]);
            record.sub_industry_id = Utils.ConvertToInt32(reader["sub_industry_id"]);
            record.employment_type_id = Utils.ConvertToInt32(reader["employment_type_id"]);
            record.employees_number = Utils.ConvertToInt32(reader["employees_number"]);
            record.resign_reason = reader["resign_reason"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();

            return record;
        }

        public static IdentityCsWorkHistoryDetail ExtractCsWorkHistoryDetailData(IDataReader reader)
        {
            var record = new IdentityCsWorkHistoryDetail();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cs_work_history_id = Utils.ConvertToInt32(reader["cs_work_history_id"]);
            record.department = reader["department"].ToString();
            record.position = reader["position"].ToString();
            record.salary = Utils.ConvertToInt32(reader["salary"]);            
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
         
            record.content_work = reader["content_work"].ToString();

            return record;
        }

        public int Insert(IdentityWorkHistory identity)
        {
            //Common syntax           
            var sqlCmd = @"WorkHistory_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@cv_id", identity.cv_id },
                { "@company", identity.company },
                { "@content_work", identity.content_work },
                { "@form", identity.form },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address }
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

        public bool Update(IdentityWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"WorkHistory_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id },
                { "@company", identity.company },
                { "@content_work", identity.content_work },
                { "@form", identity.form },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address }
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

        public IdentityWorkHistory GetById(int id)
        {
            IdentityWorkHistory info = null;
            var sqlCmd = @"WorkHistory_GetById";

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
                            info = ExtractWorkHistoryData(reader);
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
            var sqlCmd = @"WorkHistory_Delete";

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

        public List<IdentityWorkHistory> GetList()
        {
            //Common syntax            
            var sqlCmd = @"WorkHistory_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityWorkHistory> listData = new List<IdentityWorkHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractWorkHistoryData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityWorkHistoryLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityWorkHistoryLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.WorkHistoryId = Utils.ConvertToInt32(reader["WorkHistoryId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.WorkHistoryId == item.Id).ToList();
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

        #region JobSeeker

        public IdentityJobSeekerWorkHistory JobSeekerGetDetail(int id, int job_seeker_id)
        {
            IdentityJobSeekerWorkHistory info = null;
            var sqlCmd = @"JobSeeker_WorkHistory_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@job_seeker_id", job_seeker_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobSeekerWorkHistoryData(reader);
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

        public bool JobSeekerUpdate(IdentityJobSeekerWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@company", identity.company },
                { "@content_work", identity.content_work },
                { "@form", identity.form },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address }
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

        public bool JobSeekerDelete(IdentityJobSeekerWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_DeleteWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id }               
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

        public List<IdentityJobSeekerWorkHistory> GetListJobSeekerWorkHistory(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_GetWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", job_seeker_id}
            };

            List<IdentityJobSeekerWorkHistory> listData = new List<IdentityJobSeekerWorkHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerWorkHistoryData(reader);

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

        #region Agency

        public IdentityJobSeekerWorkHistory A_JobSeekerGetDetail(int id, int job_seeker_id)
        {
            IdentityJobSeekerWorkHistory info = null;
            var sqlCmd = @"A_JobSeeker_WorkHistory_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@job_seeker_id", job_seeker_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobSeekerWorkHistoryData(reader);
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

        public bool A_JobSeekerUpdate(IdentityJobSeekerWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_UpdateWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@company", identity.company },
                { "@content_work", identity.content_work },
                { "@form", identity.form },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address }
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

        public bool A_JobSeekerDelete(IdentityJobSeekerWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_DeleteWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id }
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

        public List<IdentityJobSeekerWorkHistory> A_GetListJobSeekerWorkHistory(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"A_JobSeeker_GetWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", job_seeker_id}
            };

            List<IdentityJobSeekerWorkHistory> listData = new List<IdentityJobSeekerWorkHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerWorkHistoryData(reader);

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

        #region Cv

        public IdentityCvWorkHistory CvGetDetail(int id, int cv_id)
        {
            IdentityCvWorkHistory info = null;
            var sqlCmd = @"Cv_WorkHistory_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@cv_id", cv_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCvWorkHistoryData(reader);
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

        public int CvUpdate(IdentityCvWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"Cv_UpdateWorkHistory";
            var returnId = identity.id;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id },
                { "@company", identity.company },
                { "@content_work", identity.content_work },
                { "@form", identity.form },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var objReturn = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    returnId = Utils.ConvertToInt32(objReturn);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnId;
        }

        public bool CvDelete(IdentityCvWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"Cv_DeleteWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id }
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

        public List<IdentityCvWorkHistory> GetListCvWorkHistory(int cv_id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_GetWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", cv_id}
            };

            List<IdentityCvWorkHistory> listData = new List<IdentityCvWorkHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCvWorkHistoryData(reader);

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

        #region Cs

        public IdentityCsWorkHistory CsGetDetail(int id, int cs_id)
        {
            IdentityCsWorkHistory info = null;
            List< IdentityCsWorkHistoryDetail> details = new List<IdentityCsWorkHistoryDetail>();
            var sqlCmd = @"Cs_WorkHistory_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@cs_id", cs_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCsWorkHistoryData(reader);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var dt = ExtractCsWorkHistoryDetailData(reader);
                                details.Add(dt);
                            }
                        }

                        if (info != null)
                            info.Details = details;
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

        public int CsUpdate(IdentityCsWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"Cs_UpdateWorkHistory";
            var returnId = identity.id;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cs_id", identity.cs_id },
                { "@company", identity.company },
                { "@sub_field_id", identity.sub_field_id },
                { "@sub_industry_id", identity.sub_industry_id },
                { "@employment_type_id", identity.employment_type_id },
                { "@employees_number", identity.employees_number },
                { "@resign_reason", identity.resign_reason },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@RemoveDetailItems", identity.RemoveDetailItems }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var objReturn = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    returnId = Utils.ConvertToInt32(objReturn);

                    if(returnId > 0)
                    {
                        if (identity.Details.HasData())
                        {
                            var cmdDetail = @"Cs_UpdateWorkHistoryDetail";

                            foreach (var item in identity.Details)
                            {
                                var myParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_work_history_id", returnId },
                                    {"@department", item.department },
                                    {"@position", item.position },
                                    {"@salary", item.salary },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@content_work", item.content_work }
                                };

                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdDetail, myParams);
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

            return returnId;
        }

        public bool CsDelete(IdentityCsWorkHistory identity)
        {
            //Common syntax
            var sqlCmd = @"Cs_DeleteWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cs_id", identity.cs_id }
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

        public List<IdentityCsWorkHistory> GetListCsWorkHistory(int cs_id)
        {
            //Common syntax            
            var sqlCmd = @"Cs_GetWorkHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", cs_id}
            };

            List<IdentityCsWorkHistory> listData = new List<IdentityCsWorkHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCsWorkHistoryData(reader);

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

        #endregion
    }
}
