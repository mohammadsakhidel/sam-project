using SamUtils.Utils;
using SamWeb.Code.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SamWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #region CityUtil GetXmlFunc:
            CityUtil.Func_GetXMLContent = () => {
                return System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(Urls.CitiesXmlFile));
            };
            #endregion
        }
    }
}
