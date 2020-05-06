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
    public class RpsCompanyNote
    {
        private readonly string _connectionString;

        public RpsCompanyNote(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCompanyNote()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityCompanyNote> GetByPage(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"CompanyNote_GetByPage";
            List<IdentityCompanyNote> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
                {"@company_id", filter.company_id},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListCompanyNoteFromReader(reader);
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

        private List<IdentityCompanyNote> ParsingListCompanyNoteFromReader(IDataReader reader)
        {
            List<IdentityCompanyNote> listData = new List<IdentityCompanyNote>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCompanyNoteData(reader);

                //Extends information
                record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityCompanyNote ExtractCompanyNoteData(IDataReader reader)
        {
            var record = new IdentityCompanyNote();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.company_id = Utils.ConvertToInt32(reader["company_id"]);
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
            record.note = reader["note"].ToString();
            record.type = Utils.ConvertToInt32(reader["type"]);
            return record;
        }

        public int Insert(IdentityCompanyNote identity)
        {
            //Common syntax           
            var sqlCmd = @"CompanyNote_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", identity.company_id },
                {"@staff_id", identity.staff_id },
                {"@note", identity.note },
                {"@type", identity.type },
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

        public int Update(IdentityCompanyNote identity)
        {
            //Common syntax
            var sqlCmd = @"CompanyNote_Update";
            var returnId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id },
                {"@company_id", identity.company_id },
                {"@note", identity.note },
                {"@type", identity.type },
                {"@staff_id", identity.staff_id },
                {"@agency_id", identity.agency_id }
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

        public IdentityCompanyNote GetById(int id)
        {
            IdentityCompanyNote info = null;
            
            var sqlCmd = @"CompanyNote_GetById";

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
                            info = ExtractCompanyNoteData(reader);
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
            var sqlCmd = @"CompanyNote_Delete";

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
