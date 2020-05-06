using System.Web;
using System.Web.Optimization;

namespace MySite
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //// Use the development version of Modernizr to develop with and learn from. Then, when you're
            //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //            "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/headerstyles").Include(
                "~/Content/themes/default/css/bootstrap-grid.css",
                "~/Content/themes/default/css/icons.css",
                "~/Content/themes/default/css/animate.min.css",                
                "~/Content/themes/default/css/responsive.css",
                "~/Content/themes/default/css/chosen.css",
                "~/Content/themes/default/css/colors/colors.css",
                "~/Content/themes/default/css/bootstrap.css",
                "~/Content/font-awesome.min.css",
                "~/Content/Extensions/swal/sweetalert2.min.css",
                "~/Scripts/Extensions/Select2/select2.min.css",
                "~/Scripts/Extensions/bootstrap/datepicker/bootstrap-datepicker.min.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/pagescripts").Include(                    
                     "~/Content/themes/default/js/modernizr.js",
                     "~/Content/themes/default/js/script.js",
                     //"~/Content/themes/default/js/bootstrap.min.js",
                     //"~/Content/themes/default/js/bootstrap.bundle.min.js",
                     "~/Content/themes/default/js/wow.min.js",
                     "~/Content/themes/default/js/slick.min.js",
                     "~/Content/themes/default/js/parallax.js",
                     "~/Content/themes/default/js/select-chosen.js",

                     //Extensions                     
                     "~/Scripts/jquery.validate.min.js",
                     "~/Scripts/jquery.validate.unobtrusive.js",
                     "~/Scripts/Common/bootbox-4.4-0.min.js",
                     "~/Scripts/Common/typeahead.bundle.min.js",
                     "~/Scripts/Extensions/throttle.js",
                     "~/Content/Extensions/swal/sweetalert2.min.js",
                     "~/Scripts/Common/common.notifications.js",
                     "~/Scripts/Common/common.modalform.js",
                     "~/Scripts/Common/jquery-masked-input.min.js",
                     "~/Scripts/Extensions/Select2/select2.min.js",
                     "~/Scripts/Extensions/bootstrap/datepicker/bootstrap-datepicker.min.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/headerscripts").Include(
                     "~/Content/themes/default/js/jquery.min.js"
            ));

            BundleTable.EnableOptimizations = true;
        }
    }
}
