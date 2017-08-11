using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
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
        #endregion

        #region Ctors:
        public SyncController(IConsolationRepo consolationRepo)
        {
            _consolationRepo = consolationRepo;
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
                    opts.BeforeMap((mdl, dto) =>
                    {
                        mdl.Obit = null;
                        mdl.Template = null;
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
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}