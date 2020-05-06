using System;
using SingleSignOn.Settings;
using SingleSignOn.Extensions;
using SingleSignOn.HttpTracking;

using MDS.Client;

namespace SingleSignOn.Services
{
    public class MdsService
    {
       // private readonly ILog logger = LogProvider.GetCurrentClassLogger();
        public bool SendHttpEntryInfo(HttpEntry entry)
        {
            try
            {
                //Create MDSClient object to connect to DotNetMQ
                //Name of this application: SingleSignOn
                using (var mdsClient = new MDSClient(SingleSignOnSettings.MDS_FromAppName, SingleSignOnSettings.MDS_ServerHost, SingleSignOnSettings.MDS_ServerPort))
                {
                    //Connect to DotNetMQ server
                    mdsClient.Connect();

                    //Create a DotNetMQ Message to send
                    var message = mdsClient.CreateMessage();


                    if (!string.IsNullOrEmpty(SingleSignOnSettings.MDS_FromSrvName))
                    {
                        message.SourceServerName = SingleSignOnSettings.MDS_FromSrvName;
                    }
                   
                    message.DestinationApplicationName = SingleSignOnSettings.MDS_DestAppName;

                    if (!string.IsNullOrEmpty(SingleSignOnSettings.MDS_DestSrvName))
                    {
                        message.DestinationServerName = SingleSignOnSettings.MDS_DestSrvName;
                    }
                        

                    //Get a message as JSON format                    
                    var jsonMessage = entry.ToJson();

                    //Set message data
                    message.MessageData = System.Text.Encoding.UTF8.GetBytes(jsonMessage);

                    //Send message
                    message.Send();

                    //Disconnect from DotNetMQ server
                    mdsClient.Disconnect();
                }

                return true;
            }
            catch
            {
                //logger.ErrorException("Failed to send log entry to MDS server: " + ex.Message, ex);
            }

            return false;
        }
    }
}