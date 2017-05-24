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
    public class CategoriesController : ApiController
    {
        #region Fields:
        ITemplateCategoryRepo categoryRepo;
        #endregion

        #region Ctors:
        public CategoriesController(ITemplateCategoryRepo categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                var all = categoryRepo.GetAll();
                var dtos = all.Select(o => Mapper.Map<TemplateCategory, TemplateCategoryDto>(o)).ToList();
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
