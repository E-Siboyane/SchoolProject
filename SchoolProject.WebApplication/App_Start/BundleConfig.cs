using System.Web;
using System.Web.Optimization;

namespace SchoolProject.WebApplication {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            //Bootstrap
            bundles.Add(new StyleBundle("~/bundle/css").Include(
                "~/Scripts/vendors/bootstrap/dist/css/bootstrap.min.css",
                "~/Scripts/vendors/font-awesome/css/font-awesome.min.css",
                "~/Scripts/vendors/nprogress/nprogress.css",
                "~/Scripts/vendors/iCheck/skins/flat/green.css",
                "~/Scripts/vendors/bootstrap-progressbar/css/bootstrap-progressbar-3.3.4.min.css",
               "~/Scripts/vendors/jqvmap/dist/jqvmap.min.css",
                "~/Scripts/vendors/bootstrap-daterangepicker/daterangepicker.css",
                "~/Scripts/vendors/bootstrap-daterangepicker/daterangepicker.css",
                "~/Scripts/vendors/datatables.net-bs/css/dataTables.bootstrap.min.css",
                "~/Scripts/vendors/datatables.net-buttons-bs/css/buttons.bootstrap.min.css",
                "~/Scripts/vendors/datatables.net-fixedheader-bs/css/fixedHeader.bootstrap.min.css",
                "~/Scripts/vendors/datatables.net-responsive-bs/css/responsive.bootstrap.min.css",
                "~/Scripts/vendors/datatables.net-scroller-bs/css/scroller.bootstrap.min.css",
                "~/Scripts/build/css/custom.css"
             ));

            //JQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/ScriptS/vendors/jquery/dist/jquery.min.js"));

            //BootStrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/ScriptS/vendors/bootstrap/dist/js/bootstrap.min.js"));

            //FastClick
            bundles.Add(new ScriptBundle("~/bundles/FastClick").Include(
                       "~/Scripts/vendors/fastclick/lib/fastclick.js"));

            //NProgress
            bundles.Add(new ScriptBundle("~/bundles/NProgress").Include(
                        "~/Scripts/vendors/nprogress/nprogress.js"));

            //Chart
            bundles.Add(new ScriptBundle("~/bundles/Chart").Include(
                         "~/Scripts/vendors/Chart.js/dist/Chart.min.js"));

            //Gauge
            bundles.Add(new ScriptBundle("~/bundles/Gauge").Include(
                           "~/Scripts/vendors/gauge.js/dist/gauge.min.js"));

            //Bootstrap -progressbar
            bundles.Add(new ScriptBundle("~/bundles/Progressbar").Include(
                           "~/Scripts/vendors/bootstrap-progressbar/bootstrap-progressbar.min.js"));

            //iCheck
            bundles.Add(new ScriptBundle("~/bundles/iCheck").Include(
                            "~/Scripts/vendors/iCheck/icheck.min.js"));

            //Skycons
            bundles.Add(new ScriptBundle("~/bundles/Skycons").Include(
                             "~/Scripts/vendors/skycons/skycons.js"));

            //Flot
            bundles.Add(new ScriptBundle("~/bundles/Flot").Include(
                             "~/Scripts/vendors/Flot/jquery.flot.js",
                             "~/Scripts/vendors/Flot/jquery.flot.pie.js",
                             "~/Scripts/vendors/Flot/jquery.flot.time.js",
                             "~/Scripts/vendors/Flot/jquery.flot.stack.js",
                             "~/Scripts/vendors/Flot/jquery.flot.resize.js",
                             "~/Scripts/vendors/flot.orderbars/js/jquery.flot.orderBars.js",
                             "~/Scripts/vendors/flot-spline/js/jquery.flot.spline.min.js",
                             "~/Scripts/vendors/flot.curvedlines/curvedLines.js",
                             "~/Scripts/vendors/DateJS/build/date.js"));
            //JQVMap
            bundles.Add(new ScriptBundle("~/bundles/JQVMap").Include(
                             "~/Scripts/vendors/jqvmap/dist/jquery.vmap.js",
                             "~/Scripts/vendors/jqvmap/dist/maps/jquery.vmap.world.js",
                             "~/Scripts/vendors/jqvmap/examples/js/jquery.vmap.sampledata.js"));

            //Bootstrap - daterangepicker
            bundles.Add(new ScriptBundle("~/bundles/Daterangepicker").Include(
                             "~/Scripts/vendors/moment/min/moment.min.js",
                             "~/Scripts/vendors/bootstrap-daterangepicker/daterangepicker.js",
                             "~/Scripts/vendors/jqvmap/examples/js/jquery.vmap.sampledata.js"));

            //Datatables
            bundles.Add(new ScriptBundle("~/bundles/Datatables").Include(
                             "~/Scripts/vendors/datatables.net/js/jquery.dataTables.min.js",
                             "~/Scripts/vendors/datatables.net-bs/js/dataTables.bootstrap.min.js",
                             "~/Scripts/vendors/datatables.net-buttons/js/dataTables.buttons.min.js",

                             "~/Scripts/vendors/datatables.net-buttons-bs/js/buttons.bootstrap.min.js",
                             "~/Scripts/vendors/datatables.net-buttons/js/buttons.flash.min.js",
                             "~/Scripts/vendors/datatables.net-buttons/js/buttons.html5.min.js",
                             "~/Scripts/vendors/datatables.net-buttons/js/buttons.print.min.js",
                             "~/Scripts/vendors/datatables.net-fixedheader/js/dataTables.fixedHeader.min.js",
                             "~/Scripts/vendors/datatables.net-keytable/js/dataTables.keyTable.min.js",
                             "~/Scripts/vendors/datatables.net-responsive/js/dataTables.responsive.min.js",
                             "~/Scripts/vendors/datatables.net-responsive-bs/js/responsive.bootstrap.js",
                             "~/Scripts/vendors/jszip/dist/jszip.min.js",
                             "~/Scripts/vendors/pdfmake/build/pdfmake.min.js",
                              "~/Scripts/pdfmake/build/vfs_fonts.js"
                             ));

            //Custom Theme Scripts
            bundles.Add(new ScriptBundle("~/bundles/CustomScript").Include(
                             "~/Scripts/build/js/custom.js"));

        }
    }
}
