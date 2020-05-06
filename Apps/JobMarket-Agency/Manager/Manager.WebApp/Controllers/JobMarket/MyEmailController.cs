using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Manager.WebApp.Helpers;
using Manager.WebApp.Models;
using Manager.WebApp.Settings;
using Newtonsoft.Json;
using Manager.SharedLibs;
using Manager.SharedLibs.Logging;
using System.Linq;
using Autofac;
using MsSql.AspNet.Identity.Entities;

using OpenPop.Pop3;
using OpenPop.Pop3.Exceptions;
using Message = OpenPop.Mime.Message;
using OpenPop.Mime.Header;
using System.Threading.Tasks;
using Manager.WebApp.Resources;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using MsSql.AspNet.Identity.MsSqlStores;
using Manager.WebApp.Services;

namespace Manager.WebApp.Controllers
{
    public class MyEmailController : BaseAuthedController
    {
        private readonly ILog logger = LogProvider.For<MyEmailController>();

        public MyEmailController()
        {

        }

        [VerifyLoggedInUser]
        [HttpPost]
        public ActionResult FetchMailBox()
        {
            //Begin fetch mail box
            var messagError = string.Empty;
            var hasData = FetchMailBoxFromMailServer(ref messagError);

            return Json(new { success = true, message = messagError, hasData = hasData }, JsonRequestBehavior.AllowGet);
        }

        public bool FetchMailBoxFromMailServer(ref string strMessage)
        {
            var strError = string.Empty;
            var hasData = false;

            // Disable buttons while working    
            //Dictionary<int, Message> messages = new Dictionary<int, Message>();
            List<Message> messages = new List<Message>();
            var pop3Client = new Pop3Client();

            var popServer = "";
            var myEmail = "";
            var myEmailPwd = "";
            try
            {
                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();

                var emailServers = CommonHelpers.GetListEmailServers(agencyId);
                var emailList = CommonHelpers.GetListEmailSettings(agencyId, staffId);
                if (!emailServers.HasData())
                {
                    strMessage = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                    return false;
                }

                if (!emailList.HasData())
                {
                    strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_INCOMING);
                    return false;
                }

                var mailSetting = emailList.Where(x => x.EmailType == (int)EnumEmailSettingTypes.InComing).FirstOrDefault();
                if(mailSetting == null)
                {
                    strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_INCOMING);
                    return false;
                }
                else
                {
                    myEmail = mailSetting.Email;
                }

                var mailServer = emailServers.Where(x => x.Id == mailSetting.EmailServerId).FirstOrDefault();
                if (mailServer == null)
                {
                    strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_INCOMING);
                    return false;
                }                

                if(mailServer.ReceivingConfig == null)
                {
                    strMessage = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                    return false;
                }

                popServer = mailServer.ReceivingConfig.POPServer;

                if (pop3Client.Connected)
                    pop3Client.Disconnect();
              
                myEmailPwd = Utility.TripleDESDecrypt(myEmail, mailSetting.EmailPasswordHash);

                //pop3Client.Connect(popServer, mailServer.ReceivingConfig.Port, mailServer.ReceivingConfig.SSLRequired);
                pop3Client.Connect(popServer, mailServer.ReceivingConfig.Port, mailServer.ReceivingConfig.SSLRequired, 3000, 3600000, null);
                pop3Client.Authenticate(myEmail, myEmailPwd);

                int count = pop3Client.GetMessageCount();
                if (count == 0)
                    return false;

                int success = 0;
                int fail = 0;
                if (count > 0)
                    hasData = true;

                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailMessage>();
                List<string> syncIds = null;
                var needToCheckSynIds = (!string.IsNullOrEmpty(myEmail) && !myEmail.Contains("gmail.com"));

                if (needToCheckSynIds)
                {
                    syncIds = CommonHelpers.GetEmailMessageIdSynchronized(agencyId, staffId);
                }

                var hasSyncIds = syncIds.HasData();                
                var hasUnreadEmail = false;
                //var myUIds = pop3Client.GetMessageUids();
                //// All the new messages not seen by the POP3 client
                //for (int i = 0; i < myUIds.Count; i++)
                //{
                //    string currentUidOnServer = myUIds[i];
                //    if (!syncIds.Contains(currentUidOnServer))
                //    {                        
                //        Message unseenMessage = pop3Client.GetMessage(i + 1);

                //        hasUnreadEmail = true;
                //    }
                //}

                for (int i = count; i >= 1; i -= 1)
                {
                    try
                    {
                        Message message = pop3Client.GetMessage(i);
                        if (message != null)
                        {
                            if (needToCheckSynIds && hasSyncIds)
                            {
                                if (syncIds.Contains(message.Headers.MessageId))
                                    continue;
                            }                            
                        }

                        var identity = new IdentityEmailMessage();

                        if (message.Headers.References.HasData())
                        {
                            identity.ParentMessageId = message.Headers.References[0];
                            identity.ParentMessageIdHash = Utility.Md5HashingData(message.Headers.References[0]);
                        }

                        identity.MessageId = message.Headers.MessageId;
                        identity.MessageIdHash = Utility.Md5HashingData(message.Headers.MessageId);

                        var msgPlainText = string.Empty;

                        var plainTextVer = message.FindFirstPlainTextVersion();
                        if (plainTextVer != null)
                        {
                            msgPlainText = plainTextVer.GetBodyAsText();                            
                        }                            
                        else
                        {
                            var htmlVer = message.FindFirstHtmlVersion();
                            msgPlainText = htmlVer.GetBodyAsText();
                            if (!string.IsNullOrEmpty(msgPlainText))
                            {
                                msgPlainText = HtmlRemoval.RemoveTags(msgPlainText);
                            }
                        }

                        if (!string.IsNullOrEmpty(msgPlainText))
                        {
                            if (msgPlainText.Length > 120)
                                identity.ShortMessage = msgPlainText.Substring(0, 120);
                            else
                                identity.ShortMessage = msgPlainText;
                        }

                        var attachments = message.FindAllAttachments();
                        if (attachments.HasData())
                        {
                            identity.AttachedFiles = new List<IdentityEmailAttachment>();
                            var storageFolderPath = string.Format("{0}/{1}", agencyId, staffId);
                            foreach (var item in attachments)
                            {
                                if (item.Body != null)
                                {
                                    //Storage to file
                                    var filePath = FileUploadHelper.UploadEmailAttachmentToFile(item.Body, item.FileName, storageFolderPath);

                                    if (!string.IsNullOrEmpty(filePath))
                                    {
                                        var attachFile = new IdentityEmailAttachment();
                                        attachFile.FileName = item.FileName;
                                        attachFile.FilePath = filePath;
                                        attachFile.FileSize = item.Body.LongLength.ToString();

                                        identity.AttachedFiles.Add(attachFile);
                                    }
                                }
                            }

                            if (identity.AttachedFiles.HasData())
                            {
                                identity.Attachments = JsonConvert.SerializeObject(identity.AttachedFiles);
                            }
                        }
                        
                        if (message.Headers.From != null)
                        {
                            var sender = new IdentityEmailAddress();
                            sender.Address = message.Headers.From.Address;
                            sender.DisplayName = message.Headers.From.DisplayName;

                            identity.Sender = JsonConvert.SerializeObject(sender);
                        }

                        if (message.Headers.To.HasData())
                        {
                            var receiverList = new List<IdentityEmailAddress>();
                            foreach (var item in message.Headers.To)
                            {
                                var receiver = new IdentityEmailAddress();
                                receiver.Address = item.Address;
                                receiver.DisplayName = item.DisplayName;

                                receiverList.Add(receiver);
                            }

                            identity.Receiver = JsonConvert.SerializeObject(receiverList);
                        }

                        if (message.Headers.Cc.HasData())
                        {
                            var ccList = new List<IdentityEmailAddress>();
                            foreach (var item in message.Headers.Cc)
                            {
                                var cc = new IdentityEmailAddress();
                                cc.Address = item.Address;
                                cc.DisplayName = item.DisplayName;

                                ccList.Add(cc);
                            }

                            identity.Cc = JsonConvert.SerializeObject(ccList);
                        }

                        if (message.Headers.Bcc.HasData())
                        {
                            var bccList = new List<IdentityEmailAddress>();
                            foreach (var item in message.Headers.Bcc)
                            {
                                var bcc = new IdentityEmailAddress();
                                bcc.Address = item.Address;
                                bcc.DisplayName = item.DisplayName;

                                bccList.Add(bcc);
                            }

                            identity.Bcc = JsonConvert.SerializeObject(bccList);
                        } 

                        identity.Subject = message.Headers.Subject;
                        identity.AgencyId = agencyId;
                        identity.StaffId = staffId;
                        identity.Subject = message.Headers.Subject;

                        identity.Message = message.ToMailMessage().Body;
                        identity.CreatedDate = message.Headers.DateSent;
                        
                        if(string.IsNullOrEmpty(identity.ParentMessageId))
                            storeEmail.Insert(identity);
                        else
                            storeEmail.InsertPart(identity);

                        success++;

                        hasUnreadEmail = true;
                    }
                    catch (Exception ex)
                    {
                        strError = "TestForm: Message fetching failed: " + ex.Message + "\r\n" +
                            "Stack trace:\r\n" +
                            ex.StackTrace;
                        fail++;
                    }                    
                }

                if (hasUnreadEmail)
                {
                    //pop3Client.DeleteAllMessages();
                    CachingHelpers.ClearEmailSynchronizedIds(agencyId, staffId);
                }                    

                if (fail > 0)
                {
                    strError = "Since some of the emails were not parsed correctly (exceptions were thrown)\r\n" +
                                     "please consider sending your log file to the developer for fixing.\r\n" +
                                     "If you are able to include any extra information, please do so.";
                }
            }
            catch (InvalidLoginException ex)
            {
                strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", string.Format(EmailResource.ERROR_EMAIL_INVALID_LOGIN_FORMAT, myEmail));

                strError = ex.Message;
            }
            catch (PopServerNotFoundException ex)
            {
                strMessage = string.Format(EmailResource.ERROR_EMAIL_POP_SERVER_NOT_FOUND_FORMAT, popServer);

                strError = ex.Message;
            }
            catch (PopServerLockedException ex)
            {
                strMessage = string.Format(EmailResource.ERROR_EMAIL_POP_SERVER_LOCKED_FORMAT, popServer);

                strError = ex.Message;
            }
            catch (LoginDelayException ex)
            {
                strMessage = string.Format(EmailResource.ERROR_EMAIL_LOGIN_DELAY_FORMAT, popServer);
                strError = ex.Message;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                strMessage = string.Format(EmailResource.ERROR_EMAIL_COULD_NOT_SYNC_FORMAT, myEmail, popServer);
            }
            finally
            {
                try
                {
                    if (pop3Client.Connected)
                    {
                        pop3Client.Disconnect();
                        pop3Client.Dispose();
                    }
                }
                catch
                {

                }
            }

            if (!string.IsNullOrEmpty(strError))
            {
                logger.ErrorFormat("Could not FetchMailBoxFromMailServer because: {0}", strError);

                //strMessage = string.Format(ManagerResource.ERROR_EMAIL_COULD_NOT_SYNC_FORMAT, myEmail, popServer);
            }

            return hasData;
        }

        [VerifyLoggedInUser]
        [HttpPost]
        public async Task<ActionResult> GetMailBoxCounter()
        {
            //Begin fetch mail box
            var messagError = string.Empty;
            var totalCount = GetMailBoxCounterFromMailServer(ref messagError);
            await Task.FromResult(totalCount);

            return Json(new { success = true, message = messagError, hasData = (totalCount > 0), total = totalCount }, JsonRequestBehavior.AllowGet);
        }

        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public int GetMailBoxCounterFromMailServer(ref string strMessage)
        {
            var strError = string.Empty;
            List<Message> messages = new List<Message>();
            var pop3Client = new Pop3Client();

            var popServer = "";
            var myEmail = "";
            var myEmailPwd = "";
            try
            {
                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();

                var emailServers = CommonHelpers.GetListEmailServers(agencyId);
                var emailList = CommonHelpers.GetListEmailSettings(agencyId, staffId);
                if (!emailServers.HasData())
                {
                    strMessage = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                    return 0;
                }

                if (!emailList.HasData())
                {
                    strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_INCOMING);
                    return 0;
                }

                var mailSetting = emailList.Where(x => x.EmailType == (int)EnumEmailSettingTypes.InComing).FirstOrDefault();
                if (mailSetting == null)
                {
                    strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_INCOMING);
                    return 0;
                }
                else
                {
                    myEmail = mailSetting.Email;
                }

                var mailServer = emailServers.Where(x => x.Id == mailSetting.EmailServerId).FirstOrDefault();
                if (mailServer == null)
                {
                    strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_INCOMING);
                    return 0;
                }

                if (mailServer.ReceivingConfig == null)
                {
                    strMessage = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                    return 0;
                }

                popServer = mailServer.ReceivingConfig.POPServer;

                if (pop3Client.Connected)
                    pop3Client.Disconnect();

                myEmailPwd = Utility.TripleDESDecrypt(myEmail, mailSetting.EmailPasswordHash);

                pop3Client.Connect(popServer, mailServer.ReceivingConfig.Port, mailServer.ReceivingConfig.SSLRequired, 3000, 30000, null );
                pop3Client.Authenticate(myEmail, myEmailPwd);

                int count = pop3Client.GetMessageCount();
                if (count == 0)
                    return 0;

                return count;
            }
            catch (InvalidLoginException ex)
            {
                strMessage = string.Format("<a href='/Settings/Email'>{0}</a>", string.Format(EmailResource.ERROR_EMAIL_INVALID_LOGIN_FORMAT, myEmail));

                strError = ex.Message;
            }
            catch (PopServerNotFoundException ex)
            {
                strMessage = string.Format(EmailResource.ERROR_EMAIL_POP_SERVER_NOT_FOUND_FORMAT, popServer);

                strError = ex.Message;
            }
            catch (PopServerLockedException ex)
            {
                strMessage = string.Format(EmailResource.ERROR_EMAIL_POP_SERVER_LOCKED_FORMAT, popServer);

                strError = ex.Message;
            }
            catch (LoginDelayException ex)
            {
                strMessage = string.Format(EmailResource.ERROR_EMAIL_LOGIN_DELAY_FORMAT, popServer);
                strError = ex.Message;
            }
            catch (Exception ex)
            {
                strError = ex.Message;

                strMessage = string.Format(EmailResource.ERROR_EMAIL_COULD_NOT_SYNC_FORMAT, myEmail, popServer);
            }
            finally
            {
                try
                {
                    if (pop3Client.Connected)
                    {
                        pop3Client.Disconnect();
                        pop3Client.Dispose();
                    }
                }
                catch
                {

                }
            }

            if (!string.IsNullOrEmpty(strError))
            {
                logger.ErrorFormat("Could not GetMailBoxCounterFromMailServer because: {0}", strError);
            }

            return 0;
        }

        public ActionResult LoadData()
        {
            ManageEmailModel model = new ManageEmailModel();
            PagingMeta meta = new PagingMeta();
            var models = new List<ManageEmailViewModel>();

            try
            {
                //Begin fetch mail box
                //FetchMyMailBox();

                model.Keyword = Request["query[generalSearch]"] != null ? Request["query[generalSearch]"].ToString() : string.Empty;               

                int currentPage = 1;
                int pageSize = SystemSettings.DefaultPageSize;

                if (Request["pagination[page]"] != null)
                {
                    currentPage = Utils.ConvertToInt32(Request["pagination[page]"], 1);
                }

                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();
                meta.page = Utils.ConvertToInt32(Request["pagination[page]"], 1);
                meta.pages = 2;
                meta.perpage = Utils.ConvertToInt32(Request["pagination[perpage]"]);
                meta.field = Request["sort[field]"];
                meta.sort = Request["sort[sort]"];

                if (string.IsNullOrEmpty(meta.field))
                    meta.field = "CreatedDate";

                 if (string.IsNullOrEmpty(meta.sort))
                    meta.sort = "desc";

                var filter = new IdentityEmailMessage
                {
                    Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    AgencyId = agencyId,
                    StaffId = staffId,
                    PageIndex = currentPage,
                    PageSize = Utils.ConvertToInt32(Request["pagination[perpage]"]),
                    SortField = meta.field,
                    SortType = meta.sort
                };
                
                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailMessage>();
                var results = storeEmail.GetByPage(filter);
                if (results.HasData())
                {
                    meta.total = results[0].TotalCount;
                    //for (int i = results.Count(); i >= 0; i--)
                    //{
                    //    var record = results[i];

                    //    var item = new ManageEmailViewModel();
                    //    item.Id = record.Id;
                    //    item.Subject = record.Subject;
                    //    //item.Message = record.Message;
                    //    item.ShortMessage = record.ShortMessage;
                    //    item.MessageId = record.MessageId;
                    //    item.IdHash = record.MessageIdHash;
                    //    item.IsRead = record.IsRead;
                    //    item.CreatedDateStr = record.CreatedDate.DateTimeQuestToString("dd/MM/yyyy HH:mm");
                    //    item.TotalChilds = record.TotalChilds;
                    //    item.Sender = JsonConvert.DeserializeObject<IdentityEmailAddress>(record.Sender);
                    //    item.Receiver = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(record.Receiver);

                    //    var attachFiles = JsonConvert.DeserializeObject<List<IdentityEmailAttachment>>(record.Attachments);
                    //    if (attachFiles.HasData())
                    //        item.TotalAttachments = attachFiles.Count();

                    //    item.DetailTk = SecurityHelper.GenerateSecureLink("MyEmail", "EmailDetail", new { id = record.Id });

                    //    models.Add(item);
                    //}

                    foreach (var record in results)
                    {
                        //var item = ParsingEmailDataToView(record);
                        var item = new ManageEmailViewModel();
                        item.Id = record.Id;
                        item.Subject = record.Subject;
                        //item.Message = record.Message;
                        item.ShortMessage = record.ShortMessage;
                        item.MessageId = record.MessageId;
                        item.IdHash = record.MessageIdHash;
                        item.IsRead = record.IsRead;
                        item.CreatedDateStr = record.CreatedDate.DateTimeQuestToString("yyyy/MM/dd HH:mm");
                        item.TotalChilds = record.TotalChilds;
                        item.Sender = JsonConvert.DeserializeObject<IdentityEmailAddress>(record.Sender);
                        item.Receiver = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(record.Receiver);

                        var attachFiles = JsonConvert.DeserializeObject<List<IdentityEmailAttachment>>(record.Attachments);
                        if (attachFiles.HasData())
                            item.TotalAttachments = attachFiles.Count();

                        item.DetailTk = SecurityHelper.GenerateSecureLink("MyEmail", "EmailDetail", new { id = record.Id });

                        models.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to LoadData because: " + ex.ToString());
            }

            return Json(new { data = models, meta = meta });
        }

        [HttpPost]
        public ActionResult GetMessageParts()
        {
            var parentIdHash = Request["query[Id]"] != null ? Request["query[Id]"].ToString() : string.Empty;

            ManageEmailModel model = new ManageEmailModel();
            PagingMeta meta = new PagingMeta();
            var models = new List<ManageEmailViewModel>();

            try
            {
                model.Keyword = Request["query[generalSearch]"] != null ? Request["query[generalSearch]"].ToString() : string.Empty;
                var currentPage = 1;
                if (Request["Page"] != null)
                {
                    currentPage = Utils.ConvertToInt32(Request["Page"], 1);
                }

                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();

                meta.page = Utils.ConvertToInt32(Request["pagination[page]"], 1);
                meta.pages = 2;
                meta.perpage = Utils.ConvertToInt32(Request["pagination[perpage]"]);
                meta.field = Request["sort[field]"];
                meta.sort = Request["sort[sort]"];

                if (meta.perpage == 0) meta.perpage = SystemSettings.DefaultPageSize;

                var filter = new IdentityEmailMessage
                {
                    Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    AgencyId = agencyId,
                    StaffId = staffId,
                    ParentMessageIdHash = parentIdHash,
                    PageIndex = currentPage,
                    PageSize = Utils.ConvertToInt32(Request["pagination[perpage]"]),
                    SortField = meta.field,
                    SortType = meta.sort
                };

                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailMessage>();
                var results = storeEmail.GetMessageParts(filter);
                if (results.HasData())
                {
                    meta.total = results[0].TotalCount;

                    foreach (var record in results)
                    {
                        var item = ParsingEmailDataToView(record);
                        item.DetailTk = SecurityHelper.GenerateSecureLink("MyEmail", "EmailPartDetail", new { id = record.Id });

                        models.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to GetDetails because: " + ex.ToString());
            }

            return Json(new { data = models, meta = meta });
        }

        //[AccessRoleChecker]
        public ActionResult Index()
        {
            try
            {

            }
            catch (Exception ex)
            {
                logger.Error("Failed to get data because: " + ex.ToString());
            }

            return View();
        }

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EmailDetail(int id)
        {  
            var model = new ManageEmailViewModel();
            try
            {
                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailMessage>();
                var result = storeEmail.GetDetailById(id);
                await Task.FromResult(result);

                if(result != null)
                {
                    model = ParsingEmailDataToView(result);
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get EmailDetail because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_Detail", model);
        }

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EmailPartDetail(int id)
        {
            var model = new ManageEmailViewModel();
            try
            {
                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailMessage>();
                var result = storeEmail.GetPartDetailById(id);
                await Task.FromResult(result);

                if (result != null)
                {
                    model = ParsingEmailDataToView(result);                    
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get EmailPartDetail because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_PartDetail", model);
        }

        [PreventCrossOrigin]
        [IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult ToMyJobSeeker()
        {
            ManageEmailSendingModel model = new ManageEmailSendingModel();
            try
            {
                model.job_seeker_id = Utils.ConvertToInt32(Request["job_seeker_id"]);
                model.receiver = Request["receiver"] != null ? Request["receiver"].ToString() : string.Empty;
                model.is_online = Utils.ConvertToInt32(Request["is_online"]);

                model.tk = Request["tk"];
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get MailToMyJobSeeker because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_PopupComposer", model);
        }

        [PreventCrossOrigin]
        [IsValidURLRequest]
        [AccessRoleChecker]
        public ActionResult ToMyCompany()
        {
            ManageEmailSendingModel model = new ManageEmailSendingModel();
            try
            {
                model.company_id = Utils.ConvertToInt32(Request["company_id"]);
                model.receiver = Request["receiver"] != null ? Request["receiver"].ToString() : string.Empty;
                model.tk = Request["tk"];
            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get MailToMyJobSeeker because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_PopupComposer", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("SendEmail")]
        public async Task<ActionResult> SendEmail_Post(ManageEmailSendingModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            List<IdentityEmailAttachment> attachedFiles = null;

            try
            {
                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();
                var myEmail = string.Empty;

                var tk = string.Empty;

                if (!string.IsNullOrEmpty(model.controller_name))
                {
                    if (model.job_seeker_id > 0)
                        tk = SecurityHelper.GenerateUrlToken(model.controller_name, model.action_name, new { job_seeker_id = model.job_seeker_id, receiver = model.receiver, is_online = model.is_online });
                    else
                        tk = SecurityHelper.GenerateUrlToken(model.controller_name, model.action_name, new { company_id = model.company_id, receiver = model.receiver });
                }

                //if (model.job_seeker_id > 0)
                //    tk = SecurityHelper.GenerateUrlToken("MyEmail", "ToMyJobSeeker", new { job_seeker_id = model.job_seeker_id, receiver = model.receiver, is_online = model.is_online });
                //else
                //    tk = SecurityHelper.GenerateUrlToken("MyEmail", "ToMyCompany", new { company_id = model.company_id, receiver = model.receiver });

                await Task.FromResult(tk);

                if (tk != model.tk)
                {
                    return Json(new { success = isSuccess, message = ManagerResource.COMMON_ERROR_DATA_INVALID, title = ManagerResource.LB_ERROR_OCCURED });
                }

                if (string.IsNullOrEmpty(model.body))
                {
                    message = string.Format("{0}: {1}", ManagerResource.LB_CONTENT, ManagerResource.COMMON_ERROR_NULL_VALUE);

                    return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_ERROR_OCCURED, clientcallback="ShowMyModalAgain();" });
                }

                var emailServers = CommonHelpers.GetListEmailServers(agencyId);
                var emailList = CommonHelpers.GetListEmailSettings(agencyId, staffId);
                if (!emailServers.HasData())
                {
                    message = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                    return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                }

                if (string.IsNullOrEmpty(message))
                {
                    if (!emailList.HasData())
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                if (string.IsNullOrEmpty(message))
                {
                    if (!emailList.HasData())
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                var mailSetting = emailList.Where(x => x.EmailType == (int)EnumEmailSettingTypes.OutGoing).FirstOrDefault();
                if (mailSetting == null)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        if (string.IsNullOrEmpty(mailSetting.EmailPasswordHash))
                        {
                            message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.ERROR_EMAIL_INVALID_LOGIN_FORMAT);
                            return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                        }                            
                    }

                    myEmail = mailSetting.Email;
                }

                var mailServer = emailServers.Where(x => x.Id == mailSetting.EmailServerId).FirstOrDefault();
                if (mailServer == null)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                if (mailServer.SendingConfig == null)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                if(!string.IsNullOrEmpty(message))
                    return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });

                var bodyContent = HtmlHelpers.RemoveScriptTags(model.body);

                //Send to user
                var emailModel = new EmailModel();
                if (model.receiver.Contains(";"))
                {
                    emailModel.Receivers = model.receiver.Split(';').ToList();
                }
                else
                {
                    emailModel.Receivers = new List<string>();
                    emailModel.Receivers.Add(model.receiver);
                }
                    
                emailModel.Sender = myEmail;
                emailModel.Subject = model.subject;
                emailModel.Body = bodyContent;
                emailModel.MailServer = mailServer.SendingConfig.SMTPServer;
                emailModel.Port = mailServer.SendingConfig.Port;
                emailModel.UseSSL = mailServer.SendingConfig.SSLRequired;

                emailModel.SenderPwd = Utility.TripleDESDecrypt(myEmail, mailSetting.EmailPasswordHash);

                if (model.attachments.HasData())
                {
                    attachedFiles = new List<IdentityEmailAttachment>();
                    var storageFolderPath = string.Format("OutGoing/{0}/{1}", agencyId, staffId);

                    emailModel.Attachments = new List<Attachment>();

                    foreach (var item in model.attachments)
                    {
                        if(item != null)
                        {
                            //Storage to file
                            var filePath = FileUploadHelper.UploadEmailAttachmentToFile(item, item.FileName, storageFolderPath);
                            if (!string.IsNullOrEmpty(filePath))
                            {
                                var attachFile = new IdentityEmailAttachment();
                                attachFile.FileName = item.FileName;
                                attachFile.FilePath = filePath;
                                attachFile.FileSize = item.ContentLength.ToString();
                                attachedFiles.Add(attachFile);

                                var attachment = new Attachment(item.InputStream, item.FileName);
                                emailModel.Attachments.Add(attachment);
                            }
                        }                        
                    }
                }

                message = SendEmail(emailModel);

                if (!string.IsNullOrEmpty(message))
                {
                    //Delete all uploaded files if error
                    if (attachedFiles.HasData())
                    {
                        foreach (var item in attachedFiles)
                        {
                            FileUploadHelper.DeleteFile(item.FilePath);
                        }
                    }

                    return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                }

                //Storage to DB
                var mailInfo = new IdentityEmailBusiness();
                mailInfo.AgencyId = agencyId;
                mailInfo.StaffId = staffId;
                mailInfo.Subject = model.subject;
                mailInfo.Message = bodyContent;
                mailInfo.Receiver = model.receiver;
                mailInfo.Sender = myEmail;
                mailInfo.JobSeekerId = model.job_seeker_id;
                mailInfo.CompanyId = model.company_id;
                mailInfo.IsOnlineUser = model.is_online == 1 ? true : false ;
                mailInfo.Status = 1;
                if (attachedFiles.HasData())
                    mailInfo.Attachments = JsonConvert.SerializeObject(attachedFiles);

                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailBusiness>();

                storeEmail.Insert(mailInfo);

                return Json(new { success = true, message = ManagerResource.LB_EMAIL_SENT_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                logger.Error("Failed to exec ToMyJobSeeker_Post because: " + ex.ToString());

                //Delete all uploaded files if error
                if (attachedFiles.HasData())
                {
                    foreach (var item in attachedFiles)
                    {
                        FileUploadHelper.DeleteFile(item.FilePath);
                    }
                }
            }

            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("BatchSendEmail")]
        public async Task<ActionResult> BatchSendEmail(ManageEmailBatchSendingModel model)
        {
            var message = string.Empty;
            var isSuccess = false;
            
            try
            {
                var agencyId = GetCurrentAgencyId();
                var staffId = GetCurrentStaffId();
                var myEmail = string.Empty;               
              
                if (string.IsNullOrEmpty(model.body))
                {
                    message = string.Format("{0}: {1}", ManagerResource.LB_CONTENT, ManagerResource.COMMON_ERROR_NULL_VALUE);

                    return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_ERROR_OCCURED, clientcallback = "ShowMyModalAgain();" });
                }

                var emailServers = CommonHelpers.GetListEmailServers(agencyId);
                var emailList = CommonHelpers.GetListEmailSettings(agencyId, staffId);
                if (!emailServers.HasData())
                {
                    message = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                    return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                }

                if (string.IsNullOrEmpty(message))
                {
                    if (!emailList.HasData())
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                if (string.IsNullOrEmpty(message))
                {
                    if (!emailList.HasData())
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                var mailSetting = emailList.Where(x => x.EmailType == (int)EnumEmailSettingTypes.OutGoing).FirstOrDefault();
                if (mailSetting == null)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        if (string.IsNullOrEmpty(mailSetting.EmailPasswordHash))
                        {
                            message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.ERROR_EMAIL_INVALID_LOGIN_FORMAT);
                            return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                        }
                    }

                    myEmail = mailSetting.Email;
                }

                var mailServer = emailServers.Where(x => x.Id == mailSetting.EmailServerId).FirstOrDefault();
                if (mailServer == null)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = string.Format("<a href='/Settings/Email'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SETTINGS_OUTGOING);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                if (mailServer.SendingConfig == null)
                {
                    if (string.IsNullOrEmpty(message))
                    {
                        message = string.Format("<a href='/EmailServer'>{0}</a>", EmailResource.LB_LINK_TO_CONFIG_EMAIL_SERVER);
                        return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });
                    }
                }

                if (!string.IsNullOrEmpty(message))
                    return Json(new { success = false, message = message, title = ManagerResource.LB_ERROR_OCCURED });

                var bodyContent = HtmlHelpers.RemoveScriptTags(model.body);

                if (!string.IsNullOrEmpty(model.ReceiversJsonList))
                {
                    model.SelectedObjects = JsonConvert.DeserializeObject<List<ManageEmailObjectInfoModel>>(model.ReceiversJsonList);
                }

                var emailModel = new ApiBatchEmailModel();
                emailModel.TargetType = model.target_type;

                //Identity for storage to DB
                var mailInfo = new IdentityEmailBusiness();
                mailInfo.AgencyId = agencyId;
                mailInfo.StaffId = staffId;
                mailInfo.Subject = model.subject;
                mailInfo.Message = bodyContent;
                mailInfo.Sender = myEmail;
                mailInfo.IsOnlineUser = model.is_online == 1 ? true : false;
                mailInfo.Status = 1;

                mailInfo.Receivers = new List<IdentityEmailAddObject>();

                //if (attachedFiles.HasData())
                //    mailInfo.Attachments = JsonConvert.SerializeObject(attachedFiles);


                if (model.SelectedObjects.HasData())
                {
                    emailModel.Receivers = new List<ApiEmailSendingModel>();
                    foreach (var item in model.SelectedObjects)
                    {                        
                        if (!string.IsNullOrEmpty(item.email))
                        {
                            var it = new ApiEmailSendingModel();
                            it.email = item.email;
                            it.is_online = model.is_online;
                            if(model.target_type == (int)EnumEmailTargetType.JobSeeker)
                            {
                                it.job_seeker_id = item.object_id;
                            }

                            if (model.target_type == (int)EnumEmailTargetType.Company)
                            {
                                it.company_id = item.object_id;
                            }                           

                            emailModel.Receivers.Add(it);

                            var mailAdd = new IdentityEmailAddObject();
                            mailAdd.object_id = item.object_id;
                            mailAdd.email = item.email;

                            mailInfo.Receivers.Add(mailAdd);
                        }                        
                    }                    
                }

                emailModel.Sender = myEmail;
                emailModel.Subject = model.subject;
                emailModel.Body = bodyContent;
                emailModel.MailServer = mailServer.SendingConfig.SMTPServer;
                emailModel.Port = mailServer.SendingConfig.Port;
                emailModel.UseSSL = mailServer.SendingConfig.SSLRequired;                
                //emailModel.SenderPwd = Utility.TripleDESDecrypt(myEmail, mailSetting.EmailPasswordHash);
                emailModel.SenderPwdHash = mailSetting.EmailPasswordHash;

                //Begin Generate Files
                await CdnServices.GenerateStackFiles(emailModel);
                
                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailBusiness>();

                //Storage to DB
                var status = storeEmail.BatchInsert(mailInfo);
                await Task.FromResult(status);

                return Json(new { success = true, message = ManagerResource.LB_EMAIL_SENT_SUCCESS, title = ManagerResource.LB_NOTIFICATION });
            }
            catch (Exception ex)
            {
                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;
                logger.Error("Failed to exec BatchToMyJobSeeker_Post because: " + ex.ToString());                
            }

            return Json(new { success = isSuccess, message = message, title = ManagerResource.LB_NOTIFICATION });
        }

        private ManageEmailViewModel ParsingEmailDataToView(IdentityEmailMessage item)
        {           
            if(item != null)
            {
                var model = new ManageEmailViewModel();
                model.Id = item.Id;
                model.Subject = item.Subject;
                model.Message = item.Message;
                model.ShortMessage = item.ShortMessage;
                if (!string.IsNullOrEmpty(model.ShortMessage))
                {
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = model.ShortMessage.Split(stringSeparators, StringSplitOptions.None);
                    if (lines.HasData())
                    {
                        model.ShortMessage = lines[0];
                    }
                }

                model.MessageId = item.MessageId;
                model.IdHash = item.MessageIdHash;
                model.IsRead = item.IsRead;
                model.CreatedDateStr = item.CreatedDate.DateTimeQuestToString("yyyy/MM/dd HH:mm");
                model.TotalChilds = item.TotalChilds;
                model.Sender = JsonConvert.DeserializeObject<IdentityEmailAddress>(item.Sender);
                model.Receiver = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(item.Receiver);
                model.Cc = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(item.Cc);
                model.Bcc = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(item.Bcc);
                model.AttachFiles = JsonConvert.DeserializeObject<List<IdentityEmailAttachment>>(item.Attachments);
                model.MessageParts = item.MessageParts;

                if (model.AttachFiles.HasData())
                    model.TotalAttachments = model.AttachFiles.Count();

                if (model.MessageParts.HasData())
                {
                    foreach (var part in model.MessageParts)
                    {
                        if (!string.IsNullOrEmpty(part.Attachments))
                        {
                            part.AttachedFiles = JsonConvert.DeserializeObject<List<IdentityEmailAttachment>>(part.Attachments);
                        }
                    }
                }

                return model;
            }

            return null;
        }

        //[Obsolete]
        private string SendEmail(EmailModel model)
        {
            var strError = string.Empty;
            var message = string.Empty;
            var linkToMailSv = "<a href='/EmailServer'>{0}</a>";
            var linkToMailSetting = "<a href='/Settings/Email'>{0}</a>";
            try
            {
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(model.MailServer);

                mail.From = new MailAddress(model.Sender);
                if(!string.IsNullOrEmpty(model.Receiver))
                    mail.To.Add(model.Receiver);

                if (model.Receivers.HasData())
                {
                    foreach (var item in model.Receivers)
                    {
                        mail.To.Add(item);
                    }
                }

                mail.Subject = model.Subject;
                mail.Body = model.Body;

                if (model.Attachments.HasData())
                {
                    foreach (var item in model.Attachments)
                    {
                        mail.Attachments.Add(item);
                    }
                }

                mail.IsBodyHtml = true;

                SmtpServer.Port = model.Port;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(model.Sender, model.SenderPwd);
                SmtpServer.EnableSsl = model.UseSSL;

                SmtpServer.Send(mail);
            }
            catch (SmtpException smtpException)
            {
                // ou can put a switch block to check for different exceptions or errors    
                //To checks if the destination mailbox is busy 
                if (smtpException.StatusCode == SmtpStatusCode.MailboxBusy)
                {
                    strError = string.Format("The destination mailbox [{0}] is busy.", model.MailServer);
                    message = string.Format(EmailResource.ERROR_FORMAT_SMTP_BUSY, model.MailServer);
                    message = string.Format(linkToMailSv, message);
                }                    

                //To check if the client is authenticated or is allowed to send email using the specified SMTP host 
                if (smtpException.StatusCode == SmtpStatusCode.ClientNotPermitted)
                {
                    strError = string.Format("The client [{0}] is authenticated or is allowed to send email using the specified SMTP host.", model.Sender);
                    message = string.Format(EmailResource.ERROR_FORMAT_SMTP_CLIENT_NOT_PERMITTED, model.Sender);
                    message = string.Format(linkToMailSv, message);
                }                    

                //The following code checks if the email message is too large to be stored in destination mailbox 
                if (smtpException.StatusCode == SmtpStatusCode.ExceededStorageAllocation)
                {
                    strError = string.Format("The email message is too large to be stored in destination mailbox [{0}].", model.MailServer);
                    message = string.Format(EmailResource.ERROR_FORMAT_SMTP_EMAIL_TOO_LARGE, model.MailServer);
                    message = string.Format(linkToMailSv, message);
                }

                //The SMTP server is configured to accept only TLS connections
                //The solution is for the user to set EnableSsl=true on the SMTP Client.
                if (smtpException.StatusCode == SmtpStatusCode.MustIssueStartTlsFirst)
                {
                    strError = string.Format("The SMTP server [{0}] is configured to accept only TLS connections (SSL). Or please help to check you account [{0}] information.", model.MailServer);
                    message = string.Format(EmailResource.ERROR_EMAIL_INVALID_LOGIN_FORMAT, model.MailServer, model.Sender);

                    message = string.Format(linkToMailSetting, message);
                }

                // To check if the email was successfully sent to the SMTP service 
                if (smtpException.StatusCode == SmtpStatusCode.Ok)
                    strError = string.Empty;

                // When the SMTP host is not found check for the following value 
                if (smtpException.StatusCode == SmtpStatusCode.GeneralFailure)
                {
                    strError = string.Format("The SMTP host [{0}] is not found check for the following value .", model.MailServer);
                    message = string.Format(EmailResource.ERROR_FORMAT_SMTP_NOT_FOUND, model.MailServer);
                    message = string.Format(linkToMailSv, message);
                }

                if(!string.IsNullOrEmpty(strError))
                    logger.Error(strError);
            }
            catch (Exception ex)
            {
                strError = string.Format("Failed when sending email due to {0}", ex.ToString());

                message = ManagerResource.COMMON_ERROR_EXTERNALSERVICE_TIMEOUT;

                logger.Error(strError);
            }

            return message;
        }

        //[IsValidURLRequest]
        [HttpPost]
        public ActionResult JobSeekerMailBox(ManageEmailBusinessModel model)
        {
            int currentPage = 1;
            int pageSize = SystemSettings.DefaultPageSize;

            if (Request["CurrentMailBoxPageIdx"] != null)
                currentPage = Utils.ConvertToInt32(Request["CurrentMailBoxPageIdx"], 1);
            try
            {
                var apiFilterModel = new IdentityEmailBusiness
                {
                    Keyword = !string.IsNullOrEmpty(model.Keyword) ? model.Keyword.Trim() : null,
                    PageIndex = currentPage,
                    PageSize = pageSize,
                    CompanyId = model.company_id,
                    JobSeekerId = model.job_seeker_id,
                    AgencyId = GetCurrentAgencyId(),
                    StaffId = GetCurrentStaffId(),
                    IsOnlineUser = model.is_online == 1 ? true : false
                };

                var store = GlobalContainer.IocContainer.Resolve<IStoreEmailBusiness>();
                var mailList = store.GetByPage(apiFilterModel);

                if (mailList.HasData())
                {
                    model.SearchResults = new List<ManageEmailBusinessViewModel>();

                    model.TotalCount = mailList[0].TotalCount;
                    model.CurrentPage = currentPage;
                    model.PageSize = pageSize;

                    foreach (var record in mailList)
                    {
                        var item = new ManageEmailBusinessViewModel();
                        item.Id = record.Id;
                        item.Subject = record.Subject;
                        item.Message = record.Message;
                        item.CreatedDateStr = record.CreatedDate.DateTimeQuestToString("yyyy/MM/dd HH:mm");
                        item.Sender = record.Sender;
                        item.Receiver = record.Receiver;

                        var attachFiles = JsonConvert.DeserializeObject<List<IdentityEmailAttachment>>(record.Attachments);
                        if (attachFiles.HasData())
                            item.TotalAttachments = attachFiles.Count();

                        item.DetailTk = SecurityHelper.GenerateSecureLink("MyEmail", "EmailBusinessDetail", new { id = record.Id });

                        model.SearchResults.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to get JobSeekerMailBox: " + ex.ToString());
            }

            return PartialView("Partials/_JobSeekerMailBoxList", model);
        }

        [PreventCrossOrigin]
        [PreventSpam(DelayRequest = 1)]
        [IsValidURLRequest]
        public async Task<ActionResult> EmailBusinessDetail(int id)
        {
            var model = new ManageEmailBusinessViewModel();
            try
            {
                var storeEmail = GlobalContainer.IocContainer.Resolve<IStoreEmailBusiness>();
                var result = storeEmail.GetDetailById(id);
                await Task.FromResult(result);

                if (result != null)
                {
                    model.Id = result.Id;
                    model.Subject = result.Subject;
                    model.Message = result.Message;

                    model.CreatedDateStr = result.CreatedDate.DateTimeQuestToString("yyyy/MM/dd HH:mm");
                    model.Sender = result.Sender;
                    model.Receiver = result.Receiver;

                    model.Cc = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(result.Cc);
                    model.Bcc = JsonConvert.DeserializeObject<List<IdentityEmailAddress>>(result.Bcc);
                    model.AttachFiles = JsonConvert.DeserializeObject<List<IdentityEmailAttachment>>(result.Attachments);

                    if (model.AttachFiles.HasData())
                        model.TotalAttachments = model.AttachFiles.Count();
                }

            }
            catch (Exception ex)
            {
                var strError = string.Format("Failed when trying get EmailBusinessDetail because: {0}", ex.ToString());
                logger.Error(strError);
            }

            return PartialView("Sub/_EmailBusinessDetail", model);
        }
    }
}