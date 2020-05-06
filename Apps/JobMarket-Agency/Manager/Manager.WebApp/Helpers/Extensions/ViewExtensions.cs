using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Manager.WebApp.Controllers;
using System.Web.Hosting;
using MsSql.AspNet.Identity;
using Autofac;
using System.Linq;
using MsSql.AspNet.Identity.MsSqlStores;
using System.Web.Caching;

namespace Manager.WebApp.Helpers
{
    public static class ViewExtensions
    {
        public static string RenderToString(this PartialViewResult partialView)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                throw new NotSupportedException("An HTTP context is required to render the partial view to a string");
            }

            //var controllerName = httpContext.Request.RequestContext.RouteData.Values["controller"].ToString();
            //var controller = (ControllerBase)ControllerBuilder.Current.GetControllerFactory().CreateController(httpContext.Request.RequestContext, "Templates");
            //var controllerContext = new ControllerContext(httpContext.Request.RequestContext, controller);

            var context = new HttpContextWrapper(HttpContext.Current);
            var routeData = new RouteData();
            var controllerContext = new ControllerContext(new RequestContext(context, routeData), new TemplatesController());

            var view = ViewEngines.Engines.FindPartialView(controllerContext, partialView.ViewName).View;

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                using (var tw = new HtmlTextWriter(sw))
                {
                    view.Render(new ViewContext(controllerContext, view, partialView.ViewData, partialView.TempData, tw), tw);
                }
            }

            return sb.ToString();
        }

        public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);
            else
            {
                StringBuilder builder = new StringBuilder();
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>\n");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
        }

        public static MvcHtmlString RawHtmlCustom(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);
            else
            {
                StringBuilder builder = new StringBuilder();
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>\n");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
        }

        //public static string GetRazorViewAsString(object model, string view)
        //{
        //    var guid = Guid.NewGuid();
        //    var filePath = AppDomain.CurrentDomain.BaseDirectory + guid + ".cshtml";
        //    File.WriteAllText(filePath, string.Format("@inherits System.Web.Mvc.WebViewPage<{0}>\r\n {1}", model.GetType().FullName, view));
        //    var st = new StringWriter();
        //    var context = new HttpContextWrapper(HttpContext.Current);
        //    var routeData = new RouteData();
        //    var controllerContext = new ControllerContext(new RequestContext(context, routeData), new TempController());
        //    var razor = new RazorView(controllerContext, "~/" + guid + ".cshtml", null, false, null);
        //    razor.Render(new ViewContext(controllerContext, razor, new ViewDataDictionary(model), new TempDataDictionary(), st), st);
        //    File.Delete(filePath);
        //    return st.ToString();
        //}
    }

    //public class MyVirtualPathProvider : VirtualPathProvider
    //{
    //    public override bool FileExists(string virtualPath)
    //    {
    //        var page = FindPage(virtualPath);
    //        if (page == null)
    //        {
    //            return base.FileExists(virtualPath);
    //        }
    //        else
    //        {
    //            return true;
    //        }
    //    }

    //    public override VirtualFile GetFile(string virtualPath)
    //    {
    //        var page = FindPage(virtualPath);
    //        if (page == null)
    //        {
    //            return base.GetFile(virtualPath);
    //        }
    //        else
    //        {
    //            return new MyVirtualFile(virtualPath, page.ViewData.ToArray());
    //        }
    //    }

    //    private IdentityPageLayout FindPage(string virtualPath)
    //    {
    //        var pageLayoutStore = GlobalContainer.IocContainer.Resolve<IStorePageLayout>();

    //        var allLayouts = pageLayoutStore.GetList();

    //        IdentityPageLayout res = null;

    //        if (allLayouts.HasData())
    //            res = allLayouts[0];

    //        return res;
    //    }
    //}

    //public class MyVirtualFile : VirtualFile
    //{
    //    private byte[] data;

    //    public MyVirtualFile(string virtualPath, byte[] data) : base(virtualPath)
    //    {
    //        this.data = data;
    //    }

    //    public override System.IO.Stream Open()
    //    {
    //        return new MemoryStream(data);
    //    }
    //}

    //public class CustomVirtualPathProvider : VirtualPathProvider
    //{
    //    private readonly string _rootpath;
    //    private readonly string _fileending;

    //    public CustomVirtualPathProvider(string rootpath, string fileending)
    //    {
    //        _rootpath = rootpath;
    //        _fileending = fileending;
    //    }

    //    public bool IsDynamicView(string virtualPath)
    //    {
    //        if (!virtualPath.ToLower().Contains(_rootpath)) return false;
    //        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(virtualPath);
    //        var viewexists = DoesViewExistInDataBase(fileNameWithoutExtension);

    //        return viewexists && virtualPath.ToLower().Contains(_rootpath) && virtualPath.ToLower().Contains(_fileending);
    //    }

    //    public bool DoesViewExistInDataBase(string virtualPath)
    //    {
    //        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(virtualPath);

    //        //using (var command = connection.CreateCommand())
    //        //{
    //        //    command.CommandText = "Select count(*) from Views where Name = '" + fileNameWithoutExtension + "'";
    //        //    return command.ExecuteScalar().ToString() == "1";
    //        //}

    //        var pageLayoutStore = GlobalContainer.IocContainer.Resolve<IStorePageLayout>();
    //        var allLayouts = pageLayoutStore.GetList();

    //        IdentityPageLayout res = null;

    //        if (allLayouts.HasData())
    //            res = allLayouts.Where(x=>x.ViewName == fileNameWithoutExtension).FirstOrDefault();

    //        if (res == null)
    //            return false;
    //        else
    //            return true;
    //    }

    //    public String GetViewBodyFromDb(string virtualPath)
    //    {
    //        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(virtualPath);
    //        var pageLayoutStore = GlobalContainer.IocContainer.Resolve<IStorePageLayout>();
    //        var allLayouts = pageLayoutStore.GetList();

    //        IdentityPageLayout res = null;

    //        if (allLayouts.HasData())
    //            res = allLayouts.Where(x => x.ViewName == fileNameWithoutExtension).FirstOrDefault();

    //        if (res == null)
    //            return res.Body;
    //        else
    //            return string.Empty;
    //    }

    //    public override bool FileExists(string virtualPath)
    //    {
    //        return IsDynamicView(virtualPath) || base.FileExists(virtualPath);
    //    }

    //    public override VirtualFile GetFile(string virtualPath)
    //    {
    //        return IsDynamicView(virtualPath) ? new CustomVirtualFile(virtualPath, GetViewBodyFromDb(virtualPath)) : base.GetFile(virtualPath);
    //    }

    //    public override string GetFileHash(string virtualPath, System.Collections.IEnumerable virtualPathDependencies)
    //    {
    //        return IsDynamicView(virtualPath) ? virtualPath : base.GetFileHash(virtualPath, virtualPathDependencies);
    //    }

    //    public override CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
    //    {
    //        return IsDynamicView(virtualPath) ? null : Previous.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
    //    }
    //}

    //public class CustomVirtualFile : VirtualFile
    //{
    //    private readonly string _content;
    //    public bool Exists
    //    {
    //        get { return (_content != null); }
    //    }
    //    public CustomVirtualFile(string virtualPath, string body)
    //        : base(virtualPath)
    //    {
    //        _content = body;
    //    }
    //    public override Stream Open()
    //    {
    //        var encoding = new ASCIIEncoding();
    //        return new MemoryStream(encoding.GetBytes(_content), false);
    //    }
    //}
}