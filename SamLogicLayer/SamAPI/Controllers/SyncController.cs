using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
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
                var templateDtos = updates.Item3.Select(tmpl => Mapper.Map<Template, TemplateDto>(tmpl)).ToArray();
                var imageBlobs = updates.Item4.Select(blob => Mapper.Map<ImageBlob, ImageBlobDto>(blob)).ToArray();
                var consolationDtos = updates.Item5.Select(c => Mapper.Map<Consolation, ConsolationDto>(c, opts =>
                {
                    opts.AfterMap((mdl, dto) =>
                    {
                        dto.Obit = null;
                        dto.Template = null;
                    });
                })).ToArray();

                var updateDtos = new ConsolationsUpdatePackDto
                {
                    Mosque = mosqueDto,
                    Obits = obitDtos,
                    Templates = templateDtos,
                    ImageBlobs = imageBlobs,
                    Consolations = consolationDtos,
                    QueryTime = queryTime
                };

                return Ok(updateDtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region Post Actions:
        [HttpPost]
        public IHttpActionResult UpdateDisplays(DisplayDto[] displays)
        {
            try
            {
                foreach (var displayDto in displays)
                {
                    var display = Mapper.Map<DisplayDto, Display>(displayDto);
                    _displayRepo.Add(display);
                }
                _displayRepo.Save();

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