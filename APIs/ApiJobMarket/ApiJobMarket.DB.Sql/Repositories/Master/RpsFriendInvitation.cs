using ApiJobMarket.DB.Sql.Entities;
using ApiJobMarket.SharedLib.Extensions;
using ApiJobMarket.ShareLibs;
using ApiJobMarket.ShareLibs.Exceptions;
using ApiJobMarket.ShareLibs.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;

namespace ApiJobMarket.DB.Sql.Repositories
{
    public class RpsFriendInvitation
    {
        private readonly string _connectionString;

        public RpsFriendInvitation(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsFriendInvitation()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityFriendInvitation> GetByPage(IdentityFriendInvitation filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_GetAll";
            List<IdentityFriendInvitation> listData = null;

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
                        listData = ParsingListFriendInvitationFromReader(reader);
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

        public List<IdentityFriendInvitation> GetListByJob(IdentityFriendInvitation filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_GetListByJob";
            List<IdentityFriendInvitation> listData = null;

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@job_id", filter.job_id },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        listData = ParsingListFriendInvitationFromReader(reader);
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

        public List<IdentityFriendInvitation> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_GetListByJobSeeker";
            List<IdentityFriendInvitationMaster> listMasterData = new List<IdentityFriendInvitationMaster>();
            List<IdentityFriendInvitation> listData = new List<IdentityFriendInvitation>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@job_seeker_id", filter.job_seeker_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {                        
                        while (reader.Read())
                        {
                            var invite = ExtractFriendInvitationData(reader);

                            if (reader.HasColumn("total_count"))
                                invite.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            listData.Add(invite);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var master = ExtractFriendInvitationMasterData(reader);
                                listMasterData.Add(master);
                            }
                        }

                        if(listData.HasData() && listMasterData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                var matchedMaster = listMasterData.Where(x => x.master_id == item.invite_id).FirstOrDefault();
                                if(matchedMaster != null)
                                {                                   
                                    item.job_id = matchedMaster.job_id;
                                    item.note = matchedMaster.note;
                                    item.request_time = matchedMaster.request_time;
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

        public List<IdentityFriendInvitation> GetReceivers(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_GetReceiversByAgency";
            List<IdentityFriendInvitationMaster> listMasterData = new List<IdentityFriendInvitationMaster>();
            List<IdentityFriendInvitation> listData = new List<IdentityFriendInvitation>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@staff_id", filter.staff_id },
                {"@job_id", filter.job_id },
                {"@invite_id", filter.invite_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var invite = ExtractFriendInvitationData(reader);

                            if (reader.HasColumn("total_count"))
                                invite.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            listData.Add(invite);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var master = ExtractFriendInvitationMasterData(reader);
                                listMasterData.Add(master);
                            }
                        }

                        if (listData.HasData() && listMasterData.HasData())
                        {
                            foreach (var item in listData)
                            {
                                var matchedMaster = listMasterData.Where(x => x.master_id == item.invite_id).FirstOrDefault();
                                if (matchedMaster != null)
                                {                                    
                                    item.job_id = matchedMaster.job_id;
                                    item.note = matchedMaster.note;
                                    item.request_time = matchedMaster.request_time;
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

        public List<IdentityFriendInvitation> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_GetListByAgency";
            //List<IdentityFriendInvitationMaster> listMasterData = new List<IdentityFriendInvitationMaster>();
            List<IdentityFriendInvitation> listData = new List<IdentityFriendInvitation>();

            //For paging 
            int offset = (currentPage - 1) * pageSize;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@keyword", filter.keyword },
                {"@agency_id", filter.agency_id },
                {"@company_id", filter.company_id },
                {"@staff_id", filter.staff_id },
                {"@job_id", filter.job_id },
                {"@status", filter.status },
                {"@offset", offset},
                {"@page_size", pageSize},
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = new IdentityFriendInvitation();

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("invitation_count"))
                                record.invitation_count = Utils.ConvertToInt32(reader["invitation_count"]);

                            var invite_id = Utils.ConvertToInt32(reader["id"]); ;
                            var matchedMaster = ExtractFriendInvitationMasterData(reader);
                            if (matchedMaster != null)
                            {
                                record.job_id = matchedMaster.job_id;
                                record.note = matchedMaster.note;
                                record.id = invite_id;
                                record.request_time = matchedMaster.request_time;
                            }

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

        private List<IdentityFriendInvitation> ParsingListFriendInvitationFromReader(IDataReader reader)
        {
            List<IdentityFriendInvitation> listData = listData = new List<IdentityFriendInvitation>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractFriendInvitationData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }        

        public static IdentityFriendInvitationMaster ExtractFriendInvitationMasterData(IDataReader reader)
        {
            var record = new IdentityFriendInvitationMaster();

            //Seperate properties
            record.master_id = Utils.ConvertToInt32(reader["id"]);
            record.sender_id = Utils.ConvertToInt32(reader["sender_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.request_time = reader["request_time"] == DBNull.Value ? null : (DateTime?)reader["request_time"];
            record.note = reader["note"].ToString();

            return record;
        }

        public static IdentityFriendInvitation ExtractFriendInvitationData(IDataReader reader)
        {
            var record = new IdentityFriendInvitation();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.invite_id = Utils.ConvertToInt32(reader["invite_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.sender_id = Utils.ConvertToInt32(reader["sender_id"]);            
            record.receiver_email = reader["receiver_email"].ToString();            
            record.accept_time = reader["accept_time"] == DBNull.Value ? null : (DateTime?)reader["accept_time"];
            record.visited = Utils.ConvertToBoolean(reader["visited"]);
            record.visit_time = reader["visit_time"] == DBNull.Value ? null : (DateTime?)reader["visit_time"];           
            record.sending_count = Utils.ConvertToInt32(reader["sending_count"]);
            record.status = Utils.ConvertToInt32(reader["status"]);

            return record;
        }

        public int Insert(IdentityFriendInvitation identity)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_Insert";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@cv_id", identity.cv_id},
                //{"@job_id", identity.job_id },
                //{"@job_seeker_id", identity.job_seeker_id }
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

        public bool Update(IdentityFriendInvitation identity)
        {
            //Common syntax
            var sqlCmd = @"FriendInvitation_Update";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                //{"@id", identity.id},
                //{"@cv_id", identity.cv_id},
                //{"@job_id", identity.job_id },
                //{"@job_seeker_id", identity.job_seeker_id }
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

        public IdentityFriendInvitation GetById(int id)
        {
            IdentityFriendInvitation info = null;
            var sqlCmd = @"FriendInvitation_GetById";

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
                            info = ExtractFriendInvitationData(reader);
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
            var sqlCmd = @"FriendInvitation_Delete";

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

        public int Cancel(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_Cancel";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", applyInfo.job_id},
                {"@job_seeker_id", applyInfo.job_seeker_id},
                {"@cv_id", applyInfo.cv_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    status = Utils.ConvertToInt32(statusObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status;
        }

        public int Ignore(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_Ignore";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", applyInfo.id},
                {"@job_id", applyInfo.job_id},
                {"@job_seeker_id", applyInfo.job_seeker_id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    status = Utils.ConvertToInt32(statusObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status;
        }

        public int Accept(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_Accept";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@invite_id", applyInfo.invite_id},
                {"@sender_id", applyInfo.sender_id},
                {"@job_id", applyInfo.job_id},
                {"@receiver_email", applyInfo.receiver_email}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    status = Utils.ConvertToInt32(statusObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status;
        }

        public int Interview(int id)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_Interview";

            var status = 0;
            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@id", id}
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var statusObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    status = Utils.ConvertToInt32(statusObj);
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return status;
        }

        public List<IdentityFriendInvitation> GetList()
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityFriendInvitation> listData = new List<IdentityFriendInvitation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractFriendInvitationData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityFriendInvitationLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityFriendInvitationLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.FriendInvitationId = Utils.ConvertToInt32(reader["FriendInvitationId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.FriendInvitationId == item.Id).ToList();
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

        public List<IdentityFriendInvitation> GetListByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_GetListByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityFriendInvitation> listData = new List<IdentityFriendInvitation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractFriendInvitationData(reader);

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

        public List<IdentityFriendInvitation> GetListByJob(int job_id)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_GetListByJob";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
            };

            List<IdentityFriendInvitation> listData = new List<IdentityFriendInvitation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractFriendInvitationData(reader);

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

        public IdentityFriendInvitation CheckJobApplied(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"FriendInvitation_CheckJobApplied";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
            };

            IdentityFriendInvitation info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractFriendInvitationData(reader);
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

        public int ApplicationInvite(IdentityFriendInvitation identity)
        {
            //Common syntax           
            var sqlCmd = @"JobSeeker_FriendInviteApplication";
            var newId = 0;

            var receivers = JsonConvert.SerializeObject(identity.Invitations);

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", identity.job_id },
                {"@sender_id", identity.sender_id },
                {"@receivers", receivers },
                {"@note", identity.note }
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

        public int Invite(IdentityFriendInvitation identity)
        {
            //Common syntax           
            var sqlCmd = @"FriendInvitation_Invite";
            var newId = 0;

            var receivers = JsonConvert.SerializeObject(identity.Invitations);

            try
            {
                if (identity.Invitations.HasData())
                {
                    using (var conn = new SqlConnection(_connectionString))
                    {
                        foreach (var item in identity.Invitations)
                        {
                            //For parameters
                            var parameters = new Dictionary<string, object>
                            {
                                {"@job_id", identity.job_id },
                                {"@sender_id", identity.sender_id },
                                {"@email", item.receiver_email },
                                {"@note", identity.note }
                            };

                            var returnObj = MsSqlHelper.ExecuteScalar(conn, CommandType.StoredProcedure, sqlCmd, parameters);

                            newId = Convert.ToInt32(returnObj);
                        }
                    }   
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return newId;
        }

        public dynamic FriendInvitationChecking(dynamic filter)
        {
            //Common syntax           
            var sqlCmd = @"Job_FriendInvitationChecking";
            dynamic result = new ExpandoObject();
            result.invitation_limit = 0;
            result.total_count = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", filter.agency_id},
                {"@company_id", filter.company_id },
                {"@job_id", filter.job_id }
            };

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters);
                    if (reader.Read())
                    {
                        result.invitation_limit = Utils.ConvertToInt32(reader["invitation_limit"]);
                        result.total_count = Utils.ConvertToInt32(reader["total_count"]);
                    }                    
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return result;
        }

        #endregion
    }
}
