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
    public class RpsEduHistory
    {
        private readonly string _connectionString;

        public RpsEduHistory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsEduHistory()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityEduHistory> GetByPage(IdentityEduHistory filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"EduHistory_GetAll";
            List<IdentityEduHistory> listData = null;

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
                        listData = ParsingListEduHistoryFromReader(reader);
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
       
        private List<IdentityEduHistory> ParsingListEduHistoryFromReader(IDataReader reader)
        {
            List<IdentityEduHistory> listData = listData = new List<IdentityEduHistory>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractEduHistoryData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityEduHistory ExtractEduHistoryData(IDataReader reader)
        {
            var record = new IdentityEduHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.school = reader["school"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);


            return record;
        }

        public static IdentityJobSeekerEduHistory ExtractJobSeekerEduHistoryData(IDataReader reader)
        {
            var record = new IdentityJobSeekerEduHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.school = reader["school"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);
            record.major_custom = reader["major_custom"].ToString();

            return record;
        }

        public static IdentityCvEduHistory ExtractCvEduHistoryData(IDataReader reader)
        {
            var record = new IdentityCvEduHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.school = reader["school"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);
            record.major_custom = reader["major_custom"].ToString();

            return record;
        }

        public static IdentityCsEduHistory ExtractCsEduHistoryData(IDataReader reader)
        {
            var record = new IdentityCsEduHistory();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cs_id = Utils.ConvertToInt32(reader["cs_id"]);
            record.school = reader["school"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.address = reader["address"].ToString();
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);
            record.major_custom = reader["major_custom"].ToString();

            return record;
        }

        public int Insert(IdentityEduHistory identity)
        {
            //Common syntax           
            var sqlCmd = @"EduHistory_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id },
                { "@school", identity.school },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@qualification_id", identity.qualification_id },
                { "@major_id", identity.major_id }
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

        public bool Update(IdentityEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"EduHistory_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id },
                { "@school", identity.school },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@qualification_id", identity.qualification_id },
                { "@major_id", identity.major_id }
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

        public IdentityEduHistory GetById(int id)
        {
            IdentityEduHistory info = null;
            var sqlCmd = @"EduHistory_GetById";

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
                            info = ExtractEduHistoryData(reader);
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
            var sqlCmd = @"EduHistory_Delete";

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

        public List<IdentityEduHistory> GetList()
        {
            //Common syntax            
            var sqlCmd = @"EduHistory_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityEduHistory> listData = new List<IdentityEduHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractEduHistoryData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityEduHistoryLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityEduHistoryLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.EduHistoryId = Utils.ConvertToInt32(reader["EduHistoryId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.EduHistoryId == item.Id).ToList();
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

        public IdentityJobSeekerEduHistory JobSeekerGetDetail(int id, int job_seeker_id)
        {
            IdentityJobSeekerEduHistory info = null;
            var sqlCmd = @"JobSeeker_EduHistory_GetDetail";

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
                            info = ExtractJobSeekerEduHistoryData(reader);
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

        public bool JobSeekerUpdate(IdentityJobSeekerEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@school", identity.school },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@qualification_id", identity.qualification_id },
                { "@major_id", identity.major_id },
                { "@major_custom", identity.major_custom }
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

        public bool JobSeekerDelete(IdentityJobSeekerEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_DeleteEduHistory";

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

        public List<IdentityJobSeekerEduHistory> GetListJobSeekerEduHistory(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_GetEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", job_seeker_id}
            };

            List<IdentityJobSeekerEduHistory> listData = new List<IdentityJobSeekerEduHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerEduHistoryData(reader);

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

        public IdentityJobSeekerEduHistory A_JobSeekerGetDetail(int id, int job_seeker_id)
        {
            IdentityJobSeekerEduHistory info = null;
            var sqlCmd = @"A_JobSeeker_EduHistory_GetDetail";

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
                            info = ExtractJobSeekerEduHistoryData(reader);
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

        public bool A_JobSeekerUpdate(IdentityJobSeekerEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_UpdateEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@school", identity.school },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@qualification_id", identity.qualification_id },
                { "@major_id", identity.major_id },
                { "@major_custom", identity.major_custom }
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

        public bool A_JobSeekerDelete(IdentityJobSeekerEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_DeleteEduHistory";

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

        public List<IdentityJobSeekerEduHistory> A_GetListJobSeekerEduHistory(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"A_JobSeeker_GetEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", job_seeker_id}
            };

            List<IdentityJobSeekerEduHistory> listData = new List<IdentityJobSeekerEduHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerEduHistoryData(reader);

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

        public IdentityCvEduHistory CvGetDetail(int id, int cv_id)
        {
            IdentityCvEduHistory info = null;
            var sqlCmd = @"Cv_EduHistory_GetDetail";

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
                            info = ExtractCvEduHistoryData(reader);
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

        public int CvUpdate(IdentityCvEduHistory identity)
        {
            var returnId = identity.id;

            //Common syntax
            var sqlCmd = @"Cv_UpdateEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id },
                { "@school", identity.school },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@qualification_id", identity.qualification_id },
                { "@major_id", identity.major_id },
                { "@major_custom", identity.major_custom }
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

        public bool CvDelete(IdentityCvEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"Cv_DeleteEduHistory";

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

        public List<IdentityCvEduHistory> GetListCvEduHistory(int cv_id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_GetEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", cv_id}
            };

            List<IdentityCvEduHistory> listData = new List<IdentityCvEduHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCvEduHistoryData(reader);

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

        public IdentityCsEduHistory CsGetDetail(int id, int cs_id)
        {
            IdentityCsEduHistory info = null;
            var sqlCmd = @"Cs_EduHistory_GetDetail";

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
                            info = ExtractCsEduHistoryData(reader);
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

        public int CsUpdate(IdentityCsEduHistory identity)
        {
            var returnId = identity.id;

            //Common syntax
            var sqlCmd = @"Cs_UpdateEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cs_id", identity.cs_id },
                { "@school", identity.school },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@address", identity.address },
                { "@qualification_id", identity.qualification_id },
                { "@major_id", identity.major_id },
                { "@major_custom", identity.major_custom }
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

        public bool CsDelete(IdentityCsEduHistory identity)
        {
            //Common syntax
            var sqlCmd = @"Cs_DeleteEduHistory";

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

        public List<IdentityCsEduHistory> GetListCsEduHistory(int cs_id)
        {
            //Common syntax            
            var sqlCmd = @"Cs_GetEduHistory";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", cs_id}
            };

            List<IdentityCsEduHistory> listData = new List<IdentityCsEduHistory>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCsEduHistoryData(reader);

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
