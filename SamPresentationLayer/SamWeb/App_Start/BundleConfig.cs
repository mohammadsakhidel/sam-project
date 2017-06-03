using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace SamWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Script Bundles:
            bundles.Add(new ScriptBundle("~/bundles/js/jquery")
                .Include("~/Scripts/jquery-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jquery-ajax")
                .Include("~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jquery-validate")
                .Include("~/Scripts/jquery.validate.min.js")
                .Include("~/Scripts/jquery.validate.unobtrusive.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap")
                .Include("~/Scripts/bootstrap.min.js")
                .Include("~/Scripts/bootstrap-toggle.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jquery-ui")
                .Include("~/Scripts/jquery-ui-{version}.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jquery-cookie")
                .Include("~/Scripts/js.cookie.js"));
            #endregion

            #region Style Bundles:
            bundles.Add(new StyleBundle("~/content/bootstrap")
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/bootstrap-toggle.min.css"));

            bundles.Add(new StyleBundle("~/content/samthemes/defaulttheme/styles")
                .Include("~/Content/SamThemes/DefaultTheme/layout.min.css")
                .Include("~/Content/SamThemes/DefaultTheme/styles.min.css")
                .Include("~/Content/SamThemes/DefaultTheme/elements.min.css"));
            #endregion

            BundleTable.EnableOptimizations = true;
        }
    }
}