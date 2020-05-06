
using Manager.SharedLibs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MsSql.AspNet.Identity.Repositories
{
    public class IdentityRepository
    {
        private readonly string _connectionString;
        public IdentityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<IdentityUser> FilterUserList(string keyword, int parentId, string roleId, bool isLocked, int page, int pageSize)
        {
            List<IdentityUser> users = new List<IdentityUser>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    int offset = (page - 1) * pageSize;
                    //if (email == null) email = string.Empty;
                    //if (roleId == null) roleId = string.Empty;
                    var parameters = new Dictionary<string, object>
                {
                    {"@Keyword", keyword},
                    {"@RoleId", roleId},
                    {"@LockedEnable", isLocked },
                    {"@Offset", offset},
                    {"@PageSize", pageSize},
                    {"@ParentId", parentId}
                };
                    var query = @"User_GetPageByParent";

                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, query, parameters);
                    while (reader.Read())
                    {
                        var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
                        user = ParsingUserDataFromReader(reader);
                        users.Add(user);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            
            return users;
        }
        public int CountAll(string email, string roleId, bool isLocked, int parentId)
        {
            var total = 0;
            if (email == null) email = string.Empty;
            var parameters = new Dictionary<string, object>
                {
                    {"@Email", email},
                    {"@RoleId", roleId},
                    {"@ParentId", parentId},
                    {"@lockedEnable", (isLocked)? 1:0 }
                };
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"SELECT count(*) as CountNum FROM (SELECT DISTINCT a.Id
                    FROM AspNetUsers as a
                    LEFT JOIN AspNetUserRoles b on a.Id = b.UserId 
                    Where a.Email LIKE CONCAT('%', @Email , '%')
                    AND ParentId=@ParentId
                    {0}
                    AND (@RoleId IS NULL OR b.RoleId = @RoleId) ) as tbl_count                  
                    ";
                string appendQuery = "";
                if (isLocked)
                {
                    appendQuery = "AND (a.LockoutEnabled = @lockedEnable AND a.LockoutEndDateUtc IS NOT NULL AND a.LockoutEndDateUtc > GETDATE())";

                }
                query = string.Format(query, appendQuery);
                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                if (reader.Read())
                {
                    total = Convert.ToInt32(reader["CountNum"]);
                }
            }
            return total;
        }

        public IdentityUser GetById(string userId)
        {
            var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@Id", userId}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT * FROM AspNetUsers WHERE Id=@Id", parameters);
                while (reader.Read())
                {
                    user = ParsingUserDataFromReader(reader);
                }

            }
            return user;
        }

        public IdentityUser GetByStaffId(int staffId)
        {
            var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var parameters = new Dictionary<string, object>
                {
                    {"@StaffId", staffId}
                };

                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                        @"SELECT * FROM AspNetUsers WHERE StaffId=@StaffId", parameters);
                    while (reader.Read())
                    {
                        user = ParsingUserDataFromReader(reader);
                    }

                }
            }
            catch (Exception ex)
            {
            }

            return user;
        }

        public IdentityUser ParsingUserDataFromReader(IDataReader reader)
        {
            var user = (IdentityUser)Activator.CreateInstance(typeof(IdentityUser));
            user.Id = reader["Id"].ToString();
            user.Email = reader["Email"].ToString();
            user.EmailConfirmed = Convert.ToBoolean(reader["EmailConfirmed"]);
            user.PhoneNumber = reader["PhoneNumber"].ToString();
            user.PhoneNumberConfirmed = Convert.ToBoolean(reader["PhoneNumberConfirmed"]);
            user.TwoFactorEnabled = Convert.ToBoolean(reader["TwoFactorEnabled"]);
            user.LockoutEndDateUtc = reader["LockoutEndDateUtc"] == DBNull.Value ? null : (DateTime?)reader["LockoutEndDateUtc"];
            user.LockoutEnabled = Convert.ToBoolean(reader["LockoutEnabled"]);
            user.AccessFailedCount = Convert.ToInt32(reader["AccessFailedCount"]);
            user.UserName = reader["UserName"].ToString();
            user.FullName = reader["FullName"].ToString();


            user.CreatedDateUtc = (DateTime)reader["CreatedDateUtc"];
            user.StaffId = Convert.ToInt32(reader["StaffId"]);
            user.ParentId = Convert.ToInt32(reader["ParentId"]);

            return user;
        }
        public List<IdentityUser> ParsingListUserDataFromReader(IDataReader reader)
        {
            List<IdentityUser> listData = listData = new List<IdentityUser>();
            while (reader.Read())
            {

                var record = new IdentityUser();
                record.Id = reader["Id"].ToString();
                record.UserName = reader["UserName"].ToString();
                record.Email = reader["Email"].ToString();
                record.StaffId = Utils.ConvertToInt32(reader["StaffId"]);
                record.ParentId = Utils.ConvertToInt32(reader["ParentId"]);
                record.FullName = reader["FullName"].ToString();

                listData.Add(record);
            }
            return listData;
        }
        public List<IdentityUser> GetListUser(int parentId)
        {
            var conn = new SqlConnection(_connectionString);

            var sqlCmd = @"Users_GetListUser";
            List<IdentityUser> listData = null;
            var parameters = new Dictionary<string, object>
                {
                    {"@ParentId", parentId}
                };
            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingListUserDataFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute Users_GetListUser. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        public List<IdentityUser> GetListByParentId(int parentId)
        {
            var conn = new SqlConnection(_connectionString);
            var sqlCmd = @"Users_GetListByParentId";
            var parameters = new Dictionary<string, object>
            {
                {"@ParentId", parentId}
            };
            List<IdentityUser> listData = null;
            try
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                {
                    listData = ParsingListUserDataFromReader(reader);
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute GetListByParentId. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }
        public void UpdateProfile(IdentityUser user)
        {
            var conn = new SqlConnection(_connectionString);
            var parameters = new Dictionary<string, object>
            {
                {"@UserId", user.Id},
                {"@Fullname", user.FullName }
            };

            var sqlCmd = @"UPDATE aspnetusers SET FullName = @Fullname WHERE 1=1 AND Id = @UserId";
            try
            {
                MsSqlHelper.ExecuteNonQuery(conn, CommandType.Text, sqlCmd, parameters);
            }
            catch (Exception ex)
            {
                var strError = "Failed to UPDATE user profile. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
        }
    }
}
