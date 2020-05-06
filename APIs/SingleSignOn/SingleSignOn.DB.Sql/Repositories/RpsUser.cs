using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SingleSignOn.ShareLibs.Exceptions;
using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.ShareLibs;
using Newtonsoft.Json;

namespace SingleSignOn.DB.Sql.Repositories
{
    public class RpsUser
    {
        private readonly string _connectionString;

        public RpsUser(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsUser()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDB"].ConnectionString;
        }

        public List<IdentityUser> GetByPage(IdentityFilter identity)
        {
            var mc = new ManageConnection(_connectionString);
            int offset = (identity.PageIndex - 1) * identity.PageSize;
            var parameters = new Dictionary<string, object>
                {
                    {"@Keyword", identity.Keyword},
                    {"@Offset", offset},
                    {"@PageSize", identity.PageSize}
                };
            List<IdentityUser> myList = new List<IdentityUser>();
            try
            {
                var sqlCmd = @"Search_User";
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var item = ParsingDataItem(reader);
                            myList.Add(item);
                        }
                        return myList;
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to GetUserById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public IdentityUser GetUserById(int userId)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", userId}
                };

            try
            {
                var sqlCmd = @"User_GetById";

                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserDataFromReader, parameters.ToSqlParams());
                return entity;

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetUserById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public IdentityUser GetUserByInfo(string userInfo)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@Info", userInfo}
                };

            try
            {
                var sqlCmd = @"User_GetByInfo";

                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingBaseUserDataFromReader, parameters.ToSqlParams());
                return entity;

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetUserByInfo. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public IdentityUser ApiGetUserInfo(int userId, string token)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", userId},
                    {"@TokenKey", token }
                };

            try
            {
                var sqlCmd = @"User_ApiGetInfo";

                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserDataFromReader, parameters.ToSqlParams());
                return entity;

            }
            catch (Exception ex)
            {
                var strError = "Failed to ApiGetUserInfo. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public List<IdentityUser> GetTopTraveller(int userId, string token, int numberTop)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@NumberTop", numberTop },
                    {"@UserId", userId},
                    {"@TokenKey", token }
                };

            List<IdentityUser> myList = null;
            var sqlCmd = "User_GetTopTraveller";
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        myList = ExtractListUser(reader);
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetTopTraveler. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
            return myList;
        }

        public List<IdentityUser> GetListUserInfo(string listUserId)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@ListUserId", listUserId }
                };

            List<IdentityUser> myList = null;
            var sqlCmd = "User_GetListUser";
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        myList = ExtractListUser(reader);
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetTopTraveler. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
            return myList;
        }

        public int UserFollow(IdentityUserAction identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.UserId },
                    {"@UserActionId", identity.UserActionId},
                    {"@TokenKey", identity.TokenKey},
                    {"@Status", identity.Status },
                    {"@ActionType", identity.ActionType }
                };
            var sqlCmd = "User_FollowAction";
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
                var strError = "Failed to User_FollowAction. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public IdentityUser GetUserByUserName(string userName)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserName", userName}
                };

            try
            {
                var sqlCmd = @"User_GetByUserName";

                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserDataFromReader, parameters.ToSqlParams());
                return entity;

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetUserByUserName. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        private IdentityUser ExtractUserBaseData(IDataReader reader)
        {
            var entity = new IdentityUser();
            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.UserName = reader["UserName"].ToString();
            entity.Email = reader["Email"].ToString();
            entity.PhoneNumber = reader["PhoneNumber"].ToString();
            entity.FullName = reader["FullName"].ToString();
            entity.DisplayName = reader["DisplayName"].ToString();
            entity.Avatar = reader["Avatar"].ToString();

            entity.OTPType = reader["OTPType"].ToString();
            entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
            entity.Sex = Convert.ToInt32(reader["Sex"]);
            entity.Address = reader["Address"].ToString();
            entity.IDCard = reader["IDCard"].ToString();
            entity.Note = reader["Note"].ToString();
            entity.AppCode = reader["AppCode"].ToString();

            return entity;
        }

        private List<IdentityUser> ExtractListUser(IDataReader reader)
        {
            List<IdentityUser> myList = new List<IdentityUser>();
            while (reader.Read())
            {
                var entity = new IdentityUser();
                entity.Id = Utils.ConvertToInt32(reader["Id"]);
                entity.UserName = reader["UserName"].ToString();
                entity.DisplayName = reader["DisplayName"].ToString();
                entity.Avatar = reader["Avatar"].ToString();
                entity.FullName = reader["FullName"].ToString();

                var emailConfirm = Utils.ConvertToInt32(reader["EmailConfirmed"]);
                if(emailConfirm == 1)
                    entity.EmailConfirmed = true;

                var phoneConfirm = Utils.ConvertToInt32(reader["PhoneNumberConfirmed"]);
                if (phoneConfirm == 1)
                    entity.PhoneNumberConfirmed = true;

                entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];

                myList.Add(entity);
            }
            return myList;
        }

        private IdentityUser ParsingBaseUserDataFromReader(SqlDataReader reader)
        {
            if (reader.Read())
            {
                var entity = new IdentityUser();

                entity.Id = Utils.ConvertToInt32(reader["Id"]);
                entity.UserName = reader["UserName"].ToString();
                entity.Email = reader["Email"].ToString();
                entity.PasswordHash = reader["PasswordHash"].ToString();
                entity.PhoneNumber = reader["PhoneNumber"].ToString();
                entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
                entity.FullName = reader["FullName"].ToString();
                entity.DisplayName = reader["DisplayName"].ToString();
                entity.Sex = reader["Sex"] == DBNull.Value ? 1 : Convert.ToInt32(reader["Sex"]);
                entity.Address = reader["Address"].ToString();
                entity.Avatar = reader["Avatar"].ToString();
                entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
                entity.Note = reader["Note"].ToString();
                entity.OTPType = reader["OTPType"].ToString();

                return entity;
            }

            return null;
        }

        private IdentityUser ParsingUserDataFromReader(SqlDataReader reader)
        {
            if (reader.Read())
            {
                var entity = new IdentityUser();

                entity.Id = Utils.ConvertToInt32(reader["Id"]);
                entity.EmailConfirmed = (Utils.ConvertToInt32(reader["EmailConfirmed"]) == 1) ? true : false;
                entity.UserName = reader["UserName"].ToString();
                entity.SecurityStamp = reader["SecurityStamp"].ToString();
                entity.Email = reader["Email"].ToString();
                entity.PasswordHash = reader["PasswordHash"].ToString();
                entity.PhoneNumber = reader["PhoneNumber"].ToString();
                entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
                entity.FullName = reader["FullName"].ToString();
                entity.DisplayName = reader["DisplayName"].ToString();
                entity.Sex = reader["Sex"] == DBNull.Value ? 1 : Convert.ToInt32(reader["Sex"]);
                entity.Address = reader["Address"].ToString();
                entity.Avatar = reader["Avatar"].ToString();
                entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
                entity.Note = reader["Note"].ToString();
                entity.OTPType = reader["OTPType"].ToString();

                entity.TokenKey = reader["TokenKey"].ToString();
                entity.TokenCreatedDate = (reader["TokenCreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenCreatedDate"];
                entity.TokenExpiredDate = (reader["TokenExpiredDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenExpiredDate"];
                return entity;
            }

            return null;
        }

        private IdentityUser ParsingDataItem(IDataReader reader)
        {

            var entity = new IdentityUser();

            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.UserName = reader["UserName"].ToString();
            //entity.Email = reader["Email"].ToString();
            //entity.PasswordHash = reader["PasswordHash"].ToString();
            //entity.PhoneNumber = reader["PhoneNumber"].ToString();
            //entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
            //entity.FullName = reader["FullName"].ToString();
            entity.DisplayName = reader["DisplayName"].ToString();
            //entity.Sex = reader["Sex"] == DBNull.Value ? 1 : Convert.ToInt32(reader["Sex"]);
            //entity.Address = reader["Address"].ToString();
            entity.Avatar = reader["Avatar"].ToString();
            //entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
            //entity.Note = reader["Note"].ToString();
            //entity.OTPType = reader["OTPType"].ToString();

            return entity;
        }

        public ApiAuthUserLoginIdentity ApiLogin(IdentityUser identity)
        {           
            var sqlCmd = @"User_Login";
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", identity.UserName},
                {"@Password", identity.PasswordHash},
                {"@Domain", identity.Domain }
            };

            ApiAuthUserLoginIdentity info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ApiParsingLoginDataFromReader(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Login. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public ApiAuthUserLoginIdentity ApiLoginWith(IdentityUser identity)
        {
            var sqlCmd = @"User_LoginWith";
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", identity.UserName},
                {"@SocialProvider", identity.SocialProvider},
                {"@Domain", identity.Domain },
                {"@Email", identity.Email },
                {"@DisplayName", identity.DisplayName },
                {"@Avatar", identity.Avatar },
                {"@AppCode", identity.AppCode }
            };

            ApiAuthUserLoginIdentity info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {                       
                        if (reader.Read())
                        {
                            //Base info
                            info = ApiParsingLoginDataFromReader(reader);

                            info.IsNew = Utils.ConvertToInt32(reader["IsNew"]);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to Login With. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public bool UpdateAvatar(IdentityUser identity)
        {
            var sqlCmd = @"User_UpdateAvatar";
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", identity.Id},
                {"@Avatar", identity.Avatar}
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
                var strError = "Failed to Login With. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public int ResendEmailActive(IdentityActiveAccount info)
        {
            var sqlCmd = @"User_ResendEmailActive";
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

        public int SendEmailRecoverPassword(IdentityUser info, string hasData)
        {
            var sqlCmd = @"User_SendEmailRecoverPassword";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserName", info.UserName },
                {"@HashingData", hasData }
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

        public int RecoverPassword(IdentityUser info, string hashData)
        {
            var sqlCmd = @"User_RecoverPasswordByNewPwd";
            var result = 0;

            var parameters = new Dictionary<string, object>
            {
                {"@UserId", info.Id },
                {"@PasswordHash", info.PasswordHash },
                {"@HashingData", hashData }
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

        public bool ProvideTokenKeyForUser(UserTokenIdentity identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey},
                    {"@Method", identity.Method },
                    {"@Domain", identity.Domain }
                };

            try
            {
                var sqlCmd = @"User_ProvideTokenKey";

                var result = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return result;

            }
            catch (Exception ex)
            {
                var strError = "Failed to ProvideTokenKeyForUser. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public bool WriteUserLog(UserLogIdentity identity, bool allowToWrite = true)
        {
            var mc = new ManageConnection(_connectionString);
            if (!allowToWrite)
            {
                return false;
            }

            var parameters = new Dictionary<string, object>
                {
                    {"@ActionType", identity.ActionType},
                    {"@UserIp", identity.UserIp },
                    {"@UserAgent", identity.UserAgent},
                    {"@Method", identity.Method },
                    {"@Domain", identity.Domain },
                    {"@ActionDesc", identity.ActionDesc },
                    {"@RawData", identity.RawData }
                };

            try
            {
                var sqlCmd = @"User_WriteLog_Action";

                var result = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return result;

            }
            catch (Exception ex)
            {
                var strError = "Failed to WriteUserLog. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        private ApiAuthUserLoginIdentity ApiParsingLoginDataFromReader(IDataReader reader)
        {
            var entity = new ApiAuthUserLoginIdentity();

            entity.Id = Utils.ConvertToInt32(reader["Id"]);
            entity.UserName = reader["UserName"].ToString();
            entity.Email = reader["Email"].ToString();
            entity.PhoneNumber = reader["PhoneNumber"].ToString();
            entity.PasswordHash = reader["PasswordHash"].ToString();
            entity.FullName = reader["FullName"].ToString();
            entity.DisplayName = reader["DisplayName"].ToString();
            entity.Avatar = reader["Avatar"].ToString();
            entity.LoginDurations = (reader["LoginDurations"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["LoginDurations"]);
            entity.TokenKey = reader["TokenKey"].ToString();
            entity.TokenCreatedDate = (reader["TokenCreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenCreatedDate"];
            entity.TokenExpiredDate = (reader["TokenExpiredDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenExpiredDate"];

            entity.OTPType = reader["OTPType"].ToString();
            entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
            entity.Sex = Convert.ToInt32(reader["Sex"]);
            entity.Address = reader["Address"].ToString();
            entity.IDCard = reader["IDCard"].ToString();
            entity.Note = reader["Note"].ToString();

            entity.EmailConfirmed = Utils.ConvertToInt32(reader["EmailConfirmed"]);
            entity.PhoneNumberConfirmed = Utils.ConvertToInt32(reader["PhoneNumberConfirmed"]);
            entity.SocialProviderId = Utils.ConvertToInt32(reader["SocialProviderId"]);

            return entity;
        }

        public WebAuthUserLoginIdentity WebLogin(IdentityUser identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserName", identity.UserName},
                    {"@Password", identity.PasswordHash},
                    {"@Domain", identity.Domain}
                };

            try
            {
                var sqlCmd = @"User_Login";

                var entity = (WebAuthUserLoginIdentity)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, WebParsingLoginDataFromReader, parameters.ToSqlParams());
                return entity;

            }
            catch (Exception ex)
            {
                var strError = "Failed to Login. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        private WebAuthUserLoginIdentity WebParsingLoginDataFromReader(SqlDataReader reader)
        {
            if (reader.Read())
            {
                var entity = new WebAuthUserLoginIdentity();

                entity.Id = Utils.ConvertToInt32(reader["Id"]);
                entity.UserName = reader["UserName"].ToString();
                entity.Email = reader["Email"].ToString();
                entity.PasswordHash = reader["PasswordHash"].ToString();
                entity.PhoneNumber = reader["PhoneNumber"].ToString();
                entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
                entity.FullName = reader["FullName"].ToString();
                entity.DisplayName = reader["DisplayName"].ToString();
                entity.Avatar = reader["Avatar"].ToString();
                entity.LoginDurations = (reader["LoginDurations"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["LoginDurations"]);
                entity.TokenKey = reader["TokenKey"].ToString();
                entity.TokenCreatedDate = (reader["TokenCreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenCreatedDate"];
                entity.TokenExpiredDate = (reader["TokenExpiredDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenExpiredDate"];

                entity.OTPType = reader["OTPType"].ToString();
                return entity;
            }

            return null;
        }

        public UserTokenIdentity GetCurrentTokenKeyByUser(int userId)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", userId},
                };

            try
            {
                var sqlCmd = @"User_GetCurrentTokenKey";

                var result = (UserTokenIdentity)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserTokenDataFromReader, parameters.ToSqlParams());
                return result;

            }
            catch (Exception ex)
            {
                var strError = "Failed to GetCurrentTokenKeyByUser. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        private UserTokenIdentity ParsingUserTokenDataFromReader(SqlDataReader reader)
        {
            if (reader.Read())
            {
                var entity = new UserTokenIdentity();

                entity.UserId = Utils.ConvertToInt32(reader["UserId"]);
                entity.TokenKey = reader["TokenKey"].ToString();
                entity.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
                entity.ExpiredDate = reader["ExpiredDate"] == DBNull.Value ? null : (DateTime?)reader["ExpiredDate"];
                entity.Method = reader["Method"].ToString();
                entity.Domain = reader["Domain"].ToString();

                return entity;
            }

            return null;
        }

        public int RefreshTokenKey(UserTokenIdentity identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.UserId},
                    {"@TokenKey", identity.TokenKey},
                    {"@Domain", identity.Domain },
                    {"@Time", identity.CreatedDate }
                };

            try
            {
                var sqlCmd = @"User_RefreshTokenKey";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to RefreshTokenKey. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int CreateOTPCode(UserCodeIdentity identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@TranId", identity.Id },
                    {"@UserId", identity.UserId},
                    {"@Code", identity.Code},
                    {"@Time", identity.CreatedDate},
                    {"@TokenKey", identity.TokenKey},
                    {"@Action", identity.Action },
                    {"@TargetData", identity.TargetData },
                    {"@OTPCodeType", identity.CodeType }
            };

            try
            {
                var sqlCmd = @"User_CreateOTPCode";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to CreateOTPCode. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public UserCodeIdentity GetCurrentOTP(UserCodeIdentity identity)
        {
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.UserId},
                    {"@Action", identity.Action }
            };

            var sqlCmd = @"User_GetCurrentOTP";
            UserCodeIdentity info = null;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = new UserCodeIdentity();
                            info.Id = reader["Id"].ToString();
                            info.UserId = Utils.ConvertToInt32(reader["UserId"]);
                            info.Code = reader["Code"].ToString();
                            info.CodeType = reader["CodeType"].ToString();
                            info.TargetData = reader["TargetData"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetCurrentOTP. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public int VerifyOTPCode(UserCodeIdentity identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.UserId},
                    {"@Code", identity.Code},
                    //{"@CodeType", identity.CodeType },
                    {"@Time", identity.CreatedDate},
                    {"@TokenKey", identity.TokenKey},
                    {"@Action", identity.Action }
            };

            try
            {
                var sqlCmd = @"User_VerifyOTPCode";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to VerifyOTPCode. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int ApiRegister(IdentityUser identity, ref int code)
        {
            var sqlCmd = @"User_ApiRegister";
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", identity.UserName},
                {"@PasswordHash", identity.PasswordHash},
                {"@Email", identity.Email },
                {"@Birthday", identity.Birthday},
                {"@Sex", identity.Sex},
                {"@Address", identity.Address },
                {"@FullName", identity.FullName},
                {"@DisplayName", identity.DisplayName },
                {"@IDCard", identity.IDCard},
                {"@PhoneNumber", identity.PhoneNumber },
                {"@Note", identity.Note },
                {"@OTPType", identity.OTPType },
                {"@SecurityStamp", identity.SecurityStamp },
                {"@AppCode", identity.AppCode },
            };

            var newId = 0;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        //return code
                        if (reader.Read())
                        {
                            code = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                newId = Utils.ConvertToInt32(reader[0]);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_ApiRegister. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int ChangePassword(IdentityUser identity, int passwordLevel)
        {
            var mc = new ManageConnection(_connectionString);
            identity.PasswordHash = (passwordLevel == 1) ? identity.PasswordHash : identity.PasswordHash2;

            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.Id},
                    {"@NewPassword", identity.NewPassword},
                    {"@OldPassword", identity.PasswordHash},
                    {"@Level", passwordLevel},
                    {"@TokenKey", identity.TokenKey}
            };

            try
            {
                var sqlCmd = @"User_ChangePassword";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to ChangePassword. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int UpdateProfile(IdentityUser identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.Id},
                    {"@TokenKey", identity.TokenKey},
                    {"@FullName", identity.FullName},
                    {"@Email", identity.Email},
                    {"@PhoneNumber", identity.PhoneNumber},
                    {"@DisplayName", identity.DisplayName},
                    {"@Birthday", identity.Birthday},
                    {"@Sex", identity.Sex},
                    {"@Address", identity.Address},
                    {"@Note", identity.Note},
                    {"@Avatar", identity.Avatar }
            };

            try
            {
                var sqlCmd = @"User_UpdateProfile";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to UpdateProfile. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int WebRegister(IdentityUser identity, ref int code)
        {
            var sqlCmd = @"User_WebRegister";
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", identity.UserName},
                {"@PasswordHash", identity.PasswordHash},
                {"@Email", identity.Email },
                {"@Birthday", identity.Birthday},
                {"@Sex", identity.Sex},
                {"@Address", identity.Address },
                {"@FullName", identity.FullName},
                {"@DisplayName", identity.DisplayName },
                {"@IDCard", identity.IDCard},
                {"@PhoneNumber", identity.PhoneNumber },
                {"@Note", identity.Note },
                {"@OTPType", identity.OTPType }
            };

            var newId = 0;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        //return code
                        if (reader.Read())
                        {
                            code = Utils.ConvertToInt32(reader[0]);
                        }

                        if (reader.NextResult())
                        {
                            if (reader.Read())
                            {
                                newId = Utils.ConvertToInt32(reader[0]);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_WebRegister. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public int ChangeAuthMethod(IdentityUser identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.Id},
                    {"@TokenKey", identity.TokenKey},
                    {"@OTPType", identity.OTPType}
            };

            try
            {
                var sqlCmd = @"User_ChangeAuthMethod";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to ChangePassword. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int CheckPwd2IsValid(IdentityUser identity)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", identity.Id},
                    {"@TokenKey", identity.TokenKey},
                    {"@Pwd", identity.PasswordHash2}
            };

            try
            {
                var sqlCmd = @"User_CheckPwd2IsValid";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to CheckPwd2IsValid. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int RecoverPasswordStep1(int userId, string pwd)
        {

            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", userId},
                    {"@Pwd", pwd}
            };

            try
            {
                var sqlCmd = @"User_RecoverPasswordStep1";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to RecoverPasswordStep1. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public int RecoverPasswordStep2(int userId, string pwdType)
        {

            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
                {
                    {"@UserId", userId},
                    {"@PwdType", pwdType}
            };

            try
            {
                var sqlCmd = @"User_RecoverPasswordStep2";

                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
                return Convert.ToInt32(result);

            }
            catch (Exception ex)
            {
                var strError = "Failed to RecoverPasswordStep2. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            finally
            {
                mc.Dispose();
            }
        }

        public IdentityUser GetUserProfile(int id)
        {
            var sqlCmd = @"User_GetProfile";
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", id}
            };

            IdentityUser info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractUserBaseData(reader);

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetProfile. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public IdentityUser GetUserByEmail(string email)
        {
            var sqlCmd = @"User_GetByEmail";
            var parameters = new Dictionary<string, object>
            {
                {"@Email", email}
            };

            IdentityUser info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractUserBaseData(reader);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetByEmail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return info;
        }

        public int ActiveAccountByEmail(IdentityActiveAccount info)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
            {
                {"@UserName", info.UserName },
                {"@HashingData", info.HashingData }
            };

            var sqlCmd = "User_ActiveAccountByEmail";
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
                var strError = "Failed to User_ActiveAccountByEmail. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }

        public int ActiveAccountByOTP(IdentityActiveAccount info)
        {
            var mc = new ManageConnection(_connectionString);
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", info.UserId },
                {"@Code", info.OTPCode }
            };

            var sqlCmd = "User_ActiveAccountByOTP";
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
                var strError = "Failed to User_ActiveAccountByOTP. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }
    }
}
