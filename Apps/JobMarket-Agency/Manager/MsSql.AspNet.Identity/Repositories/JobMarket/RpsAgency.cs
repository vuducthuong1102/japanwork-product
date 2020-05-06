using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLibs;
using ApiJobMarket.DB.Sql.Entities;


namespace MsSql.AspNet.Identity.Repositories
{
    public class RpsAgency
    {
        private readonly string _connectionString;

        public RpsAgency(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsAgency()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- Agency ----

        public List<IdentityAgency> GetByPage(dynamic filter)
        {
            int offset = (filter.page_index - 1) * filter.page_size;
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword},
                {"@status", filter.status },
                {"@language_code", filter.language_code },
                {"@offset", offset},
                {"@page_size", filter.page_size}
            };

            var sqlCmd = @"M_Agency_GetByPage";

            List<IdentityAgency> myList = new List<IdentityAgency>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractAgencyItem(reader);

                            myList.Add(entity);
                        }

                        //if (reader.NextResult())
                        //{
                        //    if(myList.Count > 0)
                        //    {
                        //        while (reader.Read())
                        //        {
                        //            var itemLang = ExtractAgencyLangItem(reader);
                        //            foreach (var item in myList)
                        //            {
                        //                if (item.Id == itemLang.AgencyId)
                        //                    item.MyLanguages.Add(itemLang);
                        //            }
                        //        }
                        //    }
                        //}                       
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Agency_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityAgency GetById(int id, string langCode)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
            };

            var sqlCmd = @"M_Agency_GetById";

            IdentityAgency info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractAgencyItem(reader);
                        }

                        //if (reader.NextResult())
                        //{
                        //    if (info != null)
                        //    {
                        //        while (reader.Read())
                        //        {
                        //            var itemLang = ExtractAgencyLangItem(reader);
                        //            info.MyLanguages.Add(itemLang);
                        //        }
                        //    }                            
                        //}

                        ////Images
                        //if (reader.NextResult())
                        //{
                        //    info.Images = ExtractAgencyImageData(reader);
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityAgency GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"Agency_GetDetailById";

            IdentityAgency info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractAgencyItem(reader);
                        }

                        //if (reader.NextResult())
                        //{
                        //    if (info != null)
                        //    {
                        //        while (reader.Read())
                        //        {
                        //            var itemLang = ExtractAgencyLangItem(reader);
                        //            info.MyLanguages.Add(itemLang);
                        //        }
                        //    }
                        //}

                        ////Images
                        //if (reader.NextResult())
                        //{
                        //    info.Images = ExtractAgencyImageData(reader);
                        //}
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

        private List<IdentityAgency> ParsingAgencyData(IDataReader reader)
        {
            List<IdentityAgency> listData = new List<IdentityAgency>();
            while (reader.Read())
            {
                //Get common info
                var entity = ExtractAgencyItem(reader);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityAgency ExtractAgencyItem(IDataReader reader)
        {
            var record = new IdentityAgency();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.agency = reader["agency"].ToString();
            //record.user_id = Utils.ConvertToInt32(reader["user_id"]);
            record.constract_id = Utils.ConvertToInt32(reader["constract_id"]);
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];

            if (reader.HasColumn("total_count"))
                record.total_count = Utils.ConvertToInt32(reader["total_count"]);


            return record;
        }

        private IdentityUser ExtractAccountData(IDataReader reader)
        {
            var user = new IdentityUser();

            user.Id = reader["Id"].ToString();
            user.Email = reader["Email"].ToString();
            user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
            user.PasswordHash = reader["PasswordHash"].ToString();
            user.SecurityStamp = reader["SecurityStamp"].ToString();
            user.PhoneNumber = reader["PhoneNumber"].ToString();
            user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
            user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
            user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
            user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
            user.AccessFailedCount = Utils.ConvertToInt32(reader["AccessFailedCount"]);
            user.UserName = reader["UserName"].ToString();
            user.StaffId = Utils.ConvertToInt32(reader["StaffId"]);

            return user;
        }

        //private IdentityAgencyLang ExtractAgencyLangItem(IDataReader reader)
        //{
        //    var entity = new IdentityAgencyLang();
        //    entity.Id = Utils.ConvertToInt32(reader["Id"]);
        //    entity.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);
        //    entity.Title = reader["Title"].ToString();
        //    entity.Description = reader["Description"].ToString();
        //    entity.BodyContent = reader["BodyContent"].ToString();
        //    entity.LangCode = reader["LangCode"].ToString();
        //    entity.UrlFriendly = reader["UrlFriendly"].ToString();

        //    return entity;
        //}

        //private List<IdentityAgencyImage> ExtractAgencyImageData(IDataReader reader)
        //{
        //    var myList = new List<IdentityAgencyImage>();
        //    while (reader.Read())
        //    {
        //        var record = new IdentityAgencyImage();

        //        //Seperate properties
        //        record.Id = reader["Id"].ToString();
        //        record.AgencyId = Utils.ConvertToInt32(reader["AgencyId"]);
        //        record.Name = reader["Name"].ToString();
        //        record.Url = reader["Url"].ToString();
        //        record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

        //        myList.Add(record);
        //    }

        //    return myList;
        //}

        public int CreateAccount(IdentityUser identity)
        {
            var sqlCmd = @"Agency_CreateAccount";
            var newId = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", identity.UserName},
                {"@FullName", identity.FullName},
                {"@PasswordHash", identity.PasswordHash},            
                {"@SecurityStamp", identity.SecurityStamp},            
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    newId = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int ResendEmailActive(IdentityActiveAccount info)
        {
            var sqlCmd = @"Agency_ResendEmailActive";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", info.UserName },
                {"@HashingData", info.HashingData }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    result = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }
        public int SendEmailRecover(dynamic info)
        {
            var sqlCmd = @"Agency_SendEmailRecover";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", info.UserName },
                {"@HashingData", info.HashingData }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    result = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }

        public int ChangePassword(IdentityUser info)
        {
            var sqlCmd = @"Agency_ChangePassword";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", info.UserName },
                {"@PasswordHash", info.PasswordHash }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    result = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }

        public int ActiveAccountByEmail(IdentityActiveAccount info)
        {
            var sqlCmd = @"Agency_ActiveAccountByEmail";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", info.UserName },
                {"@HashingData", info.HashingData }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    result = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }

        public int ActiveAccountByOTP(IdentityActiveAccount info)
        {

            var sqlCmd = @"Agency_ActiveAccountByOTP";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserId", info.UserId },
                {"@Code", info.OTPCode }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    result = Utils.ConvertToInt32(returnObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }

        public IdentityUser GetByUserName(string userName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", userName},
            };

            var sqlCmd = @"Account_GetByUserName";

            IdentityUser info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            //Get common info
                            info = ExtractAccountData(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public int Insert(IdentityAgency identity)
        {
            var sqlCmd = @"Agency_Insert";
            var newId = 0;

            var metaData = string.Empty;

            var parameters = new Dictionary<string, object>
            {
                //{"@Title", identity.Title},
                //{"@Description", identity.Description},
                //{"@BodyContent", identity.BodyContent},
                //{"@Cover", identity.Cover},
                //{"@UrlFriendly", identity.UrlFriendly},
                //{"@CategoryId", identity.CategoryId},
                //{"@LangCode", identity.LangCode},
                //{"@MetaData", metaData},
                //{"@CreatedBy", identity.CreatedBy},
                //{"@Status", identity.Status}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        newId = Utils.ConvertToInt32(reader[0]);
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

        public int Update(IdentityAgency identity)
        {
            var sqlCmd = @"M_Agency_Update";
            var metaData = string.Empty;
            
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@agency", identity.agency},
                {"@status", identity.status}               
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
        }

        public int Delete(int id)
        {
            var sqlCmd = @"M_Agency_Delete";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    return Utils.ConvertToInt32(result);
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }
        }

        #endregion

        //#region Images

        //public bool AddNewImage(IdentityAgencyImage identity)
        //{
        //    //Common syntax
        //    var sqlCmd = @"Agency_AddNewImage";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@Id", identity.Id},
        //        {"@AgencyId", identity.AgencyId},
        //        {"@Name", identity.Name},
        //        {"@Url", identity.Url}
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute Agency_AddNewImage. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return true;
        //}

        //public bool RemoveImage(string Id)
        //{
        //    //Common syntax
        //    var sqlCmd = @"Agency_RemoveImage";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@Id", Id},
        //    };

        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            MsSqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, sqlCmd, parameters);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute Agency_RemoveImage. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return true;
        //}

        //public List<IdentityAgencyImage> GetListImage(int Id)
        //{
        //    //Common syntax            
        //    var sqlCmd = @"Agency_GetListImage";

        //    //For parameters
        //    var parameters = new Dictionary<string, object>
        //    {
        //        {"@AgencyId", Id},
        //    };

        //    List<IdentityAgencyImage> listData = new List<IdentityAgencyImage>();
        //    try
        //    {
        //        using (var conn = new SqlConnection(_connectionString))
        //        {
        //            using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
        //            {
        //                listData = ExtractAgencyImageData(reader);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var strError = "Failed to execute Agency_GetListImage. Error: " + ex.Message;
        //        throw new CustomSQLException(strError);
        //    }

        //    return listData;
        //}

        //#endregion
    }
}
