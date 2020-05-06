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
    public class RpsJob
    {
        private readonly string _connectionString;

        public RpsJob(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsJob()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityJob> GetByPage(IdentityJob filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetAll";
            List<IdentityJob> listData = null;

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
                        listData = ParsingListJobFromReader(reader);
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

        public List<IdentityJob> GetListByCompany(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetListByCompany";
            List<IdentityJob> listData = null;
            //List<IdentityJobTranslation> translations = null;
            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@company_id", filter.company_id },
                {"@agency_id", filter.agency_id },
                {"@sub_field_id", filter.sub_field_id },
                {"@salary_min", filter.salary_min },
                {"@salary_max", filter.salary_max },
                {"@employment_type_id", filter.employment_type_id },
                {"@japanese_level_number", filter.japanese_level_number },
                {"@prefecture_id", filter.prefecture_id },
                {"@staff_id", filter.staff_id },
                {"@status", filter.status},
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@translate_status",filter.translate_status }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = new IdentityJob();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("company_pic_id"))
                                record.company_pic_id = Utils.ConvertToInt32(reader["company_pic_id"]);

                            record.Extensions.application_count = 0;
                            if (reader.HasColumn("application_count"))
                                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                            if (reader.HasColumn("candidate_count"))
                                record.Extensions.candidate_count = Utils.ConvertToInt32(reader["candidate_count"]);

                            if (reader.HasColumn("invited_count"))
                                record.Extensions.invited_count = Utils.ConvertToInt32(reader["invited_count"]);

                            if (reader.HasColumn("introduce_count"))
                                record.Extensions.introduce_count = Utils.ConvertToInt32(reader["introduce_count"]);

                            listData.Add(record);
                        }
                        //if (listData.HasData())
                        //{
                        //    //Translations
                        //    if (reader.NextResult())
                        //    {
                        //        translations = new List<IdentityJobTranslation>();
                        //        while (reader.Read())
                        //        {
                        //            var tran = ExtractJobTranslationData(reader);

                        //            translations.Add(tran);
                        //        }
                        //    }

                        //    if (translations.HasData())
                        //    {
                        //        foreach (var item in listData)
                        //        {
                        //            item.Job_translations = translations.Where(x => x.job_id == item.id).ToList();
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

        public List<IdentityJob> GetListForDelete(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetListForDelete";
            List<IdentityJob> listData = null;
            //List<IdentityJobTranslation> translations = null;
            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", filter.company_id },
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
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = new IdentityJob();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            record.Extensions.application_count = 0;
                            if (reader.HasColumn("application_count"))
                                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                            if (reader.HasColumn("company_pic_id"))
                                record.company_pic_id = Utils.ConvertToInt32(reader["company_pic_id"]);

                            if (reader.HasColumn("candidate_count"))
                                record.Extensions.candidate_count = Utils.ConvertToInt32(reader["candidate_count"]);

                            if (reader.HasColumn("invited_count"))
                                record.Extensions.invited_count = Utils.ConvertToInt32(reader["invited_count"]);

                            if (reader.HasColumn("introduce_count"))
                                record.Extensions.introduce_count = Utils.ConvertToInt32(reader["introduce_count"]);

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

        public List<IdentityJob> GetListProcess(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetListProcess";
            List<IdentityJob> listData = null;
            //List<IdentityJobTranslation> translations = null;
            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@company_id", filter.company_id },
                {"@agency_id", filter.agency_id },
                {"@sub_id", filter.sub_id },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@translate_status",filter.translate_status },
                {"@status",filter.status },
                {"@staff_id",filter.staff_id }

            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = new IdentityJob();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            record.Extensions.application_count = 0;
                            if (reader.HasColumn("application_count"))
                                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                            if (reader.HasColumn("candidate_count"))
                                record.Extensions.candidate_count = Utils.ConvertToInt32(reader["candidate_count"]);

                            if (reader.HasColumn("process_lastest"))
                                record.Extensions.process_lastest = Utils.ConvertToInt32(reader["process_lastest"]);

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


        public List<IdentityJob> GetListAssignWorkByCompanyId(dynamic filter)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetListAssignWorkByCompanyId";
            List<IdentityJob> listData = null;
            int offset = (filter.page_index - 1) * filter.page_size;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", filter.company_id },
                {"@ListStatus", filter.list_status },
                {"@job_seeker_id", filter.job_seeker_id },
                {"@agency_id", filter.agency_id },
                {"@keyword", filter.keyword },
                {"@type_job_seeker", filter.type_job_seeker },
                {"@offset", offset },
                {"@page_size", filter.page_size}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = ExtractJobData(reader);
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
        public List<IdentityJob> GetListSavedByJobSeeker(IdentityJob filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetListSavedByJobSeeker";
            List<IdentityJob> listData = null;
            List<IdentityJobTranslation> translations = null;
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
                        listData = ParsingListJobFromReader(reader);

                        if (listData.HasData())
                        {
                            //Translations
                            if (reader.NextResult())
                            {
                                translations = new List<IdentityJobTranslation>();
                                while (reader.Read())
                                {
                                    var tran = ExtractJobTranslationData(reader);

                                    translations.Add(tran);
                                }
                            }

                            if (translations.HasData())
                            {
                                foreach (var item in listData)
                                {
                                    item.Job_translations = translations.Where(x => x.job_id == item.id).ToList();
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

        public List<IdentityJob> SearchByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_SearchByPage";
            List<IdentityJob> listData = null;
            //List<IdentityJobTranslation> translations = null;
            //List<IdentityCompany> companies = null;
            //List<IdentityJobAddress> addresses = null;
            //List<IdentityJobAddressStation> stations = null;

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

            //Parsing sub industries
            var listSubIndustryIds = string.Empty;
            var sub_industry_ids = new List<int>();
            if (filter.sub_industry_ids != null)
                sub_industry_ids = (List<int>)filter.sub_industry_ids;

            if (sub_industry_ids.HasData())
                listSubIndustryIds = String.Join(",", sub_industry_ids);

            //Parsing sub fields
            var listSubFieldIds = string.Empty;
            var sub_field_ids = new List<int>();
            if (filter.sub_field_ids != null)
                sub_field_ids = (List<int>)filter.sub_field_ids;

            if (sub_field_ids.HasData())
                listSubFieldIds = String.Join(",", sub_field_ids);

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
                {"@sub_industry_ids", listSubIndustryIds },
                {"@sub_field_ids", listSubFieldIds },
                {"@japanese_level_number", filter.japanese_level_number },
                {"@job_seeker_id", filter.job_seeker_id },
                {"@sorting_date", filter.sorting_date },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = new IdentityJob();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("is_saved"))
                                record.Extensions.is_saved = Utils.ConvertToBoolean(reader["is_saved"]);

                            if (reader.HasColumn("application_count"))
                                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                            listData.Add(record);
                        }

                        //if (listData.HasData())
                        //{
                        //    //Translations
                        //    if (reader.NextResult())
                        //    {
                        //        translations = new List<IdentityJobTranslation>();
                        //        while (reader.Read())
                        //        {
                        //            var tran = ExtractJobTranslationData(reader);

                        //            translations.Add(tran);
                        //        }
                        //    }

                        //    //Companies
                        //    if (reader.NextResult())
                        //    {
                        //        companies = new List<IdentityCompany>();
                        //        while (reader.Read())
                        //        {
                        //            var com = RpsCompany.ExtractCompanyData(reader);

                        //            companies.Add(com);
                        //        }
                        //    }

                        //    //Addresses
                        //    if (reader.NextResult())
                        //    {
                        //        addresses = new List<IdentityJobAddress>();
                        //        while (reader.Read())
                        //        {
                        //            var add = RpsJob.ExtractJobAddressData(reader);

                        //            addresses.Add(add);
                        //        }
                        //    }

                        //    //Stations
                        //    if (reader.NextResult())
                        //    {
                        //        stations = new List<IdentityJobAddressStation>();
                        //        while (reader.Read())
                        //        {
                        //            var station = RpsJob.ExtractJobAddressStationData(reader);

                        //            stations.Add(station);
                        //        }
                        //    }

                        //    var hasTrans = translations.HasData();
                        //    var hasCompanies = companies.HasData();
                        //    var hasAddress = addresses.HasData();
                        //    var hasStations = stations.HasData();
                        //    foreach (var item in listData)
                        //    {
                        //        if (hasTrans)
                        //        {
                        //            item.Job_translations = translations.Where(x => x.job_id == item.id).ToList();
                        //        }

                        //        if (hasCompanies)
                        //        {
                        //            item.company_info = companies.Where(x => x.id == item.company_id).FirstOrDefault();
                        //        }

                        //        if (hasAddress)
                        //        {
                        //            item.Addresses = addresses.Where(x => x.job_id == item.id).ToList();
                        //            if (hasStations && item.Addresses.HasData())
                        //            {
                        //                foreach (var add in item.Addresses)
                        //                {
                        //                    add.Stations = stations.Where(x => x.job_address_id == add.id).ToList();
                        //                }
                        //            }
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

        public List<IdentityJob> GetRecent(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Job_GetRecent";
            List<IdentityJob> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ignore_ids", filter.Extensions.ignore_ids },
                {"@company_id", filter.Extensions.company_id },
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
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = new IdentityJob();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("is_saved"))
                                record.Extensions.is_saved = Utils.ConvertToBoolean(reader["is_saved"]);

                            if (reader.HasColumn("application_count"))
                                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

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

        private List<IdentityJob> ParsingListJobFromReader(IDataReader reader)
        {
            List<IdentityJob> listData = listData = new List<IdentityJob>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractJobData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                if (reader.HasColumn("is_saved"))
                    record.Extensions.is_saved = Utils.ConvertToBoolean(reader["is_saved"]);

                if (reader.HasColumn("application_count"))
                    record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityJob ExtractJobData(IDataReader reader, bool isFullInfo = true)
        {
            var record = new IdentityJob();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            if (reader.HasColumn("total_count"))
            {
                record.total_count = Utils.ConvertToInt32(reader["total_count"]);
            }
            if (reader.HasColumn("application_count"))
            {
                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);
            }
            record.view_count = Utils.ConvertToInt32(reader["view_count"]);

            if (isFullInfo)
            {
                record.company_id = Utils.ConvertToInt32(reader["company_id"]);
                record.quantity = Utils.ConvertToInt32(reader["quantity"]);
                record.age_min = Utils.ConvertToInt32(reader["age_min"]);
                record.age_max = Utils.ConvertToInt32(reader["age_max"]);
                record.salary_min = Utils.ConvertToInt32(reader["salary_min"]);
                record.salary_max = Utils.ConvertToInt32(reader["salary_max"]);
                record.salary_type_id = Utils.ConvertToInt32(reader["salary_type_id"]);
                record.work_start_time = reader["work_start_time"] == DBNull.Value ? null : (TimeSpan?)reader["work_start_time"];
                record.work_end_time = reader["work_end_time"] == DBNull.Value ? null : (TimeSpan?)reader["work_end_time"];
                record.probation_duration = Utils.ConvertToInt32(reader["probation_duration"]);
                record.status = Utils.ConvertToInt32(reader["status"]);
                record.closed_time = reader["closed_time"] == DBNull.Value ? null : (DateTime?)reader["closed_time"];
                record.employment_type_id = Utils.ConvertToInt32(reader["employment_type_id"]);
                record.flexible_time = Utils.ConvertToBoolean(reader["flexible_time"]);
                record.language_level = reader["language_level"].ToString();
                record.work_experience_doc_required = Utils.ConvertToBoolean(reader["work_experience_doc_required"]);
                record.duration = Utils.ConvertToInt32(reader["duration"]);
                record.view_company = Utils.ConvertToBoolean(reader["view_company"]);
                record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
                record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
                record.station_id = Utils.ConvertToInt32(reader["station_id"]);
                record.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);
                record.sub_field_id = Utils.ConvertToInt32(reader["sub_field_id"]);
                record.sub_industry_id = Utils.ConvertToInt32(reader["sub_industry_id"]);
                record.translate_status = Utils.ConvertToInt32(reader["translate_status"]);
                record.job_code =reader["job_code"].ToString();
                record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);
                record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
                record.japanese_only = Utils.ConvertToBoolean(reader["japanese_only"]);
                record.language_code = reader["language_code"].ToString();
            }

            return record;
        }

        public static IdentityJobAddress ExtractJobAddressData(IDataReader reader)
        {
            var record = new IdentityJobAddress();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.country_id = Utils.ConvertToInt32(reader["country_id"]);
            record.region_id = Utils.ConvertToInt32(reader["region_id"]);
            record.prefecture_id = Utils.ConvertToInt32(reader["prefecture_id"]);
            record.city_id = Utils.ConvertToInt32(reader["city_id"]);
            record.furigana = reader["furigana"].ToString();
            record.detail = reader["detail"].ToString();
            record.note = reader["note"].ToString();
            record.lat = reader["lat"].ToString();
            record.lng = reader["lng"].ToString();
            record.train_line_id = Utils.ConvertToInt32(reader["train_line_id"].ToString());
            record.train_line = reader["train_line"].ToString();
            if (reader.HasColumn("address"))
                record.address = reader["address"].ToString();
            return record;
        }

        public static IdentityJobTranslation ExtractJobTranslationData(IDataReader reader)
        {
            var record = new IdentityJobTranslation();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.title = reader["title"].ToString();
            record.subsidy = reader["subsidy"].ToString();
            record.paid_holiday = reader["paid_holiday"].ToString();
            record.bonus = reader["bonus"].ToString();
            record.certificate = reader["certificate"].ToString();
            record.work_content = reader["work_content"].ToString();
            record.requirement = reader["requirement"].ToString();
            record.plus = reader["plus"].ToString();
            record.welfare = reader["welfare"].ToString();
            record.training = reader["training"].ToString();
            record.recruitment_procedure = reader["recruitment_procedure"].ToString();
            record.remark = reader["remark"].ToString();
            record.language_code = reader["language_code"].ToString();
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.friendly_url = reader["friendly_url"].ToString();
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];


            return record;
        }

        public static IdentityJobAddressStation ExtractJobAddressStationData(IDataReader reader)
        {
            var record = new IdentityJobAddressStation();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_address_id = Utils.ConvertToInt32(reader["job_address_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.station_id = Utils.ConvertToInt32(reader["station_id"]);

            return record;
        }

        public static IdentityJobSubField ExtractJobSubFieldData(IDataReader reader)
        {
            var record = new IdentityJobSubField();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.sub_field_id = Utils.ConvertToInt32(reader["sub_field_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);

            return record;
        }

        public static IdentityJobTag ExtractJobTagData(IDataReader reader)
        {
            var record = new IdentityJobTag();

            //Seperate properties            
            record.tag_id = Utils.ConvertToInt32(reader["id"]);
            record.tag = reader["tag"].ToString();

            return record;
        }

        public int Insert(IdentityJob identity)
        {
            //Common syntax           
            var sqlCmd = @"Job_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", identity.company_id },
                {"@quantity", identity.quantity },
                {"@age_min", identity.age_min },
                {"@age_max", identity.age_max },
                {"@salary_min", identity.salary_min },
                {"@salary_max", identity.salary_max },
                {"@salary_type_id", identity.salary_type_id },
                {"@work_start_time", identity.work_start_time },
                {"@work_end_time", identity.work_end_time },
                {"@probation_duration", identity.probation_duration },
                {"@employment_type_id", identity.employment_type_id },
                {"@flexible_time", identity.flexible_time },
                {"@language_level", identity.language_level },
                {"@work_experience_doc_required", identity.work_experience_doc_required },
                {"@closed_time", identity.closed_time },
                {"@view_company", identity.view_company },
                {"@qualification_id", identity.qualification_id },
                {"@japanese_level_number", identity.japanese_level_number },
                {"@sub_field_id", identity.sub_field_id },
                {"@sub_industry_id", identity.sub_industry_id },
                {"@staff_id", identity.staff_id },
                {"@pic_id", identity.pic_id },
                {"@status", identity.status },
                {"@japanese_only", identity.japanese_only },
                {"@language_code", identity.language_code }
            };

            StringBuilder addressInsertCmd = new StringBuilder();
            StringBuilder stationInsertCmd = new StringBuilder();
            StringBuilder translationInsertCmd = new StringBuilder();
            //StringBuilder subFieldInsertCmd = new StringBuilder();
            StringBuilder tagInsertCmd = new StringBuilder();
            StringBuilder newTagInsertCmd = new StringBuilder();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    newId = Convert.ToInt32(returnObj);

                    if (newId > 0)
                    {
                        if (identity.Addresses.HasData())
                        {
                            foreach (var item in identity.Addresses)
                            {
                                var cmdAddress = string.Format("INSERT INTO job_address(job_id,country_id,region_id,prefecture_id,city_id,detail,furigana,note,lat,lng,train_line_id) VALUES({0},{1},{2},{3},{4},N'{5}',N'{6}',N'{7}','{8}','{9}','{10}'); SELECT SCOPE_IDENTITY();"
                           , newId, item.country_id, item.region_id, item.prefecture_id, item.city_id, item.detail, item.furigana, item.note, item.lat, item.lng, item.train_line_id);

                                var newJobAddressIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, cmdAddress, null);
                                var newJobAddressId = Utils.ConvertToInt32(newJobAddressIdObj);

                                if (item.Stations.HasData())
                                {
                                    foreach (var st in item.Stations)
                                    {
                                        var cmdStation = string.Format("INSERT INTO job_address_station(job_id,station_id,job_address_id) VALUES({0},{1},{2});"
                                        , newId, st.id, newJobAddressId);

                                        stationInsertCmd.Append(cmdStation);
                                    }
                                }

                                //addressInsertCmd.Append(cmdAddress);                                
                            }
                        }

                        if (identity.Job_translations.HasData())
                        {
                            foreach (var item in identity.Job_translations)
                            {
                                var cmdTranslation = string.Format("INSERT INTO job_translation(title,subsidy,paid_holiday,bonus,certificate,work_content,requirement,plus,welfare,training,recruitment_procedure,remark,language_code,job_id,staff_id) VALUES(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',N'{5}',N'{6}',N'{7}',N'{8}',N'{9}',N'{10}',N'{11}',N'{12}',{13},{14}); "
                               , item.title, item.subsidy, item.paid_holiday, item.bonus, item.certificate, item.work_content, item.requirement, item.plus, item.welfare, item.training, item.recruitment_procedure, item.remark, identity.language_code, newId,identity.staff_id);

                                translationInsertCmd.Append(cmdTranslation);
                            }
                        }

                        //if (identity.Sub_fields.HasData())
                        //{
                        //    foreach (var item in identity.Sub_fields)
                        //    {
                        //        var cmdSubField = string.Format("INSERT INTO job_sub_field(job_id,sub_field_id) VALUES({0},{1}); "
                        //       , newId, item.id);

                        //        subFieldInsertCmd.Append(cmdSubField);
                        //    }
                        //}

                        if (identity.Tags.HasData())
                        {
                            foreach (var item in identity.Tags)
                            {
                                var cmdTag = string.Empty;

                                if (item.id > 0)
                                {
                                    cmdTag = string.Format("INSERT INTO job_tag(job_id, tag_id) VALUES({0},{1}); "
                                    , newId, item.id);
                                }
                                else
                                {
                                    var tagParms = new Dictionary<string, object>
                                    {
                                        {"@tag", item.tag}
                                    };

                                    var newTagIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "Tag_Insert", tagParms);
                                    var newTagId = Utils.ConvertToInt32(newTagIdObj);

                                    cmdTag = string.Format("INSERT INTO job_tag(job_id, tag_id) VALUES({0},{1});"
                                   , newId, newTagId);
                                }

                                tagInsertCmd.Append(cmdTag);
                            }
                        }

                        //Begin executing
                        //if (addressInsertCmd.Length > 0)
                        //    //Insert addresses
                        //    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, addressInsertCmd.ToString(), null);

                        if (stationInsertCmd.Length > 0)
                            //Insert job address station
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, stationInsertCmd.ToString(), null);

                        if (translationInsertCmd.Length > 0)
                            //Insert translations
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, translationInsertCmd.ToString(), null);

                        //if (subFieldInsertCmd.Length > 0)
                        //    //Insert sub fields
                        //    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, subFieldInsertCmd.ToString(), null);

                        if (tagInsertCmd.Length > 0)
                            //Insert tags
                            MsSqlHelper.ExecuteScalar(conn, CommandType.Text, tagInsertCmd.ToString(), null);
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
        public bool UpdateTranslation(IdentityJobTranslation identity)
        {
            //Common syntax           
            var sqlCmd = @"Job_Translation_Insert";
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@title", identity.title },
                {"@subsidy", identity.subsidy },
                {"@paid_holiday", identity.paid_holiday },
                {"@bonus", identity.bonus },
                {"@certificate", identity.certificate },
                {"@work_content", identity.work_content },
                {"@requirement", identity.requirement },
                {"@plus", identity.plus },
                {"@welfare", identity.welfare },
                {"@training", identity.training },
                {"@recruitment_procedure", identity.recruitment_procedure },
                {"@remark", identity.remark },
                {"@language_code", identity.language_code },
                {"@job_id", identity.job_id },
                {"@translate_status", identity.translate_status },
                {"@staff_id", identity.staff_id },
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentityJobSeekerCounter GetCounterForDeletion(int id)
        {
            IdentityJobSeekerCounter info = new IdentityJobSeekerCounter();

            var sqlCmd = @"Job_GetCounterForDeletetion";

            var parameters = new Dictionary<string, object>
            {
                {"@job_id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = new IdentityJobSeekerCounter();
                            info.Interviewing = Utils.ConvertToInt32(reader["Interviewing"]);
                            info.Recruitmented = Utils.ConvertToInt32(reader["Recruitmented"]);
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
        public int Update(IdentityJob identity)
        {
            //Common syntax           
            var sqlCmd = @"Job_Update";
            var newId = identity.id;
            var status_return = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@company_id", identity.company_id },
                {"@quantity", identity.quantity },
                {"@age_min", identity.age_min },
                {"@age_max", identity.age_max },
                {"@salary_min", identity.salary_min },
                {"@salary_max", identity.salary_max },
                {"@salary_type_id", identity.salary_type_id },
                {"@work_start_time", identity.work_start_time },
                {"@work_end_time", identity.work_end_time },
                {"@probation_duration", identity.probation_duration },
                {"@employment_type_id", identity.employment_type_id },
                {"@flexible_time", identity.flexible_time },
                {"@language_level", identity.language_level },
                {"@work_experience_doc_required", identity.work_experience_doc_required },
                {"@closed_time", identity.closed_time },
                {"@view_company", identity.view_company },
                {"@qualification_id", identity.qualification_id },
                {"@japanese_level_number", identity.japanese_level_number },
                {"@sub_field_id", identity.sub_field_id },
                {"@sub_industry_id", identity.sub_industry_id },
                {"@staff_id", identity.staff_id },
                {"@pic_id", identity.pic_id },
                {"@status", identity.status },
                {"@japanese_only", identity.japanese_only },
                {"@language_code", identity.language_code }
            };

            StringBuilder addressInsertCmd = new StringBuilder();
            StringBuilder stationInsertCmd = new StringBuilder();
            StringBuilder translationInsertCmd = new StringBuilder();
            StringBuilder subFieldInsertCmd = new StringBuilder();
            StringBuilder tagInsertCmd = new StringBuilder();
            StringBuilder newTagInsertCmd = new StringBuilder();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    status_return = Utils.ConvertToInt32(returnObj);
                    if(status_return != 3)
                    {
                        return status_return;
                    }
                    var jobClearCmd = @"Job_ClearBeforeUpdate";
                    var clearParms = new Dictionary<string, object>
                    {
                        {"@job_id", identity.id },
                        {"@language_code", identity.language_code }
                    };

                    MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, jobClearCmd, clearParms);

                    if (identity.Addresses.HasData())
                    {


                        foreach (var item in identity.Addresses)
                        {
                            var cmdAddress = string.Format("INSERT INTO job_address(job_id,country_id,region_id,prefecture_id,city_id,detail,furigana,note,lat,lng,train_line_id) VALUES({0},{1},{2},{3},{4},N'{5}',N'{6}',N'{7}','{8}','{9}','{10}'); SELECT SCOPE_IDENTITY();"
                           , newId, item.country_id, item.region_id, item.prefecture_id, item.city_id, item.detail, item.furigana, item.note, item.lat, item.lng, item.train_line_id);

                            var newJobAddressIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, cmdAddress, null);
                            var newJobAddressId = Utils.ConvertToInt32(newJobAddressIdObj);

                            if (item.Stations.HasData())
                            {
                                foreach (var st in item.Stations)
                                {
                                    var cmdStation = string.Format("INSERT INTO job_address_station(job_id,station_id,job_address_id) VALUES({0},{1},{2});"
                                    , newId, st.id, newJobAddressId);

                                    stationInsertCmd.Append(cmdStation);
                                }
                            }

                            //addressInsertCmd.Append(cmdAddress);                                
                        }
                    }

                    if (identity.Job_translations.HasData())
                    {
                        foreach (var item in identity.Job_translations)
                        {
                            var job_translations_Insert = @"Job_Translation_Insert";
                            var translartionParms = new Dictionary<string, object>
                            {
                                {"@title", item.title },
                                {"@subsidy", item.subsidy },
                                {"@paid_holiday", item.paid_holiday },
                                {"@bonus", item.bonus },
                                {"@certificate", item.certificate },
                                {"@work_content", item.work_content },
                                {"@requirement", item.requirement },
                                {"@plus", item.plus },
                                {"@welfare", item.welfare },
                                {"@training", item.training },
                                {"@recruitment_procedure", item.recruitment_procedure },
                                {"@remark", item.remark },
                                {"@language_code", identity.language_code },
                                {"@job_id", newId },
                                {"@translate_status", -99 },
                                {"@staff_id", identity.staff_id },
                            };

                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, job_translations_Insert, translartionParms);
                            // var cmdTranslation = string.Format("INSERT INTO job_translation(title,subsidy,paid_holiday,bonus,certificate,work_content,requirement,plus,welfare,training,recruitment_procedure,remark,language_code,job_id) VALUES(N'{0}',N'{1}',N'{2}',N'{3}',N'{4}',N'{5}',N'{6}',N'{7}',N'{8}',N'{9}',N'{10}',N'{11}',N'{12}',{13}); "
                            //, item.title, item.subsidy, item.paid_holiday, item.bonus, item.certificate, item.work_content, item.requirement, item.plus, item.welfare, item.training, item.recruitment_procedure, item.remark, item.language_code, newId);

                            // translationInsertCmd.Append(cmdTranslation);
                        }
                    }

                    //if (identity.Sub_fields.HasData())
                    //{
                    //    foreach (var item in identity.Sub_fields)
                    //    {
                    //        var cmdSubField = string.Format("INSERT INTO job_sub_field(job_id,sub_field_id) VALUES({0},{1}); "
                    //       , newId, item.id);

                    //        subFieldInsertCmd.Append(cmdSubField);
                    //    }
                    //}

                    if (identity.Tags.HasData())
                    {
                        foreach (var item in identity.Tags)
                        {
                            var cmdTag = string.Empty;

                            if (item.id > 0)
                            {
                                cmdTag = string.Format("INSERT INTO job_tag(job_id, tag_id) VALUES({0},{1}); "
                                , newId, item.id);
                            }
                            else
                            {
                                var tagParms = new Dictionary<string, object>
                                    {
                                        {"@tag", item.tag}
                                    };

                                var newTagIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, "Tag_Insert", tagParms);
                                var newTagId = Utils.ConvertToInt32(newTagIdObj);

                                cmdTag = string.Format("INSERT INTO job_tag(job_id, tag_id) VALUES({0},{1});"
                               , newId, newTagId);
                            }

                            tagInsertCmd.Append(cmdTag);
                        }
                    }

                    //Begin executing
                    //if (addressInsertCmd.Length > 0)
                    //    //Insert addresses
                    //    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, addressInsertCmd.ToString(), null);

                    if (stationInsertCmd.Length > 0)
                        //Insert job address station
                        MsSqlHelper.ExecuteScalar(conn, CommandType.Text, stationInsertCmd.ToString(), null);

                    //if (translationInsertCmd.Length > 0)
                    //    //Insert translations
                    //    MsSqlHelper.ExecuteScalar(conn, CommandType.Text, translationInsertCmd.ToString(), null);

                    if (subFieldInsertCmd.Length > 0)
                        //Insert sub fields
                        MsSqlHelper.ExecuteScalar(conn, CommandType.Text, subFieldInsertCmd.ToString(), null);

                    if (tagInsertCmd.Length > 0)
                        //Insert tags
                        MsSqlHelper.ExecuteScalar(conn, CommandType.Text, tagInsertCmd.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status_return;
        }

        public IdentityJob GetById(int id)
        {
            IdentityJob info = null;

            var sqlCmd = @"Job_GetById";

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
                            info = ExtractJobData(reader);
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

        public IdentityJob GetDetail(int id)
        {
            IdentityJob info = null;
            List<IdentityJobAddress> addresses = null;
            List<IdentityJobTranslation> translations = null;
            List<IdentityJobAddressStation> stations = null;
            List<IdentityJobTag> tags = null;
            var sqlCmd = @"Job_GetDetail";

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
                            info = ExtractJobData(reader);
                        }

                        if (info != null && info.id > 0)
                        {
                            //Translations
                            if (reader.NextResult())
                            {
                                translations = new List<IdentityJobTranslation>();
                                while (reader.Read())
                                {
                                    var tran = ExtractJobTranslationData(reader);

                                    translations.Add(tran);
                                }

                                info.Job_translations = translations;
                            }

                            //Addresses
                            if (reader.NextResult())
                            {
                                addresses = new List<IdentityJobAddress>();
                                while (reader.Read())
                                {
                                    var address = ExtractJobAddressData(reader);
                                    addresses.Add(address);
                                }
                            }

                            //Stations
                            if (reader.NextResult())
                            {
                                stations = new List<IdentityJobAddressStation>();
                                while (reader.Read())
                                {
                                    var station = ExtractJobAddressStationData(reader);
                                    stations.Add(station);
                                }

                                if (addresses.HasData() && stations.HasData())
                                {
                                    foreach (var item in addresses)
                                    {
                                        item.Stations = stations.Where(x => x.job_address_id == item.id).ToList();
                                    }
                                }
                            }

                            info.Addresses = addresses;

                            //SubFields
                            //if (reader.NextResult())
                            //{
                            //    sub_fields = new List<IdentityJobSubField>();
                            //    while (reader.Read())
                            //    {
                            //        var subField = ExtractJobSubFieldData(reader);
                            //        sub_fields.Add(subField);
                            //    }

                            //    info.Sub_fields = sub_fields;
                            //}

                            //Tags
                            if (reader.NextResult())
                            {
                                tags = new List<IdentityJobTag>();
                                while (reader.Read())
                                {
                                    var tag = ExtractJobTagData(reader);
                                    tags.Add(tag);
                                }

                                info.Tags = tags;
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

        public int CheckInvite(dynamic model)
        {
            var sqlCmd = @"Job_CheckInvite";
            var result = 0;
            var parameters = new Dictionary<string, object>
            {
                {"@id", model.id},
                {"@job_seeker_id", model.job_seeker_id}
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

        public IdentityJob GetMetaData(dynamic filter)
        {
            IdentityJob info = null;
            var sqlCmd = @"Job_GetMetaData";

            var parameters = new Dictionary<string, object>
            {
                {"@id", filter.id},
                {"@job_seeker_id", filter.job_seeker_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobData(reader, false);


                            if (reader.HasColumn("is_saved"))
                                info.Extensions.is_saved = Utils.ConvertToBoolean(reader["is_saved"]);

                            if (reader.HasColumn("application_count"))
                                info.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

                            if (reader.HasColumn("is_applied"))
                                info.Extensions.is_applied = Utils.ConvertToBoolean(reader["is_applied"]);
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

        public IdentityJob GetBaseInfo(int id, string language_code)
        {
            IdentityJob info = null;
            List<IdentityJobTranslation> translations = null;
            //List<IdentityCompany> companies = null;
            List<IdentityJobAddress> addresses = null;
            List<IdentityJobAddressStation> stations = null;
            //List<IdentityJobSubField> subFields = null;
            List<IdentityJobTag> tags = null;
            var sqlCmd = @"Job_GetBaseInfo";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@language_code", language_code }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobData(reader);
                        }

                        if (info != null && info.id > 0)
                        {
                            //Translations
                            if (reader.NextResult())
                            {
                                translations = new List<IdentityJobTranslation>();
                                while (reader.Read())
                                {
                                    var tran = ExtractJobTranslationData(reader);

                                    translations.Add(tran);
                                }
                            }

                            //Companies
                            //if (reader.NextResult())
                            //{
                            //    companies = new List<IdentityCompany>();
                            //    while (reader.Read())
                            //    {
                            //        var com = RpsCompany.ExtractCompanyData(reader);

                            //        companies.Add(com);
                            //    }
                            //}

                            //Addresses
                            if (reader.NextResult())
                            {
                                addresses = new List<IdentityJobAddress>();
                                while (reader.Read())
                                {
                                    var add = RpsJob.ExtractJobAddressData(reader);

                                    addresses.Add(add);
                                }
                            }

                            //Stations
                            if (reader.NextResult())
                            {
                                stations = new List<IdentityJobAddressStation>();
                                while (reader.Read())
                                {
                                    var station = RpsJob.ExtractJobAddressStationData(reader);

                                    stations.Add(station);
                                }
                            }

                            ////SubFields
                            //if (reader.NextResult())
                            //{
                            //    subFields = new List<IdentityJobSubField>();
                            //    while (reader.Read())
                            //    {
                            //        var subField = RpsSubField.ExtractJobSubFieldData(reader);

                            //        subFields.Add(subField);
                            //    }

                            //    info.Sub_fields = subFields;
                            //}

                            //Tags
                            if (reader.NextResult())
                            {
                                tags = new List<IdentityJobTag>();
                                while (reader.Read())
                                {
                                    var tag = RpsTag.ExtractJobTagData(reader);

                                    tags.Add(tag);
                                }

                                info.Tags = tags;
                            }

                            var hasTrans = translations.HasData();
                            //var hasCompanies = companies.HasData();
                            var hasAddress = addresses.HasData();
                            var hasStations = stations.HasData();
                            var hasTags = tags.HasData();
                            if (hasTrans)
                            {
                                info.Job_translations = translations.Where(x => x.job_id == info.id).ToList();
                            }

                            //if (hasCompanies)
                            //{
                            //    info.company_info = companies.Where(x => x.id == info.company_id).FirstOrDefault();
                            //}

                            if (hasAddress)
                            {
                                info.Addresses = addresses.Where(x => x.job_id == info.id).ToList();
                                if (hasStations && info.Addresses.HasData())
                                {
                                    foreach (var add in info.Addresses)
                                    {
                                        add.Stations = stations.Where(x => x.job_address_id == add.id).ToList();
                                    }
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
            return info;
        }

        public bool Delete(int id,int agency_id)
        {
            //Common syntax            
            var sqlCmd = @"Job_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@agency_id", agency_id}
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
            var sqlCmd = @"Job_Close";

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

        public bool UpdateStatus(int id, int status)
        {
            //Common syntax            
            var sqlCmd = @"Job_UpdateStatus";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@status", status}
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
            var sqlCmd = @"Job_Apply";

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
            var sqlCmd = @"Job_Save";

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

        public List<IdentityJob> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Job_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentityJob> listData = new List<IdentityJob>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityJobLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityJobLang();
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
        public List<IdentityApplication> GetListHot(int job_seeker_id, int page_size)
        {
            //Common syntax            
            var sqlCmd = @"Job_GetListHot";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
                {"@page_size", page_size}
            };

            List<IdentityApplication> listData = new List<IdentityApplication>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityApplication>();
                        while (reader.Read())
                        {
                            var record = new IdentityApplication();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("is_saved"))
                                record.is_saved = Utils.ConvertToBoolean(reader["is_saved"]);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityJobLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityJobLang();
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
        public List<IdentityJob> GetListByCompany(int company_id)
        {
            //Common syntax            
            var sqlCmd = @"Job_GetListByCompany";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", company_id},
            };

            List<IdentityJob> listData = new List<IdentityJob>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityJobLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityJobLang();
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

        public List<IdentityJob> GetListSavedByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Job_GetListSavedByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityJob> listData = new List<IdentityJob>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobData(reader);

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

        public IdentityJob CheckJobSaved(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Job_CheckJobSaved";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
            };

            IdentityJob info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobData(reader);
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
            var sqlCmd = @"Job_DeleteJobSaved";

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

        #endregion

        #region Admin

        public List<IdentityJob> M_GetListByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_Job_GetListByPage";
            List<IdentityJob> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@company_id", filter.company_id },
                {"@agency_id", filter.agency_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@translate_status", filter.translate_status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityJob>();
                        while (reader.Read())
                        {
                            var record = new IdentityJob();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            record.Extensions.application_count = 0;
                            if (reader.HasColumn("application_count"))
                                record.Extensions.application_count = Utils.ConvertToInt32(reader["application_count"]);

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
