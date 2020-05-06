using System;
using System.Linq;
using System.IO;
using EmailConsoler.Tasking;
using EmailConsoler.Logging;
using EmailConsoler.Helpers;
using System.Collections.Generic;
using EmailConsoler.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EmailConsoler
{
    public class EmailFileSender : WorkerThreadEx
    {
        private static ILog logger = LogProvider.GetCurrentClassLogger();

        public EmailFileSender(string workerName)
            : base(workerName, (worker) => ProcessEmailFileSender(worker), SystemSettings.WorkerFileSenderSpeedMS)
        {

        }

        private static void ProcessEmailFileSender(WorkerThreadEx worker)
        {
            var vodWorker = worker as EmailFileSender;
            var toSendFolderPath = SystemSettings.EmailToSendFolder;
            var sentFolderPath = SystemSettings.EmailToSendFolder;

            if (worker != null && worker.IsSignaled)
            {
                var wkName = worker.Name;
                if (!Directory.Exists(toSendFolderPath))
                {
                    Consoler.WriteErrorLine(string.Format("The folder is no longer existed: {0}", toSendFolderPath));
                    return;
                }

                if (!Directory.Exists(sentFolderPath))
                {
                    Consoler.WriteErrorLine(string.Format("The folder is no longer existed: {0}", sentFolderPath));
                    return;
                }

                var myFile = GetOldestJsonFileByType(toSendFolderPath);
                if (myFile == null)
                {
                    //If cannot find any files then return
                    Consoler.WriteErrorLine(string.Format("There are no JSON files in {0}", toSendFolderPath));
                    return;
                }

               
                Consoler.WriteDoneLine(string.Format("Found new file [{0}] for sending", myFile.Name));

                //Begin read content and send
                ReadFileContentAndSend(myFile);
            }
        }

        private static void ReadFileContentAndSend(FileInfo myFile)
        {
            var strError = "";
            var emailInfo = new EmailModel();
            try
            {
                using (StreamReader r = myFile.OpenText())
                {
                    string json = r.ReadToEnd();
                    emailInfo = JsonConvert.DeserializeObject<EmailModel>(json);
                }
            }
            catch(Exception ex)
            {
                strError = string.Format("The input file {0} is invalid format. Ex: {1}", myFile.Name, ex.GetInnerMessage().ToString());
                Consoler.WriteErrorLine(string.Format("The input file {0} is invalid format", myFile.Name));

                //Trying to move file to FailedFolder if any errors
                myFile.CopyTo(SystemSettings.EmailFailedFolder + myFile.Name, true);
                //myFile.Delete();

                return;
            }

            if(emailInfo == null)
            {
                strError = string.Format("The input file {0} is empty content", myFile.Name);
                Consoler.WriteErrorLine(strError);
                return;
            }

            //Begin send email
            var result = SendEmail(emailInfo);

            if (!string.IsNullOrEmpty(result))
            {
                //Sending email was failed
                strError = string.Format("Cannot send email [{0}] because: {1}", myFile.Name, result);

                Consoler.WriteErrorLine(strError);

                //Move file to Failed
                myFile.CopyTo(SystemSettings.EmailFailedFolder + myFile.Name, true);
                //myFile.Delete();
            }
            else
            {
                strError = string.Format("An email [{0}] has been sent successfully.", myFile.Name);
                Consoler.WriteDoneLine(strError);

                try
                {
                    //Move file to SentFolder
                    myFile.CopyTo(SystemSettings.EmailSentFolder + myFile.Name, true);
                   //myFile.Delete();

                    strError = string.Format("The file {0} has been moved to SentFolder.", myFile.Name);
                    Consoler.WriteDoneLine(strError);
                    logger.Debug(strError);
                }
                catch (Exception ex)
                {
                    strError = string.Format("Cannot move the file {0} to SentFolder due to: {1}.", myFile.Name, ex.GetInnerMessage());
                    Consoler.WriteErrorLine(strError);
                    logger.Error(strError);

                    //Trying to move file to FailedFolder if any errors
                    myFile.CopyTo(SystemSettings.EmailFailedFolder + myFile.Name, true);
                    //myFile.Delete();
                }
            }
        }        

        private static FileInfo GetOldestJsonFileByType(string containerFolder)
        {
            var directory = new DirectoryInfo(containerFolder);

            //Get oldest file
            var myFile = (from f in directory.GetFiles("*.json")
                          //where f.Name.Contains(fileNamePrefix)
                          orderby f.LastWriteTime ascending
                          select f).FirstOrDefault();

            if (myFile == null)
            {
                Consoler.WriteInfoLine("There are no JSON files to send.");
                return null;
            }

            //if (myFile != null)
            //{
            //    var fileName = Path.GetFileNameWithoutExtension(myFile.Name);
            //    var jsonFileNameArr = fileName.Split('_');
            //    if (jsonFileNameArr.Length > 0)
            //    {
            //        objectId = jsonFileNameArr[1];
            //    }
            //    else
            //    {
            //        Consoler.WriteErrorLine(string.Format("The file {0} is invalid name.", fileName));
            //        return null;
            //    }
            //}

            return myFile;
        }

        private static string SendEmail(EmailModel emailInfo)
        {
            //Begin sending email
            emailInfo.Sender = SystemSettings.EmailSender;
            emailInfo.SenderPwd = SystemSettings.EmailSenderPwd;

            var result = string.Empty;
            try
            {
                result = EmailHelpers.SendEmail(emailInfo);
            }
            catch (Exception ex)
            {
                //Sending email was failed
                result = ex.GetInnerMessage().ToString();
            }            

            return result;       
        }        
    }
}
