//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;

//using StructureMap;

//using EmailConsoler.Configuration;
//using EmailConsoler.Services;
//using EmailConsoler.Models;
//using EmailConsoler.Helpers;
//using System.Web;
//using System.Web.Hosting;
//using EmailConsoler.Libraries;
//using EmailConsoler.Logging;
//using EmailConsoler.DataStorage;

//namespace EmailConsoler.Tasking
//{
//    public class NotificationFileCreator : WorkerThreadEx
//    {
//        private static ILog logger = LogProvider.GetCurrentClassLogger();

//        public NotificationFileCreator(string workerName)
//            : base(workerName, (worker) => ProcessNotification(worker), SystemSettings.WorkerFileCreatorSpeedMS)
//        {

//        }

//        private static void ProcessNotification(WorkerThreadEx worker)
//        {
//            var vodWorker = worker as NotificationFileCreator;
//            ;
//            var toSendFolderPath = SystemSettings.NotifToSendFolder;
//            var sentFolderPath = SystemSettings.NotifSentFolder;

//            if (worker != null && worker.IsSignaled)
//            {
//                var wkName = worker.Name;
//                if (!Directory.Exists(toSendFolderPath))
//                {
//                    Consoler.WriteErrorLine(string.Format("The folder is not found: {0}", toSendFolderPath));
//                    return;
//                }

//                if (!Directory.Exists(sentFolderPath))
//                {
//                    Consoler.WriteErrorLine(string.Format("The folder is not found: {0}", sentFolderPath));
//                    return;
//                }

//                //if (currentDataType == VodDataTypes.Genre.ToLower())
//                //{
//                //    ProcessFileCreatingGenre();
//                //}
//                //else if (currentDataType == VodDataTypes.Container.ToLower())
//                //{
//                //    ProcessFileCreatingContainer();
//                //}
//                //else if (currentDataType == VodDataTypes.Collection.ToLower())
//                //{
//                //    ProcessFileCreatingCollection();
//                //}
//                //else if (currentDataType == VodDataTypes.Program.ToLower())
//                //{
//                //    ProcessFileCreatingProgram();
//                //}
//            }
//        }

//        private static void CreateJsonFile()
//        {
//            var _vodService = Program.DependencyContainer.GetInstance<INotificationService>();
//            var strError = "";

//            //1. Lay thong tin metadata tu queue de tao file
//            var sqlResponsePick = _vodService.PickMetadataFromQueueGenre().Result;

//            if (!sqlResponsePick.IsSuccess)
//            {
//                strError = string.Format("Cannot pick metadata from Queue due to: {0}", sqlResponsePick.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//                return;
//            }

//            if (sqlResponsePick.ObjectResult == null)
//            {
//                strError = string.Format("No metadata found to pick from Queue");
//                Consoler.WriteInfoLine(strError);
//                //logger.Info(strError);
//                return;
//            }

//            var genreXMLMetadata = sqlResponsePick.ObjectResult as GenreXMLEntity;

//            //2. Lay thong danh sach cac genre tu list
//            var sqlResponseGetGenres = _vodService.GetMetadataFromListGenre(genreXMLMetadata.GenreIdValues).Result;

//            if (!sqlResponseGetGenres.IsSuccess)
//            {
//                strError = string.Format("Cannot get metadata from Queue due to: {0}", sqlResponseGetGenres.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//                return;
//            }

//            if (sqlResponseGetGenres.ObjectResult == null)
//            {
//                strError = string.Format("No metadata found to pick from Queue");
//                Consoler.WriteInfoLine(strError);
//                //logger.Info(strError);
//                return;
//            }

//            var genreMetadata = sqlResponseGetGenres.ObjectResult as List<GenreEntity>;

//            //3. Tao file tu Metadata da lay duoc
//            var xmlFileResponse = _vodService.CreateGenreXmlFile(genreMetadata, genreXMLMetadata.XMLFileId).Result;
//            if (!xmlFileResponse.IsSuccess)
//            {
//                strError = string.Format("Cannot create xml file for Metadata due to: {0}", xmlFileResponse.Message);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);

//                // If create file fail:
//                // Select record by XMLFileId then move it to vod_collection_failed + update message
//                var sqlMoveQueueToFailResponse = _vodService.MoveQueueToFailedGenre(genreXMLMetadata.XMLFileId, strError).Result;
//                if (sqlMoveQueueToFailResponse.IsSuccess)
//                {
//                    if (sqlMoveQueueToFailResponse.RowsAffected > 0)
//                    {
//                        Consoler.WriteDoneLine(string.Format("The file with ID = {0} has been moved to FAILED area.", genreXMLMetadata.XMLFileId));
//                    }
//                    else
//                    {
//                        Consoler.WriteInfoLine("No files to move to FAILED area.");
//                    }
//                }
//                else
//                {
//                    strError = string.Format("Failed to move file with ID = {0} to FAILED area due to: {1}.", genreXMLMetadata.XMLFileId, sqlMoveQueueToFailResponse.ErrorMessage);
//                    Consoler.WriteErrorLine(strError);
//                    logger.Error(strError);
//                }
//                return;
//            }


//            //4. Sau khi tao xong file thi chuyen vao bang process va cap nhat lai trang thai
//            var sqlResponse = _vodService.MoveQueueToProcessGenre(genreXMLMetadata.XMLFileId, xmlFileResponse.FileName).Result;
//            if (sqlResponse.IsSuccess)
//            {
//                if (sqlResponse.RowsAffected > 0)
//                {
//                    Consoler.WriteDoneLine(string.Format("The file {0} has been moved to PROCESS area.", xmlFileResponse.FileName));
//                }
//                else
//                {
//                    Consoler.WriteInfoLine("No files to move to PROCESS area.");
//                }
//            }
//            else
//            {
//                strError = string.Format("Failed to move file to PROCESS area due to: {0}.", sqlResponse.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//            }
//        }

//        private static void ProcessFileCreatingContainer()
//        {
//            var _vodService = Program.DependencyContainer.GetInstance<INotificationService>();
//            var strError = "";

//            //1. Lay thong tin metadata tu queue de tao file
//            var sqlResponsePick = _vodService.PickMetadataFromQueueContainer().Result;

//            if (!sqlResponsePick.IsSuccess)
//            {
//                strError = string.Format("Cannot pick metadata from Queue due to: {0}", sqlResponsePick.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//                return;
//            }

//            if (sqlResponsePick.ObjectResult == null)
//            {
//                strError = string.Format("No metadata found to pick from Queue");
//                Consoler.WriteInfoLine(strError);
//                //logger.Info(strError);
//                return;
//            }

//            var containerMetadata = sqlResponsePick.ObjectResult as ContainerEntityXml;


//            //2. Tao file tu Metadata da lay duoc
//            var xmlFileResponse = _vodService.CreateContainerXmlFile(containerMetadata).Result;
//            if (!xmlFileResponse.IsSuccess)
//            {
//                strError = string.Format("Cannot create xml file for Metadata due to: {0}", xmlFileResponse.Message);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);

//                // If create file fail:
//                // Select record by XMLFileId then move it to vod_collection_failed + update message
//                var sqlMoveQueueToFailResponse = _vodService.MoveQueueToFailedContainer(containerMetadata.XMLFileId, strError).Result;
//                if (sqlMoveQueueToFailResponse.IsSuccess)
//                {
//                    if (sqlMoveQueueToFailResponse.RowsAffected > 0)
//                    {
//                        Consoler.WriteDoneLine(string.Format("The file with ID = {0} has been moved to FAILED area.", containerMetadata.XMLFileId));
//                    }
//                    else
//                    {
//                        Consoler.WriteInfoLine("No files to move to FAILED area.");
//                    }
//                }
//                else
//                {
//                    strError = string.Format("Failed to move file with ID = {0} to FAILED area due to: {1}.", containerMetadata.XMLFileId, sqlMoveQueueToFailResponse.ErrorMessage);
//                    Consoler.WriteErrorLine(strError);
//                    logger.Error(strError);
//                }
//                return;
//            }


//            //3. Sau khi tao xong file thi chuyen vao bang process va cap nhat lai trang thai
//            var sqlResponse = _vodService.MoveQueueToProcessContainer(containerMetadata.XMLFileId, xmlFileResponse.FileName).Result;
//            if (sqlResponse.IsSuccess)
//            {
//                if (sqlResponse.RowsAffected > 0)
//                {
//                    Consoler.WriteDoneLine(string.Format("The file {0} has been moved to PROCESS area.", xmlFileResponse.FileName));
//                }
//                else
//                {
//                    Consoler.WriteInfoLine("No files to move to PROCESS area.");
//                }
//            }
//            else
//            {
//                strError = string.Format("Failed to move file to PROCESS area due to: {0}.", sqlResponse.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//            }
//        }

//        private static void ProcessFileCreatingCollection()
//        {
//            var _vodService = Program.DependencyContainer.GetInstance<INotificationService>();
//            var strError = "";

//            //1. Lay thong tin metadata tu queue de tao file
//            var sqlResponsePick = _vodService.PickMetadataFromQueueCollection().Result;

//            if (!sqlResponsePick.IsSuccess)
//            {
//                strError = string.Format("Cannot pick metadata from Queue due to: {0}", sqlResponsePick.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//                return;
//            }

//            if (sqlResponsePick.ObjectResult==null)
//            {
//                strError = string.Format("No metadata found to pick from Queue");
//                Consoler.WriteInfoLine(strError);
//                //logger.Info(strError);
//                return;
//            }

//            var collectionMetadata = sqlResponsePick.ObjectResult as CollectionEntityXml;


//            //2. Tao file tu Metadata da lay duoc
//            var xmlFileResponse = _vodService.CreateCollectionXmlFile(collectionMetadata).Result;
//            if (!xmlFileResponse.IsSuccess)
//            {
//                strError = string.Format("Cannot create xml file for Metadata due to: {0}", xmlFileResponse.Message);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);

//                // If create file fail:
//                // Select record by XMLFileId then move it to vod_collection_failed + update message
//                var sqlMoveQueueToFailResponse = _vodService.MoveQueueToFailedCollection(collectionMetadata.XMLFileId, strError).Result;
//                if (sqlMoveQueueToFailResponse.IsSuccess)
//                {
//                    if (sqlMoveQueueToFailResponse.RowsAffected > 0)
//                    {
//                        Consoler.WriteDoneLine(string.Format("The file with ID = {0} has been moved to FAILED area.", collectionMetadata.XMLFileId));
//                    }
//                    else
//                    {
//                        Consoler.WriteInfoLine("No files to move to FAILED area.");
//                    }
//                }
//                else
//                {
//                    strError = string.Format("Failed to move file with ID = {0} to FAILED area due to: {1}.", collectionMetadata.XMLFileId, sqlMoveQueueToFailResponse.ErrorMessage);
//                    Consoler.WriteErrorLine(strError);
//                    logger.Error(strError);
//                }
//                return;
//            }            


//            //3. Sau khi tao xong file thi chuyen vao bang process va cap nhat lai trang thai
//            var sqlResponse = _vodService.MoveQueueToProcessCollection(collectionMetadata.XMLFileId,xmlFileResponse.FileName).Result;
//            if (sqlResponse.IsSuccess)
//            {
//                if (sqlResponse.RowsAffected > 0)
//                {
//                    Consoler.WriteDoneLine(string.Format("The file {0} has been moved to PROCESS area.", xmlFileResponse.FileName));
//                }
//                else
//                {
//                    Consoler.WriteInfoLine("No files to move to PROCESS area.");
//                }
//            }
//            else
//            {
//                strError = string.Format("Failed to move file to PROCESS area due to: {0}.", sqlResponse.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//            }
//        }

//        private static void ProcessFileCreatingProgram()
//        {
//            var _vodService = Program.DependencyContainer.GetInstance<INotificationService>();
//            var strError = "";

//            //1. Lay thong tin metadata tu queue de tao file
//            var sqlResponsePick = _vodService.PickMetadataFromQueueProgram().Result;

//            if (!sqlResponsePick.IsSuccess)
//            {
//                strError = string.Format("Cannot pick metadata from Queue due to: {0}", sqlResponsePick.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//                return;
//            }

//            if (sqlResponsePick.ObjectResult == null)
//            {
//                strError = string.Format("No metadata found to pick from Queue");
//                Consoler.WriteInfoLine(strError);
//                //logger.Info(strError);
//                return;
//            }

//            var programMetadata = sqlResponsePick.ObjectResult as ProgramEntityXml;

//            //2. Tao file tu Metadata da lay duoc
//            var xmlFileResponse = _vodService.CreateProgramXmlFile(programMetadata).Result;           
//            if (!xmlFileResponse.IsSuccess)
//            {
//                strError = string.Format("Cannot create xml file for Metadata due to: {0}", xmlFileResponse.Message);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);

//                // If create file fail:
//                // Select record by XMLFileId then move it to vod_collection_failed + update message
//                var sqlMoveQueueToFailResponse = _vodService.MoveQueueToFailedProgram(programMetadata.XMLFileId, strError).Result;
//                if (sqlMoveQueueToFailResponse.IsSuccess)
//                {
//                    if (sqlMoveQueueToFailResponse.RowsAffected > 0)
//                    {
//                        Consoler.WriteDoneLine(string.Format("The file with ID = {0} has been moved to FAILED area.", programMetadata.XMLFileId));
//                    }
//                    else
//                    {
//                        Consoler.WriteInfoLine("No files to move to FAILED area.");
//                    }
//                }
//                else
//                {
//                    strError = string.Format("Failed to move file with ID = {0} to FAILED area due to: {1}.", programMetadata.XMLFileId, sqlMoveQueueToFailResponse.ErrorMessage);
//                    Consoler.WriteErrorLine(strError);
//                    logger.Error(strError);
//                }
//                return;
//            }


//            //3. Sau khi tao xong file thi chuyen vao bang process va cap nhat lai trang thai
//            var sqlResponse = _vodService.MoveQueueToProcessProgram(programMetadata.XMLFileId, xmlFileResponse.FileName).Result;
//            if (sqlResponse.IsSuccess)
//            {
//                if (sqlResponse.RowsAffected > 0)
//                {
//                    Consoler.WriteDoneLine(string.Format("The file {0} has been moved to PROCESS area.", xmlFileResponse.FileName));
//                }
//                else
//                {
//                    Consoler.WriteInfoLine("No files to move to PROCESS area.");
//                }
//            }
//            else
//            {
//                strError = string.Format("Failed to move file to PROCESS area due to: {0}.", sqlResponse.ErrorMessage);
//                Consoler.WriteErrorLine(strError);
//                logger.Error(strError);
//            }
//        }        
//    }
//}
