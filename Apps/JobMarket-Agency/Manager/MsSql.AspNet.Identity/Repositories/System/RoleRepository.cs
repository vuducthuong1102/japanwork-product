using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using Manager.SharedLibs;

namespace MsSql.AspNet.Identity.Repositories
{
    public class RoleRepository
    {
        private readonly string _connectionString;
        public RoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int Insert(IdentityRole role)
        {
            var result = 0;
            var parameters = new Dictionary<string, object>
                {
                    {"@Name", role.Name},
                    {"@UserId", role.UserId},
                    {"@Agency_Id", role.Agency_Id}
                };

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,
                  @"Role_Insert", parameters))
                {
                    if (reader.Read())
                    {
                        result = Utils.ConvertToInt32(reader[0]);
                    }
                }
                //MsSqlHelper.ExecuteNonQuery(conn, @"INSERT INTO AspNetRoles (Id, Name,UserId,Agency_Id) VALUES (@id, @name,@UserId,@Agency_Id)", parameters);
            }
            return result;
        }

        public void Delete(string roleId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@id", roleId}
                };

                MsSqlHelper.ExecuteNonQuery(conn, @"DELETE FROM AspNetRoles WHERE Id = @id and lower(Name) <> lower('admin')", parameters);
            }
        }

        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            IdentityRole role = null;

            if (roleName != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;

        }

        private string GetRoleName(string roleId)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>
                {
                    {"@id", roleId}
                };

                var result = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, @"SELECT Name FROM AspNetRoles WHERE Id = @id", parameters);
                if (result != null)
                {
                    return result.ToString();
                }
            }
            return null;
        }

        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);
            IdentityRole role = null;

            if (roleId != null)
            {
                role = new IdentityRole(roleName, roleId);
            }

            return role;
        }

        private string GetRoleId(string roleName)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>()
                {
                    {"@name", roleName}
                };

                var result = MsSqlHelper.ExecuteScalar(conn, CommandType.Text, @"SELECT Id FROM AspNetRoles WHERE Name = @name", parameters);
                if (result != null)
                {
                    return result.ToString();
                }
            }

            return null;
        }

        public IQueryable<IdentityRole> GetRoleByUserId(int agency_id)
        {
            List<IdentityRole> roles = new List<IdentityRole>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var parameters = new Dictionary<string, object>()
                {
                    {"@agency_id", agency_id}
                };

                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, Name,UserId FROM AspNetRoles WHERE agency_id=@agency_id", parameters);

                while (reader.Read())
                {
                    var role = new IdentityRole();

                    role.Id = reader["ID"].ToString();
                    role.Name = reader["NAME"].ToString();
                    role.UserId = reader["UserId"].ToString();

                    roles.Add(role);
                }
            }

            return roles.AsQueryable<IdentityRole>();
        }

        public int Update(IdentityRole role)
        {
            var parameters = new Dictionary<string, object>{
                { "@name", role.Name},
                { "@id", role.Id},
                { "@Agency_id", role.Agency_Id}
            };
            var result = 0;
            //using (var conn = new SqlConnection(_connectionString))
            //{
            //    var parameters = new Dictionary<string, object>

            //    MsSqlHelper.ExecuteNonQuery(conn, @"UPDATE AspNetRoles SET Name = @name WHERE Id = @id", parameters);
            //}

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure,
                  @"Role_Update", parameters))
                {
                    if (reader.Read())
                    {
                        result = Utils.ConvertToInt32(reader[0]);
                    }
                }
                //MsSqlHelper.ExecuteNonQuery(conn, @"INSERT INTO AspNetRoles (Id, Name,UserId,Agency_Id) VALUES (@id, @name,@UserId,@Agency_Id)", parameters);
            }
            return result;
        }



        /// <summary>
        /// Get the all Roles
        /// </summary>
        /// <returns>IdentityRole</returns>
        public IQueryable<IdentityRole> GetAll()
        {
            List<IdentityRole> roles = new List<IdentityRole>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var reader = MsSqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, Name FROM AspNetRoles", null);

                while (reader.Read())
                {
                    var role = new IdentityRole();

                    role.Id = reader["ID"].ToString();
                    role.Name = reader["NAME"].ToString();
                    roles.Add(role);
                }
            }

            return roles.AsQueryable<IdentityRole>();
        }

    }
}
