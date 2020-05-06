using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.SharedLib.Extensions;
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
    public class RpsEmploymentType
    {
        private readonly string _connectionString;

        public RpsEmploymentType(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsEmploymentType()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityEmploymentType> GetByPage(IdentityEmploymentType filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"EmploymentType_GetAll";
            List<IdentityEmploymentType> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListEmploymentTypeFromReader(reader);
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
       
        private List<IdentityEmploymentType> ParsingListEmploymentTypeFromReader(IDataReader reader)
        {
            List<IdentityEmploymentType> listData = listData = new List<IdentityEmploymentType>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractEmploymentTypeData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityEmploymentType ExtractEmploymentTypeData(IDataReader reader)
        {
            var record = new IdentityEmploymentType();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.calculate_by = Utils.ConvertToInt32(reader["calculate_by"]);
            record.employment_type = reader["employment_type"].ToString();            
            record.show_trains = Utils.ConvertToBoolean(reader["show_trains"]);            

            return record;
        }

        public static IdentityEmploymentTypeLang ExtractEmploymentTypeLangData(IDataReader reader)
        {
            var record = new IdentityEmploymentTypeLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.employment_type_id = Utils.ConvertToInt32(reader["employment_type_id"]);
            record.employment_type = reader["employment_type"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentityEmploymentType identity)
        {
            //Common syntax           
            var sqlCmd = @"EmploymentType_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@employment_type", identity.employment_type}
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

        public bool Update(IdentityEmploymentType identity)
        {
            //Common syntax
            var sqlCmd = @"EmploymentType_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@employment_type", identity.employment_type}
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

        public IdentityEmploymentType GetById(int id)
        {
            IdentityEmploymentType info = null;
            var sqlCmd = @"EmploymentType_GetById";

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
                            info = ExtractEmploymentTypeData(reader);
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
            var sqlCmd = @"EmploymentType_Delete";

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

        public List<IdentityEmploymentType> GetList()
        {
            //Common syntax            
            var sqlCmd = @"EmploymentType_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityEmploymentType> listData = new List<IdentityEmploymentType>();
            List<IdentityEmploymentTypeLang> langList = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractEmploymentTypeData(reader);

                            listData.Add(record);
                        }

                        //Languages
                        if (reader.NextResult())
                        {
                            langList = new List<IdentityEmploymentTypeLang>();
                            while (reader.Read())
                            {
                                var lang = ExtractEmploymentTypeLangData(reader);
                                langList.Add(lang);
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = langList.Where(x => x.employment_type_id == item.id).ToList();
                            }
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
        
        #endregion
    }
}
