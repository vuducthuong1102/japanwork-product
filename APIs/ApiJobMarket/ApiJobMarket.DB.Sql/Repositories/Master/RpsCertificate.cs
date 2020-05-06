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
    public class RpsCertificate
    {
        private readonly string _connectionString;

        public RpsCertificate(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsCertificate()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityCertificate> GetByPage(IdentityCertificate filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Certificate_GetAll";
            List<IdentityCertificate> listData = null;

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
                        listData = ParsingListCertificateFromReader(reader);
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
       
        private List<IdentityCertificate> ParsingListCertificateFromReader(IDataReader reader)
        {
            List<IdentityCertificate> listData = listData = new List<IdentityCertificate>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractCertificateData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (listData == null)
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityCertificate ExtractCertificateData(IDataReader reader)
        {
            var record = new IdentityCertificate();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.name = reader["name"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.pass = Utils.ConvertToInt32(reader["pass"]);
            record.point = reader["point"].ToString();

            return record;
        }

        public static IdentityJobSeekerCertificate ExtractJobSeekerCertificateData(IDataReader reader)
        {
            var record = new IdentityJobSeekerCertificate();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);
            record.name = reader["name"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            //record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.pass = Utils.ConvertToInt32(reader["pass"]);
            record.point = reader["point"].ToString();

            return record;
        }

        public static IdentityCvCertificate ExtractCvCertificateData(IDataReader reader)
        {
            var record = new IdentityCvCertificate();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.name = reader["name"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            //record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.pass = Utils.ConvertToInt32(reader["pass"]);
            record.point = reader["point"].ToString();

            return record;
        }

        public static IdentityCsCertificate ExtractCsCertificateData(IDataReader reader)
        {
            var record = new IdentityCsCertificate();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.cs_id = Utils.ConvertToInt32(reader["cs_id"]);
            record.name = reader["name"].ToString();
            record.start_date = reader["start_date"] == DBNull.Value ? null : (DateTime?)reader["start_date"];
            //record.end_date = reader["end_date"] == DBNull.Value ? null : (DateTime?)reader["end_date"];
            record.status = Utils.ConvertToInt32(reader["status"]);
            record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
            record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];
            record.pass = Utils.ConvertToInt32(reader["pass"]);
            record.point = reader["point"].ToString();

            return record;
        }

        public int Insert(IdentityCertificate identity)
        {
            //Common syntax           
            var sqlCmd = @"Certificate_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@name", identity.name },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@pass", identity.pass },
                { "@point", identity.point }
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

        public bool Update(IdentityCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"Certificate_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@name", identity.name },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@pass", identity.pass },
                { "@point", identity.point }
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

        public IdentityCertificate GetById(int id)
        {
            IdentityCertificate info = null;
            var sqlCmd = @"Certificate_GetById";

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
                            info = ExtractCertificateData(reader);
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
            var sqlCmd = @"Certificate_Delete";

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

        public List<IdentityCertificate> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Certificate_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityCertificate> listData = new List<IdentityCertificate>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCertificateData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityCertificateLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityCertificateLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.CertificateId = Utils.ConvertToInt32(reader["CertificateId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.CertificateId == item.Id).ToList();
                        //    }
                        //}
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

        #region JobSeeker

        public IdentityJobSeekerCertificate JobSeekerGetDetail(int id, int job_seeker_id)
        {
            IdentityJobSeekerCertificate info = null;
            var sqlCmd = @"JobSeeker_Certificate_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
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
                            info = ExtractJobSeekerCertificateData(reader);
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

        public bool JobSeekerUpdate(IdentityJobSeekerCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_UpdateCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@name", identity.name },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@pass", identity.pass },
                { "@point", identity.point }
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

        public bool JobSeekerDelete(IdentityJobSeekerCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"JobSeeker_DeleteCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id }               
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

        public List<IdentityJobSeekerCertificate> GetListJobSeekerCertificate(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"JobSeeker_GetCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", job_seeker_id}
            };

            List<IdentityJobSeekerCertificate> listData = new List<IdentityJobSeekerCertificate>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerCertificateData(reader);

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

        #endregion

        #region Agency

        public IdentityJobSeekerCertificate A_JobSeekerGetDetail(int id, int job_seeker_id)
        {
            IdentityJobSeekerCertificate info = null;
            var sqlCmd = @"A_JobSeeker_Certificate_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
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
                            info = ExtractJobSeekerCertificateData(reader);
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

        public bool A_JobSeekerUpdate(IdentityJobSeekerCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_UpdateCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id },
                { "@name", identity.name },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@pass", identity.pass },
                { "@point", identity.point }
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

        public bool A_JobSeekerDelete(IdentityJobSeekerCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"A_JobSeeker_DeleteCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@job_seeker_id", identity.job_seeker_id }
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

        public List<IdentityJobSeekerCertificate> A_GetListJobSeekerCertificate(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"A_JobSeeker_GetCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", job_seeker_id}
            };

            List<IdentityJobSeekerCertificate> listData = new List<IdentityJobSeekerCertificate>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractJobSeekerCertificateData(reader);

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

        #endregion

        #region Cv

        public IdentityCvCertificate CvGetDetail(int id, int cv_id)
        {
            IdentityCvCertificate info = null;
            var sqlCmd = @"Cv_Certificate_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@cv_id", cv_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCvCertificateData(reader);
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

        public int CvUpdate(IdentityCvCertificate identity)
        {
            var returnId = identity.id;

            //Common syntax
            var sqlCmd = @"Cv_UpdateCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id },
                { "@name", identity.name },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@pass", identity.pass },
                { "@point", identity.point }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var objReturn = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    returnId = Utils.ConvertToInt32(objReturn);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnId;
        }

        public bool CvDelete(IdentityCvCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"Cv_DeleteCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cv_id", identity.cv_id }
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

        public List<IdentityCvCertificate> GetListCvCertificate(int cv_id)
        {
            //Common syntax            
            var sqlCmd = @"Cv_GetCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", cv_id}
            };

            List<IdentityCvCertificate> listData = new List<IdentityCvCertificate>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCvCertificateData(reader);

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

        #endregion

        #region Cs

        public IdentityCsCertificate CsGetDetail(int id, int cs_id)
        {
            IdentityCsCertificate info = null;
            var sqlCmd = @"Cs_Certificate_GetDetail";

            var parameters = new Dictionary<string, object>
            {
                {"@id", id},
                {"@cs_id", cs_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractCsCertificateData(reader);
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

        public int CsUpdate(IdentityCsCertificate identity)
        {
            var returnId = identity.id;

            //Common syntax
            var sqlCmd = @"Cs_UpdateCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cs_id", identity.cs_id },
                { "@name", identity.name },
                { "@start_date", identity.start_date },
                { "@end_date", identity.end_date },
                { "@status", identity.status },
                { "@pass", identity.pass },
                { "@point", identity.point }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var objReturn = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                    returnId = Utils.ConvertToInt32(objReturn);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return returnId;
        }

        public bool CsDelete(IdentityCsCertificate identity)
        {
            //Common syntax
            var sqlCmd = @"Cs_DeleteCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                { "@id", identity.id },
                { "@cs_id", identity.cs_id }
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

        public List<IdentityCsCertificate> GetListCsCertificate(int cs_id)
        {
            //Common syntax            
            var sqlCmd = @"Cs_GetCertificate";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", cs_id}
            };

            List<IdentityCsCertificate> listData = new List<IdentityCsCertificate>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractCsCertificateData(reader);

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

        #endregion

        #endregion
    }
}
