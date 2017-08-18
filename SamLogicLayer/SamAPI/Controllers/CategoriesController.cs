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
        ITemplateCategoryRepo _categoryRepo;
        #endregion

        #region Ctors:
        public CategoriesController(ITemplateCategoryRepo categoryRepo)
        {
            this._categoryRepo = categoryRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                var all = _categoryRepo.GetAll();
                var dtos = all.Select(o => Mapper.Map<TemplateCategory, TemplateCategoryDto>(o)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion
    }
}
