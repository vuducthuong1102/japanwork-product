using System.Web;
using System.Web.Optimization;

namespace SingleSignOn
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/avatar").Include(
                      "~/Scripts/Extensions/Image/site.avatar.js"));

            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                      "~/Scripts/Extensions/Image/jquery.Jcrop.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
                      "~/Scripts/Extensions/Image/jquery.form.js"));

            bundles.Add(new StyleBundle("~/Content/jcrop").Include(
                      "~/Content/Extensions/Image/jquery.Jcrop.css"));

            bundles.Add(new StyleBundle("~/Content/avatar").Include(
                      "~/Content/Extensions/Image/site.avatar.css"));
        }
    }
}
