using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;
using System.Web.Http.Filters;
using System.Net.Http.Headers;

namespace SingleSignOn.Attributes
{
    public class CacheFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 10 seconds.
        /// </summary>
        /// <value>The cache duration in seconds.</value>
        public int Duration
        {
            get;
            set;
        }

        /// <summary>
        /// Specify the duration value in configuration file
        /// </summary>
        public string DurationAppSetingKey
        {
            get;
            set;
        }

        public CacheFilterAttribute()
        {
            
                //Default duration
                Duration = 10;            
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            //Using Duration prioirty in setting key
            if (!string.IsNullOrEmpty(DurationAppSetingKey))
            {
                try
                {
                    Duration = int.Parse(ConfigurationManager.AppSettings[DurationAppSetingKey]);
                }
                catch { }
            }

            if (Duration <= 0) return;
            TimeSpan cacheDuration = TimeSpan.FromSeconds(Duration);
            var response = actionExecutedContext.Response;

            if (response != null)
            { 
                response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    //cache.SetCacheability(HttpCacheability.Public);
                    Public = true,

                    //cache.SetMaxAge(cacheDuration);
                    MaxAge = cacheDuration,                    
                };
               
                if (response.Content!=null)
                {
                    // cache.SetExpires(DateTime.Now.Add(cacheDuration));
                    response.Content.Headers.Expires = DateTimeOffset.Now.Add(cacheDuration);                    
                }

                //cache.SetVaryByCustom("*");
                response.Headers.TryAddWithoutValidation("Vary","*");
            }           
        }       
    }
}