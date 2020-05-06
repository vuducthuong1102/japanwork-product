//using System;
//using System.Configuration;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.SqlClient;
//using ApiJobMarket.ShareLibs.Exceptions;
//using ApiJobMarket.DB.Sql.Entities;
//using ApiJobMarket.ShareLibs;
//using Newtonsoft.Json;

//namespace ApiJobMarket.DB.Sql.Repositories
//{
//    public class RpsUser
//    {
//        private readonly string _connectionString;

//        public RpsUser(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public RpsUser()
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
//        }

//        public List<IdentityUser> GetTopTraveller(int userId, string token, int numberTop)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@NumberTop", numberTop },
//                    {"@UserId", userId},
//                    {"@TokenKey", token }
//                };

//            List<IdentityUser> myList = null;
//            var sqlCmd = "User_GetTopTraveller";
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        myList = ExtractListUser(reader);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetTopTraveler. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//            return myList;
//        }

//        private List<IdentityUser> ExtractListUser(IDataReader reader)
//        {
//            List<IdentityUser> myList = new List<IdentityUser>();
//            while (reader.Read())
//            {
//                var entity = new IdentityUser();
//                entity.Id = Utils.ConvertToInt32(reader["UserId"]);
//                //Extends
//                entity.MetaData.FollowerCount = Utils.ConvertToInt32(reader["FollowerCount"]);
//                entity.MetaData.PostCount = Utils.ConvertToInt32(reader["PostCount"]);

//                myList.Add(entity);
//            }
//            return myList;
//        }

//        public List<int> GetListFollower(int userId, int ownerId, int pageIndex,int pageSize)
//        {
//            var mc = new ManageConnection(_connectionString);
//           var offset = (pageIndex - 1) * pageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@OwnerId", ownerId},
//                    {"@Offset", offset},
//                    {"@PageSize", pageSize}
//                };

//            var sqlCmd = @"User_GetFollower";

//            List<int> myList = new List<int>();
          
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while(reader.Read())
//                        {
//                            myList.Add(Utils.ConvertToInt32(reader["UserId"]));
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetFollower. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            return myList;
//        }

//        public List<int> GetListFollowing(int userId, int ownerId, int pageIndex, int pageSize)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var offset = (pageIndex - 1) * pageSize;
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@OwnerId", ownerId},
//                    {"@Offset", offset},
//                    {"@PageSize", pageSize}
//                };

//            var sqlCmd = @"User_GetFollowing";

//            List<int> myList = new List<int>();

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            myList.Add(Utils.ConvertToInt32(reader["UserId"]));
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetFollowing. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            return myList;
//        }

//        public List<int> CheckFollow(int userId, List<int> listUserId)
//        {
//            var strList = string.Join(",", listUserId);
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@ListUserId", strList}
//                };

//            var sqlCmd = @"User_Sp_CheckFollow";

//            List<int> myList = new List<int>();

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            myList.Add(Utils.ConvertToInt32(reader["UserId"]));
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_CheckFollow. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            return myList;
//        }

//        public List<IdentityUserData> GetDataByList(int userId, List<int> listUserId)
//        {
//            var strList = string.Join(",", listUserId);
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@ListUserId", strList}
//                };

//            var sqlCmd = @"User_GetData";

//            List<IdentityUserData> myList = new List<IdentityUserData>();

//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        while (reader.Read())
//                        {
//                            var item = new IdentityUserData();

//                            item.UserId = Utils.ConvertToInt32(reader["UserId"]);
//                            item.FollowerCount = Utils.ConvertToInt32(reader["FollowerCount"]);

//                            myList.Add(item);
//                        }
//                        return myList;
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_CheckFollow. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public IdentityUser GetUserById(int userId)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId}
//                };

//            try
//            {
//                var sqlCmd = @"User_GetById";

//                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUserById. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public IdentityUser GetUserByInfo(string userInfo)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@Info", userInfo}
//                };

//            try
//            {
//                var sqlCmd = @"User_GetByInfo";

//                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingBaseUserDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUserByInfo. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public IdentityUser ApiGetUserInfo(int userId, string token)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@TokenKey", token }
//                };

//            try
//            {
//                var sqlCmd = @"User_ApiGetInfo";

//                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to ApiGetUserInfo. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int Follow(IdentityUserAction identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.UserId },
//                    {"@UserActionId", identity.UserActionId},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Status", identity.Status },
//                    {"@ActionType", identity.ActionType }
//                };
//            var sqlCmd = "User_Follow";
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_Follow. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public int UnFollow(IdentityUserAction identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.UserId },
//                    {"@UserActionId", identity.UserActionId},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Status", identity.Status },
//                    {"@ActionType", identity.ActionType }
//                };
//            var sqlCmd = "User_UnFollow";
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_UnFollow. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public IdentityUser GetUserByUserName(string userName)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserName", userName}
//                };

//            try
//            {
//                var sqlCmd = @"User_GetByUserName";

//                var entity = (IdentityUser)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUserByUserName. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        private IdentityUser ExtractUserBaseData(IDataReader reader)
//        {
//            var entity = new IdentityUser();
//            entity.Id = Utils.ConvertToInt32(reader["Id"]);
//            entity.UserName = reader["UserName"].ToString();
//            entity.Email = reader["Email"].ToString();
//            entity.PhoneNumber = reader["PhoneNumber"].ToString();
//            entity.FullName = reader["FullName"].ToString();
//            entity.DisplayName = reader["DisplayName"].ToString();
//            entity.Avatar = reader["Avatar"].ToString();

//            entity.OTPType = reader["OTPType"].ToString();
//            entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
//            entity.Sex = Convert.ToInt32(reader["Sex"]);
//            entity.Address = reader["Address"].ToString();
//            entity.IDCard = reader["IDCard"].ToString();
//            entity.Note = reader["Note"].ToString();

//            return entity;
//        }

//        private IdentityUser ParsingBaseUserDataFromReader(SqlDataReader reader)
//        {
//            if (reader.Read())
//            {
//                var entity = new IdentityUser();

//                entity.Id = Utils.ConvertToInt32(reader["Id"]);
//                entity.UserName = reader["UserName"].ToString();
//                entity.Email = reader["Email"].ToString();
//                entity.PasswordHash = reader["PasswordHash"].ToString();
//                entity.PhoneNumber = reader["PhoneNumber"].ToString();
//                entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
//                entity.FullName = reader["FullName"].ToString();
//                entity.DisplayName = reader["DisplayName"].ToString();
//                entity.Sex = reader["Sex"] == DBNull.Value ? 1 : Convert.ToInt32(reader["Sex"]);
//                entity.Address = reader["Address"].ToString();
//                entity.Avatar = reader["Avatar"].ToString();
//                entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
//                entity.Note = reader["Note"].ToString();
//                entity.OTPType = reader["OTPType"].ToString();

//                return entity;
//            }

//            return null;
//        }

//        private IdentityUser ParsingUserDataFromReader(SqlDataReader reader)
//        {
//            if (reader.Read())
//            {
//                var entity = new IdentityUser();

//                entity.Id = Utils.ConvertToInt32(reader["Id"]);
//                entity.UserName = reader["UserName"].ToString();
//                entity.Email = reader["Email"].ToString();
//                entity.PasswordHash = reader["PasswordHash"].ToString();
//                entity.PhoneNumber = reader["PhoneNumber"].ToString();
//                entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
//                entity.FullName = reader["FullName"].ToString();
//                entity.DisplayName = reader["DisplayName"].ToString();
//                entity.Sex = reader["Sex"] == DBNull.Value ? 1 : Convert.ToInt32(reader["Sex"]);
//                entity.Address = reader["Address"].ToString();
//                entity.Avatar = reader["Avatar"].ToString();
//                entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
//                entity.Note = reader["Note"].ToString();
//                entity.OTPType = reader["OTPType"].ToString();

//                entity.TokenKey = reader["TokenKey"].ToString();
//                entity.TokenCreatedDate = (reader["TokenCreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenCreatedDate"];
//                entity.TokenExpiredDate = (reader["TokenExpiredDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenExpiredDate"];
//                return entity;
//            }

//            return null;
//        }

//        public ApiAuthUserLoginIdentity ApiLogin(IdentityUser identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserName", identity.UserName},
//                    {"@Password", identity.PasswordHash},
//                    {"@Domain", identity.Domain }
//                };

//            try
//            {
//                var sqlCmd = @"User_Login";

//                var entity = (ApiAuthUserLoginIdentity)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ApiParsingLoginDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Login. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public ApiAuthUserLoginIdentity ApiLoginWith(IdentityUser identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserName", identity.UserName},
//                    {"@SocialProvider", identity.SocialProvider},
//                    {"@Domain", identity.Domain }
//                };

//            try
//            {
//                var sqlCmd = @"User_LoginWith";

//                var entity = (ApiAuthUserLoginIdentity)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ApiParsingLoginDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Login With. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public bool ProvideTokenKeyForUser(UserTokenIdentity identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Method", identity.Method },
//                    {"@Domain", identity.Domain }                    
//                };

//            try
//            {
//                var sqlCmd = @"User_ProvideTokenKey";

//                var result = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return result;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to ProvideTokenKeyForUser. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public bool WriteUserLog(UserLogIdentity identity, bool allowToWrite = true)
//        {
//            var mc = new ManageConnection(_connectionString);
//            if (!allowToWrite)
//            {
//                return false;
//            }

//            var parameters = new Dictionary<string, object>
//                {
//                    {"@ActionType", identity.ActionType},
//                    {"@UserIp", identity.UserIp },
//                    {"@UserAgent", identity.UserAgent},
//                    {"@Method", identity.Method },
//                    {"@Domain", identity.Domain },
//                    {"@ActionDesc", identity.ActionDesc },
//                    {"@RawData", identity.RawData }
//                };

//            try
//            {
//                var sqlCmd = @"User_WriteLog_Action";

//                var result = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return result;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to WriteUserLog. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        private ApiAuthUserLoginIdentity ApiParsingLoginDataFromReader(SqlDataReader reader)
//        {
//            if(reader.Read())
//            {
//                var entity = new ApiAuthUserLoginIdentity();

//                entity.Id = Utils.ConvertToInt32(reader["Id"]);
//                entity.UserName = reader["UserName"].ToString();
//                entity.Email = reader["Email"].ToString();
//                entity.PhoneNumber = reader["PhoneNumber"].ToString();
//                entity.PasswordHash = reader["PasswordHash"].ToString();
//                entity.FullName = reader["FullName"].ToString();
//                entity.DisplayName = reader["DisplayName"].ToString();
//                entity.Avatar = reader["Avatar"].ToString();
//                entity.LoginDurations = (reader["LoginDurations"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["LoginDurations"]);
//                entity.TokenKey = reader["TokenKey"].ToString();
//                entity.TokenCreatedDate = (reader["TokenCreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenCreatedDate"];
//                entity.TokenExpiredDate = (reader["TokenExpiredDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenExpiredDate"];

//                entity.OTPType = reader["OTPType"].ToString();
//                entity.Birthday = (reader["Birthday"] == DBNull.Value) ? null : (DateTime?)reader["Birthday"];
//                entity.Sex = Convert.ToInt32(reader["Sex"]);
//                entity.Address = reader["Address"].ToString();
//                entity.IDCard = reader["IDCard"].ToString();
//                entity.Note = reader["Note"].ToString();

//                entity.EmailConfirmed = Utils.ConvertToInt32(reader["EmailConfirmed"]);
//                entity.PhoneNumberConfirmed = Utils.ConvertToInt32(reader["PhoneNumberConfirmed"]);
//                entity.SocialProviderId = Utils.ConvertToInt32(reader["SocialProviderId"]);

//                return entity;
//            }

//            return null;
//        }

//        public WebAuthUserLoginIdentity WebLogin(IdentityUser identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserName", identity.UserName},
//                    {"@Password", identity.PasswordHash},
//                    {"@Domain", identity.Domain}
//                };

//            try
//            {
//                var sqlCmd = @"User_Login";

//                var entity = (WebAuthUserLoginIdentity)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, WebParsingLoginDataFromReader, parameters.ToSqlParams());
//                return entity;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to Login. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        private WebAuthUserLoginIdentity WebParsingLoginDataFromReader(SqlDataReader reader)
//        {
//            if (reader.Read())
//            {
//                var entity = new WebAuthUserLoginIdentity();

//                entity.Id = Utils.ConvertToInt32(reader["Id"]);
//                entity.UserName = reader["UserName"].ToString();
//                entity.Email = reader["Email"].ToString();
//                entity.PasswordHash = reader["PasswordHash"].ToString();
//                entity.PhoneNumber = reader["PhoneNumber"].ToString();
//                entity.CreatedDateUtc = reader["CreatedDateUtc"] == DBNull.Value ? null : (DateTime?)reader["CreatedDateUtc"];
//                entity.FullName = reader["FullName"].ToString();
//                entity.DisplayName = reader["DisplayName"].ToString();
//                entity.Avatar = reader["Avatar"].ToString();
//                entity.LoginDurations = (reader["LoginDurations"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["LoginDurations"]);
//                entity.TokenKey = reader["TokenKey"].ToString();
//                entity.TokenCreatedDate = (reader["TokenCreatedDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenCreatedDate"];
//                entity.TokenExpiredDate = (reader["TokenExpiredDate"] == DBNull.Value) ? null : (DateTime?)reader["TokenExpiredDate"];

//                entity.OTPType = reader["OTPType"].ToString();
//                return entity;
//            }

//            return null;
//        }

//        public UserTokenIdentity GetCurrentTokenKeyByUser(int userId)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                };

//            try
//            {
//                var sqlCmd = @"User_GetCurrentTokenKey";

//                var result = (UserTokenIdentity)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingUserTokenDataFromReader, parameters.ToSqlParams());
//                return result;

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetCurrentTokenKeyByUser. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }            
//        }

//        private UserTokenIdentity ParsingUserTokenDataFromReader(SqlDataReader reader)
//        {
//            if (reader.Read())
//            {
//                var entity = new UserTokenIdentity();

//                entity.UserId = Utils.ConvertToInt32(reader["UserId"]);
//                entity.TokenKey = reader["TokenKey"].ToString();
//                entity.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];
//                entity.ExpiredDate = reader["ExpiredDate"] == DBNull.Value ? null : (DateTime?)reader["ExpiredDate"];
//                entity.Method = reader["Method"].ToString();
//                entity.Domain = reader["Domain"].ToString();

//                return entity;
//            }

//            return null;
//        }

//        public int RefreshTokenKey(UserTokenIdentity identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.UserId},
//                    {"@TokenKey", identity.TokenKey},                   
//                    {"@Domain", identity.Domain },
//                    {"@Time", identity.CreatedDate }
//                };

//            try
//            {
//                var sqlCmd = @"User_RefreshTokenKey";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to RefreshTokenKey. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int CreateOTPCode(UserCodeIdentity identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@TranId", identity.Id },
//                    {"@UserId", identity.UserId},
//                    {"@Code", identity.Code},
//                    {"@Time", identity.CreatedDate},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Action", identity.Action },
//                    {"@TargetData", identity.TargetData },
//                    {"@OTPCodeType", identity.CodeType }
//            };

//            try
//            {
//                var sqlCmd = @"User_CreateOTPCode";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to CreateOTPCode. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int VerifyOTPCode(UserCodeIdentity identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.UserId},
//                    {"@Code", identity.Code},
//                    //{"@CodeType", identity.CodeType },
//                    {"@Time", identity.CreatedDate},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Action", identity.Action }
//            };

//            try
//            {
//                var sqlCmd = @"User_VerifyOTPCode";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to VerifyOTPCode. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int ApiRegister(IdentityUser identity, ref int code)
//        {
//            var sqlCmd = @"User_ApiRegister";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserName", identity.UserName},
//                {"@PasswordHash", identity.PasswordHash},
//                {"@Email", identity.Email },
//                {"@Birthday", identity.Birthday},
//                {"@Sex", identity.Sex},
//                {"@Address", identity.Address },
//                {"@FullName", identity.FullName},
//                {"@DisplayName", identity.DisplayName },
//                {"@IDCard", identity.IDCard},
//                {"@PhoneNumber", identity.PhoneNumber },
//                {"@Note", identity.Note },
//                {"@OTPType", identity.OTPType },
//                {"@SecurityStamp", identity.SecurityStamp }
//            };

//            var newId = 0;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        //return code
//                        if (reader.Read())
//                        {
//                            code = Utils.ConvertToInt32(reader[0]);
//                        }

//                        if (reader.NextResult())
//                        {
//                            if (reader.Read())
//                            {
//                                newId = Utils.ConvertToInt32(reader[0]);
//                            }
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_WebRegister. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return newId;
//        }

//        public int ChangePassword(IdentityUser identity, int passwordLevel)
//        {
//            var mc = new ManageConnection(_connectionString);
//            identity.PasswordHash = (passwordLevel == 1) ? identity.PasswordHash : identity.PasswordHash2;

//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.Id},
//                    {"@NewPassword", identity.NewPassword},
//                    {"@OldPassword", identity.PasswordHash},
//                    {"@Level", passwordLevel},
//                    {"@TokenKey", identity.TokenKey}
//            };
            
//            try
//            {
//                var sqlCmd = @"User_ChangePassword";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to ChangePassword. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int UpdateProfile(IdentityUser identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.Id},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@FullName", identity.FullName},
//                    {"@Email", identity.Email},
//                    {"@PhoneNumber", identity.PhoneNumber},
//                    {"@DisplayName", identity.DisplayName},
//                    {"@Birthday", identity.Birthday},
//                    {"@Sex", identity.Sex},
//                    {"@Address", identity.Address},
//                    {"@Note", identity.Note},
//                    {"@Avatar", identity.Avatar }
//            };

//            try
//            {
//                var sqlCmd = @"User_UpdateProfile";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to UpdateProfile. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int WebRegister(IdentityUser identity, ref int code)
//        {
//            var sqlCmd = @"User_WebRegister";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserName", identity.UserName},
//                {"@PasswordHash", identity.PasswordHash},
//                {"@Email", identity.Email },
//                {"@Birthday", identity.Birthday},
//                {"@Sex", identity.Sex},
//                {"@Address", identity.Address },
//                {"@FullName", identity.FullName},
//                {"@DisplayName", identity.DisplayName },
//                {"@IDCard", identity.IDCard},
//                {"@PhoneNumber", identity.PhoneNumber },
//                {"@Note", identity.Note },
//                {"@OTPType", identity.OTPType }
//            };

//            var newId = 0;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        //return code
//                        if (reader.Read())
//                        {
//                            code = Utils.ConvertToInt32(reader[0]);                           
//                        }

//                        if (reader.NextResult())
//                        {
//                            if (reader.Read())
//                            {
//                                newId = Utils.ConvertToInt32(reader[0]);
//                            }
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_WebRegister. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return newId;
//        }

//        public int ChangeAuthMethod(IdentityUser identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.Id},                   
//                    {"@TokenKey", identity.TokenKey},
//                    {"@OTPType", identity.OTPType}
//            };

//            try
//            {
//                var sqlCmd = @"User_ChangeAuthMethod";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to ChangePassword. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int CheckPwd2IsValid(IdentityUser identity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", identity.Id},
//                    {"@TokenKey", identity.TokenKey},
//                    {"@Pwd", identity.PasswordHash2}
//            };

//            try
//            {
//                var sqlCmd = @"User_CheckPwd2IsValid";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to CheckPwd2IsValid. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int RecoverPasswordStep1(int userId, string pwd)
//        {

//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@Pwd", pwd}
//            };

//            try
//            {
//                var sqlCmd = @"User_RecoverPasswordStep1";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to RecoverPasswordStep1. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public int RecoverPasswordStep2(int userId, string pwdType)
//        {
            
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//                {
//                    {"@UserId", userId},
//                    {"@PwdType", pwdType}
//            };

//            try
//            {
//                var sqlCmd = @"User_RecoverPasswordStep2";

//                var result = mc.ExecuteScalar(sqlCmd, CommandType.StoredProcedure, parameters.ToSqlParams());
//                return Convert.ToInt32(result);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to RecoverPasswordStep2. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }
//        }

//        public IdentityUser GetUserProfile(int id)
//        {
//            var sqlCmd = @"User_GetProfile";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserId", id}
//            };

//            IdentityUser info = null;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            info = ExtractUserBaseData(reader);

//                            //Extends
//                            //info.MetaData.FollowerCount = Utils.ConvertToInt32(reader["FollowerCount"]);
//                            //info.MetaData.PostCount = Utils.ConvertToInt32(reader["PostCount"]);
//                            //info.MetaData.FollowingCount = Utils.ConvertToInt32(reader["FollowingCount"]);
//                            //info.MetaData.MessageCount = Utils.ConvertToInt32(reader["MessageCount"]);
//                            //info.MetaData.LikePostCount = Utils.ConvertToInt32(reader["LikePostCount"]);
//                            //info.MetaData.PhotoCount = Utils.ConvertToInt32(reader["PhotoCount"]);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetProfile. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return info;
//        }

//        public IdentityUserData GetCounter(int id)
//        {
//            var sqlCmd = @"User_GetCounter";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserId", id}
//            };

//            IdentityUserData info = new IdentityUserData();
//            info.UserId = id;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            //Extends
//                            info.FollowerCount = Utils.ConvertToInt32(reader["FollowerCount"]);
//                            info.PostCount = Utils.ConvertToInt32(reader["PostCount"]);
//                            info.FollowingCount = Utils.ConvertToInt32(reader["FollowingCount"]);
//                            info.MessageCount = Utils.ConvertToInt32(reader["MessageCount"]);
//                            info.LikePostCount = Utils.ConvertToInt32(reader["LikePostCount"]);
//                            info.SavedPostCount = Utils.ConvertToInt32(reader["SavedPostCount"]);
//                            info.PhotoCount = Utils.ConvertToInt32(reader["PhotoCount"]);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetCounter. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return info;
//        }

//        public IdentityUserData GetFeedCounter(int id)
//        {
//            var sqlCmd = @"User_GetFeedCounter";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserId", id}
//            };

//            IdentityUserData info = new IdentityUserData();
//            info.UserId = id;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            //Extends
//                            info.FollowerCount = Utils.ConvertToInt32(reader["FollowerCount"]);
//                            info.NotificationCount = Utils.ConvertToInt32(reader["NotificationCount"]);
//                        }
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetFeedCounter. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return info;
//        }

//        public IdentityUser GetUserByEmail(string email)
//        {
//            var sqlCmd = @"User_GetByEmail";
//            var parameters = new Dictionary<string, object>
//            {
//                {"@Email", email}
//            };

//            IdentityUser info = null;
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
//                    {
//                        if (reader.Read())
//                        {
//                            info = ExtractUserBaseData(reader);
//                        }
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_GetByEmail. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }

//            return info;
//        }

//        public int ActiveAccountByEmail(IdentityActiveAccount info)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserName", info.UserName },
//                {"@HashingData", info.HashingData }
//            };

//            var sqlCmd = "User_ActiveAccountByEmail";
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_ActiveAccountByEmail. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }

//        public int ActiveAccountByOTP(IdentityActiveAccount info)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var parameters = new Dictionary<string, object>
//            {
//                {"@UserId", info.UserId },
//                {"@Code", info.OTPCode }
//            };

//            var sqlCmd = "User_ActiveAccountByOTP";
//            try
//            {
//                using (var conn = new SqlConnection(_connectionString))
//                {
//                    var result = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

//                    return Utils.ConvertToInt32(result);
//                }

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to User_ActiveAccountByOTP. Error: " + ex.Message;
//                throw new CustomSQLException(strError);
//            }
//        }
//    }
// }
