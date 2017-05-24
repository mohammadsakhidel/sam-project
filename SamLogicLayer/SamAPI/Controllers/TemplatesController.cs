using AutoMapper;
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
    public class TemplatesController : ApiController
    {
        #region Fields:
        ITemplateRepo templateRepo;
        #endregion

        #region Ctors:
        public TemplatesController(ITemplateRepo templateRepo)
        {
            this.templateRepo = templateRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                var templates = templateRepo.GetAll();
                var dtos = templates.Select(t => Mapper.Map<Template, TemplateDto>(t)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}
