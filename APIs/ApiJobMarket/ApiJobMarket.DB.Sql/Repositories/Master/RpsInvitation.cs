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
    public class RpsInvitation
    {
        private readonly string _connectionString;

        public RpsInvitation(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RpsInvitation()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["JobMarketDB"].ConnectionString;
        }

        #region Common

        public List<IdentityInvitation> GetByPage(IdentityInvitation filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Invitation_GetAll";
            List<IdentityInvitation> listData = null;

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
                        listData = ParsingListInvitationFromReader(reader);
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

        public List<IdentityInvitation> GetListByJob(IdentityInvitation filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Invitation_GetListByJob";
            List<IdentityInvitation> listData = null;

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
                        listData = ParsingListInvitationFromReader(reader);
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

        public List<IdentityInvitation> GetListByJobSeeker(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Invitation_GetListByJobSeeker";
            List<IdentityInvitationMaster> listMasterData = new List<IdentityInvitationMaster>();
            List<IdentityInvitation> listData = new List<IdentityInvitation>();

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
                            var invite = ExtractInvitationData(reader);

                            if (reader.HasColumn("total_count"))
                                invite.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            listData.Add(invite);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var master = ExtractInvitationMasterData(reader);
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
                                    item.agency_id = matchedMaster.agency_id;
                                    item.company_id = matchedMaster.company_id;
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

        public List<IdentityInvitation> GetReceivers(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Invitation_GetReceiversByAgency";
            List<IdentityInvitationMaster> listMasterData = new List<IdentityInvitationMaster>();
            List<IdentityInvitation> listData = new List<IdentityInvitation>();

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
                            var invite = ExtractInvitationData(reader);

                            if (reader.HasColumn("total_count"))
                                invite.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("is_show_info"))
                                invite.is_show_info = Utils.ConvertToBoolean(reader["is_show_info"]);

                            listData.Add(invite);
                        }

                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var master = ExtractInvitationMasterData(reader);
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
                                    item.agency_id = matchedMaster.agency_id;
                                    item.company_id = matchedMaster.company_id;
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

        public List<IdentityInvitation> GetListByAgency(dynamic filter, int currentPage, int pageSize)
        {
            //Common syntax           
            var sqlCmd = @"Invitation_GetListByAgency";
            //List<IdentityInvitationMaster> listMasterData = new List<IdentityInvitationMaster>();
            List<IdentityInvitation> listData = new List<IdentityInvitation>();

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
                            var record = new IdentityInvitation();

                            if (reader.HasColumn("total_count"))
                                record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                            if (reader.HasColumn("invitation_count"))
                                record.invitation_count = Utils.ConvertToInt32(reader["invitation_count"]);

                            //var invite_id = Utils.ConvertToInt32(reader["id"]); 

                            if (reader.HasColumn("job_id"))
                                record.job_id = Utils.ConvertToInt32(reader["job_id"]);

                            //var matchedMaster = ExtractInvitationMasterData(reader);
                            //if (matchedMaster != null)
                            //{
                            //    record.agency_id = matchedMaster.agency_id;
                            //    record.company_id = matchedMaster.company_id;
                            //    record.job_id = matchedMaster.job_id;
                            //    record.note = matchedMaster.note;
                            //    record.id = invite_id;
                            //    record.request_time = matchedMaster.request_time;
                            //}

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

        private List<IdentityInvitation> ParsingListInvitationFromReader(IDataReader reader)
        {
            List<IdentityInvitation> listData = listData = new List<IdentityInvitation>();
            while (reader.Read())
            {
                //Get common information
                var record = ExtractInvitationData(reader);

                //Extends information
                //record.created_at = reader["created_at"] == DBNull.Value ? null : (DateTime?)reader["created_at"];
                //record.updated_at = reader["updated_at"] == DBNull.Value ? null : (DateTime?)reader["updated_at"];

                if (reader.HasColumn("total_count"))
                    record.total_count = Utils.ConvertToInt32(reader["total_count"]);

                listData.Add(record);
            }

            return listData;
        }

        public static IdentityInvitationMaster ExtractInvitationMasterData(IDataReader reader)
        {
            var record = new IdentityInvitationMaster();

            //Seperate properties
            record.master_id = Utils.ConvertToInt32(reader["id"]);
            record.agency_id = Utils.ConvertToInt32(reader["agency_id"]);
            record.company_id = Utils.ConvertToInt32(reader["company_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.staff_id = Utils.ConvertToInt32(reader["staff_id"]);
            record.request_time = reader["request_time"] == DBNull.Value ? null : (DateTime?)reader["request_time"];
            record.note = reader["note"].ToString();

            return record;
        }

        public static IdentityInvitation ExtractInvitationData(IDataReader reader)
        {
            var record = new IdentityInvitation();

            //Seperate properties
            record.id = Utils.ConvertToInt32(reader["id"]);
            record.invite_id = Utils.ConvertToInt32(reader["invite_id"]);
            record.cv_id = Utils.ConvertToInt32(reader["cv_id"]);
            record.job_id = Utils.ConvertToInt32(reader["job_id"]);
            record.job_seeker_id = Utils.ConvertToInt32(reader["job_seeker_id"]);            
            record.accept_time = reader["accept_time"] == DBNull.Value ? null : (DateTime?)reader["accept_time"];           
            record.status = Utils.ConvertToInt32(reader["status"]);

            return record;
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
            record.visited = Utils.ConvertToBoolean(reader["sender_id"]);
            record.visit_time = reader["visit_time"] == DBNull.Value ? null : (DateTime?)reader["visit_time"];           
            record.status = Utils.ConvertToInt32(reader["status"]);

            return record;
        }

        public int Insert(IdentityInvitation identity)
        {
            //Common syntax           
            var sqlCmd = @"Invitation_Insert";
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

        public bool Update(IdentityInvitation identity)
        {
            //Common syntax
            var sqlCmd = @"Invitation_Update";

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

        public IdentityInvitation GetById(int id)
        {
            IdentityInvitation info = null;
            var sqlCmd = @"Invitation_GetById";

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
                            info = ExtractInvitationData(reader);
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
            var sqlCmd = @"Invitation_Delete";

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
            var sqlCmd = @"Invitation_Cancel";

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
            var sqlCmd = @"Invitation_Ignore";

            var pic_id = 0;
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
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            pic_id = Utils.ConvertToInt32(reader["pic_id"]); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed to execute {0}. Error: {1}", sqlCmd, ex.Message);
                throw new CustomSQLException(strError);
            }

            return pic_id;
        }

        public int Accept(dynamic applyInfo)
        {
            //Common syntax            
            var sqlCmd = @"Invitation_Accept";

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

        public int Interview(int id)
        {
            //Common syntax            
            var sqlCmd = @"Invitation_Interview";

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

        public List<IdentityInvitation> GetList()
        {
            //Common syntax            
            var sqlCmd = @"Invitation_GetList";

            //For parameters
            var parameters = new Dictionary<string, object>
            {                
            };

            List<IdentityInvitation> listData = new List<IdentityInvitation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, null))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractInvitationData(reader);

                            listData.Add(record);
                        }

                        //var allLangList = new List<IdentityInvitationLang>();

                        ////Get data for all languages
                        //if (reader.NextResult())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var langItem = new IdentityInvitationLang();
                        //        langItem.Id = Utils.ConvertToInt32(reader["Id"]);
                        //        langItem.LangCode = reader["LangCode"].ToString();
                        //        langItem.Name = reader["Name"].ToString();
                        //        langItem.UrlFriendly = reader["UrlFriendly"].ToString();
                        //        langItem.InvitationId = Utils.ConvertToInt32(reader["InvitationId"]);

                        //        allLangList.Add(langItem);
                        //    }
                        //}

                        //if (listData.Count > 0)
                        //{
                        //    foreach (var item in listData)
                        //    {
                        //        item.LangList = allLangList.Where(x => x.InvitationId == item.Id).ToList();
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

        public List<IdentityInvitation> GetListByJobSeeker(int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Invitation_GetListByJobSeeker";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_seeker_id", job_seeker_id},
            };

            List<IdentityInvitation> listData = new List<IdentityInvitation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractInvitationData(reader);

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

        public List<IdentityInvitation> GetListByJob(int job_id)
        {
            //Common syntax            
            var sqlCmd = @"Invitation_GetListByJob";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
            };

            List<IdentityInvitation> listData = new List<IdentityInvitation>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        while (reader.Read())
                        {
                            var record = ExtractInvitationData(reader);

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

        public IdentityInvitation CheckJobApplied(int job_id, int job_seeker_id)
        {
            //Common syntax            
            var sqlCmd = @"Invitation_CheckJobApplied";

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@job_id", job_id},
                {"@job_seeker_id", job_seeker_id}
            };

            IdentityInvitation info = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var reader = MsSqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, sqlCmd, parameters))
                    {
                        if (reader.Read())
                        {
                            info = ExtractInvitationData(reader);
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

        public int Invite(IdentityInvitation identity)
        {
            //Common syntax           
            var sqlCmd = @"Job_ApplicationInvite";
            var newId = 0;

            //var jobSeekerIds = string.Join(",", identity.Invitations.Select(x=>x.job_seeker_id).ToList());
            var job_seekers = JsonConvert.SerializeObject(identity.Invitations);

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@company_id", identity.company_id },
                {"@job_id", identity.job_id },
                {"@job_seekers", job_seekers },
                {"@note", identity.note },
                {"@staff_id", identity.staff_id }
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

        public int InviteMultiJobs(IdentityInvitation identity)
        {
            //Common syntax           
            var sqlCmd = @"Job_ApplicationInvite_MultiJobs";
            var newId = 0;

            //For parameters
            var parameters = new Dictionary<string, object>
            {
                {"@agency_id", identity.agency_id},
                {"@job_seeker_id", identity.job_seeker_id },
                {"@job_ids", identity.job_ids },
                {"@note", identity.note },
                {"@staff_id", identity.staff_id }
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

        public dynamic InvitationChecking(dynamic filter)
        {
            //Common syntax           
            var sqlCmd = @"Job_InvitationChecking";
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
