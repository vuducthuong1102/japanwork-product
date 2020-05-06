//using MySql.Data.MySqlClient;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using EmailConsoler.Logging;

//namespace EmailConsoler.DataStorage
//{
//    public interface IMySQLVodStore
//    {
//        Task<List<GenreEntity>> GetUnProcessGenres();
//        Task<string> InsertGenre(GenreEntity entity);

//        Task<List<ContainerEntity>> GetUnProcessContainers(int status);
//        Task<string> InsertContainer(ContainerEntity entity);

//        Task<List<CollectionEntity>> GetUnProcessCollections(int status);
//        Task<string> InsertCollection(CollectionEntity entity);

//        Task<List<ProgramEntity>> GetUnProcessPrograms(int status);
//        Task<string> InsertProgram(ProgramEntity entity);

//        Task<string> UpdateStatus(string type, string ids, int status, string message);
//    }

//    public class MySQLVodStore: IMySQLVodStore
//    {
//        private ILog logger = LogProvider.For<MySQLVodStore>();
//        private readonly string _connectionString;

//        public MySQLVodStore()
//            : this("MySQLVodDB")
//        {

//        }

//        public MySQLVodStore(string connectionStringName)
//        {
//            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;           
//        }

//        #region Genres

//        public virtual async Task<List<GenreEntity>> GetUnProcessGenres()
//        {
//            var mc = new ManageConnection(_connectionString);

//            try
//            {
//                var sqlCmd = @"Vod_Genre_GetUnprocess";
//                //var parameters = new Dictionary<string, object>
//                //{
//                //    {"@p_Status", status}
//                //};

//                var entities = (List<GenreEntity>)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingGenresFromDataReader, null);
//                return await Task.FromResult(entities);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUnprocessGenres. Error: " + ex.ToString();
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            return await Task.FromResult(default(List<GenreEntity>));
//        }

//        private List<GenreEntity> ParsingGenresFromDataReader(MySqlDataReader reader)
//        {
//            List<GenreEntity> entities = new List<GenreEntity>();

//            while (reader.Read())
//            {
//                var entity = new GenreEntity();

//                entity.GenreId = reader["GenreId"].ToString();
//                entity.ParentId = reader["ParentId"].ToString();
//                entity.Title_EN = reader["Title_EN"].ToString();

//                entity.Title_VI = reader["Title_VI"].ToString();
//                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
//                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
//                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
//                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
//                entity.Status = Convert.ToInt32(reader["Status"]);
//                entity.StatusDesc = reader["StatusDesc"].ToString();

//                entities.Add(entity);
//            }

//            return entities;
//        }

//        public virtual async Task<string> InsertGenre(GenreEntity entity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var strError = string.Empty;
//            try
//            {
//                var sqlCmd = @"Vod_Genre_Insert";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_GenreId", entity.GenreId},
//                    {"@p_ParentId", entity.ParentId},
//                    {"@p_Title_EN", entity.Title_EN},
//                    {"@p_Title_VI", entity.Title_VI},
//                    {"@p_CreationDateTime", entity.CreationDateTime},
//                    {"@p_PublishDateTime", entity.PublishDateTime},
//                    {"@p_StartDateTime", entity.StartDateTime},
//                    {"@p_EndDateTime", entity.EndDateTime}
//                };

//                var paramList = parameters.ToMySqlParams().ToList();
//                var newId = ManageConnection.CreateOutParam("p_new_id", MySqlDbType.VarChar, 128);
//                paramList.Add(newId);

//                var executeResult = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, paramList.ToArray());

//                await Task.FromResult(executeResult);
//                if (executeResult)
//                {
//                    if (newId.Value.ToString() == "-1")
//                    {
//                        strError = string.Format("The Genre with Id = {0} is existed", entity.GenreId);
//                    }
//                }                
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed to InsertGenre: {0}. Error: {1}", entity.GenreId, ex.ToString());
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            if (!string.IsNullOrEmpty(strError))
//            {
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }

//            return await Task.FromResult(strError);
//        }

//        #endregion

//        #region Containers

//        public virtual async Task<List<ContainerEntity>> GetUnProcessContainers(int status)
//        {
//            var mc = new ManageConnection(_connectionString);

//            try
//            {
//                var sqlCmd = @"Vod_Container_GetUnprocess";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_Status", status}
//                };

//                var entities = (List<ContainerEntity>)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingContainersFromDataReader, parameters.ToMySqlParams());
//                return await Task.FromResult(entities);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUnprocessContainers. Error: " + ex.ToString();
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            return await Task.FromResult(default(List<ContainerEntity>));
//        }

//        private List<ContainerEntity> ParsingContainersFromDataReader(MySqlDataReader reader)
//        {
//            List<ContainerEntity> entities = new List<ContainerEntity>();

//            while (reader.Read())
//            {
//                var entity = new ContainerEntity();

//                entity.ContainerId = reader["ContainerId"].ToString();
//                entity.Name = reader["Name"].ToString();
//                entity.IsHD = Convert.ToInt32(reader["IsHD"]);
//                entity.IsFree = Convert.ToInt32(reader["IsFree"]);
//                entity.Title_EN = reader["Title_EN"].ToString();
//                entity.Title_VI = reader["Title_VI"].ToString();
//                entity.Summary_EN = reader["Summary_EN"].ToString();
//                entity.Summary_VI = reader["Summary_VI"].ToString();
//                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
//                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
//                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
//                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
//                entity.Rating = reader["Rating"].ToString();
//                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
//                entity.Year = Convert.ToInt32(reader["Year"]);
//                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
//                entity.Genre = reader["Genre"].ToString();
//                entity.Subgenre = reader["Subgenre"].ToString();
//                entity.SubGenres = reader["SubGenres"].ToString();
//                entity.Image = reader["Image"].ToString();

//                entity.Status = Convert.ToInt32(reader["Status"]);
//                entity.StatusDesc = reader["StatusDesc"].ToString();

//                entities.Add(entity);
//            }

//            return entities;
//        }

//        public virtual async Task<string> InsertContainer(ContainerEntity entity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var strError = string.Empty;
//            try
//            {
//                var sqlCmd = @"Vod_Container_Insert";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_ContainerId", entity.ContainerId},
//                    {"@p_Name", entity.Name},
//                    {"@p_IsHD", entity.IsHD},
//                    {"@p_IsFree", entity.IsFree},
//                    {"@p_CreationDateTime", entity.CreationDateTime},
//                    {"@p_PublishDateTime", entity.PublishDateTime},
//                    {"@p_StartDateTime", entity.StartDateTime},
//                    {"@p_EndDateTime", entity.EndDateTime},
//                    {"@p_Title_EN", entity.Title_EN},
//                    {"@p_Title_VI", entity.Title_VI},
//                    {"@p_Summary_EN", entity.Summary_EN},
//                    {"@p_Summary_VI", entity.Summary_VI},
//                    {"@p_Rating", entity.Rating},
//                    {"@p_DisplayRunTime", entity.DisplayRunTime},
//                    {"@p_Year", entity.Year},
//                    {"@p_CountryOfOrigin", entity.CountryOfOrigin},
//                    {"@p_Genre", entity.Genre},
//                    {"@p_Subgenre", entity.Subgenre},
//                    {"@p_Subgenres", entity.SubGenres},
//                    {"@p_Image", entity.Image}
//                };

//                var paramList = parameters.ToMySqlParams().ToList();
//                var newId = ManageConnection.CreateOutParam("p_new_id", MySqlDbType.VarChar, 128);
//                paramList.Add(newId);

//                var executeResult = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, paramList.ToArray());
//                await Task.FromResult(executeResult);
//                if (executeResult)
//                {
//                    if (newId.Value.ToString() == "-1")
//                    {
//                        strError = string.Format("The Container with Id = {0} is existed", entity.ContainerId);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed to InsertContainer: {0}. Error: {1}", entity.ContainerId, ex.ToString());
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            if (!string.IsNullOrEmpty(strError))
//            {
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }

//            return await Task.FromResult(strError);
//        }

//        #endregion

//        #region Collections

//        public virtual async Task<List<CollectionEntity>> GetUnProcessCollections(int status)
//        {
//            var mc = new ManageConnection(_connectionString);

//            try
//            {
//                var sqlCmd = @"Vod_Collection_GetUnprocess";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_Status", status}
//                };

//                var entities = (List<CollectionEntity>)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingCollectionsFromDataReader, parameters.ToMySqlParams());
//                return await Task.FromResult(entities);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUnprocessCollections. Error: " + ex.ToString();
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            return await Task.FromResult(default(List<CollectionEntity>));
//        }

//        private List<CollectionEntity> ParsingCollectionsFromDataReader(MySqlDataReader reader)
//        {
//            List<CollectionEntity> entities = new List<CollectionEntity>();

//            while (reader.Read())
//            {
//                var entity = new CollectionEntity();

//                entity.ContainerId = reader["ContainerId"].ToString();
//                entity.CollectionId = reader["CollectionId"].ToString();
//                entity.Name = reader["Name"].ToString();
//                entity.Title_EN = reader["Title_EN"].ToString();
//                entity.Title_VI = reader["Title_VI"].ToString();
//                entity.Summary_EN = reader["Summary_EN"].ToString();
//                entity.Summary_VI = reader["Summary_VI"].ToString();
//                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
//                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
//                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
//                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
//                entity.Rating = reader["Rating"].ToString();
//                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
//                entity.Year = Convert.ToInt32(reader["Year"]);
//                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
//                entity.Genre = reader["Genre"].ToString();
//                entity.Subgenre = reader["Subgenre"].ToString();
//                entity.SubGenres = reader["SubGenres"].ToString();
//                entity.Image = reader["Image"].ToString();

//                entity.Status = Convert.ToInt32(reader["Status"]);
//                entity.StatusDesc = reader["StatusDesc"].ToString();

//                entities.Add(entity);
//            }

//            return entities;
//        }

//        public virtual async Task<string> InsertCollection(CollectionEntity entity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var strError = string.Empty;
//            try
//            {
//                var sqlCmd = @"Vod_Collection_Insert";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_CollectionId", entity.CollectionId},
//                    {"@p_ContainerId", entity.ContainerId},
//                    {"@p_Name", entity.Name},
//                    {"@p_CreationDateTime", entity.CreationDateTime},
//                    {"@p_PublishDateTime", entity.PublishDateTime},
//                    {"@p_StartDateTime", entity.StartDateTime},
//                    {"@p_EndDateTime", entity.EndDateTime},
//                    {"@p_Title_EN", entity.Title_EN},
//                    {"@p_Title_VI", entity.Title_VI},
//                    {"@p_Summary_EN", entity.Summary_EN},
//                    {"@p_Summary_VI", entity.Summary_VI},
//                    {"@p_Rating", entity.Rating},
//                    {"@p_DisplayRunTime", entity.DisplayRunTime},
//                    {"@p_Year", entity.Year},
//                    {"@p_CountryOfOrigin", entity.CountryOfOrigin},
//                    {"@p_Genre", entity.Genre},
//                    {"@p_Subgenre", entity.Subgenre},
//                    {"@p_Subgenres", entity.SubGenres},
//                    {"@p_Image", entity.Image}
//                };

//                var paramList = parameters.ToMySqlParams().ToList();
//                var newId = ManageConnection.CreateOutParam("p_new_id", MySqlDbType.VarChar, 128);
//                paramList.Add(newId);

//                var executeResult = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, paramList.ToArray());
//                await Task.FromResult(executeResult);
//                if (executeResult)
//                {
//                    if (newId.Value.ToString() == "-1")
//                    {
//                        strError = string.Format("The Collection with Id = {0} is existed", entity.CollectionId);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed to InsertCollection: {0}. Error: {1}", entity.CollectionId, ex.ToString());
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            if (!string.IsNullOrEmpty(strError))
//            {
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }

//            return await Task.FromResult(strError);
//        }



//        public virtual async Task<SqlResponse> TransferQueueForCollection()
//        {
//            var mc = new ManageConnection(_connectionString);
//            var strError = string.Empty;
//            try
//            {
//                var sqlCmd = @"Vod_Collection_TransferQueue";
//                var entity = (SqlResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, null);
//                return await Task.FromResult(entity);
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed to TransferQueueForCollection: {0}. Error: {1}", entity.CollectionId, ex.ToString());
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            if (!string.IsNullOrEmpty(strError))
//            {
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }

//            return await Task.FromResult(strError);
//        }

//        #endregion

//        #region Programs

//        public virtual async Task<List<ProgramEntity>> GetUnProcessPrograms(int status)
//        {
//            var mc = new ManageConnection(_connectionString);

//            try
//            {
//                var sqlCmd = @"Vod_Program_GetUnprocess";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_Status", status}
//                };

//                var entities = (List<ProgramEntity>)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingProgramsFromDataReader, parameters.ToMySqlParams());
//                return await Task.FromResult(entities);

//            }
//            catch (Exception ex)
//            {
//                var strError = "Failed to GetUnprocessPrograms. Error: " + ex.ToString();
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            return await Task.FromResult(default(List<ProgramEntity>));
//        }

//        private List<ProgramEntity> ParsingProgramsFromDataReader(MySqlDataReader reader)
//        {
//            List<ProgramEntity> entities = new List<ProgramEntity>();

//            while (reader.Read())
//            {
//                var entity = new ProgramEntity();

//                entity.ProgramId = reader["ProgramId"].ToString();
//                entity.CollectionId = reader["CollectionId"].ToString();
//                entity.ContainerId = reader["ContainerId"].ToString();
//                entity.Name = reader["Name"].ToString();
//                entity.IsHD = Convert.ToInt32(reader["IsHD"]);
//                entity.IsFree = Convert.ToInt32(reader["IsFree"]);
//                entity.Title_EN = reader["Title_EN"].ToString();
//                entity.Title_VI = reader["Title_VI"].ToString();
//                entity.Summary_EN = reader["Summary_EN"].ToString();
//                entity.Summary_VI = reader["Summary_VI"].ToString();
//                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
//                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
//                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
//                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
//                entity.TopContentStart = reader["TopContentStart"] == DBNull.Value ? null : (DateTime?)reader["TopContentStart"];
//                entity.TopContentEnd = reader["TopContentEnd"] == DBNull.Value ? null : (DateTime?)reader["TopContentEnd"];
//                entity.Rating = reader["Rating"].ToString();
//                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
//                entity.Year = Convert.ToInt32(reader["Year"]);
//                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
//                entity.Genre = reader["Genre"].ToString();
//                entity.Subgenre = reader["Subgenre"].ToString();
//                entity.SubGenres = reader["SubGenres"].ToString();
//                entity.Image = reader["Image"].ToString();

//                entity.Actors = reader["Actors"].ToString();
//                entity.Directors = reader["Directors"].ToString();
//                entity.Producers = reader["Producers"].ToString();
//                entity.EpisodeName_EN = reader["EpisodeName_EN"].ToString();
//                entity.EpisodeName_VI = reader["EpisodeName_VI"].ToString();
//                entity.EpisodeID = reader["EpisodeID"].ToString();
//                entity.MovieFileName = reader["MovieFileName"].ToString();
//                entity.MovieFileSize = reader["MovieFileSize"].ToString();
//                entity.MovieCheckSum = reader["MovieCheckSum"].ToString();
//                entity.MovieDuration = Convert.ToInt32(reader["MovieDuration"]);

//                entity.Status = Convert.ToInt32(reader["Status"]);
//                entity.StatusDesc = reader["StatusDesc"].ToString();

//                entities.Add(entity);
//            }

//            return entities;
//        }

//        public virtual async Task<string> InsertProgram(ProgramEntity entity)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var strError = string.Empty;
//            try
//            {
//                var sqlCmd = @"Vod_Program_Insert";
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_ProgramId", entity.ProgramId},
//                    {"@p_CollectionId", entity.CollectionId},
//                    {"@p_ContainerId", entity.ContainerId},
//                    {"@p_Name", entity.Name},
//                    {"@p_IsHD", entity.IsHD},
//                    {"@p_IsFree", entity.IsFree},
//                    {"@p_CreationDateTime", entity.CreationDateTime},
//                    {"@p_PublishDateTime", entity.PublishDateTime},
//                    {"@p_StartDateTime", entity.StartDateTime},
//                    {"@p_EndDateTime", entity.EndDateTime},
//                    {"@p_TopContentStart", entity.TopContentStart},
//                    {"@p_TopContentEnd", entity.TopContentEnd},
//                    {"@p_Title_EN", entity.Title_EN},
//                    {"@p_Title_VI", entity.Title_VI},
//                    {"@p_Summary_EN", entity.Summary_EN},
//                    {"@p_Summary_VI", entity.Summary_VI},
//                    {"@p_Rating", entity.Rating},
//                    {"@p_DisplayRunTime", entity.DisplayRunTime},
//                    {"@p_Year", entity.Year},
//                    {"@p_CountryOfOrigin", entity.CountryOfOrigin},
//                    {"@p_Genre", entity.Genre},
//                    {"@p_Subgenre", entity.Subgenre},
//                    {"@p_Subgenres", entity.SubGenres},
//                    {"@p_Image", entity.Image},
//                    {"@p_Actors", entity.Actors},
//                    {"@p_Directors", entity.Directors},
//                    {"@p_Producers", entity.Producers},
//                    {"@p_EpisodeName_EN", entity.EpisodeName_EN},
//                    {"@p_EpisodeName_VI", entity.EpisodeName_VI},
//                    {"@p_EpisodeID", entity.EpisodeID},
//                    {"@p_MovieFileName", entity.MovieFileName},
//                    {"@p_MovieFileSize", entity.MovieFileSize},
//                    {"@p_MovieCheckSum", entity.MovieCheckSum},
//                    {"@p_MovieDuration", entity.MovieDuration}
//                };

//                var paramList = parameters.ToMySqlParams().ToList();
//                var newId = ManageConnection.CreateOutParam("p_new_id", MySqlDbType.VarChar, 128);
//                paramList.Add(newId);

//                var executeResult = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, paramList.ToArray());
//                await Task.FromResult(executeResult);
//                if (executeResult)
//                {
//                    if (newId.Value.ToString() == "-1")
//                    {
//                        strError = string.Format("The Program with Id = {0} is existed", entity.ProgramId);
//                    }
//                }

//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed to InsertProgram: {0}. Error: {1}", entity.ProgramId, ex.ToString());
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            if (!string.IsNullOrEmpty(strError))
//            {
//                logger.Error(strError);
//                Consoler.WriteErrorLine(strError);
//            }

//            return await Task.FromResult(strError);
//        }

//        #endregion

//        #region Common

//        public virtual async Task<string> UpdateStatus(string type, string ids, int status, string message)
//        {
//            var mc = new ManageConnection(_connectionString);
//            var strError = string.Empty;
//            try
//            {
//                var sqlCmd = string.Format(@"Vod_{0}_UpdateStatus", type);
//                var parameters = new Dictionary<string, object>
//                {
//                    {"@p_Ids", ids},
//                    {"@p_Status", status},
//                    {"@p_StatusDesc", message}
//                };

//                var executed = mc.ExecuteNonQuery(sqlCmd, CommandType.StoredProcedure, parameters.ToMySqlParams());
//                await Task.FromResult(executed);
//                if (!executed)
//                {
//                    strError = "SQL command has been failed.";
//                }
//            }
//            catch (Exception ex)
//            {
//                strError = string.Format("Failed to Update {0} status. Error: {1}", type, ex.ToString());
//            }
//            finally
//            {
//                mc.Dispose();
//            }

//            if (!string.IsNullOrEmpty(strError))
//            {
//                logger.Error(strError);
//            }

//            return await Task.FromResult(strError);
//        }

//        #endregion
//    }
//}
