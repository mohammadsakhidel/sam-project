using AutoMapper;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
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
        public IHttpActionResult All(bool onlyactives = true)
        {
            try
            {
                var all = _categoryRepo.GetAll(onlyactives);
                var dtos = all.OrderBy(c => c.Order)
                    .Select(o => Mapper.Map<TemplateCategory, TemplateCategoryDto>(o))
                    .ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region POST:
        [HttpPost]
        public IHttpActionResult Create(TemplateCategoryDto model)
        {
            try
            {
                var category = Mapper.Map<TemplateCategoryDto, TemplateCategory>(model);
                _categoryRepo.AddWithSave(category);
                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region PUT:
        [HttpPut]
        public IHttpActionResult Update(TemplateCategoryDto model)
        {
            try
            {
                var category = Mapper.Map<TemplateCategoryDto, TemplateCategory>(model);
                _categoryRepo.UpdateWithSave(category);
                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region DELETE:
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var categoryToDelete = _categoryRepo.Get(id);
                if (categoryToDelete == null)
                    return NotFound();

                #region Check if Delete is Allowed:
                if (categoryToDelete.Templates.Any())
                    return BadRequest();
                #endregion

                _categoryRepo.RemoveWithSave(id);

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
