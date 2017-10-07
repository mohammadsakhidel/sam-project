﻿using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Utils;
using SamUxLib.Resources;
using SamUxLib.Resources.Values;
using SamWeb.Code.Utils;
using SamWeb.Models.ViewModels;
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
        public ActionResult Track()
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
                foreach (var obit in obits)
                {
                    obit.ObitTypeDisplay = ResourceManager.GetValue($"ObitType_{obit.ObitType}", "Enums");
                }
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
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Create_ObitSelectionStep(CreateConsolationObitSelectionStepVM model)
        {
            try
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
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Create_CustomerInfoStep(CreateConsolationCustomerInfoStep model)
        {
            try
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
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Create_TemplateSelectionStep(CreateConsolationTemplateSelectionStepVM model)
        {
            try
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
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Create_TemplateInfoStep(CreateConsolationTemplateInfoStep model)
        {
            try
            {
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

                #region Create Consolation and return preview page:
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
                    TemplateInfo = templateInfo
                };

                using (var hc = HttpUtil.CreateClient())
                {
                    #region create consolation:
                    var response = await hc.PostAsJsonAsync<ConsolationDto>(ApiActions.consolations_create, dto);
                    HttpUtil.EnsureSuccessStatusCode(response);
                    var info = await response.Content.ReadAsAsync<Dictionary<string, string>>();
                    var consolationId = Convert.ToInt32(info["ID"]);
                    var paymentId = info["PaymentID"];
                    var paymentToken = info["PaymentToken"];
                    var bankPageUrl = info["BankPageUrl"];
                    #endregion
                    #region return view:
                    var response2 = await hc.GetAsync($"{ApiActions.consolations_findbyid}/{consolationId}");
                    response2.EnsureSuccessStatusCode();
                    var createdConsolation = await response2.Content.ReadAsAsync<ConsolationDto>();
                    var vm = new CreateConsolationPreviewStepVM
                    {
                        CreatedConsolation = createdConsolation,
                        PaymentID = paymentId,
                        PaymentToken = paymentToken,
                        BankPageUrl = bankPageUrl
                    };
                    return PartialView("Partials/_CreateConsolationWizard_PreviewStep", vm);
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> Track(string trackingNumber)
        {
            try
            {
                var dtos = await FindByTrackingNumberFromApi(trackingNumber);
                return PartialView("Partials/_ConsolationsList", dtos);
            }
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }
        #endregion

        #region Methods:
        private async Task<List<MosqueDto>> GetCityMosquesFromApi(int cityId)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.mosques_findbycity}?cityId={cityId}");
                HttpUtil.EnsureSuccessStatusCode(response);
                var mosques = await response.Content.ReadAsAsync<List<MosqueDto>>();
                return mosques;
            }
        }

        private async Task<List<ObitDto>> GetMosqueObitsFromApi(int mosqueId)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.obits_getallobits}?mosqueId={mosqueId}");
                HttpUtil.EnsureSuccessStatusCode(response);
                var obits = await response.Content.ReadAsAsync<List<ObitDto>>();
                return obits;
            }
        }

        private async Task<List<TemplateDto>> GetTemplatesFromApi()
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync(ApiActions.templates_all);
                HttpUtil.EnsureSuccessStatusCode(response);
                var templates = await response.Content.ReadAsAsync<List<TemplateDto>>();
                return templates;
            }
        }

        private async Task<TemplateDto> GetTemplateFromApi(int templateId)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.templates_get}/{templateId}");
                HttpUtil.EnsureSuccessStatusCode(response);
                var template = await response.Content.ReadAsAsync<TemplateDto>();
                return template;
            }
        }

        private async Task<List<ConsolationDto>> FindByTrackingNumberFromApi(string trackingNumber)
        {
            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.PostAsJsonAsync<string[]>($"{ApiActions.consolations_find}", new string[] { trackingNumber });
                HttpUtil.EnsureSuccessStatusCode(response);
                var consolations = await response.Content.ReadAsAsync<List<ConsolationDto>>();
                return consolations;
            }
        }
        #endregion
    }
}