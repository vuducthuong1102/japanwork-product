using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

using SingleSignOn.Services;
using SingleSignOn.Settings;
using System.Threading.Tasks;

namespace SingleSignOn.HttpTracking
{
    /// <summary>
    /// Dummy implementation of the <see cref="HttpEntry"/> interface to file, for illustration purposes.
    /// </summary>
    public sealed class HttpTrackingStore : IHttpTrackingStore
    {      
        public HttpTrackingStore()
        {
        }

        private readonly string path_ = Path.GetTempPath();

        public async Task InsertRecordAsync(HttpEntry record)
        {          
            record.CallBackTime = DateTime.Now;
            record.CallDuration = record.CallBackTime - record.CallDateTime;
            await Task.FromResult(record);

            if (SingleSignOnSettings.AllowToTraceHttpMiddleWare)
            {
                if (record.RequestUri.OriginalString.EndsWith("api/user", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (SingleSignOnSettings.MDS_EnableTracking)
                    {
                        new MdsService().SendHttpEntryInfo(record);
                    }
                }                    
            }                
        }


        public void InsertRecord(HttpEntry record)
        {
            record.CallBackTime = DateTime.Now;
            record.CallDuration = record.CallBackTime - record.CallDateTime;

            if (SingleSignOnSettings.AllowToTraceHttpMiddleWare)
            {
                if (record.RequestUri.OriginalString.EndsWith("api/user", StringComparison.InvariantCultureIgnoreCase))
                {
                    new MdsService().SendHttpEntryInfo(record);
                }
            }
        }
    }
}