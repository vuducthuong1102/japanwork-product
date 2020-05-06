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
    public class RpsJobSeekerNote
    {
        private readonly string _connectionString;

        public RpsJobSeekerNote(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsJobSeekerNote()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityJobSeekerNote> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"JobSeekerNote_GetByPage";
            List<IdentityJobSeekerNote> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@job_seeker_id", filter.job_seeker_id},
                {"@type_job_seeker", filter.type_job_seeker},
                {"@staff_id", filter.staff_id},
                {"@agency_id", filter.agency_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListJobSeekerNoteFromReader(reader);
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

        private List<IdentityJobSeekerNote> ParsingListJobSeekerNoteFromReader(IDataReader reader)
        {
            List<IdentityJobSeekerNote> listData = new List<IdentityJobSeekerNote>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractJobSeekerNoteData(reader);

                //Extends information
                record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityJobSeekerNote ExtractJobSeekerNoteData(IDataReader reader)
        {
            var record = new IdentityJobSeekerNote();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
            record.note = reader["note"].ToString();
            record.type = Utils.ConvertToInt32(reader["type"]);
            record.type_job_seeker = Utils.ConvertToInt32(reader["type_job_seeker"]);
            return record;
        }

        public int Insert(IdentityJobSeekerNote identity)
        {
            //Common syntax           
            var sqlCmd = @"JobSeekerNote_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", identity.job_seeker_id },
                {"@staff_id", identity.staff_id },
                {"@note", identity.note },
                {"@type", identity.type },
                {"@type_job_seeker", identity.type_job_seeker },
                {"@agency_id", identity.agency_id }
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

        public int Update(IdentityJobSeekerNote identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeekerNote_Update";
            var returnId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@job_seeker_id", identity.job_seeker_id },
                {"@note", identity.note },
                {"@type", identity.type },
                {"@type_job_seeker", identity.type_job_seeker },
                {"@staff_id", identity.staff_id },
                {"@agency_id", identity.agency_id },

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

        public IdentityJobSeekerNote GetById(int id)
        {
            IdentityJobSeekerNote info = null;
            
            var sqlCmd = @"JobSeekerNote_GetById";

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
                            info = ExtractJobSeekerNoteData(reader);
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
            var sqlCmd = @"JobSeekerNote_Delete";

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
