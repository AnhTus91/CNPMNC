using System.Web;
using System.Web.Optimization;

namespace GameStore
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/checkout").Include(
                "~/Scripts/checkout.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/store").Include(
                 "~/Content/store.css"));

            bundles.Add(new StyleBundle("~/Content/thanhtoan").Include(
               "~/Content/thanhtoan.css"));





            /*   admin*/
            bundles.Add(new StyleBundle("~/Content/admin/base").Include(
           "~/Content/admin/base.css"));
            bundles.Add(new StyleBundle("~/Content/admin/bstr").Include(
       "~/Content/admin/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/Content/admin/custom").Include(
"~/Content/admin/custom.css"));
            bundles.Add(new StyleBundle("~/Content/admin/index").Include(
"~/Content/admin/index.css"));

            bundles.Add(new StyleBundle("~/Content/admin/style").Include(
"~/Content/admin/style.css"));


            bundles.Add(new ScriptBundle("~/bundles/admin/allproduct").Include("~/Scripts/admin/allproduct.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin/bstr").Include("~/Scripts/admin/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin/donhang").Include("~/Scripts/admin/donhang.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin/jquery").Include("~/Scripts/admin/jquery.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin/main").Include("~/Scripts/admin/main.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin/popper").Include("~/Scripts/admin/popper.js"));
            bundles.Add(new ScriptBundle("~/bundles/admin/header").Include("~/Scripts/admin/header.js"));


            bundles.Add(new ScriptBundle("~/bundles/poppermin", "https://cdn.jsdelivr.net/npm/@popperjs/core@2.9.2/dist/umd/popper.min.js"));
        }
    }
}
