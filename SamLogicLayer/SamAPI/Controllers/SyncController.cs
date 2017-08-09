using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
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
        public IHttpActionResult GetUpdates(int mosqueId, DateTime? lastUpdateTime = null)
        {
            try
            {
                var queryTime = DateTimeUtils.Now;
                var updates = _consolationRepo.GetUpdates(mosqueId, lastUpdateTime, queryTime);
                var updateDtos = new ConsolationsUpdatePackDto
                {
                    Mosque = Mapper.Map<Mosque, MosqueDto>(updates.Item1),
                    Obits = updates.Item2.Select(obit => Mapper.Map<Obit, ObitDto>(obit)).ToArray(),
                    Templates = updates.Item3.Select(tmpl => Mapper.Map<Template, TemplateDto>(tmpl)).ToArray(),
                    Consolations = updates.Item4.Select(c => Mapper.Map<Consolation, ConsolationDto>(c, opts =>
                    {
                        opts.BeforeMap((mdl, dto) =>
                        {
                            mdl.Obit = null;
                            mdl.Template = null;
                        });
                    })).ToArray(),
                    QueryTime = queryTime
                };
                return Ok(updates);

                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}