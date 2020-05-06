using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using SoftwareArchitects.Web.Configuration;

namespace SoftwareArchitects.Web
{
	/// <summary>
	/// Handler for sending files to the client
	/// </summary>
	public class CachingHandler : DelegatingHandler
    {
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			string file = context.Server.MapPath(context.Request.FilePath.Replace(".ashx", ""));
			string filename = file.Substring(file.LastIndexOf('\\') + 1);
			string extension = file.Substring(file.LastIndexOf('.') + 1);

			CachingSection config = (CachingSection)context.GetSection("SoftwareArchitects/Caching");
			if (config != null)
			{
				context.Response.Cache.SetExpires(DateTime.Now.Add(config.CachingTimeSpan));
				context.Response.Cache.SetCacheability(HttpCacheability.Public);
				context.Response.Cache.SetValidUntilExpires(false);
				
				FileExtension fileExtension = config.FileExtensions[extension];
				if (fileExtension != null)
				{
					context.Response.ContentType = fileExtension.ContentType;
				}
			}

			context.Response.AddHeader("content-disposition", "inline; filename=" + filename);
			context.Response.WriteFile(file);
		}

        /// <summary>
        /// Send method.
        /// </summary>
        /// <param name="request">Request parameter</param>
        /// <param name="cancellationToken">Cancellation token parameter</param>
        /// <returns>Return HTTP response.</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = HttpContext.Current;
            string file = context.Server.MapPath(context.Request.FilePath.Replace(".ashx", ""));
            string filename = file.Substring(file.LastIndexOf('\\') + 1);
            string extension = file.Substring(file.LastIndexOf('.') + 1);

            CachingSection config = (CachingSection)context.GetSection("SoftwareArchitects/Caching");
            if (config != null)
            {
                context.Response.Cache.SetExpires(DateTime.Now.Add(config.CachingTimeSpan));
                context.Response.Cache.SetCacheability(HttpCacheability.Public);
                context.Response.Cache.SetValidUntilExpires(false);

                FileExtension fileExtension = config.FileExtensions[extension];
                if (fileExtension != null)
                {
                    context.Response.ContentType = fileExtension.ContentType;
                }
            }

            context.Response.AddHeader("content-disposition", "inline; filename=" + filename);
            context.Response.WriteFile(file);

            // Info.
            return base.SendAsync(request, cancellationToken);
        }

    }
}
