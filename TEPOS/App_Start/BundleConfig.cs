using System;
using System.Web;
using System.Web.Optimization;

namespace ERP
{
    public class BundleConfig
    {
        public static void AddDefaultIgnorePatterns(IgnoreList ignoreList)
        {
            if (ignoreList == null)
                throw new ArgumentNullException("ignoreList");
            ignoreList.Ignore("*.intellisense.js", OptimizationMode.WhenEnabled);
            ignoreList.Ignore("*-vsdoc.js", OptimizationMode.WhenEnabled);
            ignoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
            ignoreList.Ignore("*.min.js", OptimizationMode.WhenEnabled);
            ignoreList.Ignore("*.min.css", OptimizationMode.WhenEnabled);
        }

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region jquery

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/lib/assets/js/core/jquery.3.2.1.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/CoreJSFiles").Include(
                "~/Scripts/lib/assets/js/core/popper.min.js",
                "~/Scripts/lib/assets/js/core/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jQueryUI").Include(
                "~/Scripts/lib/assets/js/plugin/jquery-ui-1.12.1.custom/jquery-ui.min.js",
                "~/Scripts/lib/assets/js/plugin/jquery-ui-touch-punch/jquery.ui.touch-punch.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/WebFont").Include(
                "~/Scripts/lib/assets/js/plugin/webfont/webfont.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/BjqueryCmn").Include(
                "~/Scripts/lib/raphael-min.js",
                "~/Scripts/lib/jquery.unobtrusive-ajax.js",
                "~/Scripts/jquery.validate*",
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js",
                "~/Scripts/lib/assets/js/core/popper.min.js",
                "~/Scripts/lib/assets/js/core/bootstrap.min.js",
                "~/Scripts/lib/assets/js/plugin/jquery-ui-1.12.1.custom/jquery-ui.min.js",
                "~/Scripts/lib/assets/js/plugin/jquery-ui-touch-punch/jquery.ui.touch-punch.min.js",
                //jQuery Scrollbar
                "~/Scripts/lib/assets/js/plugin/jquery-scrollbar/jquery.scrollbar.min.js",
                //Datatables
                "~/Scripts/lib/DataTables/datatables.min.js",
                //theam JS
                "~/Scripts/lib/assets/js/atlantis.js",
                //notify JS
                "~/Scripts/lib/assets/js/plugin/bootstrap-notify/bootstrap-notify.min.js",
                //Theme color settings, don't include it in your project!
                "~/Scripts/lib/assets/js/theme-color-setting.js",
                //JQ bootstrap validation
                "~/Scripts/lib/assets/js/plugin/jquery-bootstrap-validation/jqBootstrapValidation.js",
                //site js
                "~/Scripts/site.js",
                "~/Scripts/lib/js-manager.js",
                "~/Scripts/lib/js-message.js"
            ));

            #endregion


            #region CSS

             bundles.Add(new StyleBundle("~/CustomCSS/TCommonCss").Include(
                            "~/Scripts/lib/assets/css/bootstrap.min.css",
                            "~/Scripts/lib/DataTables/datatables.min.css",
                            "~/Scripts/lib/assets/css/atlantis.min.css",
                            "~/Content/site.css"
                        ));



            #endregion

            

            #region Select Chosen

            bundles.Add(new ScriptBundle("~/bundles/SelectChosen").Include(
                "~/Scripts/lib/SelectChosen/js/chosen.jquery.min.js"
            ));

            bundles.Add(new StyleBundle("~/CustomCSS/SelectChosen").Include(
                "~/Scripts/lib/SelectChosen/css/chosen.css"
            ));

            #endregion



            #region CkEditor

            bundles.Add(new ScriptBundle("~/bundles/CkEditor").Include(
                "~/Scripts/lib/ckeditor/ckeditor.js"
                ));

            bundles.Add(new StyleBundle("~/CustomCSS/CkEditor").Include(
              "~/Scripts/lib/ckeditor/contents.css"
              ));

            #endregion



            #region DateTime Picker

            bundles.Add(new ScriptBundle("~/bundles/DateTimePicker").Include(
                "~/Scripts/lib/DatePicker/jquery.datetimepicker.js",
                "~/Scripts/lib/DatePicker/dateFormatter.js",
                "~/Scripts/lib/DatePicker/dateTimePickerLoader.js"
            ));

            bundles.Add(new StyleBundle("~/CustomCSS/DateTimePicker").Include(
                "~/Scripts/lib/DatePicker/jquery.datetimepicker.css"
            ));

            #endregion


            #region Jq TreeView

            bundles.Add(new ScriptBundle("~/bundles/JqTreeView").Include(
               "~/Scripts/lib/JqTreeView/jquery.treeview.js",
               "~/Scripts/lib/JqTreeView/jquery.cookie.js"
               ));

            bundles.Add(new StyleBundle("~/CustomCSS/JqTreeView").Include(
              "~/Scripts/lib/JqTreeView/jquery.treeview.css"
              ));

            #endregion

        }

    }
}
