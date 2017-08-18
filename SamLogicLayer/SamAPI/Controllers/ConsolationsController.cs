using AutoMapper;
using Newtonsoft.Json;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities.Core;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace SamAPI.Controllers
{
    public class ConsolationsController : ApiController
    {
        #region Fields:
        IConsolationRepo _consolationRepo;
        #endregion

        #region Ctors:
        public ConsolationsController(IConsolationRepo consolationRepo)
        {
            _consolationRepo = consolationRepo;
        }
        #endregion

        #region POST Actions:
        [HttpPost]
        public IHttpActionResult Create(ConsolationDto model)
        {
            try
            {
                #region Create Consolation:
                var consolation = Mapper.Map<ConsolationDto, Consolation>(model);
                consolation.TrackingNumber = IDGenerator.GenerateTrackingNumber();
                consolation.CreationTime = DateTimeUtils.Now;
                consolation.LastUpdateTime = consolation.CreationTime;
                consolation.Customer.IsMember = false;
                _consolationRepo.Add(consolation);
                _consolationRepo.Save();
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region GET Actions:
        [HttpGet]
        public IHttpActionResult Filter(int cityId = 0, string status = "", int count = 30)
        {
            try
            {
                var consolations = _consolationRepo.Filter(cityId, status, count);
                var dtos = consolations.Select(c => Mapper.Map<Consolation, ConsolationDto>(c, opts =>
                {
                    opts.AfterMap((mdl, dto) =>
                    {
                        dto.Template = Mapper.Map<Template, TemplateDto>(mdl.Template);
                    });
                })).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region PUT ACTIONS:
        [HttpPut]
        public IHttpActionResult UpdateFields(int id, Dictionary<string, string> fields)
        {
            try
            {
                var consolationToEdit = _consolationRepo.Get(id);
                if (consolationToEdit == null)
                    return NotFound();

                #region update fields:
                if (fields.ContainsKey("Audience"))
                    consolationToEdit.Audience = fields["Audience"];
                if (fields.ContainsKey("From"))
                    consolationToEdit.From = fields["From"];

                var json = JsonConvert.SerializeObject(fields);
                consolationToEdit.TemplateInfo = json;
                #endregion

                consolationToEdit.LastUpdateTime = DateTimeUtils.Now;
                _consolationRepo.Save();

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
