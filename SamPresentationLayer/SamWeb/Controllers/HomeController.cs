using SamUtils.Utils;
using SamWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SamWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Create", "Consolations");
        }

        public JsonResult GetProvinceCities(int id)
        {
            return Json(CityUtil.GetProvinceCities(id), JsonRequestBehavior.AllowGet);
        }
    }
}