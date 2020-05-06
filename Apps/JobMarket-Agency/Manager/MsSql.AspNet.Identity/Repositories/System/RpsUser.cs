using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Manager.SharedLibs;

namespace MsSql.AspNet.Identity.Repositories
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
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        #region --- User ----

        public List<IdentityUser> GetByPage(dynamic filter)
        {
            int offset = (filter.page_index - 1) * filter.page_size;
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword},
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", filter.page_size}
            };

            var sqlCmd = @"User_GetByPage";

            List<IdentityUser> myList = new List<IdentityUser>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            //Get common info
                            var entity = ExtractUserItem(reader);

                            myList.Add(entity);
                        }                        
                    }
                }

            }
            catch (Exception ex)
            {
                var strError = "Failed to User_GetByPage. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return myList;
        }

        public IdentityUser GetById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
            };

            var sqlCmd = @"User_GetById";

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
                            info = ExtractUserItem(reader);
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

        public IdentityUser GetDetailById(int id)
        {
            var parameters = new Dictionary<string, object>
            {
                {"@Id", id}
            };

            var sqlCmd = @"User_GetDetailById";

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
                            info = ExtractUserItem(reader);
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

        private List<IdentityUser> ParsingUserData(IDataReader reader)
        {
            List<IdentityUser> listData = new List<IdentityUser>();
            while (reader.Read())
            {
               // Get common info
                var entity = ExtractUserItem(reader);

                listData.Add(entity);
            }

            return listData;
        }

        private IdentityUser ExtractUserItem(IDataReader reader)
        {
            var user = new IdentityUser();

            //Seperate properties
            user.Id = reader["Id"].ToString();
            user.StaffId = Utils.ConvertToInt32(reader["StaffId"]);
            user.Email = reader["Email"].ToString();            
            user.PhoneNumber = reader["PhoneNumber"].ToString();
            user.UserName = reader["UserName"].ToString();
            user.FullName = reader["FullName"].ToString();

            if (reader.HasColumn("TotalCount"))
                user.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

            return user;
        }

        #endregion
    }
}
