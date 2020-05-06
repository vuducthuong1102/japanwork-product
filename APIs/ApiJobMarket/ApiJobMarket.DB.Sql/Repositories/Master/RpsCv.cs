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
using System.Text;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsCv
    {
        private readonly string _connectionString;

        public RpsCv(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCv()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityCv> GetByPage(IdentityCv filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_GetAll";
            List<IdentityCv> listData = null;

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
                        listData = ParsingListCvFromReader(reader);
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

        public List<IdentityCv> GetListByJobSeeker(IdentityCv filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_GetListByJobSeeker";
            List<IdentityCv> listData = null;

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
                        listData = ParsingListCvFromReader(reader);
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

        public List<IdentityCv> GetListCVSentToAgency(IdentityCv filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_GetListSentToAgency";
            List<IdentityCv> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@job_seeker_id", filter.job_seeker_id },
                {"@agency_id", filter.agency_id },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListCvFromReader(reader);
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

        public List<IdentityCv> SearchByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_SearchByPage";
            List<IdentityCv> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //Parsing cities
            var listCityIds = string.Empty;
            var city_ids = new List<int>();
            if (filter.city_ids != null)
                city_ids = (List<int>)filter.city_ids;

            if (city_ids.HasData())
                listCityIds = String.Join(",", city_ids);

            //Parsing prefectures
            var listPrefectureIds = string.Empty;
            var prefecture_ids = new List<int>();
            if (filter.prefecture_ids != null)
                prefecture_ids = (List<int>)filter.prefecture_ids;

            if (prefecture_ids.HasData())
                listPrefectureIds = String.Join(",", prefecture_ids);

            //Parsing stations
            var listStationIds = string.Empty;
            var station_ids = new List<int>();
            if (filter.station_ids != null)
                station_ids = (List<int>)filter.station_ids;

            if (station_ids.HasData())
                listStationIds = String.Join(",", station_ids);

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.title },
                {"@employment_type_id", filter.employment_type_id },
                {"@field_id", filter.field_id },
                {"@language_code", filter.language_code },
                {"@salary_min", filter.salary_min },
                {"@salary_max", filter.salary_max },
                {"@city_ids", listCityIds },
                {"@prefecture_ids", listPrefectureIds },
                {"@station_ids", listStationIds },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListCvFromReader(reader);
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

        public List<IdentityCv> GetByAgency(IdentityCv filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_GetListByAgency";
            List<IdentityCv> listData = new List<IdentityCv>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_parent_id", filter.agency_parent_id },
                {"@agency_id", filter.agency_id },
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
                            var cvInfo = new IdentityCv();
                            if (listData.Count == 0)
                            {
                                cvInfo.total_count = Utils.ConvertToInt32(reader["total_count"]);
                            }
                            cvInfo.fullname = reader["fullname"].ToString();
                            cvInfo.email = reader["email"].ToString();
                            cvInfo.phone = reader["phone"].ToString();
                            cvInfo.id = Utils.ConvertToInt32(reader["id"]);
                            cvInfo.image = reader["image"].ToString();
                            cvInfo.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
                            cvInfo.company_count = Utils.ConvertToInt32(reader["company_count"]);
                            cvInfo.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);
                            listData.Add(cvInfo);
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

        private List<IdentityCv> ParsingListCvFromReader(IDataReader reader)
        {
            List<IdentityCv> listData = listData = new List<IdentityCv>();
            while (reader.Read())
            {
                //Get common information
                var record = new IdentityCv();
                 record.id = Utils.ConvertToInt32(reader["id"]);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityCv ExtractCvData(IDataReader reader)
        {
            var record = new IdentityCv();

            //Seperate properties;
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.cv_title = reader["cv_title"].ToString();
            record.date = reader["date"] == DBNull.Value ? null : (DateTime?)reader["date"];

            //record.fullname = reader["fullname"].ToString();
            //record.fullname_furigana = reader["fullname_furigana"].ToString();
            //record.gender = Utils.ConvertToInt32(reader["gender"]);
            //record.birthday = reader["birthday"] == DBNull.Value ? null : (DateTime?)reader["birthday"];
            //record.email = reader["email"].ToString();
            //record.phone = reader["phone"].ToString();
            //record.marriage = Utils.ConvertToBoolean(reader["marriage"]);
            //record.dependent_num = Utils.ConvertToInt32(reader["dependent_num"]);            
            //record.image = reader["image"].ToString();

            record.highest_edu = Utils.ConvertToInt32(reader["highest_edu"]);
            record.pr = reader["pr"].ToString();
            record.hobby_skills = reader["hobby_skills"].ToString();
            record.reason = reader["reason"].ToString();
            record.time_work = reader["time_work"].ToString();
            record.aspiration = reader["aspiration"].ToString();
            record.form = Utils.ConvertToInt32(reader["form"]);            
            record.pdf = reader["pdf"].ToString();
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.reason_pr = reader["reason_pr"].ToString();
            record.contact_phone = reader["contact_phone"].ToString();
            record.check_address = Utils.ConvertToBoolean(reader["check_address"]);
            record.check_work = Utils.ConvertToBoolean(reader["check_work"]);
            record.check_ceti = Utils.ConvertToBoolean(reader["check_ceti"]);
            record.check_timework = Utils.ConvertToBoolean(reader["check_timework"]);
            record.main_cv = Utils.ConvertToInt32(reader["main_cv"]);
            //record.station_id = Utils.ConvertToInt32(reader["station_id"]);
            //record.train_line_id = Utils.ConvertToInt32(reader["train_line_id"]);
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);

            return record;
        }

        public static IdentityJobSeekerAddress ExtractJobSeekerAddressData(IDataReader reader)
        {
            var record = new IdentityJobSeekerAddress();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.country_id = Utils.ConvertToInt32(reader["country_id"]);
            record.region_id = Utils.ConvertToInt32(reader["region_id"]);
            record.prefecture_id = Utils.ConvertToInt32(reader["prefecture_id"]);
            record.city_id = Utils.ConvertToInt32(reader["city_id"]);
            record.furigana = reader["furigana"].ToString();
            record.detail = reader["detail"].ToString();
            record.note = reader["note"].ToString();
            record.lat = reader["lat"].ToString();
            record.lng = reader["lng"].ToString();
            record.is_contact_address = Utils.ConvertToBoolean(reader["is_contact_address"]);
            record.postal_code = reader["postal_code"].ToString();
            record.train_line_id = Utils.ConvertToInt32(reader["train_line_id"]);
            record.station_id = Utils.ConvertToInt32(reader["station_id"]);

            return record;
        }

        public static IdentityJobSeekerCertificate ExtractJobSeekerCertificationData(IDataReader reader)
        {
            var record = new IdentityJobSeekerCertificate();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.name = reader["name"].ToString();            
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.point = reader["point"].ToString();
            record.pass = Utils.ConvertToInt32(reader["pass"]);

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
            record.address = reader["address"].ToString();
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);
            record.major_custom = reader["major_custom"].ToString();
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);

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
            record.address = reader["address"].ToString();

            return record;
        }

        public static IdentityCvPdfCode ExtractCvPdfCodeData(IDataReader reader)
        {
            var record = new IdentityCvPdfCode();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.user_id = Utils.ConvertToInt32(reader["user_id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.code_id = reader["code_id"].ToString();
            record.form = Utils.ConvertToInt32(reader["form"]);
            record.work_background_id = Utils.ConvertToInt32(reader["work_background_id"]);
            record.title = reader["title"].ToString();
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.expired_at_utc = reader["expired_at_utc"] == DBNull.Value ? null : (DateTime?)reader["expired_at_utc"];

            return record;
        }

        public int Insert(IdentityCv identity)
        {
            //Common syntax           
            var sqlCmd = @"Cv_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", identity.job_seeker_id },
                {"@cv_title", identity.cv_title },
                {"@date", identity.date },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@gender", identity.gender },
                {"@birthday", identity.birthday },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@marriage", identity.marriage },
                {"@dependent_num", identity.dependent_num },
                {"@highest_edu", identity.highest_edu },
                {"@pr", identity.pr },
                {"@hobby_skills", identity.hobby_skills },
                {"@reason", identity.reason },
                {"@time_work", identity.time_work },
                {"@aspiration", identity.aspiration },
                {"@form", identity.form },
                {"@image", identity.image },
                {"@pdf", identity.pdf },               
                {"@reason_pr", identity.reason_pr },
                {"@contact_phone", identity.contact_phone },
                {"@check_address", identity.check_address },
                {"@check_work", identity.check_work },
                {"@check_ceti", identity.check_ceti },
                {"@check_timework", identity.check_timework },
                {"@check_aspiration", identity.check_aspiration },
                {"@station_id", identity.station_id },
                {"@train_line_id", identity.train_line_id },
                {"@japanese_level_number", identity.japanese_level_number }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);

                    if (newId > 0)
                    {
                        if (identity.certification.HasData())
                        {
                            var cmdCertificate = @"Cv_AddCertificate";

                            foreach (var item in identity.certification)
                            {
                                var cerParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cv_id", newId },
                                    {"@name", item.name },
                                    {"@start_date", item.start_date },
                                    {"@pass", item.pass },
                                    {"@point", item.point }
                                };
                               
                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdCertificate, cerParams);                               
                            }                               
                        }

                        if (identity.edu_history.HasData())
                        {
                            var cmdEduHistory = @"Cv_AddEduHistory";

                            foreach (var item in identity.edu_history)
                            {
                                var eduParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cv_id", newId },
                                    {"@school", item.school },
                                    {"@status", item.status },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@address", item.address },
                                    {"@qualification_id", item.qualification_id },
                                    {"@major_id", item.major_id }
                                };
                                
                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdEduHistory, eduParams);
                            }
                        }

                        if (identity.work_history.HasData())
                        {
                            var cmdWorkHistory = @"Cv_AddWorkHistory";

                            foreach (var item in identity.work_history)
                            {
                                var workParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cv_id", newId },
                                    {"@company", item.company },
                                    {"@content_work", item.content_work },
                                    {"@form", item.form },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@status", item.status },
                                    {"@address", item.address }
                                };

                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdWorkHistory, workParams);
                            }
                        }    
                        
                        if(identity.address != null)
                        {
                            var cmdAddress = @"Cv_AddAddress";
                            var addressParams = new Dictionary<string, object>
                            {
                                {"@cv_id", newId },
                                {"@country_id", identity.address.country_id },
                                {"@region_id", identity.address.region_id },
                                {"@prefecture_id", identity.address.prefecture_id },
                                {"@city_id", identity.address.city_id },
                                {"@detail", identity.address.detail },
                                {"@furigana", identity.address.furigana },
                                {"@note", identity.address.note },
                                {"@lat", identity.address.lat },
                                {"@lng", identity.address.lng },
                                {"@is_contact_address", identity.address.is_contact_address },
                                {"@postal_code", identity.address.postal_code },
                                {"@train_line_id", identity.address.train_line_id },
                                {"@station_id", identity.address.station_id }
                            };

                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdAddress, addressParams);
                        }

                        if (identity.address_contact != null)
                        {
                            var cmdAddressContact = @"Cv_AddAddress";
                            var addressContactParams = new Dictionary<string, object>
                            {
                                {"@cv_id", newId },
                                {"@country_id", identity.address_contact.country_id },
                                {"@region_id", identity.address_contact.region_id },
                                {"@prefecture_id", identity.address_contact.prefecture_id },
                                {"@city_id", identity.address_contact.city_id },
                                {"@detail", identity.address_contact.detail },
                                {"@furigana", identity.address_contact.furigana },
                                {"@note", identity.address_contact.note },
                                {"@lat", identity.address_contact.lat },
                                {"@lng", identity.address_contact.lng },
                                {"@is_contact_address", identity.address_contact.is_contact_address },
                                {"@postal_code", identity.address_contact.postal_code },
                                {"@train_line_id", identity.address_contact.train_line_id },
                                {"@station_id", identity.address_contact.station_id }
                            };

                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdAddressContact, addressContactParams);
                        }
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

        public bool Update(IdentityCv identity)
        {
            //Common syntax
            var sqlCmd = @"Cv_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@job_seeker_id", identity.job_seeker_id },
                {"@cv_title", identity.cv_title },
                {"@date", identity.date },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@gender", identity.gender },
                {"@birthday", identity.birthday },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@marriage", identity.marriage },
                {"@dependent_num", identity.dependent_num },
                {"@highest_edu", identity.highest_edu },
                {"@pr", identity.pr },
                {"@hobby_skills", identity.hobby_skills },
                {"@reason", identity.reason },
                {"@time_work", identity.time_work },
                {"@aspiration", identity.aspiration },
                {"@form", identity.form },
                {"@image", identity.image },
                {"@pdf", identity.pdf },
                {"@reason_pr", identity.reason_pr },
                {"@contact_phone", identity.contact_phone },
                {"@check_address", identity.check_address },
                {"@check_work", identity.check_work },
                {"@check_ceti", identity.check_ceti },
                {"@check_timework", identity.check_timework },
                {"@check_aspiration", identity.check_aspiration },
                {"@station_id", identity.station_id },
                {"@train_line_id", identity.train_line_id },
                {"@japanese_level_number", identity.japanese_level_number }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var currentIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    var currentId = Utils.ConvertToInt32(currentIdObj);
                    if (currentId > 0)
                    {
                        if (identity.certification.HasData())
                        {
                            var cmdCertificate = @"Cv_AddCertificate";

                            foreach (var item in identity.certification)
                            {
                                var cerParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cv_id", currentId },
                                    {"@name", item.name },
                                    {"@start_date", item.start_date },
                                    {"@pass", item.pass },
                                    {"@point", item.point }
                                };

                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdCertificate, cerParams);
                            }
                        }

                        if (identity.edu_history.HasData())
                        {
                            var cmdEduHistory = @"Cv_AddEduHistory";

                            foreach (var item in identity.edu_history)
                            {
                                var eduParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cv_id", currentId },
                                    {"@school", item.school },
                                    {"@status", item.status },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@address", item.address },
                                    {"@qualification_id", item.qualification_id },
                                    {"@major_id", item.major_id }
                                };

                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdEduHistory, eduParams);
                            }
                        }

                        if (identity.work_history.HasData())
                        {
                            var cmdWorkHistory = @"Cv_AddWorkHistory";

                            foreach (var item in identity.work_history)
                            {
                                var workParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cv_id", currentId },
                                    {"@company", item.company },
                                    {"@content_work", item.content_work },
                                    {"@form", item.form },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@status", item.status },
                                    {"@address", item.address }
                                };

                                MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdWorkHistory, workParams);
                            }
                        }

                        if (identity.address != null)
                        {
                            var cmdAddress = @"Cv_AddAddress";
                            var addressParams = new Dictionary<string, object>
                            {
                                {"@cv_id", currentId },
                                {"@country_id", identity.address.country_id },
                                {"@region_id", identity.address.region_id },
                                {"@prefecture_id", identity.address.prefecture_id },
                                {"@city_id", identity.address.city_id },
                                {"@detail", identity.address.detail },
                                {"@furigana", identity.address.furigana },
                                {"@note", identity.address.note },
                                {"@lat", identity.address.lat },
                                {"@lng", identity.address.lng },
                                {"@is_contact_address", identity.address.is_contact_address },
                                {"@postal_code", identity.address.postal_code },
                                {"@train_line_id", identity.address.train_line_id },
                                {"@station_id", identity.address.station_id }
                            };

                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdAddress, addressParams);
                        }

                        if (identity.address_contact != null)
                        {
                            var cmdAddressContact = @"Cv_AddAddress";
                            var addressContactParams = new Dictionary<string, object>
                            {
                                {"@cv_id", currentId },
                                {"@country_id", identity.address_contact.country_id },
                                {"@region_id", identity.address_contact.region_id },
                                {"@prefecture_id", identity.address_contact.prefecture_id },
                                {"@city_id", identity.address_contact.city_id },
                                {"@detail", identity.address_contact.detail },
                                {"@furigana", identity.address_contact.furigana },
                                {"@note", identity.address_contact.note },
                                {"@lat", identity.address_contact.lat },
                                {"@lng", identity.address_contact.lng },
                                {"@is_contact_address", identity.address_contact.is_contact_address },
                                {"@postal_code", identity.address_contact.postal_code },
                                {"@train_line_id", identity.address_contact.train_line_id },
                                {"@station_id", identity.address_contact.station_id }
                            };

                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdAddressContact, addressContactParams);
                        }
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

        public int Clone(IdentityCv identity)
        {
            //Common syntax           
            var sqlCmd = @"Cv_Clone";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@cv_id", identity.id },
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

        public IdentityCv GetById(int id)
        {
            IdentityCv info = null;
           
            var sqlCmd = @"Cv_GetById";

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
                        if(reader.Read())
                        {
                            info = ExtractCvData(reader);
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

        public IdentityCv GetDetail(int id)
        {
            IdentityCv info = null;
            //List<IdentityJobSeekerAddress> addresses = null;
            List<IdentityJobSeekerEduHistory> eduHistories = null;
            List<IdentityJobSeekerWorkHistory> workHistories = null;
            List<IdentityJobSeekerCertificate> certifications = null;

            var sqlCmd = @"Cv_GetDetail";

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
                            info = ExtractCvData(reader);
                        }

                        if (info != null && info.id > 0)
                        {
                            //EduHistories
                            if (reader.NextResult())
                            {
                                eduHistories = new List<IdentityJobSeekerEduHistory>();
                                while (reader.Read())
                                {
                                    var edu = ExtractJobSeekerEduHistoryData(reader);

                                    eduHistories.Add(edu);
                                }

                                info.edu_history = eduHistories;
                            }

                            //WorkHistories
                            if (reader.NextResult())
                            {
                                workHistories = new List<IdentityJobSeekerWorkHistory>();
                                while (reader.Read())
                                {
                                    var work = ExtractJobSeekerWorkHistoryData(reader);
                                    workHistories.Add(work);
                                }

                                info.work_history = workHistories;
                            }

                            //Certificates
                            if (reader.NextResult())
                            {
                                certifications = new List<IdentityJobSeekerCertificate>();
                                while (reader.Read())
                                {
                                    var cer = ExtractJobSeekerCertificationData(reader);
                                    certifications.Add(cer);
                                }

                                info.certification = certifications;
                            }

                            //Addresses
                            //if (reader.NextResult())
                            //{
                            //    addresses = new List<IdentityJobSeekerAddress>();
                            //    while (reader.Read())
                            //    {
                            //        var address = ExtractJobSeekerAddressData(reader);
                            //        addresses.Add(address);
                            //    }

                            //    if (addresses.HasData())
                            //    {
                            //        foreach (var item in addresses)
                            //        {
                            //            if (!item.is_contact_address)
                            //            {
                            //                info.address = item;
                            //            }
                            //            else
                            //            {
                            //                info.address_contact = item;
                            //            }
                            //        }
                            //    }
                            //}
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
            var sqlCmd = @"Cv_Delete";

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

        public bool Close(int id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_Close";

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

        public int Apply(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"Cv_Apply";

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

        public int Save(dynamic info)
        {
            //Common syntax            
            var sqlCmd = @"Cv_Save";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", info.job_id},
                {"@job_seeker_id", info.job_seeker_id},
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

        public List<IdentityCv> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Cv_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityCv> listData = new List<IdentityCv>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCvData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityCvLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityCvLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.JobId = Utils.ConvertToInt32(reader["JobId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.JobId == item.Id).ToList();
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

        //public List<IdentityCv> GetListByJobSeeker(int job_seeker_id)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"Cv_GetListByJobSeeker";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@job_seeker_id", job_seeker_id},
        //    };

        //    List<IdentityCv> listData = new List<IdentityCv>();
        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //            {
        //                while (reader.Read())
        //                {
        //                    var record = ExtractCvData(reader);

        //                    listData.Add(record);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
        //        throw new CustomSQLException(strError);
        //    }

        //    return listData;
        //}


        public List<IdentityCvPdfCode> GetListCodeByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_GetListCodeByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityCvPdfCode> listData = new List<IdentityCvPdfCode>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCvPdfCodeData(reader);

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

        public IdentityCv CheckJobSaved(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_CheckJobSaved";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
            };

            IdentityCv info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCvData(reader);
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

        public bool DeleteJobSaved(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_DeleteJobSaved";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
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

        public int SavePrintCode(IdentityCvPdfCode codeInfo)
        {
            //Common syntax            
            var sqlCmd = @"Cv_SavePrintCode";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@code_id", codeInfo.code_id},
                {"@user_id", codeInfo.user_id},
                {"@cv_id", codeInfo.cv_id}
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

        public int DeletePrintCode(IdentityCvPdfCode codeInfo)
        {
            //Common syntax            
            var sqlCmd = @"Cv_DeletePrintCode";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@code_id", codeInfo.code_id},
                {"@user_id", codeInfo.user_id},
                {"@cv_id", codeInfo.cv_id}
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

        public List<int> SetMainCv(IdentityCv info)
        {
            //Common syntax            
            var sqlCmd = @"Cv_SetMainCv";

            List<int> affectedIds = new List<int>();
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", info.id},
                {"@job_seeker_id", info.job_seeker_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var id = Utils.ConvertToInt32(reader["id"]);

                            affectedIds.Add(id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return affectedIds;
        }

        public List<IdentityCv> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_GetSuggestionsByPage";
            List<IdentityCv> listData = null;

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
                        listData = ParsingListCvFromReader(reader);
                        while (reader.Read())
                        {
                            var record = new IdentityCv();
                            record.id = Utils.ConvertToInt32(reader["id"]);
                            //record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
                            //record.Extensions.is_invited = false;

                            //if (reader.HasColumn("is_invited"))
                            //    record.Extensions.is_invited = false;

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

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

        public List<IdentityCv> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cv_GetSuggestionsForInvitationByPage";
            List<IdentityCv> listData = new List<IdentityCv>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", filter.job_id },
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
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
                            var record = new IdentityCv();
                            record.id = Utils.ConvertToInt32(reader["id"]);
                            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
                            record.Extensions.is_invited = false;

                            if (reader.HasColumn("is_invited"))
                                record.Extensions.is_invited = Utils.ConvertToBoolean(reader["is_invited"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

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

        #region Admin

        public List<IdentityCv> M_GetListByJobSeeker(dynamic filter)
        {
            //Common syntax           
            var sqlCmd = @"M_Cv_GetListByJobSeeker";
            List<IdentityCv> listData = new List<IdentityCv>();
            try
            {
                var currentPage = Utils.ConvertToIntFromQuest(filter.page_index);
                var pageSize = Utils.ConvertToIntFromQuest(filter.page_size);

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

                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityCv();
                            record.id = Utils.ConvertToInt32(reader["id"]);
                            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);

                            record.Extensions.pdf_code_count = Utils.ConvertToInt32(reader["pdf_code_count"]);
                            record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

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
