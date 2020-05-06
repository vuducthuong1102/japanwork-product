//using System.Linq;
//using System.IO;
//using EmailConsoler.Helpers;
//using EmailConsoler.Logging;
//using System;
//using EmailConsoler.Services;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using EmailConsoler.Models;
//using Newtonsoft.Json;

//namespace EmailConsoler.Tasking
//{
//    public class NotificationSender : WorkerThreadEx
//    {
//        private static ILog logger = LogProvider.GetCurrentClassLogger();

//        public NotificationSender(string workerName)
//            : base(workerName, (worker) => Process(worker), SystemSettings.WorkerFileSenderSpeedMS)
//        {

//        }

//        private static void Process(WorkerThreadEx worker)
//        {
//            var vodWorker = worker as NotificationSender;
//            var xmlFileId = string.Empty;
//            var toSendFolderPath = SystemSettings.NotifToSendFolder;
//            var sentFolderPath = SystemSettings.NotifSentFolder;

//            if (worker != null && worker.IsSignaled)
//            {
//                var wkName = worker.Name;
//                if (!Directory.Exists(toSendFolderPath))
//                {
//                    Consoler.WriteErrorLine(string.Format("The folder was not found: {0}", toSendFolderPath));
//                    return;
//                }

//                if (!Directory.Exists(sentFolderPath))
//                {
//                    Consoler.WriteErrorLine(string.Format("The folder was not found: {0}", sentFolderPath));
//                    return;
//                }

//                var listEmails = new List<CustomerEmailAddress>();
//                var inputFilePath = string.Format("{0}{1}", SystemSettings.NotifToSendFolder, "email_list.json");

//                try
//                {
//                    using (StreamReader r = new StreamReader(inputFilePath))
//                    {
//                        string json = r.ReadToEnd();
//                        listEmails = JsonConvert.DeserializeObject<List<CustomerEmailAddress>>(json);
//                    }
//                }
//                catch
//                {
//                    Consoler.WriteErrorLine("The input file is invalid format");
//                }

//                var email = TakeOneFromListAndRemove(listEmails, inputFilePath);
//                if(email == null)
//                {
//                    return;
//                }

//                //var fileNamePrefix = string.Empty;
//                //var myFile = GetOldestFileByPrefix(toSendFolderPath, fileNamePrefix);

//                //if (myFile == null)
//                //{
//                //    //If cannot find any file then return
//                //    return;
//                //}

//                //SendNotif(myFile).Wait();

//                SendEmail(email).Wait();
//            }
//        }

//        public static CustomerEmailAddress TakeOneFromListAndRemove(List<CustomerEmailAddress> emailsList, string inputFilePath = "")
//        {
//            CustomerEmailAddress emailInfo = null; 
//            if(emailsList != null && emailsList.Count > 0)
//            {
//                emailInfo = emailsList.Take(1).FirstOrDefault();

//                if (emailInfo != null)
//                    emailsList.Remove(emailInfo);

//                //write string to file
//                System.IO.File.WriteAllText(inputFilePath, JsonConvert.SerializeObject(emailsList));
//            }

//            return emailInfo;
//        }

//        public static CustomerEmailAddress TakeOneFromListAndRemove(List<CustomerEmailAddress> emailsList)
//        {
//            CustomerEmailAddress emailInfo = null;
//            if (emailsList != null && emailsList.Count > 0)
//            {
//                emailInfo = emailsList.Take(1).FirstOrDefault();
//            }

//            return emailInfo;
//        }

//        private static async Task SendEmail(CustomerEmailAddress emailInfo)
//        {
//            var msg = string.Format("Found new email '{0}'. Begin sending...", emailInfo.Email);
//            Consoler.WriteDoneLine(msg);
//            logger.Debug(msg);

//            //Begin sending email
//            var emailModel = new EmailModel
//            {
//                Sender = SystemSettings.EmailSender,
//                SenderPwd = SystemSettings.EmailSenderPwd,
//                Receiver = emailInfo.Email,
//                Subject = "Mời sử dụng hệ thống Job Market"
//                //SenderName = "Job Market",
//                //SenderEmail = "contact@job-market.jp
//            };

//            //emailModel.ActiveLink = string.Format("{0}?&token={1}", SystemSettings.Email_AccepFriendInvitationLink, hashingData);

//            var htmlFilePath = string.Format("{0}{1}", SystemSettings.NotifToSendFolder, "email_content.html");

//            try
//            {
//                using (StreamReader r = new StreamReader(htmlFilePath))
//                {
//                    string bodyContent = r.ReadToEnd();
//                    if(!string.IsNullOrEmpty(bodyContent))
//                        emailModel.Body = bodyContent;
//                }
//            }
//            catch
//            {
//                Consoler.WriteErrorLine("The html file content is invalid format");
//            }

//            var sendEmailResult = EmailHelpers.SendEmail(emailModel);
//            if (!string.IsNullOrEmpty(sendEmailResult))
//            {
//                //Sending email was failed
//                logger.Error(sendEmailResult);

//                msg = string.Format("Cannot send email to {0} because: {1}", emailInfo.Email, sendEmailResult);

//                Consoler.WriteErrorLine(msg);
//                logger.Error(msg);

//                var errorListPath = string.Format("{0}{1}", SystemSettings.NotifFailedFolder, "email_list.json");
//                if (File.Exists(errorListPath))
//                {
//                    File.AppendAllText(errorListPath, emailInfo.Email + Environment.NewLine);
//                }
//                else
//                {
//                    Consoler.WriteErrorLine(string.Format("The failed file was not found: {0}", errorListPath));
//                }
//            }
//            else
//            {
//                msg = string.Format("An email has been sent to {0} successfully.", emailInfo.Email);
//                Consoler.WriteDoneLine(msg);
//            }

//            //Begining send notif
//            //var _notifService = Program.DependencyContainer.GetInstance<INotificationService>();

//            //var inputModel = FileHelpers<NotificationInputModel>.ReadJsonFileToObject(myFile.FullName);

//            //var result = await _notifService.SendNotifAsync(inputModel);

//            //if (result.code == (int)EnumNotifStatus.Error)
//            //{
//            //    msg = string.Format("Could not send the notification in file {0} due to: {1}", myFile.Name, result.message);
//            //    Consoler.WriteErrorLine(msg);
//            //    logger.Error(msg);

//            //    //Move file to Failed
//            //    myFile.CopyTo(SystemSettings.NotifFailedFolder + myFile.Name, true);
//            //    myFile.Delete();

//            //    msg = string.Format("The file {0} has been moved to FailedFolder.", myFile.Name);
//            //    Consoler.WriteDoneLine(msg);
//            //    logger.Debug(msg);
//            //}

//            //if (result.code == (int)EnumNotifStatus.Success)
//            //{
//            //    msg = string.Format("The notification in file {0} has been sent successfully.", myFile.Name);
//            //    Consoler.WriteDoneLine(msg);
//            //    logger.Debug(msg);

//            //    try
//            //    {
//            //        //Move file to SentFolder
//            //        myFile.CopyTo(SystemSettings.NotifSentFolder + myFile.Name, true);
//            //        myFile.Delete();

//            //        msg = string.Format("The file {0} has been moved to SentFolder.", myFile.Name);
//            //        Consoler.WriteDoneLine(msg);
//            //        logger.Debug(msg);
//            //    }
//            //    catch (Exception ex)
//            //    {
//            //        msg = string.Format("Cannot move the file {0} to SentFolder due to: {1}.", myFile.Name, ex.GetInnerMessage());
//            //        Consoler.WriteErrorLine(msg);
//            //        logger.Error(msg);

//            //        //Trying to move file to FailedFolder if any errors
//            //        myFile.CopyTo(SystemSettings.NotifFailedFolder + myFile.Name, true);
//            //        myFile.Delete();
//            //    }
//            //}
//        }

//        protected int SendEmailForUsingSystem(int inviteId, List<string> emails, string hashingData, string senderEmail, string senderName, string note)
//        {
//            var result = 1;
//            //Begin sending email
//            var emailModel = new EmailModel
//            {
//                Sender = SystemSettings.EmailSender,
//                SenderPwd = SystemSettings.EmailSenderPwd,
//                Receiver = string.Join(",", emails),
//                Subject = "Mời sử dụng hệ thống Job Market",
//                SenderName = senderName,
//                SenderEmail = senderEmail,
//                Note = note
//            };

//            emailModel.ActiveLink = string.Format("{0}?&token={1}", SystemSettings.Email_AccepFriendInvitationLink, hashingData);

//            emailModel.Body = "";

//            var sendEmailResult = EmailHelpers.SendEmail(emailModel);
//            if (!string.IsNullOrEmpty(sendEmailResult))
//            {
//                //Sending email was failed
//                logger.Error(sendEmailResult);

//                var msg = string.Format("Cannot send email to {0} because: {1}", emails[0], sendEmailResult);
//                Consoler.WriteErrorLine(msg);
//                logger.Error(msg);
//            }

//            return result;
//        }

//        private static async Task SendNotif(FileInfo myFile)
//        {
//            var msg = string.Format("Found new file '{0}'. Begin sending...", myFile.Name);
//            Consoler.WriteDoneLine(msg);
//            logger.Debug(msg);

//            //Begining send notif
//            var _notifService = Program.DependencyContainer.GetInstance<INotificationService>();

//            var inputModel = FileHelpers<NotificationInputModel>.ReadJsonFileToObject(myFile.FullName);

//            var result = await _notifService.SendNotifAsync(inputModel);

//            if(result.code == (int)EnumNotifStatus.Error)           
//            {
//                msg = string.Format("Could not send the notification in file {0} due to: {1}", myFile.Name, result.message);
//                Consoler.WriteErrorLine(msg);
//                logger.Error(msg);

//                //Move file to Failed
//                myFile.CopyTo(SystemSettings.NotifFailedFolder + myFile.Name, true);
//                myFile.Delete();

//                msg = string.Format("The file {0} has been moved to FailedFolder.", myFile.Name);
//                Consoler.WriteDoneLine(msg);
//                logger.Debug(msg);
//            }

//            if (result.code == (int)EnumNotifStatus.Success)
//            {
//                msg = string.Format("The notification in file {0} has been sent successfully.", myFile.Name);
//                Consoler.WriteDoneLine(msg);
//                logger.Debug(msg);

//                try
//                {
//                    //Move file to SentFolder
//                    myFile.CopyTo(SystemSettings.NotifSentFolder + myFile.Name, true);
//                    myFile.Delete();

//                    msg = string.Format("The file {0} has been moved to SentFolder.", myFile.Name);
//                    Consoler.WriteDoneLine(msg);
//                    logger.Debug(msg);
//                }
//                catch (Exception ex)
//                {
//                    msg = string.Format("Cannot move the file {0} to SentFolder due to: {1}.", myFile.Name, ex.GetInnerMessage());
//                    Consoler.WriteErrorLine(msg);
//                    logger.Error(msg);

//                    //Trying to move file to FailedFolder if any errors
//                    myFile.CopyTo(SystemSettings.NotifFailedFolder + myFile.Name, true);
//                    myFile.Delete();
//                }
//            }
//        }

//        private static FileInfo GetOldestFileByPrefix(string containerFolder, string fileNamePrefix = "")
//        {
//            var directory = new DirectoryInfo(containerFolder);

//            //Get oldest file
//            var myFile = (from f in directory.GetFiles("*.json")
//                          where f.Name.Contains(fileNamePrefix)
//                          orderby f.LastWriteTime ascending
//                          select f).FirstOrDefault();

//            if (myFile == null)
//            {
//                Consoler.WriteInfoLine("There are no files to send.");
//                return null;
//            }

//            return myFile;
//        }
//    }
//}
