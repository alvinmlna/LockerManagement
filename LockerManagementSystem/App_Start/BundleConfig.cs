using System.Web;
using System.Web.Optimization;

namespace LockerManagementSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Content/jquery-ui-1.13.0/jquery-ui.min.js",
                        "~/Scripts/sweetalert2.all.min.js",
                        "~/Scripts/admin-scripts.js",
                        "~/Scripts/font-awesome/all.min.js"
                        ));


            bundles.Add(new ScriptBundle("~/bundles/datatable").Include(
                        "~/Content/my-datatable/datatables.min.js"));

            bundles.Add(new StyleBundle("~/Content/datatable").Include(
                        "~/Content/my-datatable/datatables.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                    "~/Scripts/umd/popper.min.js",
                      "~/Scripts/bootstrap.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/PagedList.css",
                        "~/Content/site.css",
                        "~/Content/icheck-bootstrap.min.css",
                        "~/Content/jquery-ui-1.13.0/theme-jquery-ui.css",
                        "~/Content/font-awesome.min.css"
                      ));


            bundles.Add(new StyleBundle("~/Content/admincss").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/PagedList.css",
                        "~/Content/icheck-bootstrap.min.css",
                        "~/Content/jquery-ui-1.13.0/theme-jquery-ui.css",
                        "~/Content/font-awesome.min.css",
                        "~/Content/admin-styles.css"
                      ));
        }
    }
}
