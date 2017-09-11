using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        #region Post Actions:
        [HttpPost]
        public async System.Threading.Tasks.Task<PartialViewResult> Index(int? provinceId, int? cityId, string name)
        {
            try
            {
                #region Validate:
                if (!provinceId.HasValue && !cityId.HasValue && string.IsNullOrEmpty(name))
                    throw new ValidationException(Messages.PleaseFillSomeConditions);
                #endregion

                #region Call Api:
                List<MosqueDto> mosques = null;
                using (var hc = HttpUtil.CreateClient())
                {
                    string qs = (provinceId.HasValue ? $"provinceId={provinceId.Value.ToString()}" : "");
                    qs += (cityId.HasValue ? $"{(!string.IsNullOrEmpty(qs) ? "&" : "")}cityId={cityId.Value.ToString()}" : "");
                    qs += (!string.IsNullOrEmpty(name) ? $"{(!string.IsNullOrEmpty(qs) ? "&" : "")}name={name}" : "");
                    string url = $"{ApiActions.mosques_search}?{qs}";

                    var response = await hc.GetAsync(url);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                }
                #endregion

                return PartialView("Partials/_MosquesList", mosques);
            }
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }
        #endregion
    }
}