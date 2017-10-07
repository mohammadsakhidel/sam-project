using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Controllers
{
    public class PaymentController : Controller
    {
        [HttpPost]
        public ActionResult MabnaCallback()
        {
            ViewBag.Result = Request.Form["RESCODE"];
            ViewBag.TRN = Request.Form["TRN"];
            ViewBag.CRN = Request.Form["CRN"];
            ViewBag.Amount = Request.Form["AMOUNT"];
            return View();
        }
    }
}