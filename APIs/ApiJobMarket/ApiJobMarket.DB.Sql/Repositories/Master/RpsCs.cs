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
    public class RpsCs
    {
        private readonly string _connectionString;

        public RpsCs(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCs()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityCs> GetByPage(IdentityCs filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cs_GetAll";
            List<IdentityCs> listData = null;

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
                        listData = ParsingListCsFromReader(reader);
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

        public List<IdentityCs> GetListByJobSeeker(IdentityCs filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cs_GetListByJobSeeker";
            List<IdentityCs> listData = null;

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
                        listData = ParsingListCsFromReader(reader);
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

        public List<IdentityCs> SearchByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cs_SearchByPage";
            List<IdentityCs> listData = null;

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
                        listData = ParsingListCsFromReader(reader);
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

        private List<IdentityCs> ParsingListCsFromReader(IDataReader reader)
        {
            List<IdentityCs> listData = listData = new List<IdentityCs>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCsData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityCs ExtractCsData(IDataReader reader)
        {
            var record = new IdentityCs();

            //Seperate properties;
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.cs_title = reader["cs_title"].ToString();
            record.date = reader["date"] == DBNull.Value ? null : (DateTime?)reader["date"];
            record.fullname = reader["fullname"].ToString();
            record.fullname_furigana = reader["fullname_furigana"].ToString();
            record.gender = Utils.ConvertToInt32(reader["gender"]);
            record.birthday = reader["birthday"] == DBNull.Value ? null : (DateTime?)reader["birthday"];
            record.email = reader["email"].ToString();
            record.phone = reader["phone"].ToString();
            record.highest_edu = Utils.ConvertToInt32(reader["highest_edu"]);
            record.image = reader["image"].ToString();
            record.pdf = reader["pdf"].ToString();
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.main_cs = Utils.ConvertToInt32(reader["main_cs"]);
            record.station_id = Utils.ConvertToInt32(reader["station_id"]);
            record.train_line_id = Utils.ConvertToInt32(reader["train_line_id"]);
            record.status = Utils.ConvertToInt32(reader["status"]);

            return record;
        }

        public static IdentityCsAddress ExtractCsAddressData(IDataReader reader)
        {
            var record = new IdentityCsAddress();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cs_id = Utils.ConvertToInt32(reader["cs_id"]);
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

        public static IdentityCsCertificate ExtractCsCertificationData(IDataReader reader)
        {
            var record = new IdentityCsCertificate();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cs_id = Utils.ConvertToInt32(reader["cs_id"]);
            record.name = reader["name"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.point = reader["point"].ToString();
            record.pass = Utils.ConvertToInt32(reader["pass"]);

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
            record.address = reader["address"].ToString();
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.major_id = Utils.ConvertToInt32(reader["major_id"]);
            record.major_custom = reader["major_custom"].ToString();
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);

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
            record.address = reader["address"].ToString();

            return record;
        }

        //public static IdentityCsPdfCode ExtractCsPdfCodeData(IDataReader reader)
        //{
        //    var record = new IdentityCsPdfCode();

        //    //Seperate properties
        //    record.id = Utils.ConvertToInt32(reader["id"]);
        //    record.user_id = Utils.ConvertToInt32(reader["user_id"]);
        //    record.cs_id = Utils.ConvertToInt32(reader["cs_id"]);
        //    record.code_id = reader["code_id"].ToString();
        //    record.form = Utils.ConvertToInt32(reader["form"]);
        //    record.work_background_id = Utils.ConvertToInt32(reader["work_background_id"]);
        //    record.title = reader["title"].ToString();
        //    record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
        //    record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
        //    record.expired_at_utc = reader["expired_at_utc"] == DBNull.Value ? null : (DateTime?)reader["expired_at_utc"];

        //    return record;
        //}

        public int Insert(IdentityCs identity)
        {
            //Common syntax           
            var sqlCmd = @"Cs_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", identity.job_seeker_id },
                {"@cs_title", identity.cs_title },
                {"@date", identity.date },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@gender", identity.gender },
                {"@birthday", identity.birthday },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@highest_edu", identity.highest_edu },
                {"@image", identity.image },
                {"@pdf", identity.pdf },
                {"@station_id", identity.station_id },
                {"@train_line_id", identity.train_line_id }
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
                            var cmdCertificate = @"Cs_AddCertificate";

                            foreach (var item in identity.certification)
                            {
                                var cerParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_id", newId },
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
                            var cmdEduHistory = @"Cs_AddEduHistory";

                            foreach (var item in identity.edu_history)
                            {
                                var eduParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_id", newId },
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
                            var cmdWorkHistory = @"Cs_AddWorkHistory";

                            foreach (var item in identity.work_history)
                            {
                                var workParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_id", newId },
                                    {"@company", item.company },
                                    {"@sub_field_id", item.sub_field_id },
                                    {"@sub_industry_id", item.sub_industry_id },
                                    {"@employment_type_id", item.employment_type_id },
                                    {"@employees_number", item.employees_number },
                                    {"@resign_reason", item.resign_reason },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@status", item.status },
                                    {"@address", item.address }
                                };

                                var newWorkIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdWorkHistory, workParams);
                                var newWorkId = Utils.ConvertToInt32(newWorkIdObj);

                                if (newWorkId > 0)
                                {
                                    if (item.Details.HasData())
                                    {
                                        var cmdDetail = @"Cs_UpdateWorkHistoryDetail";

                                        foreach (var dt in item.Details)
                                        {
                                            var myParams = new Dictionary<string, object>
                                            {
                                                {"@id", dt.id },
                                                {"@cs_work_history_id", newWorkId },
                                                {"@department", dt.department },
                                                {"@position", dt.position },
                                                {"@salary", dt.salary },
                                                {"@start_date", dt.start_date },
                                                {"@end_date", dt.end_date },
                                                {"@content_work", dt.content_work }
                                            };

                                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdDetail, myParams);
                                        }
                                    }
                                }
                            }
                        }

                        if (identity.address != null)
                        {
                            var cmdAddress = @"Cs_AddAddress";
                            var addressParams = new Dictionary<string, object>
                            {
                                {"@cs_id", newId },
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

                        //if (identity.address_contact != null)
                        //{
                        //    var cmdAddressContact = @"Cs_AddAddress";
                        //    var addressContactParams = new Dictionary<string, object>
                        //    {
                        //        {"@cs_id", newId },
                        //        {"@country_id", identity.address_contact.country_id },
                        //        {"@region_id", identity.address_contact.region_id },
                        //        {"@prefecture_id", identity.address_contact.prefecture_id },
                        //        {"@city_id", identity.address_contact.city_id },
                        //        {"@detail", identity.address_contact.detail },
                        //        {"@furigana", identity.address_contact.furigana },
                        //        {"@note", identity.address_contact.note },
                        //        {"@lat", identity.address_contact.lat },
                        //        {"@lng", identity.address_contact.lng },
                        //        {"@is_contact_address", identity.address_contact.is_contact_address },
                        //        {"@postal_code", identity.address_contact.postal_code },
                        //        {"@train_line_id", identity.address_contact.train_line_id },
                        //        {"@station_id", identity.address_contact.station_id }
                        //    };

                        //    MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdAddressContact, addressContactParams);
                        //}
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

        public bool Update(IdentityCs identity)
        {
            //Common syntax
            var sqlCmd = @"Cs_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@job_seeker_id", identity.job_seeker_id },
                {"@cs_title", identity.cs_title },
                {"@date", identity.date },
                {"@fullname", identity.fullname },
                {"@fullname_furigana", identity.fullname_furigana },
                {"@gender", identity.gender },
                {"@birthday", identity.birthday },
                {"@email", identity.email },
                {"@phone", identity.phone },
                {"@highest_edu", identity.highest_edu },
                {"@image", identity.image },
                {"@pdf", identity.pdf },
                {"@station_id", identity.station_id },
                {"@train_line_id", identity.train_line_id }
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
                            var cmdCertificate = @"Cs_AddCertificate";

                            foreach (var item in identity.certification)
                            {
                                var cerParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_id", currentId },
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
                            var cmdEduHistory = @"Cs_AddEduHistory";

                            foreach (var item in identity.edu_history)
                            {
                                var eduParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_id", currentId },
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
                            var cmdWorkHistory = @"Cs_AddWorkHistory";

                            foreach (var item in identity.work_history)
                            {
                                var workParams = new Dictionary<string, object>
                                {
                                    {"@id", item.id },
                                    {"@cs_id", currentId },
                                    {"@company", item.company },
                                    {"@sub_field_id", item.sub_field_id },
                                    {"@sub_industry_id", item.sub_industry_id },
                                    {"@employment_type_id", item.employment_type_id },
                                    {"@employees_number", item.employees_number },
                                    {"@resign_reason", item.resign_reason },
                                    {"@start_date", item.start_date },
                                    {"@end_date", item.end_date },
                                    {"@status", item.status },
                                    {"@address", item.address }
                                };

                                var newWorkIdObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdWorkHistory, workParams);
                                var newWorkId = Utils.ConvertToInt32(newWorkIdObj);

                                if (newWorkId > 0)
                                {
                                    if (item.Details.HasData())
                                    {
                                        var cmdDetail = @"Cs_UpdateWorkHistoryDetail";

                                        foreach (var dt in item.Details)
                                        {
                                            var myParams = new Dictionary<string, object>
                                            {
                                                {"@id", dt.id },
                                                {"@cs_work_history_id", newWorkId },
                                                {"@department", dt.department },
                                                {"@position", dt.position },
                                                {"@salary", dt.salary },
                                                {"@start_date", dt.start_date },
                                                {"@end_date", dt.end_date },
                                                {"@content_work", dt.content_work }
                                            };

                                            MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdDetail, myParams);
                                        }
                                    }
                                }
                            }
                        }

                        if (identity.address != null)
                        {
                            var cmdAddress = @"Cs_AddAddress";
                            var addressParams = new Dictionary<string, object>
                            {
                                {"@cs_id", currentId },
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

                        //if (identity.address_contact != null)
                        //{
                        //    var cmdAddressContact = @"Cs_AddAddress";
                        //    var addressContactParams = new Dictionary<string, object>
                        //    {
                        //        {"@cs_id", currentId },
                        //        {"@country_id", identity.address_contact.country_id },
                        //        {"@region_id", identity.address_contact.region_id },
                        //        {"@prefecture_id", identity.address_contact.prefecture_id },
                        //        {"@city_id", identity.address_contact.city_id },
                        //        {"@detail", identity.address_contact.detail },
                        //        {"@furigana", identity.address_contact.furigana },
                        //        {"@note", identity.address_contact.note },
                        //        {"@lat", identity.address_contact.lat },
                        //        {"@lng", identity.address_contact.lng },
                        //        {"@is_contact_address", identity.address_contact.is_contact_address },
                        //        {"@postal_code", identity.address_contact.postal_code },
                        //        {"@train_line_id", identity.address_contact.train_line_id },
                        //        {"@station_id", identity.address_contact.station_id }
                        //    };

                        //    MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, cmdAddressContact, addressContactParams);
                        //}
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

        public int Clone(IdentityCs identity)
        {
            //Common syntax           
            var sqlCmd = @"Cs_Clone";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@cs_id", identity.id },
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

        public IdentityCs GetById(int id)
        {
            IdentityCs info = null;

            var sqlCmd = @"Cs_GetById";

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
                            info = ExtractCsData(reader);
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

        public IdentityCs GetDetail(int id)
        {
            IdentityCs info = null;
            List<IdentityCsAddress> addresses = null;
            List<IdentityCsEduHistory> eduHistories = null;
            List<IdentityCsWorkHistory> workHistories = null;
            List<IdentityCsCertificate> certifications = null;

            var sqlCmd = @"Cs_GetDetail";

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
                            info = ExtractCsData(reader);
                        }

                        if (info != null && info.id > 0)
                        {
                            //EduHistories
                            if (reader.NextResult())
                            {
                                eduHistories = new List<IdentityCsEduHistory>();
                                while (reader.Read())
                                {
                                    var edu = ExtractCsEduHistoryData(reader);

                                    eduHistories.Add(edu);
                                }

                                info.edu_history = eduHistories;
                            }

                            //WorkHistories
                            if (reader.NextResult())
                            {
                                workHistories = new List<IdentityCsWorkHistory>();
                                while (reader.Read())
                                {
                                    var work = ExtractCsWorkHistoryData(reader);
                                    workHistories.Add(work);
                                }

                                info.work_history = workHistories;
                            }

                            //Certificates
                            if (reader.NextResult())
                            {
                                certifications = new List<IdentityCsCertificate>();
                                while (reader.Read())
                                {
                                    var cer = ExtractCsCertificationData(reader);
                                    certifications.Add(cer);
                                }

                                info.certification = certifications;
                            }

                            //Addresses
                            if (reader.NextResult())
                            {
                                addresses = new List<IdentityCsAddress>();
                                while (reader.Read())
                                {
                                    var address = ExtractCsAddressData(reader);
                                    addresses.Add(address);
                                }

                                if (addresses.HasData())
                                {
                                    foreach (var item in addresses)
                                    {
                                        if (!item.is_contact_address)
                                        {
                                            info.address = item;
                                        }
                                        //else
                                        //{
                                        //    info.address_contact = item;
                                        //}
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

        public bool Delete(int id)
        {
            //Common syntax            
            var sqlCmd = @"Cs_Delete";

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
            var sqlCmd = @"Cs_Close";

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
            var sqlCmd = @"Cs_Apply";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", applyInfo.job_id},
                {"@job_seeker_id", applyInfo.job_seeker_id},
                {"@cs_id", applyInfo.cs_id }
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
            var sqlCmd = @"Cs_Save";

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

        public List<IdentityCs> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Cs_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentityCs> listData = new List<IdentityCs>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCsData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityCsLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityCsLang();
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

        //public List<IdentityCs> GetListByJobSeeker(int job_seeker_id)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"Cs_GetListByJobSeeker";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@job_seeker_id", job_seeker_id},
        //    };

        //    List<IdentityCs> listData = new List<IdentityCs>();
        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //            {
        //                while (reader.Read())
        //                {
        //                    var record = ExtractCsData(reader);

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


        //public List<IdentityCsPdfCode> GetListCodeByJobSeeker(int job_seeker_id)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"Cs_GetListCodeByJobSeeker";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@job_seeker_id", job_seeker_id},
        //    };

        //    List<IdentityCsPdfCode> listData = new List<IdentityCsPdfCode>();
        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //            {
        //                while (reader.Read())
        //                {
        //                    var record = ExtractCsPdfCodeData(reader);

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

        public IdentityCs CheckJobSaved(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Cs_CheckJobSaved";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
            };

            IdentityCs info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCsData(reader);
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
            var sqlCmd = @"Cs_DeleteJobSaved";

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

        //public int SavePrintCode(IdentityCsPdfCode codeInfo)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"Cs_SavePrintCode";

        //    var status = 0;
        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@code_id", codeInfo.code_id},
        //        {"@user_id", codeInfo.user_id},
        //        {"@cs_id", codeInfo.cs_id}
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
        //            status = Utils.ConvertToInt32(statusObj);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
        //        throw new CustomSQLException(strError);
        //    }

        //    return status;
        //}

        //public int DeletePrintCode(IdentityCsPdfCode codeInfo)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"Cs_DeletePrintCode";

        //    var status = 0;
        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@code_id", codeInfo.code_id},
        //        {"@user_id", codeInfo.user_id},
        //        {"@cs_id", codeInfo.cs_id}
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
        //            status = Utils.ConvertToInt32(statusObj);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
        //        throw new CustomSQLException(strError);
        //    }

        //    return status;
        //}

        public List<int> SetMainCs(IdentityCs info)
        {
            //Common syntax            
            var sqlCmd = @"Cs_SetMainCs";

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

        public List<IdentityCs> GetSuggestionsByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cs_GetSuggestionsByPage";
            List<IdentityCs> listData = null;

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
                        listData = ParsingListCsFromReader(reader);
                        while (reader.Read())
                        {
                            var record = new IdentityCs();
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

        public List<IdentityCs> GetSuggestionsForInvitationByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Cs_GetSuggestionsForInvitationByPage";
            List<IdentityCs> listData = new List<IdentityCs>();

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
                            var record = new IdentityCs();
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
    }
}
