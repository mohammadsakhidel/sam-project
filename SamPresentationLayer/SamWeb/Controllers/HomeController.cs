using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SamWeb.Controllers
{
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            using (var hc = HttpUtil.CreateClient())
            {

                #region Get Mosques from API:
                try
                {
                    var response = await hc.GetAsync($"{ApiActions.mosques_getlatests}");
                    response.EnsureSuccessStatusCode();
                    var mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                    ViewBag.Mosques = mosques;
                }
                catch { }
                #endregion
                #region Get Latest Obits from API:
                try
                {
                    var response = await hc.GetAsync($"{ApiActions.obits_getlatests}");
                    response.EnsureSuccessStatusCode();
                    var obits = await response.Content.ReadAsAsync<List<ObitDto>>();
                    ViewBag.Obits = obits;
                }
                catch { }
                #endregion

            }

            return View();
        }

        public JsonResult GetProvinceCities(int id)
        {
            return Json(CityUtil.GetProvinceCities(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadApp()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}