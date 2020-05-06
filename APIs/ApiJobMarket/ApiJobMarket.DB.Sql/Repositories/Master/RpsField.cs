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

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsField
    {
        private readonly string _connectionString;

        public RpsField(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsField()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityField> GetByPage(IdentityField filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Field_GetAll";
            List<IdentityField> listData = null;

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
                        listData = ParsingListFieldFromReader(reader);
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
       
        private List<IdentityField> ParsingListFieldFromReader(IDataReader reader)
        {
            List<IdentityField> listData = listData = new List<IdentityField>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractFieldData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityField ExtractFieldData(IDataReader reader)
        {
            var record = new IdentityField();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.field = reader["field"].ToString();
            record.icon = reader["icon"].ToString();
            if (reader.HasColumn("employment_type"))
                record.employment_type = Utils.ConvertToInt32(reader["employment_type"]);

            return record;
        }

        public static IdentityFieldLang ExtractFieldLangData(IDataReader reader)
        {
            var record = new IdentityFieldLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.field_id = Utils.ConvertToInt32(reader["field_id"]);
            record.field = reader["field"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentityField identity)
        {
            //Common syntax           
            var sqlCmd = @"Field_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@field", identity.field}
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

        public bool Update(IdentityField identity)
        {
            //Common syntax
            var sqlCmd = @"Field_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@field", identity.field}
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

        public IdentityField GetById(int id)
        {
            IdentityField info = null;
            var sqlCmd = @"Field_GetById";

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
                            info = ExtractFieldData(reader);
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
            var sqlCmd = @"Field_Delete";

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

        public List<IdentityField> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Field_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityField> listData = new List<IdentityField>();
            List<IdentitySubField> subFields = new List<IdentitySubField>();

            List<IdentityFieldLang> listFieldLang = null;
            List<IdentitySubFieldLang> listSubFieldLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractFieldData(reader);

                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                subFields.Add(RpsSubField.ExtractSubFieldData(reader));
                            }
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listFieldLang = new List<IdentityFieldLang>();
                            while (reader.Read())
                            {
                                listFieldLang.Add(ExtractFieldLangData(reader));
                            }
                        }

                        //Sub Lang
                        if (reader.NextResult())
                        {
                            listSubFieldLang = new List<IdentitySubFieldLang>();
                            while (reader.Read())
                            {
                                listSubFieldLang.Add(RpsSubField.ExtractSubFieldLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.Sub_fields = subFields.Where(x => x.field_id == item.id).ToList();

                                item.LangList = listFieldLang.Where(x => x.field_id == item.id).ToList();

                                if (item.Sub_fields.HasData())
                                {
                                    foreach (var sub in item.Sub_fields)
                                    {
                                        sub.LangList = listSubFieldLang.Where(x => x.sub_field_id == sub.id).ToList();
                                    }
                                }
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


        public List<IdentityField> GetListFieldsByEmploymentType(int employment_type)
        {
            //Common syntax            
            var sqlCmd = @"Field_GetListByEmploymentType";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@employment_type", employment_type}
            };

            List<IdentityField> listData = new List<IdentityField>();
            List<IdentitySubField> subFields = new List<IdentitySubField>();

            List<IdentityFieldLang> listFieldLang = null;
            List<IdentitySubFieldLang> listSubFieldLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractFieldData(reader);

                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                subFields.Add(RpsSubField.ExtractSubFieldData(reader));
                            }
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listFieldLang = new List<IdentityFieldLang>();
                            while (reader.Read())
                            {
                                listFieldLang.Add(ExtractFieldLangData(reader));
                            }
                        }

                        //Sub Lang
                        if (reader.NextResult())
                        {
                            listSubFieldLang = new List<IdentitySubFieldLang>();
                            while (reader.Read())
                            {
                                listSubFieldLang.Add(RpsSubField.ExtractSubFieldLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.Sub_fields = subFields.Where(x => x.field_id == item.id).ToList();

                                item.LangList = listFieldLang.Where(x => x.field_id == item.id).ToList();

                                if (item.Sub_fields.HasData())
                                {
                                    foreach (var sub in item.Sub_fields)
                                    {
                                        sub.LangList = listSubFieldLang.Where(x => x.sub_field_id == sub.id).ToList();
                                    }
                                }
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

        public List<IdentityField> F_GetListCount()
        {
            //Common syntax            
            var sqlCmd = @"F_Field_GetListCount";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
            };

            List<IdentityField> listData = new List<IdentityField>();
            List<IdentitySubField> subFields = new List<IdentitySubField>();

            List<IdentityFieldLang> listFieldLang = null;
            List<IdentitySubFieldLang> listSubFieldLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractFieldData(reader);
                            record.count_num = Utils.ConvertToInt32(reader["CountNum"]);
                            record.count_add_today = Utils.ConvertToInt32(reader["CountAddToday"]);
                            listData.Add(record);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                subFields.Add(RpsSubField.ExtractSubFieldData(reader));
                            }
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listFieldLang = new List<IdentityFieldLang>();
                            while (reader.Read())
                            {
                                listFieldLang.Add(ExtractFieldLangData(reader));
                            }
                        }

                        //Sub Lang
                        if (reader.NextResult())
                        {
                            listSubFieldLang = new List<IdentitySubFieldLang>();
                            while (reader.Read())
                            {
                                listSubFieldLang.Add(RpsSubField.ExtractSubFieldLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.Sub_fields = subFields.Where(x => x.field_id == item.id).ToList();

                                item.LangList = listFieldLang.Where(x => x.field_id == item.id).ToList();

                                if (item.Sub_fields.HasData())
                                {
                                    foreach (var sub in item.Sub_fields)
                                    {
                                        sub.LangList = listSubFieldLang.Where(x => x.sub_field_id == sub.id).ToList();
                                    }
                                }
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
