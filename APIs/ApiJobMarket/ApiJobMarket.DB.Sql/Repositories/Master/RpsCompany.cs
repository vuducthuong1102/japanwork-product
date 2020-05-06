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
    public class RpsCompany
    {
        private readonly string _connectionString;

        public RpsCompany(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCompany()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityCompany> GetByPage(IdentityCompany filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Company_GetByPage";
            List<IdentityCompany> listData = null;
            List<IdentityCompanyLang> listLang = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@language_code", filter.language_code}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListCompanyFromReader(reader);

                        //Lang
                        if (reader.NextResult())
                        {
                            listLang = new List<IdentityCompanyLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ExtractCompanyLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listLang.Where(x => x.company_id == item.id).ToList();
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

        public List<IdentityCompany> GetListByAgency(IdentityCompany filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Company_GetListByAgency";

            List<IdentityCompany> listData = null;
            List<IdentityCompanyLang> listLang = null;

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
                {"@language_code", filter.language_code},
                {"@prefecture_id", filter.prefecture_id},
                {"@sub_industry_id", filter.sub_industry_id},
                {"@ishiring", filter.ishiring}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListCompanyFromReader(reader);

                        //Lang
                        if (reader.NextResult())
                        {
                            listLang = new List<IdentityCompanyLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ExtractCompanyLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listLang.Where(x => x.company_id == item.id).ToList();
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
        //public List<IdentityCompany> GetPageByAgency(dynamic filter)
        //{
        //    //Common syntax           
        //    var sqlCmd = @"Company_GetPageByAgency";

        //    List<IdentityCompany> listData = null;
        //    List<IdentityCompanyLang> listLang = null;

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@keyword", filter.keyword },
        //        {"@agency_id", filter.agency_id },
        //        {"@offset", filter.offset()},
        //        {"@page_size", filter.pagesize},
        //        {"@language_code", filter.language_code},
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //            {
        //                listData = ParsingListCompanyFromReader(reader);

        //                //Lang
        //                if (reader.NextResult())
        //                {
        //                    listLang = new List<IdentityCompanyLang>();
        //                    while (reader.Read())
        //                    {
        //                        listLang.Add(ExtractCompanyLangData(reader));
        //                    }
        //                }

        //                if (listData.HasData())
        //                {
        //                    foreach (var item in listData)
        //                    {
        //                        item.LangList = listLang.Where(x => x.company_id == item.id).ToList();
        //                    }
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

        private List<IdentityCompany> ParsingListCompanyFromReader(IDataReader reader)
        {
            List<IdentityCompany> listData = listData = new List<IdentityCompany>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCompanyData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                if (reader.HasColumn("job_count"))
                    record.job_count = Utils.ConvertToInt32(reader["job_count"]);

                if (reader.HasColumn("application_count"))
                    record.application_count = Utils.ConvertToInt32(reader["application_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityCompany ExtractCompanyData(IDataReader reader)
        {
            var record = new IdentityCompany();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.company_name = reader["company_name"].ToString();
            record.description = reader["description"].ToString();
            record.company_size_id = Utils.ConvertToInt32(reader["company_size_id"]);
            record.logo_path = reader["logo_path"].ToString();
            record.sub_industry_id = Utils.ConvertToInt32(reader["sub_industry_id"]);
            record.establish_year = Utils.ConvertToInt32(reader["establish_year"]);
            record.email = reader["email"].ToString();
            record.website = reader["website"].ToString();
            record.phone = reader["phone"].ToString();
            record.fax = reader["fax"].ToString();
            record.branch = Utils.ConvertToInt32(reader["branch"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.desciption_translation_id = Utils.ConvertToInt32(reader["desciption_translation_id"]);
            record.headquater_id = Utils.ConvertToInt32(reader["headquater_id"]);
            record.country_id = Utils.ConvertToInt32(reader["country_id"]);
            record.region_id = Utils.ConvertToInt32(reader["region_id"]);
            record.prefecture_id = Utils.ConvertToInt32(reader["prefecture_id"]);
            record.city_id = Utils.ConvertToInt32(reader["city_id"]);
            record.address_detail = reader["address_detail"].ToString();
            record.address_furigana = reader["address_furigana"].ToString();
            record.lat = reader["lat"].ToString();
            record.lng = reader["lng"].ToString();
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.invitation_limit = Utils.ConvertToInt32(reader["invitation_limit"]);
            record.pic_id = Utils.ConvertToInt32(reader["pic_id"]);
            record.company_code = reader["company_code"].ToString();
            //record.map = reader["map"].ToString();

            return record;
        }

        public static IdentityCompanyLang ExtractCompanyLangData(IDataReader reader)
        {
            var record = new IdentityCompanyLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.company_id = Utils.ConvertToInt32(reader["company_id"]);
            record.company_name = reader["company_name"].ToString();
            record.description = reader["description"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentityCompany identity)
        {
            //Common syntax           
            var sqlCmd = @"Company_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_name", identity.company_name },
                {"@description", identity.description },
                {"@company_size_id", identity.company_size_id },
                {"@logo_path", identity.logo_path },
                {"@sub_industry_id", identity.sub_industry_id },
                {"@establish_year", identity.establish_year },
                {"@email", identity.email },
                {"@website", identity.website },
                {"@phone", identity.phone },
                {"@fax", identity.fax },
                {"@branch", identity.branch },
                {"@agency_id", identity.agency_id },
                {"@desciption_translation_id", identity.desciption_translation_id },
                {"@headquater_id", identity.headquater_id },
                {"@region_id", identity.region_id },
                {"@prefecture_id", identity.prefecture_id },
                {"@city_id", identity.city_id },
                {"@lat", identity.lat },
                {"@lng", identity.lng },
                {"@pic_id", identity.pic_id }
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

        public int Update(IdentityCompany identity)
        {
            //Common syntax
            var sqlCmd = @"Company_Update";
            var returnId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@company_name", identity.company_name },
                {"@description", identity.description },
                {"@company_size_id", identity.company_size_id },
                {"@logo_path", identity.logo_path },
                {"@sub_industry_id", identity.sub_industry_id },
                {"@establish_year", identity.establish_year },
                {"@email", identity.email },
                {"@website", identity.website },
                {"@phone", identity.phone },
                {"@fax", identity.fax },
                //{"@branch", identity.branch },
                {"@agency_id", identity.agency_id },
                {"@address_detail", identity.address_detail },
                {"@address_furigana", identity.address_furigana },
                {"@country_id", identity.country_id },
                {"@region_id", identity.region_id },
                {"@prefecture_id", identity.prefecture_id },
                {"@city_id", identity.city_id },
                //{"@lat", identity.lat },
                //{"@lng", identity.lng }
                {"@language_code", identity.language_code },
                {"@pic_id", identity.pic_id },
                {"@staff_id", identity.staff_id },
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

        public IdentityCompany GetById(int id)
        {
            IdentityCompany info = null;
            List<IdentityCompanyLang> listLang = null;
            var sqlCmd = @"Company_GetById";

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
                            info = ExtractCompanyData(reader);

                            //Lang
                            if (reader.NextResult())
                            {
                                listLang = new List<IdentityCompanyLang>();
                                while (reader.Read())
                                {
                                    listLang.Add(ExtractCompanyLangData(reader));
                                }
                            }

                            if (info != null)
                            {
                                info.LangList = listLang;
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

        public IdentityCompany CountJobById(int id)
        {
            IdentityCompany info = new IdentityCompany();
            var sqlCmd = @"Company_CountJob";

            var parameters = new Dictionary<string, object>
            {
                {"@company_id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info.total = Utils.ConvertToInt32(reader["total"]);
                            info.total_public = Utils.ConvertToInt32(reader["total_public"]);
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

        public bool Delete(string ids, int staff_id)
        {
            //Common syntax            
            var sqlCmd = @"Company_Delete";
            bool result = false;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ids", ids},
                {"@staff_id", staff_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            result = Utils.ConvertToBoolean(reader[0]);
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
        public List<IdentityCompany> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Company_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentityCompany> listData = new List<IdentityCompany>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCompanyData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityCompanyLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityCompanyLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.CompanyId = Utils.ConvertToInt32(reader["CompanyId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.CompanyId == item.Id).ToList();
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

        public List<IdentityCompany> GetListByAgency(int agency_id)
        {
            //Common syntax            
            var sqlCmd = @"Company_GetListByAgency";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", agency_id},
            };

            List<IdentityCompany> listData = new List<IdentityCompany>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCompanyData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityCompanyLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityCompanyLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.CompanyId = Utils.ConvertToInt32(reader["CompanyId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.CompanyId == item.Id).ToList();
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

        public IdentityCompanyCounter GetCounterForDeletion(int id)
        {
            IdentityCompanyCounter info = new IdentityCompanyCounter();

            var sqlCmd = @"Company_GetCounterForDeletetion";

            var parameters = new Dictionary<string, object>
            {
                {"@company_id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info.JobCounter = new IdentityJobCounter();
                            info.JobCounter.Draft = Utils.ConvertToInt32(reader["Draft"]);
                            info.JobCounter.Published = Utils.ConvertToInt32(reader["Published"]);
                            info.JobCounter.Closed = Utils.ConvertToInt32(reader["Closed"]);
                            info.JobCounter.Saved = Utils.ConvertToInt32(reader["Saved"]);
                            info.JobCounter.Expired = Utils.ConvertToInt32(reader["Expired"]);
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

        public List<int> DeleteAllJobs(int id)
        {
            List<int> deletedItems = new List<int>();

            //Common syntax            
            var sqlCmd = @"Company_DeleteAllJobs";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            deletedItems.Add(Utils.ConvertToInt32(reader[0]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return deletedItems;
        }

        #endregion
    }
}
