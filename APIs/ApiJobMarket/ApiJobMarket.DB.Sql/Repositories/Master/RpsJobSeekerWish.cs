using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using ApiJobMarket.ShareLibs.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsJobSeekerWish
    {
        private readonly string _connectionString;

        public RpsJobSeekerWish(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsJobSeekerWish()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region JobSeekerWish

        public bool Update(IdentityJobSeekerWish identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeekerWish_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@job_seeker_id", identity.job_seeker_id },
                { "@employment_type_ids", identity.employment_type_ids },
                { "@prefecture_ids", identity.prefecture_ids },
                { "@sub_field_ids", identity.sub_field_ids },
                { "@start_date", identity.start_date },
                { "@salary_min", identity.salary_min },
                { "@salary_max", identity.salary_max }
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

        public IdentityJobSeekerWish GetById(int job_seeker_id)
        {
            IdentityJobSeekerWish info = null;
            var sqlCmd = @"JobSeekerWish_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractJobSeekerWishData(reader);
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

        #endregion
        #region A_JobSeekerWish
        public bool A_Update(IdentityJobSeekerWish identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeekerWish_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@employment_type_ids", identity.employment_type_ids },
                { "@employment_type_id", identity.employment_type_id },
                { "@prefecture_ids", identity.prefecture_ids },
                { "@sub_field_ids", identity.sub_field_ids },
                { "@start_date", identity.start_date },
                { "@salary_min", identity.salary_min },
                { "@salary_max", identity.salary_max }
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

        public bool A_Delete(IdentityJobSeekerWish identity)
        {
            //Common syntax            
            var sqlCmd = @"A_JobSeekerWish_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@job_seeker_id", identity.job_seeker_id},
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

        public List<IdentityJobSeekerWish> A_GetById(int job_seeker_id)
        {
            List<IdentityJobSeekerWish> info = new List<IdentityJobSeekerWish>();
            var sqlCmd = @"A_JobSeekerWish_GetById";

            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerWishData(reader);
                            info.Add(record);
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
        #endregion
        public static IdentityJobSeekerWish ExtractJobSeekerWishData(IDataReader reader)
        {
            var record = new IdentityJobSeekerWish();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.employment_type_ids = reader["employment_type_ids"].ToString();
            record.employment_type_id = Utils.ConvertToInt32(reader["employment_type_id"]);
            record.prefecture_ids = reader["prefecture_ids"].ToString();
            record.sub_field_ids = reader["sub_field_ids"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.salary_min = Utils.ConvertToInt32(reader["salary_min"]);
            record.salary_max = Utils.ConvertToInt32(reader["salary_max"]);

            return record;
        }
    }
}
