using System;
using ApiJobMarket.Settings;
using ApiJobMarket.Extensions;
using ApiJobMarket.HttpTracking;

using MDS.Client;

namespace ApiJobMarket.Services
{
    public class MdsService
    {
       // private readonly ILog logger = LogProvider.GetCurrentClassLogger();
        public bool SendHttpEntryInfo(HttpEntry entry)
        {
            try
            {
                //Create MDSClient object to connect to DotNetMQ
                //Name of this application: ApiJobMarket
                using (var mdsClient = new MDSClient(ApiJobMarketSettings.MDS_FromAppName, ApiJobMarketSettings.MDS_ServerHost, ApiJobMarketSettings.MDS_ServerPort))
                {
                    //Connect to DotNetMQ server
                    mdsClient.Connect();

                    //Create a DotNetMQ Message to send
                    var message = mdsClient.CreateMessage();


                    if (!string.IsNullOrEmpty(ApiJobMarketSettings.MDS_FromSrvName))
                    {
                        message.SourceServerName = ApiJobMarketSettings.MDS_FromSrvName;
                    }
                   
                    message.DestinationApplicationName = ApiJobMarketSettings.MDS_DestAppName;

                    if (!string.IsNullOrEmpty(ApiJobMarketSettings.MDS_DestSrvName))
                    {
                        message.DestinationServerName = ApiJobMarketSettings.MDS_DestSrvName;
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