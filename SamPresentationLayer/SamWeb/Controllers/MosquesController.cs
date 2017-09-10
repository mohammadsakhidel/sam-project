using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Controllers
{
    public class MosquesController : Controller
    {
        #region Get Actions:
        public ActionResult Index()
        {
            return View();
        }
        #endregion
    }
}