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
    public class RpsJobSeeker
    {
        private readonly string _connectionString;

        public RpsJobSeeker(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsJobSeeker()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityJobSeeker> GetByPage(IdentityJobSeeker filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_GetAll";
            List<IdentityJobSeeker> listData = null;

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
                        listData = ParsingListJobSeekerFromReader(reader);
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

        public List<IdentityJobSeeker> GetByAgency(IdentityJobSeeker filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_GetListByAgency";
            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@staff_id", filter.staff_id },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@type_job_seeker", filter.type_job_seeker},
                {"@status",filter.status },
                {"@japanese_level",filter.japanese_level }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var jsInfo = new IdentityJobSeeker();
                            if (listData.Count == 0)
                            {
                                jsInfo.total_count = Utils.ConvertToInt32(reader["total_count"]);
                            }
                            jsInfo.fullname = reader["fullname"].ToString();
                            jsInfo.email = reader["email"].ToString();
                            jsInfo.phone = reader["phone"].ToString();
                            jsInfo.code = reader["code"].ToString();
                            jsInfo.id = Utils.ConvertToInt32(reader["id"]);
                            jsInfo.image = reader["image"].ToString();
                            jsInfo.visa_id = Utils.ConvertToInt32(reader["visa_id"]);
                            jsInfo.company_count = Utils.ConvertToInt32(reader["company_count"]);
                            jsInfo.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);
                            jsInfo.process_lastest = Utils.ConvertToInt32(reader["process_lastest"]);
                            listData.Add(jsInfo);
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

        private List<IdentityJobSeeker> ParsingListJobSeekerFromReader(IDataReader reader)
        {
            List<IdentityJobSeeker> listData = listData = new List<IdentityJobSeeker>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractJobSeekerData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null && reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityJobSeeker ExtractJobSeekerData(IDataReader reader)
        {
            var record = new IdentityJobSeeker();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.user_id = Utils.ConvertToInt32(reader["user_id"]);
            record.email = reader["email"].ToString();
            record.phone = reader["phone"].ToString();
            record.marriage = Utils.ConvertToInt32(reader["marriage"]);
            record.dependent_num = Utils.ConvertToInt32(reader["dependent_num"]);
            record.fullname = reader["fullname"].ToString();
            record.fullname_furigana = reader["fullname_furigana"].ToString();
            record.display_name = reader["display_name"].ToString();
            record.image = reader["image"].ToString();
            record.birthday = reader["birthday"] == DBNull.Value ? null : (DateTime?)reader["birthday"];
            record.gender = Utils.ConvertToInt32(reader["gender"]);
            record.id_card = reader["id_card"].ToString();
            record.note = reader["note"].ToString();
            record.video_path = reader["video_path"].ToString();
            record.expected_job_title = reader["expected_job_title"].ToString();
            record.expected_salary_min = Utils.ConvertToInt32(reader["expected_salary_min"]);
            record.expected_salary_max = Utils.ConvertToInt32(reader["expected_salary_max"]);
            record.work_status = Utils.ConvertToInt32(reader["work_status"]);
            record.google_id = reader["google_id"].ToString();
            record.facebook_id = reader["facebook_id"].ToString();
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
            record.job_seeking_status_id = Utils.ConvertToInt32(reader["job_seeking_status_id"]);
            record.salary_type_id = Utils.ConvertToInt32(reader["salary_type_id"]);
            record.view_count = Utils.ConvertToInt32(reader["view_count"]);
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.japanese_level_number = Utils.ConvertToInt32(reader["japanese_level_number"]);
            record.nationality_id = Utils.ConvertToInt32(reader["nationality_id"]);
            record.visa_id = Utils.ConvertToInt32(reader["visa_id"]);
            record.religion = Utils.ConvertToBoolean(reader["religion"]);
            record.religion_detail = reader["religion_detail"].ToString();
            record.duration_visa = reader["duration_visa"] == DBNull.Value ? null : (DateTime?)reader["duration_visa"];

            if (reader.HasColumn("pic_id"))
                record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);

            if (reader.HasColumn("code"))
                record.code = reader["code"].ToString();

            if (reader.HasColumn("metadata"))
                record.metadata = reader["metadata"].ToString();

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
            record.postal_code = reader["postal_code"].ToString();
            record.is_contact_address = Utils.ConvertToBoolean(reader["is_contact_address"]);
            record.train_line_id = Utils.ConvertToInt32(reader["train_line_id"]);
            record.station_id = Utils.ConvertToInt32(reader["station_id"]);
            if (reader.HasColumn("train_line"))
                record.train_line = reader["train_line"].ToString();

            if (reader.HasColumn("train_line_furigana"))
                record.train_line_furigana = reader["train_line_furigana"].ToString();

            if (reader.HasColumn("address"))
                record.address = reader["address"].ToString();

            if (reader.HasColumn("address_furigana"))
                record.address_furigana = reader["address_furigana"].ToString();
            return record;
        }

        public static IdentityJobSeekerConfig ExtractJobSeekerConfigData(IDataReader reader)
        {
            var record = new IdentityJobSeekerConfig();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.working_status = Utils.ConvertToInt32(reader["working_status"]);
            record.working_detail = reader["working_detail"].ToString();
            record.salary_min = Utils.ConvertToInt32(reader["salary_min"]);
            record.salary_max = Utils.ConvertToInt32(reader["salary_max"]);
            record.salary_type = Utils.ConvertToInt32(reader["salary_type"]);
            record.field_ids = reader["field_ids"].ToString();
            record.country_id = Utils.ConvertToInt32(reader["country_id"]);
            record.region_id = Utils.ConvertToInt32(reader["region_id"]);
            record.prefecture_id = Utils.ConvertToInt32(reader["prefecture_id"]);
            record.city_id = Utils.ConvertToInt32(reader["city_id"]);

            return record;
        }

        public int Insert(IdentityJobSeeker identity)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@user_id", identity.user_id },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@marriage", identity.marriage },
                {"@dependent_num", identity.dependent_num },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@display_name", identity.display_name },
                {"@image", identity.image },
                {"@birthday", identity.birthday },
                {"@gender", identity.gender },
                {"@id_card", identity.id_card },
                {"@note", identity.note },
                {"@video_path", identity.video_path },
                {"@expected_job_title", identity.expected_job_title },
                {"@expected_salary_min", identity.expected_salary_min },
                {"@expected_salary_max", identity.expected_salary_max },
                {"@work_status", identity.work_status },
                {"@google_id", identity.google_id },
                {"@facebook_id", identity.facebook_id },
                {"@created_at", identity.created_at },
                {"@updated_at", identity.updated_at },
                {"@job_seeking_status_id", identity.job_seeking_status_id },
                {"@salary_type_id", identity.salary_type_id },
                {"@view_count", identity.view_count }
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

        public bool Update(IdentityJobSeeker identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@user_id", identity.user_id },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@marriage", identity.marriage },
                {"@dependent_num", identity.dependent_num },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@display_name", identity.display_name },
                {"@image", identity.image },
                {"@birthday", identity.birthday },
                {"@gender", identity.gender },
                {"@id_card", identity.id_card },
                {"@note", identity.note },
                {"@video_path", identity.video_path },
                {"@expected_job_title", identity.expected_job_title },
                {"@expected_salary_min", identity.expected_salary_min },
                {"@expected_salary_max", identity.expected_salary_max },
                {"@work_status", identity.work_status },
                {"@google_id", identity.google_id },
                {"@facebook_id", identity.facebook_id },
                {"@created_at", identity.created_at },
                {"@updated_at", identity.updated_at },
                {"@job_seeking_status_id", identity.job_seeking_status_id },
                {"@salary_type_id", identity.salary_type_id },
                {"@view_count", identity.view_count },
                {"@japanese_level_number", identity.japanese_level_number }
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

        public IdentityJobSeekerConfig GetConfig(int job_seeker_id)
        {
            IdentityJobSeekerConfig info = null;
            var sqlCmd = @"JobSeeker_GetConfig";

            var parameters = new Dictionary<string, object>
            {
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
                            info = ExtractJobSeekerConfigData(reader);
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

        public bool UpdateConfig(IdentityJobSeekerConfig identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateConfig";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", identity.job_seeker_id},
                {"@working_status", identity.working_status},
                {"@working_detail", identity.working_detail},
                {"@salary_min", identity.salary_min},
                {"@salary_max", identity.salary_max},
                {"@salary_type", identity.salary_type},
                {"@field_ids", identity.field_ids},
                {"@country_id", identity.country_id},
                {"@region_id", identity.region_id},
                {"@prefecture_id", identity.prefecture_id},
                {"@city_id", identity.city_id}
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

        public bool UpdateVideoProfile(IdentityJobSeeker identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateVideoProfile";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", identity.user_id},
                {"@video_path", identity.video_path }
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

        public bool UpdateProfile(IdentityJobSeeker identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateProfile";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@user_id", identity.user_id },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@marriage", identity.marriage },
                {"@dependent_num", identity.dependent_num },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@display_name", identity.display_name },
                {"@image", identity.image },
                {"@birthday", identity.birthday },
                {"@gender", identity.gender },
                {"@id_card", identity.id_card },
                {"@note", identity.note },
                {"@video_path", identity.video_path },
                {"@expected_job_title", identity.expected_job_title },
                {"@expected_salary_min", identity.expected_salary_min },
                {"@expected_salary_max", identity.expected_salary_max },
                {"@work_status", identity.work_status },
                {"@qualification_id", identity.qualification_id },
                {"@job_seeking_status_id", identity.job_seeking_status_id },
                {"@salary_type_id", identity.salary_type_id },
                {"@japanese_level_number", identity.japanese_level_number },
                {"@google_id", identity.google_id },
                {"@facebook_id", identity.facebook_id },
                {"@nationality_id", identity.nationality_id },
                {"@visa_id", identity.visa_id },
                {"@duration_visa", identity.duration_visa },
                {"@religion", identity.religion },
                {"@religion_detail", identity.religion_detail }
            };

            try
            {
                StringBuilder addressesCmd = new StringBuilder();
                using (var conn = new SqlConnection(_connectionString))
                {
                    MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    if (identity.Addresses.HasData())
                    {
                        foreach (var add in identity.Addresses)
                        {
                            var addParms = new Dictionary<string, object>
                            {
                                {"@id", add.id },
                                {"@job_seeker_id", add.job_seeker_id },
                                {"@country_id", add.country_id },
                                {"@region_id", add.region_id },
                                {"@prefecture_id", add.prefecture_id },
                                {"@city_id", add.city_id },
                                {"@detail", add.detail },
                                {"@furigana", add.furigana },
                                {"@note", add.note },
                                {"@lat", add.lat },
                                {"@lng", add.lng },
                                {"@is_contact_address", add.is_contact_address },
                                {"@postal_code", add.postal_code },
                                {"@train_line_id", add.train_line_id },
                                {"@station_id", add.station_id }
                            };

                            MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "JobSeeker_UpdateAddress", addParms);
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

        public IdentityJobSeeker GetById(int id)
        {
            IdentityJobSeeker info = null;
            var sqlCmd = @"JobSeeker_GetById";

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
                            info = ExtractJobSeekerData(reader);
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
            var sqlCmd = @"JobSeeker_Delete";

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

        public List<IdentityJobSeeker> GetList()
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityJobSeekerLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityJobSeekerLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.JobSeekerId = Utils.ConvertToInt32(reader["JobSeekerId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.JobSeekerId == item.Id).ToList();
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

        public List<IdentityJobSeeker> GetListByPrefecture(int prefecture_id)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_GetListByPrefecture";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@prefecture_id", prefecture_id},
            };

            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityJobSeekerLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityJobSeekerLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.JobSeekerId = Utils.ConvertToInt32(reader["JobSeekerId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.JobSeekerId == item.Id).ToList();
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

        public IdentityJobSeeker GetBaseInfo(int id, int agency_id = 0)
        {
            IdentityJobSeeker info = null;
            var sqlCmd = @"JobSeeker_GetBaseInfo";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@agency_id", agency_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobSeekerData(reader);
                        }

                        if (reader.NextResult())
                        {
                            var listAddress = new List<IdentityJobSeekerAddress>();
                            while (reader.Read())
                            {
                                listAddress.Add(ExtractJobSeekerAddressData(reader));
                            }

                            if (listAddress.HasData())
                            {
                                info.Addresses = listAddress;
                            }
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                info.WishInfo = ExtractJobSeekerWishData(reader);
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
        public static IdentityJobSeekerWish ExtractJobSeekerWishData(IDataReader reader)
        {
            var record = new IdentityJobSeekerWish();

            //Seperate properties
            record.employment_type_ids = reader["employment_type_ids"].ToString();
            record.prefecture_ids = reader["prefecture_ids"].ToString();
            record.sub_field_ids = reader["sub_field_ids"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.salary_min = Utils.ConvertToInt32(reader["salary_min"]);
            record.salary_max = Utils.ConvertToInt32(reader["salary_max"]);

            return record;
        }

        public IdentityJobSeeker GetDetailForUpdate(int id)
        {
            IdentityJobSeeker info = null;
            var sqlCmd = @"JobSeeker_GetDetailForUpdate";

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
                            info = ExtractJobSeekerData(reader);
                        }

                        if (reader.NextResult())
                        {
                            var listAddress = new List<IdentityJobSeekerAddress>();
                            while (reader.Read())
                            {
                                listAddress.Add(ExtractJobSeekerAddressData(reader));
                            }

                            if (listAddress.HasData())
                            {
                                info.Extensions.Addresses = listAddress;
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

        public IdentityJobSeeker GetByEmail(string email)
        {
            IdentityJobSeeker info = null;
            var sqlCmd = @"JobSeeker_GetByEmail";

            var parameters = new Dictionary<string, object>
            {
                {"@email", email}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobSeekerData(reader);
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

        public int SaveTokenFireBase(IdentityTokenFireBase info)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_SaveTokenFireBase";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@user_id", info.user_id},
                {"@token", info.token}
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

        public int MarkIsReadNotification(IdentityNotification info)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_MarkIsReadNotification";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", info.id},
                {"@user_id", info.user_id},
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

        public List<int> SetMainCv(IdentityCv info)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_SetMainCv";

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

        public List<IdentityJobSeeker> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_GetSuggestionsForInvitationByPage";
            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
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
                        while (reader.Read())
                        {
                            var record = new IdentityJobSeeker();
                            record.user_id = Utils.ConvertToInt32(reader["user_id"]);

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

        public IdentityJobSeekerCounter GetCounterForDeletion(int job_seeker_id, int agency_id)
        {
            IdentityJobSeekerCounter info = new IdentityJobSeekerCounter();

            var sqlCmd = @"JobSeeker_GetCounterForDeletetion";

            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id },
                {"@agency_id", agency_id }
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

        #region Agency

        public IdentityJobSeeker A_GetBaseInfo(int id, int agency_id)
        {
            IdentityJobSeeker info = null;
            var sqlCmd = @"A_JobSeeker_GetBaseInfo";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@agency_id", agency_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobSeekerData(reader);
                            info.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
                            info.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
                        }

                        if (reader.NextResult())
                        {
                            var listAddress = new List<IdentityJobSeekerAddress>();
                            while (reader.Read())
                            {
                                listAddress.Add(ExtractJobSeekerAddressData(reader));
                            }

                            if (listAddress.HasData())
                            {
                                info.Addresses = listAddress;
                            }
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                info.WishInfo = ExtractJobSeekerWishData(reader);
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

        public List<IdentityJobSeeker> A_GetListByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"A_JobSeeker_GetListByPage";
            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();

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
                        while (reader.Read())
                        {
                            var record = new IdentityJobSeeker();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);
                            if (reader.HasColumn("school"))
                                record.school = reader["school"].ToString();
                            if (reader.HasColumn("major"))
                                record.major = reader["major"].ToString();
                            if (reader.HasColumn("code"))
                                record.code = reader["code"].ToString();

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
        public List<IdentityJobSeeker> A_GetListAssignmentWorkByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"A_JobSeeker_GetListAssignmentWorkByPage";
            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();

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
                {"@country_id",filter.country_id },
                {"@gender",filter.gender },
                {"@major_id",filter.major_id },
                {"@job_id",filter.job_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityJobSeeker();
                            record.id = Utils.ConvertToInt32(reader["id"]);

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);
                            if (reader.HasColumn("school"))
                                record.school = reader["school"].ToString();
                            if (reader.HasColumn("major"))
                                record.major = reader["major"].ToString();

                            if (reader.HasColumn("qualification"))
                                record.qualification = reader["qualification"].ToString();
                            if (reader.HasColumn("cv_id"))
                                record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);

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
        public int A_UpdateProfile(IdentityJobSeeker identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_UpdateProfile";
            var returnId = 0;

            try
            {
                //For parameters
                var parameters = new Dictionary<string, object>
                {
                    {"@id", identity.id },
                    {"@email", identity.email },
                    {"@phone", identity.phone },
                    {"@marriage", identity.marriage },
                    {"@dependent_num", identity.dependent_num },
                    {"@fullname", identity.fullname },
                    {"@fullname_furigana", identity.fullname_furigana },
                    {"@display_name", identity.display_name },
                    {"@image", identity.image },
                    {"@birthday", identity.birthday },
                    {"@gender", identity.gender },
                    {"@id_card", identity.id_card },
                    {"@note", identity.note },
                    {"@video_path", identity.video_path },
                    {"@expected_job_title", identity.expected_job_title },
                    {"@expected_salary_min", identity.expected_salary_min },
                    {"@expected_salary_max", identity.expected_salary_max },
                    {"@work_status", identity.work_status },
                    {"@qualification_id", identity.qualification_id },
                    {"@job_seeking_status_id", identity.job_seeking_status_id },
                    {"@salary_type_id", identity.salary_type_id },
                    {"@agency_id", identity.agency_id },
                    {"@staff_id", identity.staff_id },
                    {"@japanese_level_number", identity.japanese_level_number },
                    {"@pic_id", identity.pic_id },
                    {"@metadata", identity.metadata },
                    {"@nationality_id", identity.nationality_id },
                    {"@visa_id", identity.visa_id },
                    {"@duration_visa", identity.duration_visa },
                    {"@religion", identity.religion },
                    {"@religion_detail", identity.religion_detail }
                };

                StringBuilder addressesCmd = new StringBuilder();
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    returnId = Utils.ConvertToInt32(returnObj);

                    if (identity.Addresses.HasData() && returnId > 0)
                    {
                        foreach (var add in identity.Addresses)
                        {
                            var addParms = new Dictionary<string, object>
                            {
                                {"@id", add.id },
                                {"@job_seeker_id", returnId },
                                {"@country_id", add.country_id },
                                {"@region_id", add.region_id },
                                {"@prefecture_id", add.prefecture_id },
                                {"@city_id", add.city_id },
                                {"@detail", add.detail },
                                {"@furigana", add.furigana },
                                {"@note", add.note },
                                {"@lat", add.lat },
                                {"@lng", add.lng },
                                {"@is_contact_address", add.is_contact_address },
                                {"@postal_code", add.postal_code },
                                {"@train_line_id", add.train_line_id },
                                {"@station_id", add.station_id }
                            };

                            MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "A_JobSeeker_UpdateAddress", addParms);
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

        public int A_Delete(IdentityJobSeeker identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_Delete";
            var returnValue = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@agency_id", identity.agency_id },
                {"@staff_id", identity.staff_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    returnValue = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnValue;
        }

        public int A_Deletes(string ids, int type)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_Deletes";
            var returnValue = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ids", ids },
                {"@type", type }

            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    returnValue = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnValue;
        }

        #endregion

        #region Admin

        public List<IdentityJobSeeker> M_GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"M_JobSeeker_GetListByPage";
            List<IdentityJobSeeker> listData = new List<IdentityJobSeeker>();

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
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityJobSeeker();
                            record.user_id = Utils.ConvertToInt32(reader["user_id"]);
                            record.Extensions.cv_count = Utils.ConvertToInt32(reader["cv_count"]);
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

        public IdentityJobSeekerCounter A_GetCounterForDeletion(int job_seeker_id, int agency_id)
        {
            IdentityJobSeekerCounter info = new IdentityJobSeekerCounter();

            var sqlCmd = @"A_JobSeeker_GetCounterForDeletetion";

            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id },
                {"@agency_id", agency_id }
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
