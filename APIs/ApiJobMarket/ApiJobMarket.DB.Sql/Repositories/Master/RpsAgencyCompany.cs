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
    public class RpsAgencyCompany
    {
        private readonly string _connectionString;

        public RpsAgencyCompany(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsAgencyCompany()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityAgencyCompany> GetCompaniesByAgencyId(int agency_id)
        {
            //Common syntax           
            var sqlCmd = @"AgencyCompany_GetCompaniesByAgencyId";
            List<IdentityAgencyCompany> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", agency_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityAgencyCompany>();
                        while (reader.Read())
                        {
                            var record = ExtractData(reader);
                            listData.Add(record);
                        }
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

        public List<IdentityAgencyCompany> GetAgencysByCompanyId(int company_id)
        {
            //Common syntax           
            var sqlCmd = @"AgencyCompany_GetAgencysByCompanyId";
            List<IdentityAgencyCompany> listData = null;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@company_id", company_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = new List<IdentityAgencyCompany>();
                        while (reader.Read())
                        {
                            var record = ExtractData(reader);
                            listData.Add(record);
                        }
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

        private IdentityAgencyCompany ExtractData(IDataReader reader)
        {
            var record = new IdentityAgencyCompany();
            //record.id = Utils.ConvertToInt32(reader["id"]);
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.company_id = Utils.ConvertToInt32(reader["company_id"]);
            return record;
        }


        public bool InsertAgency(int company_id, List<int> ListAgencyId, int agency_parent_id)
        {
            StringBuilder InsertCmd = new StringBuilder();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    if (ListAgencyId.HasData())
                    {
                        foreach (var item in ListAgencyId)
                        {
                            var cmdItem = string.Format("INSERT INTO agency_company(agency_id,company_id, agency_parent_id) VALUES({0},{1},{2});"
                       , item, company_id, agency_parent_id);
                            InsertCmd.Append(cmdItem);

                        }
                    }
                    if (InsertCmd.Length > 0)
                        MsSqlHelper.ExecuteScalar(conn, CommandType.Text, InsertCmd.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", "InsertUserManager", ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }
        public bool InsertCompany(int agency_id, List<int> ListCompanyId, int agency_parent_id)
        {
            StringBuilder InsertCmd = new StringBuilder();


            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    if (ListCompanyId.HasData())
                    {
                        foreach (var item in ListCompanyId)
                        {
                            var cmdItem = string.Format("INSERT INTO agency_company(agency_id,company_id, agency_parent_id) VALUES({0},{1},{2});"
                       , agency_id, item, agency_parent_id);
                            InsertCmd.Append(cmdItem);

                        }
                    }
                    if (InsertCmd.Length > 0)
                        MsSqlHelper.ExecuteScalar(conn, CommandType.Text, InsertCmd.ToString(), null);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", "InsertUserManager", ex.Message);
                throw new CustomSQLException(strError);
            }

            return true;
        }
        public bool Delete(List<int> ListId)
        {
            //Common syntax            
            var sqlCmd = @"AgencyCompany_Delete";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@ListId", string.Join(",", ListId)},
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
