using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailConsoler.Logging;
using EmailConsoler.Models;

namespace EmailConsoler.DataStorage
{
    public interface IMySQLVodStore
    {
        //Genre
        Task<SqlObjectResponse> TransferQueueForGenre();
        Task<SqlObjectResponse> PickMetadataFromQueueGenre();
        Task<SqlObjectResponse> GetMetadataFromListGenre(string genreIdValues);
        Task<SqlObjectResponse> MoveQueueToProcessGenre(string fileId, string fileName);
        Task<SqlObjectResponse> MoveQueueToFailedGenre(string fileId, string message);
        Task<SqlObjectResponse> MoveProcessToFailedGenre(string fileId, string message);
        Task<SqlObjectResponse> UpdateAfterSentFileGenre(string fileId);
        Task<SqlObjectResponse> GetAllGenresAndPackages(bool isIncludeParent = true);

        //Container
        Task<SqlObjectResponse> TransferQueueForContainer();
        Task<SqlObjectResponse> PickMetadataFromQueueContainer();
        Task<SqlObjectResponse> MoveQueueToProcessContainer(string fileId, string fileName);
        Task<SqlObjectResponse> MoveQueueToFailedContainer(string fileId, string message);
        Task<SqlObjectResponse> MoveProcessToFailedContainer(string fileId, string message);
        Task<SqlObjectResponse> UpdateAfterSentFileContainer(string fileId);

        //Collection
        Task<SqlObjectResponse> TransferQueueForCollection();
        Task<SqlObjectResponse> PickMetadataFromQueueCollection();
        Task<SqlObjectResponse> MoveQueueToProcessCollection(string fileId, string fileName);
        Task<SqlObjectResponse> MoveQueueToFailedCollection(string fileId, string message);
        Task<SqlObjectResponse> MoveProcessToFailedCollection(string fileId, string message);
        Task<SqlObjectResponse> UpdateAfterSentFileCollection(string fileId);

        //Program
        Task<SqlObjectResponse> TransferQueueForProgram();
        Task<SqlObjectResponse> PickMetadataFromQueueProgram();
        Task<SqlObjectResponse> MoveQueueToProcessProgram(string fileId, string fileName);
        Task<SqlObjectResponse> MoveQueueToFailedProgram(string fileId, string message);
        Task<SqlObjectResponse> MoveProcessToFailedProgram(string fileId, string message);
        Task<SqlObjectResponse> UpdateAfterSentFileProgram(string fileId);
    }

    public class MySQLVodStore : IMySQLVodStore
    {
        private ILog logger = LogProvider.For<MySQLVodStore>();
        private readonly string _connectionString;

        public MySQLVodStore()
            : this("MySQLVodDB")
        {

        }

        public MySQLVodStore(string connectionStringName)
        {
            _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        #region Genres

        public virtual async Task<SqlObjectResponse> TransferQueueForGenre()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Genre_TransferQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, null);
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to TransferQueueForGenre: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> PickMetadataFromQueueGenre()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Genre_PickFromQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingGenreXMLMetadataFromDataReader, null);
                sqlResponse = executedResult;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to PickMetadataFromQueueGenre: {0}.", ex.ToString());
                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> GetMetadataFromListGenre(string genreIdValues)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            var parameters = new Dictionary<string, object>
            {
                {"@p_GenreIdValues", genreIdValues}
            };

            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Genre_GetFromList";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingGenreMetadataFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to GetMetadataFromListGenre: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        private List<GenreEntity> ParsingGenreMetadataFromDataReader(MySqlDataReader reader)
        {
            GenreEntity entity = null;
            List<GenreEntity> list = new List<GenreEntity>();
            while (reader.Read())
            {
                entity = new GenreEntity();
                entity.GenreId = reader["GenreId"].ToString();
                entity.ParentId = reader["ParentId"].ToString();
                entity.Title_EN = reader["Title_EN"].ToString();

                entity.Title_VI = reader["Title_VI"].ToString();
                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
                entity.Status = Convert.ToInt32(reader["Status"]);
                entity.StatusDesc = reader["StatusDesc"].ToString();

                list.Add(entity);
            }

            return list;
        }

        private GenreXMLEntity ParsingGenreXMLMetadataFromDataReader(MySqlDataReader reader)
        {
            GenreXMLEntity entity = null;
            if (reader.Read())
            {
                entity = new GenreXMLEntity();
                entity.GenreIdValues = reader["GenreIdValues"].ToString();

                entity.Status = Convert.ToInt32(reader["Status"]);
                entity.StatusDesc = reader["StatusDesc"].ToString();

                entity.XMLFileId = reader["XMLFileId"].ToString();
                entity.XmlFileName = reader["XmlFileName"].ToString();
                entity.XMLCreatedDate = (DateTime)reader["XMLCreatedDate"];
                entity.XMLAckStatus = reader["XMLAckStatus"] == DBNull.Value ? null : reader["XMLAckStatus"].ToString();
                entity.XMLAckDate = reader["XMLAckDate"] == DBNull.Value ? null : (DateTime?)reader["XMLAckDate"];
                entity.XMLAckMessage = reader["XMLAckMessage"] == DBNull.Value ? null : reader["XMLAckMessage"].ToString();
            }

            return entity;
        }

        private VodMetadata ParsingGenresAndPackagesFromDataReader(MySqlDataReader reader)
        {
            VodMetadata vodMetadata = new VodMetadata();
            while (reader.Read())
            {
                vodMetadata.Genres = ParsingGenreMetadataFromDataReader(reader);
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var package = new SubscriptionPackageEntity();
                    package.PackageId = reader["PackageId"].ToString();
                    package.IsHD = Convert.ToInt32(reader["IsHD"]);
                    package.IsFree = Convert.ToInt32(reader["IsFree"]);
                    package.PackageRef = reader["PackageRef"].ToString();
                    package.Active = Convert.ToInt32(reader["Active"]);

                    vodMetadata.Packages.Add(package);
                }
            }

            return vodMetadata;
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToProcessGenre(string fileId, string fileName)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_XMLFileName", fileName}
            };
            try
            {
                var sqlCmd = @"Vod_Genre_MoveQueueToProcess";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToProcessForGenre: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToFailedGenre(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Genre_MoveQueueToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToFailedGenre: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveProcessToFailedGenre(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Genre_MoveProcessToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveProcessToFailedGenre: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> UpdateAfterSentFileGenre(string fileId)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId}
            };

            try
            {
                var sqlCmd = @"Vod_Genre_UpdateAfterSentFile";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to UpdateAfterSentFileGenre: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> GetAllGenresAndPackages(bool isIncludeParent = true)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
               { "p_IsIncludeParent", (isIncludeParent)? 1 : 0 }
            };

            try
            {
                var sqlCmd = @"Vod_GetAllGenresAndPackages";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingGenresAndPackagesFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to GetAllGenresAndPackages: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        #endregion

        #region Containers

        public virtual async Task<SqlObjectResponse> TransferQueueForContainer()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Container_TransferQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, null);
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to TransferQueueForContainer: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> PickMetadataFromQueueContainer()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Container_PickFromQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingContainerMetadataFromDataReader, null);
                sqlResponse = executedResult;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to PickMetadataFromQueueContainer: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        private ContainerEntityXml ParsingContainerMetadataFromDataReader(MySqlDataReader reader)
        {
            ContainerEntityXml entity = null;

            if (reader.Read())
            {
                entity = new ContainerEntityXml();
                entity.ContainerId = reader["ContainerId"].ToString();
                entity.Name = reader["Name"].ToString();
                entity.IsFree = Convert.ToInt32(reader["IsFree"]);
                entity.IsHD = Convert.ToInt32(reader["IsHD"]);
                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
                entity.Title_EN = reader["Title_EN"].ToString();
                entity.Title_VI = reader["Title_VI"].ToString();
                entity.Summary_EN = reader["Summary_EN"].ToString();
                entity.Summary_VI = reader["Summary_VI"].ToString();                
                entity.Rating = reader["Rating"].ToString();
                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
                entity.Year = Convert.ToInt32(reader["Year"]);
                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
                entity.Genre = reader["Genre"].ToString();
                entity.Subgenre = reader["Subgenre"].ToString();
                entity.SubGenres = reader["SubGenres"].ToString();
                entity.Image = reader["Image"].ToString();

                entity.Status = Convert.ToInt32(reader["Status"]);
                entity.StatusDesc = reader["StatusDesc"].ToString();
                entity.NumLine = Convert.ToInt32(reader["NumLine"]);

                entity.XMLFileId = reader["XMLFileId"].ToString();
                entity.XmlFileName = reader["XmlFileName"].ToString();
                entity.XMLCreatedDate = (DateTime)reader["XMLCreatedDate"];
                entity.XMLAckStatus = reader["XMLAckStatus"] == DBNull.Value ? null : reader["XMLAckStatus"].ToString();
                entity.XMLAckDate = reader["XMLAckDate"] == DBNull.Value ? null : (DateTime?)reader["XMLAckDate"];
                entity.XMLAckMessage = reader["XMLAckMessage"] == DBNull.Value ? null : reader["XMLAckMessage"].ToString();
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var collectionEntity = ParsingCollectionEntityFromDataReader(reader);
                    if (collectionEntity != null)
                    {
                        entity.Collections.Add(collectionEntity);
                    }
                }
            }

            return entity;
        }

        private ContainerEntity ParsingContaineEntityFromDataReader(MySqlDataReader reader)
        {
            ContainerEntityXml entity = null;

            if (reader.Read())
            {
                entity = new ContainerEntityXml();
                entity.ContainerId = reader["ContainerId"].ToString();
                entity.Name = reader["Name"].ToString();
                entity.IsFree = Convert.ToInt32(reader["IsFree"]);
                entity.IsHD = Convert.ToInt32(reader["IsHD"]);
                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
                entity.Title_EN = reader["Title_EN"].ToString();
                entity.Title_VI = reader["Title_VI"].ToString();
                entity.Summary_EN = reader["Summary_EN"].ToString();
                entity.Summary_VI = reader["Summary_VI"].ToString();
                entity.Rating = reader["Rating"].ToString();
                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
                entity.Year = Convert.ToInt32(reader["Year"]);
                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
                entity.Genre = reader["Genre"].ToString();
                entity.Subgenre = reader["Subgenre"].ToString();
                entity.SubGenres = reader["SubGenres"].ToString();
                entity.Image = reader["Image"].ToString();

                entity.Status = Convert.ToInt32(reader["Status"]);
                entity.StatusDesc = reader["StatusDesc"].ToString();
                entity.NumLine = Convert.ToInt32(reader["NumLine"]);
            }

            return entity;
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToProcessContainer(string fileId, string fileName)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_XMLFileName", fileName}
            };
            try
            {
                var sqlCmd = @"Vod_Container_MoveQueueToProcess";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToProcessForContainer: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToFailedContainer(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Container_MoveQueueToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToFailedContainer: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveProcessToFailedContainer(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Container_MoveProcessToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveProcessToFailedContainer: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> UpdateAfterSentFileContainer(string fileId)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId}
            };

            try
            {
                var sqlCmd = @"Vod_Container_UpdateAfterSentFile";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to UpdateAfterSentFileContainer: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }
        
        #endregion

        #region Collections

        public virtual async Task<SqlObjectResponse> TransferQueueForCollection()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Collection_TransferQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, null);
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to TransferQueueForCollection: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> PickMetadataFromQueueCollection()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Collection_PickFromQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingCollectionMetadataFromDataReader, null);
                sqlResponse = executedResult;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to PickMetadataFromQueueCollection: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        private CollectionEntityXml ParsingCollectionMetadataFromDataReader(MySqlDataReader reader)
        {
            CollectionEntityXml entity = null;

            if (reader.Read())
            {
                entity = new CollectionEntityXml();
                entity.ContainerId = reader["ContainerId"].ToString();
                entity.CollectionId = reader["CollectionId"].ToString();
                entity.Name = reader["Name"].ToString();
                entity.Title_EN = reader["Title_EN"].ToString();
                entity.Title_VI = reader["Title_VI"].ToString();
                entity.Summary_EN = reader["Summary_EN"].ToString();
                entity.Summary_VI = reader["Summary_VI"].ToString();
                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
                entity.Rating = reader["Rating"].ToString();
                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
                entity.Year = Convert.ToInt32(reader["Year"]);
                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
                entity.Genre = reader["Genre"].ToString();
                entity.Subgenre = reader["Subgenre"].ToString();
                entity.SubGenres = reader["SubGenres"].ToString();
                entity.Image = reader["Image"].ToString();

                entity.Status = Convert.ToInt32(reader["Status"]);
                entity.StatusDesc = reader["StatusDesc"].ToString();
                entity.NumLine = Convert.ToInt32(reader["NumLine"]);

                entity.XMLFileId = reader["XMLFileId"].ToString();
                entity.XmlFileName = reader["XmlFileName"].ToString();
                entity.XMLCreatedDate = (DateTime)reader["XMLCreatedDate"];
                entity.XMLAckStatus = reader["XMLAckStatus"] == DBNull.Value ? null : reader["XMLAckStatus"].ToString();
                entity.XMLAckDate = reader["XMLAckDate"] == DBNull.Value ? null : (DateTime?)reader["XMLAckDate"];
                entity.XMLAckMessage = reader["XMLAckMessage"] == DBNull.Value ? null : reader["XMLAckMessage"].ToString();
            }

            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var programEntity = ParsingProgramEntityFromDataReader(reader);
                    if (programEntity != null)
                    {
                        entity.Programs.Add(programEntity);
                    }
                }
            }

            if (reader.NextResult())
            {
                entity.Container = ParsingContaineEntityFromDataReader(reader);
            }

            return entity;
        }

        private CollectionEntity ParsingCollectionEntityFromDataReader(MySqlDataReader reader)
        {
            CollectionEntity entity = null;

            entity = new CollectionEntity();
            entity.ContainerId = reader["ContainerId"].ToString();
            entity.CollectionId = reader["CollectionId"].ToString();
            entity.Name = reader["Name"].ToString();
            entity.Title_EN = reader["Title_EN"].ToString();
            entity.Title_VI = reader["Title_VI"].ToString();
            entity.Summary_EN = reader["Summary_EN"].ToString();
            entity.Summary_VI = reader["Summary_VI"].ToString();
            entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
            entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
            entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
            entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
            entity.Rating = reader["Rating"].ToString();
            entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
            entity.Year = Convert.ToInt32(reader["Year"]);
            entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
            entity.Genre = reader["Genre"].ToString();
            entity.Subgenre = reader["Subgenre"].ToString();
            entity.SubGenres = reader["SubGenres"].ToString();
            entity.Image = reader["Image"].ToString();

            entity.Status = Convert.ToInt32(reader["Status"]);
            entity.StatusDesc = reader["StatusDesc"].ToString();
            entity.NumLine = Convert.ToInt32(reader["NumLine"]);

            return entity;
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToProcessCollection(string fileId, string fileName)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_XMLFileName", fileName}
            };
            try
            {
                var sqlCmd = @"Vod_Collection_MoveQueueToProcess";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToProcessForCollection: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToFailedCollection(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Collection_MoveQueueToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToFailedCollection: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveProcessToFailedCollection(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Collection_MoveProcessToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveProcessToFailedCollection: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> UpdateAfterSentFileCollection(string fileId)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId}
            };

            try
            {
                var sqlCmd = @"Vod_Collection_UpdateAfterSentFile";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to UpdateAfterSentFileCollection: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        #endregion

        #region Programs

        public virtual async Task<SqlObjectResponse> TransferQueueForProgram()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Program_TransferQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, null);
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to TransferQueueForProgram: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> PickMetadataFromQueueProgram()
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();
            try
            {
                var sqlCmd = @"Vod_Program_PickFromQueue";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingProgramMetadataFromDataReader, null);
                sqlResponse = executedResult;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to PickMetadataFromQueueProgram: {0}.", ex.ToString());
                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        private ProgramEntityXml ParsingProgramMetadataFromDataReader(MySqlDataReader reader)
        {
            ProgramEntityXml entity = null;

            if (reader.Read())
            {
                entity = new ProgramEntityXml();
                entity.ProgramId = reader["ProgramId"].ToString();
                entity.CollectionId = reader["CollectionId"].ToString();
                entity.ContainerId = reader["ContainerId"].ToString();
                entity.Name = reader["Name"].ToString();
                entity.IsFree = Convert.ToInt32(reader["IsFree"]);
                entity.IsHD = Convert.ToInt32(reader["IsHD"]);
                entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
                entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
                entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
                entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
                entity.TopContentStart = reader["TopContentStart"] == DBNull.Value ? null : (DateTime?)reader["TopContentStart"];
                entity.TopContentEnd = reader["TopContentEnd"] == DBNull.Value ? null : (DateTime?)reader["TopContentEnd"];
                entity.Title_EN = reader["Title_EN"].ToString();
                entity.Title_VI = reader["Title_VI"].ToString();
                entity.Summary_EN = reader["Summary_EN"].ToString();
                entity.Summary_VI = reader["Summary_VI"].ToString();
                entity.Rating = reader["Rating"].ToString();
                entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
                entity.Year = Convert.ToInt32(reader["Year"]);
                entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
                entity.Genre = reader["Genre"].ToString();
                entity.Subgenre = reader["Subgenre"].ToString();
                entity.SubGenres = reader["SubGenres"].ToString();
                entity.Image = reader["Image"].ToString();

                entity.Actors = reader["Actors"].ToString();
                entity.Directors = reader["Directors"].ToString();
                entity.Producers = reader["Producers"].ToString();
                entity.EpisodeName_EN = reader["EpisodeName_EN"].ToString();
                entity.EpisodeName_VI = reader["EpisodeName_VI"].ToString();
                entity.EpisodeID = reader["EpisodeID"].ToString();
                entity.MovieFileName = reader["MovieFileName"].ToString();
                entity.MovieFileSize = reader["MovieFileSize"].ToString();
                entity.MovieCheckSum = reader["MovieCheckSum"].ToString();
                entity.MovieDuration = Convert.ToInt32(reader["MovieDuration"]);

                entity.Status = Convert.ToInt32(reader["Status"]);
                entity.StatusDesc = reader["StatusDesc"].ToString();
                entity.NumLine = Convert.ToInt32(reader["NumLine"]);

                entity.XMLFileId = reader["XMLFileId"].ToString();
                entity.XmlFileName = reader["XmlFileName"].ToString();
                entity.XMLCreatedDate = (DateTime)reader["XMLCreatedDate"];
                entity.XMLAckStatus = reader["XMLAckStatus"] == DBNull.Value ? null : reader["XMLAckStatus"].ToString();
                entity.XMLAckDate = reader["XMLAckDate"] == DBNull.Value ? null : (DateTime?)reader["XMLAckDate"];
                entity.XMLAckMessage = reader["XMLAckMessage"] == DBNull.Value ? null : reader["XMLAckMessage"].ToString();              
            }

            if (reader.NextResult())
            {
                if (reader.Read())
                {
                    entity.Collection = ParsingCollectionEntityFromDataReader(reader);                    
                }
            }

            return entity;
        }

        private ProgramEntity ParsingProgramEntityFromDataReader(MySqlDataReader reader)
        {
            ProgramEntity entity = null;
            entity = new ProgramEntity();
            entity.ProgramId = reader["ProgramId"].ToString();
            entity.CollectionId = reader["CollectionId"].ToString();
            entity.ContainerId = reader["ContainerId"].ToString();
            entity.Name = reader["Name"].ToString();
            entity.IsFree = Convert.ToInt32(reader["IsFree"]);
            entity.IsHD = Convert.ToInt32(reader["IsHD"]);
            entity.CreationDateTime = reader["CreationDateTime"] == DBNull.Value ? null : (DateTime?)reader["CreationDateTime"];
            entity.PublishDateTime = reader["PublishDateTime"] == DBNull.Value ? null : (DateTime?)reader["PublishDateTime"];
            entity.StartDateTime = reader["StartDateTime"] == DBNull.Value ? null : (DateTime?)reader["StartDateTime"];
            entity.EndDateTime = reader["EndDateTime"] == DBNull.Value ? null : (DateTime?)reader["EndDateTime"];
            entity.Title_EN = reader["Title_EN"].ToString();
            entity.Title_VI = reader["Title_VI"].ToString();
            entity.Summary_EN = reader["Summary_EN"].ToString();
            entity.Summary_VI = reader["Summary_VI"].ToString();
            entity.Rating = reader["Rating"].ToString();
            entity.DisplayRunTime = reader["DisplayRunTime"].ToString();
            entity.Year = Convert.ToInt32(reader["Year"]);
            entity.CountryOfOrigin = reader["CountryOfOrigin"].ToString();
            entity.Genre = reader["Genre"].ToString();
            entity.Subgenre = reader["Subgenre"].ToString();
            entity.SubGenres = reader["SubGenres"].ToString();
            entity.Image = reader["Image"].ToString();

            entity.Actors = reader["Actors"].ToString();
            entity.Directors = reader["Directors"].ToString();
            entity.Producers = reader["Producers"].ToString();
            entity.EpisodeName_EN = reader["EpisodeName_EN"].ToString();
            entity.EpisodeName_VI = reader["EpisodeName_VI"].ToString();
            entity.EpisodeID = reader["EpisodeID"].ToString();
            entity.MovieFileName = reader["MovieFileName"].ToString();
            entity.MovieFileSize = reader["MovieFileSize"].ToString();
            entity.MovieCheckSum = reader["MovieCheckSum"].ToString();
            entity.MovieDuration = Convert.ToInt32(reader["MovieDuration"]);

            entity.Status = Convert.ToInt32(reader["Status"]);
            entity.StatusDesc = reader["StatusDesc"].ToString();
            entity.NumLine = Convert.ToInt32(reader["NumLine"]);

            return entity;
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToProcessProgram(string fileId, string fileName)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_XMLFileName", fileName}
            };
            try
            {
                var sqlCmd = @"Vod_Program_MoveQueueToProcess";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToProcessForProgram: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveQueueToFailedProgram(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Program_MoveQueueToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveQueueToFailedProgram: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> MoveProcessToFailedProgram(string fileId, string message)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId},
                {"@p_Message", message}
            };
            try
            {
                var sqlCmd = @"Vod_Program_MoveProcessToFailed";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to MoveProcessToFailedProgram: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        public virtual async Task<SqlObjectResponse> UpdateAfterSentFileProgram(string fileId)
        {
            var mc = new ManageConnection(_connectionString);
            var strError = string.Empty;
            SqlObjectResponse sqlResponse = new SqlObjectResponse();

            var parameters = new Dictionary<string, object>
            {
                {"@p_XMLFileId", fileId}
            };

            try
            {
                var sqlCmd = @"Vod_Program_UpdateAfterSentFile";

                var executedResult = (SqlObjectResponse)mc.ExecuteReader(sqlCmd, CommandType.StoredProcedure, ParsingSqlResponseFromDataReader, parameters.ToMySqlParams());
                sqlResponse = executedResult.ObjectResult as SqlObjectResponse;

                return await Task.FromResult(sqlResponse);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed to UpdateAfterSentFileProgram: {0}.", ex.ToString());

                sqlResponse.ErrorMessage = strError;
            }
            finally
            {
                mc.Dispose();
            }

            return await Task.FromResult(sqlResponse);
        }

        #endregion

        private SqlObjectResponse ParsingSqlResponseFromDataReader(MySqlDataReader reader)
        {
            var entity = new SqlObjectResponse();

            if (reader.Read())
            {
                entity.RowsAffected = Convert.ToInt32(reader["ROWSAFFECTED"]);
                entity.ErrorCode = reader["ERRORCODE"].ToString();
                entity.ErrorMessage = reader["ERRORMSG"].ToString();
            }

            return entity;
        }
    }
}
