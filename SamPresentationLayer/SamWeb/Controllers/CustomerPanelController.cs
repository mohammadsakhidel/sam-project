using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Controllers
{
    [Authorize]
    public class CustomerPanelController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}