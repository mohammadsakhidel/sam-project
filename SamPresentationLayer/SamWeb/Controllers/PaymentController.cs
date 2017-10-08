using SamUtils.Constants;
using SamUtils.Utils;
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
        public async System.Threading.Tasks.Task<ActionResult> MabnaCallback()
        {
            var statusCode = Convert.ToInt32(Request.Form["RESCODE"]);
            var trn = Request.Form["TRN"].ToString();
            var crn = Request.Form["CRN"].ToString();

            #region Verify for successfull payment:
            if (statusCode == 0)
            {
                #region verify payment:
                using (var hc = HttpUtil.CreateClient())
                {
                    var resVerify = await hc.PutAsync($"{ApiActions.payment_verify}/{crn}?refcode={trn}", null);
                    if (resVerify.StatusCode == System.Net.HttpStatusCode.OK)
                        ViewBag.IsSuccessfull = true;
                }
                #endregion
            }
            else if (statusCode == 200)
            {
                ViewBag.IsCanceled = true;
            }
            #endregion

            return View();
        }
    }
}