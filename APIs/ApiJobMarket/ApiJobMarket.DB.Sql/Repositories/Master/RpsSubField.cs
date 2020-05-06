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
    public class RpsSubField
    {
        private readonly string _connectionString;

        public RpsSubField(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsSubField()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentitySubField> GetByPage(IdentitySubField filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"SubField_GetAll";
            List<IdentitySubField> listData = null;

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
                        listData = ParsingListSubFieldFromReader(reader);
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

        private List<IdentitySubField> ParsingListSubFieldFromReader(IDataReader reader)
        {
            List<IdentitySubField> listData = listData = new List<IdentitySubField>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractSubFieldData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentitySubField ExtractSubFieldData(IDataReader reader)
        {
            var record = new IdentitySubField();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.field_id = Utils.ConvertToInt32(reader["field_id"]);
            record.sub_field = reader["sub_field"].ToString();

            return record;
        }

        public static IdentityJobSubField ExtractJobSubFieldData(IDataReader reader)
        {
            var record = new IdentityJobSubField();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.field_id = Utils.ConvertToInt32(reader["field_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.sub_field_id = Utils.ConvertToInt32(reader["sub_field_id"]);

            return record;
        }

        public static IdentitySubFieldLang ExtractSubFieldLangData(IDataReader reader)
        {
            var record = new IdentitySubFieldLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.sub_field_id = Utils.ConvertToInt32(reader["sub_field_id"]);
            record.sub_field = reader["sub_field"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentitySubField identity)
        {
            //Common syntax           
            var sqlCmd = @"SubField_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@field_id", identity.field_id},
                {"@sub_field", identity.sub_field},
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

        public bool Update(IdentitySubField identity)
        {
            //Common syntax
            var sqlCmd = @"SubField_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@field_id", identity.field_id},
                {"@sub_field", identity.sub_field},
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

        public IdentitySubField GetById(int id)
        {
            IdentitySubField info = null;
            var sqlCmd = @"SubField_GetById";

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
                            info = ExtractSubFieldData(reader);
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
            var sqlCmd = @"SubField_Delete";

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

        public List<IdentitySubField> GetList()
        {
            //Common syntax            
            var sqlCmd = @"SubField_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentitySubField> listData = new List<IdentitySubField>();
            List<IdentitySubFieldLang> listDataLang = new List<IdentitySubFieldLang>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractSubFieldData(reader);

                            listData.Add(record);
                        }
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var recordLang = ExtractSubFieldLangData(reader);

                                listDataLang.Add(recordLang);
                            }
                        }
                        if (listData != null && listData.Count > 0)
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listDataLang.Where(s => s.sub_field_id == item.id).ToList();
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
