using Newtonsoft.Json;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Utils;
using SamWeb.Code.Utils;
using SamWeb.Models.ViewModels;
using SamWeb.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SamWeb.Controllers
{
    public class ConsolationsController : Controller
    {
        #region GET Actions:
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCityMosques(int id)
        {
            try
            {
                var mosques = await GetCityMosquesFromApi(id);
                return Json(mosques, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ExceptionManager.GetProperMessage(ex), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetAllObits(int mosqueId)
        {
            try
            {
                var obits = await GetMosqueObitsFromApi(mosqueId);
                return Json(obits, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ExceptionManager.GetProperMessage(ex), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region POST Actions:
        [HttpPost]
        public async Task<PartialViewResult> Create_ObitSelectionStep(CreateConsolationObitSelectionStepVM model)
        {
            Thread.Sleep(1000);

            #region Validation:
            if (!ModelState.IsValid)
            {
                #region set cities collection to maintain view state:
                if (model.ProvinceID.HasValue)
                    model.Cities = CityUtil.GetProvinceCities(model.ProvinceID.Value);
                #endregion

                #region set mosques collection to maintain view state:
                if (model.CityID.HasValue)
                {
                    var mosques = await GetCityMosquesFromApi(model.CityID.Value);
                    model.Mosques = mosques;
                }
                #endregion

                #region set obits collection to maintain view state:
                if (model.MosqueID.HasValue)
                {
                    var obits = await GetMosqueObitsFromApi(model.MosqueID.Value);
                    model.Obits = obits;
                }
                #endregion

                return PartialView("Partials/_CreateConsolationWizard_ObitSelectionStep", model);
            }
            #endregion

            #region Return Customer Info Step View:
            var nextModel = new CreateConsolationCustomerInfoStep();
            return PartialView("Partials/_CreateConsolationWizard_CustomerInfo", nextModel);
            #endregion
        }

        [HttpPost]
        public async Task<PartialViewResult> Create_CustomerInfoStep(CreateConsolationCustomerInfoStep model)
        {
            Thread.Sleep(1000);

            #region Validation:
            if (!ModelState.IsValid)
            {
                return PartialView("Partials/_CreateConsolationWizard_CustomerInfo", model);
            }
            #endregion

            #region Return Template Selection Step View:
            var templates = await GetTemplatesFromApi();
            var nextModel = new CreateConsolationTemplateSelectionStepVM
            {
                Categories = templates.GroupBy(t => t.TemplateCategoryID).Select(g => g.First().Category)
                                    .OrderBy(c => c.Order).ToList(),
                Templates = templates.OrderBy(t => t.Order).ToList()
            };
            return PartialView("Partials/_CreateConsolationWizard_TemplateSelectionStep", nextModel);
            #endregion
        }

        [HttpPost]
        public async Task<PartialViewResult> Create_TemplateSelectionStep(CreateConsolationTemplateSelectionStepVM model)
        {
            Thread.Sleep(1000);

            #region Validation:
            if (!ModelState.IsValid)
            {
                var templates = await GetTemplatesFromApi();
                var step2Model = new CreateConsolationTemplateSelectionStepVM
                {
                    Categories = templates.GroupBy(t => t.TemplateCategoryID).Select(g => g.First().Category)
                                    .OrderBy(c => c.Order).ToList(),
                    Templates = templates.OrderBy(t => t.Order).ToList(),
                    CategoryStates = model.CategoryStates
                };
                return PartialView("Partials/_CreateConsolationWizard_TemplateSelectionStep", step2Model);
            }
            #endregion

            #region Return TemplateInfo step view:
            var nextModel = new CreateConsolationTemplateInfoStep();
            var template = await GetTemplateFromApi(model.TemplateID.Value);
            nextModel.Fields = template.TemplateFields.Select(f => new TemplateFieldPresenter { Name = f.Name, DisplayName = f.DisplayName, Description = f.Description }).ToList();
            return PartialView("Partials/_CreateConsolationWizard_TemplateInfoStep", nextModel);
            #endregion
        }

        public async Task<PartialViewResult> Create_TemplateInfoStep(CreateConsolationTemplateInfoStep model)
        {
            Thread.Sleep(1000);

            #region Validation:
            foreach (var field in model.Fields)
            {
                if (String.IsNullOrEmpty(field.Value.Replace(" ", "")))
                {
                    ModelState.AddModelError("Fields", Messages.FillAllFields);
                    break;
                }
            }
            if (!ModelState.IsValid)
                return PartialView("Partials/_CreateConsolationWizard_TemplateInfoStep", model);
            #endregion

            #region Create Consolation:
            var fieldValues = model.Fields.ToDictionary(f => f.Name, f => f.Value);
            var templateInfo = JsonConvert.SerializeObject(fieldValues);
            var dto = new ConsolationDto
            {
                ObitID = model.ObitID,
                TemplateID = model.TemplateID,
                Customer = new CustomerDto { FullName = model.FullName, CellPhoneNumber = model.CellPhoneNumber },
                Audience = model.Fields.SingleOrDefault(f => f.Name == "Audience")?.Value,
                From = model.Fields.SingleOrDefault(f => f.Name == "From")?.Value,
                PaymentStatus = PaymentStatus.pending.ToString(),
                Status = ConsolationStatus.pending.ToString(),
                TemplateInfo =templateInfo
            };

            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.PostAsJsonAsync<ConsolationDto>(ApiActions.consolations_create, dto);
                response.EnsureSuccessStatusCode();
                return PartialView("Partials/_CreateConsolationWizard_SuccessStep");
            }

            #endregion
        }
        #endregion

        #region Methods:
        private async Task<List<MosqueDto>> GetCityMosquesFromApi(int cityId)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.mosques_findbycity}?cityId={cityId}");
                response.EnsureSuccessStatusCode();
                var mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                return mosques;
            }
        }

        private async Task<List<ObitDto>> GetMosqueObitsFromApi(int mosqueId)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.obits_getallobits}?mosqueId={mosqueId}");
                response.EnsureSuccessStatusCode();
                var obits = await response.Content.ReadAsAsync<List<ObitDto>>();
                return obits;
            }
        }

        public async Task<List<TemplateDto>> GetTemplatesFromApi()
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync(ApiActions.templates_all);
                response.EnsureSuccessStatusCode();
                var templates = await response.Content.ReadAsAsync<List<TemplateDto>>();
                return templates;
            }
        }

        public async Task<TemplateDto> GetTemplateFromApi(int templateId)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.templates_get}/{templateId}");
                response.EnsureSuccessStatusCode();
                var template = await response.Content.ReadAsAsync<TemplateDto>();
                return template;
            }
        }
        #endregion
    }
}