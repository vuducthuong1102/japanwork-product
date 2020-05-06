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
    public class RpsQualification
    {
        private readonly string _connectionString;

        public RpsQualification(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsQualification()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityQualification> GetByPage(IdentityQualification filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Qualification_GetAll";
            List<IdentityQualification> listData = null;

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
                        listData = ParsingListQualificationFromReader(reader);
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
       
        private List<IdentityQualification> ParsingListQualificationFromReader(IDataReader reader)
        {
            List<IdentityQualification> listData = listData = new List<IdentityQualification>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractQualificationData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityQualification ExtractQualificationData(IDataReader reader)
        {
            var record = new IdentityQualification();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.qualification = reader["qualification"].ToString();
            record.show_major = Utils.ConvertToBoolean(reader["show_major"]);

            return record;
        }

        public static IdentityQualificationLang ExtractQualificationLangData(IDataReader reader)
        {
            var record = new IdentityQualificationLang();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.qualification_id = Utils.ConvertToInt32(reader["qualification_id"]);
            record.qualification = reader["qualification"].ToString();
            record.language_code = reader["language_code"].ToString();

            return record;
        }

        public int Insert(IdentityQualification identity)
        {
            //Common syntax           
            var sqlCmd = @"Qualification_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@qualification", identity.qualification}
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

        public bool Update(IdentityQualification identity)
        {
            //Common syntax
            var sqlCmd = @"Qualification_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", identity.id},
                {"@qualification", identity.qualification}
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

        public IdentityQualification GetById(int id)
        {
            IdentityQualification info = null;
            var sqlCmd = @"Qualification_GetById";

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
                            info = ExtractQualificationData(reader);
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
            var sqlCmd = @"Qualification_Delete";

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

        public List<IdentityQualification> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Qualification_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityQualification> listData = new List<IdentityQualification>();
            List<IdentityQualificationLang> listLang = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractQualificationData(reader);

                            listData.Add(record);
                        }

                        //Lang
                        if (reader.NextResult())
                        {
                            listLang = new List<IdentityQualificationLang>();
                            while (reader.Read())
                            {
                                listLang.Add(ExtractQualificationLangData(reader));
                            }
                        }

                        if (listData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                item.LangList = listLang.Where(x => x.qualification_id == item.id).ToList();
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
