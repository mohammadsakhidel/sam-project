using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamModels.DTOs;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Objects.Exceptions;
using SamUtils.Utils;
using SamUxLib.Resources;
using SamUxLib.Resources.Values;
using SamWeb.Code.Utils;
using SamWeb.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
        #region ARGS:
        const string ARG_PROVINCE_ID = "province_id";
        const string ARG_CITY_ID = "city_id";
        const string ARG_MOSQUE_ID = "mosque_id";
        const string ARG_OBIT_ID = "obit_id";
        const string ARG_FULLNAME = "fullname";
        const string ARG_CELLPHONE = "cellphone";
        const string ARG_TEMPLATE_ID = "template_id";
        const string ARG_TEMPLATE_FIELDS = "template_fields";
        const string ARG_CONSOLATION_ID = "consolation_id";
        #endregion

        #region GET Actions:
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create(string step = "")
        {
            if (string.IsNullOrEmpty(step))
                ClearSession();

            var vm = await GetObitSelectionViewModel();
            ViewBag.DefaultView = RenderRazorViewToString("Partials/_CreateConsolationWizard_ObitSelectionStep", vm);

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
                return Json(mosques.OrderBy(m => m.Name), JsonRequestBehavior.AllowGet);
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

        [HttpGet]
        public async Task<ActionResult> Preview(string tn)
        {
            #region create privew partial model:
            using (var hc = HttpUtil.CreateClient())
            {
                var resConsolation = await hc.GetAsync($"{ApiActions.consolations_findbytrackingnumber}?tn={tn}");
                var consolationDto = await resConsolation.Content.ReadAsAsync<ConsolationDto>();
                var resPayment = await hc.GetAsync($"{ApiActions.payment_find}/{consolationDto.PaymentID}");
                var paymentDto = await resPayment.Content.ReadAsAsync<PaymentDto>();

                var vm = new CreateConsolationPreviewStepVM()
                {
                    CreatedConsolation = consolationDto,
                    BankPageUrl = paymentDto.BankPageUrl,
                    PaymentID = paymentDto.ID,
                    PaymentToken = paymentDto.Token
                };

                return View(vm);
            }
            #endregion
        }
        #endregion

        #region POST Actions:
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

        #region Consolation Creation POSTs:
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

                #region update session data:
                Session[ARG_PROVINCE_ID] = model.ProvinceID;
                Session[ARG_CITY_ID] = model.CityID;
                Session[ARG_MOSQUE_ID] = model.MosqueID;
                Session[ARG_OBIT_ID] = model.ObitID;
                #endregion

                #region Return Customer Info Step View:
                return GetCustomerInfoView();
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

                #region update session data:
                Session[ARG_FULLNAME] = model.FullName;
                Session[ARG_CELLPHONE] = model.CellPhoneNumber;
                #endregion

                #region Return Template Selection Step View:
                return await GetTemplateSelectionView();
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

                #region update session data:
                Session[ARG_TEMPLATE_ID] = model.TemplateID;
                #endregion

                #region Return TemplateInfo step view:
                return await GetTemplateInfoView();
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

                #region prepare dto:
                var obitId = (int)Session[ARG_OBIT_ID];
                var fullName = Session[ARG_FULLNAME].ToString();
                var cellphone = Session[ARG_CELLPHONE].ToString();
                var templateId = (int)Session[ARG_TEMPLATE_ID];

                var fieldValues = model.Fields.ToDictionary(f => f.Name, f => f.Value);
                var templateInfo = JsonConvert.SerializeObject(fieldValues);
                #endregion

                #region create consolation:
                if (Session[ARG_CONSOLATION_ID] == null)
                {
                    var dto = new ConsolationDto
                    {
                        ObitID = obitId,
                        TemplateID = templateId,
                        Customer = new CustomerDto { FullName = fullName, CellPhoneNumber = cellphone },
                        Audience = model.Fields.SingleOrDefault(f => f.Name == "Audience")?.Value,
                        From = model.Fields.SingleOrDefault(f => f.Name == "From")?.Value,
                        PaymentStatus = PaymentStatus.pending.ToString(),
                        Status = ConsolationStatus.pending.ToString(),
                        TemplateInfo = templateInfo
                    };

                    using (var hc = HttpUtil.CreateClient())
                    {
                        var response = await hc.PostAsJsonAsync(ApiActions.consolations_create, dto);
                        HttpUtil.EnsureSuccessStatusCode(response);
                        var info = await response.Content.ReadAsAsync<Dictionary<string, string>>();
                        var consolationId = Convert.ToInt32(info["ID"]);

                        Session[ARG_CONSOLATION_ID] = consolationId;
                    }
                }
                #endregion

                #region update consolation:
                else
                {
                    using (var hc = HttpUtil.CreateClient())
                    {
                        var consolationId = (int)Session[ARG_CONSOLATION_ID];
                        var dto = new ConsolationDto() {
                            ID = consolationId,
                            ObitID = obitId,
                            TemplateID = templateId,
                            TemplateInfo = templateInfo,
                            Customer = new CustomerDto { FullName = fullName, CellPhoneNumber = cellphone }
                        };
                        var response = await hc.PutAsJsonAsync(ApiActions.consolations_updatev2, dto);
                        HttpUtil.EnsureSuccessStatusCode(response);
                    }
                }
                #endregion

                #region update session:
                Session[ARG_TEMPLATE_FIELDS] = templateInfo;
                #endregion

                #region return view:
                return await GetPreviewView();
                #endregion
            }
            catch (Exception ex)
            {
                return PartialView("Partials/_Error", ex);
            }
        }
        #endregion

        #region Consolation Creation Views:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> GetObitSelectionView()
        {
            var vm = await GetObitSelectionViewModel();
            return PartialView("Partials/_CreateConsolationWizard_ObitSelectionStep", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetCustomerInfoView()
        {
            var vm = new CreateConsolationCustomerInfoStep();

            vm.FullName = Session[ARG_FULLNAME] != null ? Session[ARG_FULLNAME].ToString() : "";
            vm.CellPhoneNumber = Session[ARG_CELLPHONE] != null ? Session[ARG_CELLPHONE].ToString() : "";

            return PartialView("Partials/_CreateConsolationWizard_CustomerInfo", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> GetTemplateSelectionView()
        {
            var vm = new CreateConsolationTemplateSelectionStepVM();

            vm.TemplateID = Session[ARG_TEMPLATE_ID] != null ? (int)Session[ARG_TEMPLATE_ID] : (int?)null;

            #region collections:
            var templates = await GetTemplatesFromApi();
            vm.Categories = templates.GroupBy(t => t.TemplateCategoryID)
                .Select(g => g.First().Category).OrderBy(c => c.Order).ToList();
            vm.Templates = templates.OrderBy(t => t.Order).ToList();
            #endregion

            return PartialView("Partials/_CreateConsolationWizard_TemplateSelectionStep", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> GetTemplateInfoView()
        {
            if (Session[ARG_TEMPLATE_ID] == null)
                throw new ValidationException("Template is not selected!");

            var vm = new CreateConsolationTemplateInfoStep();

            vm.TemplateID = (int)Session[ARG_TEMPLATE_ID];
            var template = await GetTemplateFromApi(vm.TemplateID);
            vm.Fields = template.TemplateFields.Select(f => new TemplateFieldPresenter
            {
                Name = f.Name,
                DisplayName = f.DisplayName,
                Description = f.Description
            }).ToList();

            if (Session[ARG_TEMPLATE_FIELDS] != null)
            {
                var fieldsDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(Session[ARG_TEMPLATE_FIELDS].ToString());
                foreach (var f in vm.Fields)
                {
                    f.Value = fieldsDic.ContainsKey(f.Name) ? fieldsDic[f.Name] : "";
                }
            }

            return PartialView("Partials/_CreateConsolationWizard_TemplateInfoStep", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<PartialViewResult> GetPreviewView()
        {
            var vm = new CreateConsolationPreviewStepVM();
            var consolationId = (int)Session[ARG_CONSOLATION_ID];

            using (var hc = HttpUtil.CreateClient())
            {
                var response = await hc.GetAsync($"{ApiActions.consolations_getpreviewinfo}/{consolationId}");
                response.EnsureSuccessStatusCode();
                var infoDto = await response.Content.ReadAsAsync<ConsolationPreviewDto>();
                vm.CreatedConsolation = infoDto.Consolation;
                vm.PaymentID = infoDto.Payment.ID;
                vm.PaymentToken = infoDto.Payment.Token;
                vm.BankPageUrl = infoDto.MoreData;
            }

            return PartialView("Partials/_CreateConsolationWizard_PreviewStep", vm);
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
                var response = await hc.GetAsync($"{ApiActions.obits_gethenceforwardobits}?mosqueId={mosqueId}");
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
        private async Task<CreateConsolationObitSelectionStepVM> GetObitSelectionViewModel()
        {
            var vm = new CreateConsolationObitSelectionStepVM();

            vm.ProvinceID = Session[ARG_PROVINCE_ID] != null ? (int)Session[ARG_PROVINCE_ID] : (int?)null;
            vm.CityID = Session[ARG_CITY_ID] != null ? (int)Session[ARG_CITY_ID] : (int?)null;
            vm.MosqueID = Session[ARG_MOSQUE_ID] != null ? (int)Session[ARG_MOSQUE_ID] : (int?)null;
            vm.ObitID = Session[ARG_OBIT_ID] != null ? (int)Session[ARG_OBIT_ID] : (int?)null;

            #region set cities collection to maintain view state:
            if (vm.ProvinceID.HasValue)
                vm.Cities = CityUtil.GetProvinceCities(vm.ProvinceID.Value);
            #endregion
            #region set mosques collection to maintain view state:
            if (vm.CityID.HasValue)
            {
                var mosques = await GetCityMosquesFromApi(vm.CityID.Value);
                vm.Mosques = mosques;
            }
            #endregion
            #region set obits collection to maintain view state:
            if (vm.MosqueID.HasValue)
            {
                var obits = await GetMosqueObitsFromApi(vm.MosqueID.Value);
                vm.Obits = obits;
            }
            #endregion
            return vm;
        }
        private string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
        private void ClearSession()
        {
            Session[ARG_PROVINCE_ID] = null;
            Session[ARG_CITY_ID] = null;
            Session[ARG_MOSQUE_ID] = null;
            Session[ARG_OBIT_ID] = null;
            Session[ARG_FULLNAME] = null;
            Session[ARG_CELLPHONE] = null;
            Session[ARG_TEMPLATE_ID] = null;
            Session[ARG_TEMPLATE_FIELDS] = null;
            Session[ARG_CONSOLATION_ID] = null;
        }
        #endregion
    }
}