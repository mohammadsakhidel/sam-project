using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamAPI.Resources;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Constants;
using SamUtils.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Transactions;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class SyncController : ApiController
    {
        #region Fields:
        IConsolationRepo _consolationRepo;
        IDisplayRepo _displayRepo;
        #endregion

        #region Ctors:
        public SyncController(IConsolationRepo consolationRepo, IDisplayRepo displayRepo)
        {
            _consolationRepo = consolationRepo;
            _displayRepo = displayRepo;
        }
        #endregion

        #region Get Actions:
        [HttpGet]
        public IHttpActionResult GetUpdates(int mosqueId, string saloonId, string lastUpdate)
        {
            try
            {
                DateTime? lastUpdateTimeDateTime = null;
                if (!string.IsNullOrEmpty(lastUpdate))
                    lastUpdateTimeDateTime = DateTime.ParseExact(lastUpdate, StringFormats.url_date_time, CultureInfo.InvariantCulture);

                var queryTime = DateTimeUtils.Now;
                var updates = _consolationRepo.GetUpdates(mosqueId, saloonId, lastUpdateTimeDateTime, queryTime);

                var mosqueDto = Mapper.Map<Mosque, MosqueDto>(updates.Item1);
                var obitDtos = updates.Item2.Select(obit => Mapper.Map<Obit, ObitDto>(obit)).ToArray();
                var imageBlobIds = updates.Item4.ToArray();
                var consolationDtos = updates.Item5.Select(c => Mapper.Map<Consolation, ConsolationDto>(c, opts =>
                {
                    opts.AfterMap((mdl, dto) =>
                    {
                        dto.Obit = null;
                        dto.Template = null;
                    });
                })).ToArray();
                var bannerDtos = updates.Item6.Select(banner => (BannerHierarchyDto)Mapper.Map(banner, banner.GetType(), typeof(BannerHierarchyDto))).ToArray();
                var removedEntityDtos = updates.Item7.Select(re => Mapper.Map<RemovedEntity, RemovedEntityDto>(re)).ToArray();

                var updateDtos = new ConsolationsUpdatePackDto
                {
                    Mosque = mosqueDto,
                    Obits = obitDtos,
                    Templates = null, // templates removed from clients
                    ImageBlobs = imageBlobIds,
                    Consolations = consolationDtos,
                    Banners = bannerDtos,
                    RemovedEntities = removedEntityDtos,
                    QueryTime = queryTime
                };

                return Ok(updateDtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult AppVersion()
        {
            try
            {
                var appVersionName = ConfigurationManager.AppSettings["AppVersionName"];
                var appVersionCode = ConfigurationManager.AppSettings["AppVersionCode"];
                var newVersionUrl = ConfigurationManager.AppSettings["AppNewVersionUrl"];
                return Ok(new
                {
                    VersionName = appVersionName,
                    VersionCode = appVersionCode,
                    NewVersionUrl = newVersionUrl
                });
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public HttpResponseMessage GetServerDateTime()
        {
            try
            {
                var dateTime = DateTime.UtcNow.ToString(StringFormats.datetime_long);
                var response = Request.CreateResponse(HttpStatusCode.OK, dateTime);
                response.Content = new StringContent(dateTime);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                return response;
            }
            catch (Exception ex)
            {
                return ExceptionManager.GetExceptionResponse(this, ex);
            }
        }
        #endregion

        #region Post Actions:
        [HttpPost]
        public IHttpActionResult UpdateDisplays(DisplayDto[] displays)
        {
            try
            {
                #region send sms for first time displays:
                var displayedConsolations = displays.Select(d => d.ConsolationID).Distinct();
                foreach (var cId in displayedConsolations)
                {
                    if (!_consolationRepo.IsDisplayed(cId))
                    {
                        var c = _consolationRepo.Get(cId);
                        if (c != null)
                        {
                            var message = string.Format(SmsMessages.ConsolationDisplaySms, c.TrackingNumber);
                            SmsUtil.Send(message, c.Customer.CellPhoneNumber);
                        }
                    }
                }
                #endregion

                #region add display record and update consolation status:
                using (var ts = new TransactionScope())
                {
                    var consolationsUpdated = false;
                    var displaysAdded = false;

                    foreach (var displayDto in displays)
                    {
                        var display = Mapper.Map<DisplayDto, Display>(displayDto);
                        var consolation = _consolationRepo.Get(display.ConsolationID);

                        if (consolation != null)
                        {
                            _displayRepo.Add(display);
                            displaysAdded = true;

                            if (consolation.Status == ConsolationStatus.confirmed.ToString())
                            {
                                consolation.Status = ConsolationStatus.displayed.ToString();
                                consolationsUpdated = true;
                            }
                        }
                    }

                    if (displaysAdded)
                        _displayRepo.Save();
                    if (consolationsUpdated)
                        _consolationRepo.Save();

                    ts.Complete();
                }
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion
    }
}