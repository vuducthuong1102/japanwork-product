using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsInterviewProcess
    {
        private readonly string _connectionString;

        public RpsInterviewProcess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsInterviewProcess()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityInterviewProcess> GetListByCandidate_Id(int candidate_id)
        {
            //Common syntax           
            var sqlCmd = @"InterviewProcess_GetListByCandidate_Id";
            List<IdentityInterviewProcess> listData = null;

            var parameters = new Dictionary<string, object>
            {
                {"@candidate_id", candidate_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListInterviewProcessFromReader(reader);
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
        public List<IdentityInterviewProcess> GetListByCandidate_Ids(string candidate_ids)
        {
            //Common syntax           
            var sqlCmd = @"InterviewProcess_GetListByCandidate_Ids";
            List<IdentityInterviewProcess> listData = null;

            var parameters = new Dictionary<string, object>
            {
                {"@candidate_ids", candidate_ids}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListInterviewProcessFromReader(reader);
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
        private List<IdentityInterviewProcess> ParsingListInterviewProcessFromReader(IDataReader reader)
        {
            List<IdentityInterviewProcess> listData = new List<IdentityInterviewProcess>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractInterviewProcessData(reader);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityInterviewProcess ExtractInterviewProcessData(IDataReader reader)
        {
            var record = new IdentityInterviewProcess();

            //Seperate properties
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.note = reader["note"].ToString();
            record.status_id = Utils.ConvertToInt32(reader["status_id"]);
            record.candidate_id = Utils.ConvertToInt32(reader["candidate_id"]);
            record.modified_at = reader["modified_at"] == DBNull.Value ? null : (DateTime?)reader["modified_at"];
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);

            return record;
        }

        public int Insert(IdentityInterviewProcess identity)
        {
            //Common syntax           
            var sqlCmd = @"InterviewProcess_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@cv_id", identity.cv_id},
                {"@agency_id", identity.agency_id},
                {"@job_id", identity.job_id },
                {"@note", identity.note },
                {"@status_id", identity.status_id },
                {"@candidate_id", identity.candidate_id },
                {"@modified_at", identity.modified_at },
                {"@staff_id", identity.staff_id }
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

        public bool Update(IdentityInterviewProcess identity)
        {
            //Common syntax
            var sqlCmd = @"InterviewProcess_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@note", identity.note },
                {"@modified_at", identity.modified_at },
                {"@staff_id", identity.staff_id }
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

        public IdentityInterviewProcess GetById(int id)
        {
            IdentityInterviewProcess info = null;
            var sqlCmd = @"InterviewProcess_GetById";

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
                            info = ExtractInterviewProcessData(reader);
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
            var sqlCmd = @"InterviewProcess_Delete";

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
        #endregion
    }
}
