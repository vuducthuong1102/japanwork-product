using SingleSignOn.DB.Sql.Entities;
using SingleSignOn.ShareLibs;
using SingleSignOn.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SingleSignOn.DB.Sql.Repositories
{
    public class RpsSystemEmail
    {
        private readonly string _connectionString;

        public RpsSystemEmail(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSystemEmail()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["SingleSignOnDB"].ConnectionString;
        }

        #region  Common

        public List<IdentitySystemEmail> GetAll(IdentitySystemEmail filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"SystemEmail_GetAll";
            List<IdentitySystemEmail> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@Name", filter.Name },
                //{"@Code", filter.Code },
                //{"@Status", filter.Status },
                //{"@TuNgay", filter.FromDate },
                //{"@DenNgay", filter.ToDate },
                {"@Offset", offset},
                {"@PageSize", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListSystemEmailFromReader(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute SystemEmail_GetAll. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        private List<IdentitySystemEmail> ParsingListSystemEmailFromReader(IDataReader reader)
        {
            List<IdentitySystemEmail> listData = listData = new List<IdentitySystemEmail>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSystemEmailData(reader);

                //Extends information
                record.TotalCount = Utils.ConvertToInt32(reader["TotalCount"]);

                listData.Add(record);
            }

            return listData;
        }

        private IdentitySystemEmail ExtractSystemEmailData(IDataReader reader)
        {
            var record = new IdentitySystemEmail();

            //Seperate properties
            record.Id = Utils.ConvertToInt32(reader["Id"]);
            record.Subject = reader["Subject"].ToString();
            record.Body = reader["Body"].ToString();
            record.Sender = reader["Sender"].ToString();
            record.Receiver = reader["Receiver"].ToString();
            record.Action = reader["Action"].ToString();
            record.ReceiverId = Utils.ConvertToInt32(reader["ReceiverId"]);
            record.IsSent = Utils.ConvertToBoolean(reader["IsSent"], false);
            record.IsRead = Utils.ConvertToBoolean(reader["IsRead"], false);
            record.ReadDate = reader["ReadDate"] == DBNull.Value ? null : (DateTime?)reader["ReadDate"];
            record.CreatedDate = reader["CreatedDate"] == DBNull.Value ? null : (DateTime?)reader["CreatedDate"];

            return record;
        }

        public int Insert(IdentitySystemEmail identity)
        {
            //Common syntax           
            var sqlCmd = @"SystemEmail_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Subject", identity.Subject},
                {"@Body", identity.Body },
                {"@Sender", identity.Sender},
                {"@Receiver", identity.Receiver},
                {"@Action", identity.Action},
                {"@ReceiverId", identity.ReceiverId}
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
                var strError = "Failed to execute SystemEmail_Insert. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public bool Update(IdentitySystemEmail identity)
        {
            //Common syntax
            var sqlCmd = @"SystemEmail_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Subject", identity.Subject},
                {"@Body", identity.Body },
                {"@Sender", identity.Sender},
                {"@Receiver", identity.Receiver},
                {"@ReceiverId", identity.ReceiverId},
                {"@IsSent", identity.IsSent},
                {"@IsRead", identity.IsRead}
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
                var strError = "Failed to execute SystemEmail_Update. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public IdentitySystemEmail GetById(int Id)
        {
            var info = new IdentitySystemEmail();
            var sqlCmd = @"SystemEmail_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            info = ExtractSystemEmailData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute SystemEmail_GetById. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public IdentitySystemEmail GetEmailToResend(string receiverEmail, string action)
        {
            IdentitySystemEmail info = null;
            var sqlCmd = @"SystemEmail_GetEmailToResend";

            var parameters = new Dictionary<string, object>
            {
                {"@Receiver", receiverEmail},
                {"@Action", action}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractSystemEmailData(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute SystemEmail_GetEmailToResend. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }
            return info;
        }

        public bool Delete(int Id)
        {
            //Common syntax            
            var sqlCmd = @"SystemEmail_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Id", Id},
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
                var strError = "Failed to execute SystemEmail_Delete. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return true;
        }

        public List<IdentitySystemEmail> GetList(string keyword)
        {
            //Common syntax            
            var sqlCmd = @"SystemEmail_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@Keyword", keyword}
            };

            List<IdentitySystemEmail> listData = new List<IdentitySystemEmail>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractSystemEmailData(reader);

                            listData.Add(record);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = "Failed to execute SystemEmail_GetList. Error: " + ex.Message;
                throw new CustomSQLException(strError);
            }

            return listData;
        }

        #endregion
    }
}
