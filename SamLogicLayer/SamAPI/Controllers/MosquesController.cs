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
using System.Threading;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class MosquesController : ApiController
    {
        #region Fields:
        IMosqueRepo _mosqueRepo;
        #endregion

        #region Ctors:
        public MosquesController(IMosqueRepo mosqueRepo)
        {
            this._mosqueRepo = mosqueRepo;
        }
        #endregion

        #region GET ACTIONS:
        [HttpGet]
        public IHttpActionResult Find(int id)
        {
            try
            {
                var mosque = _mosqueRepo.Get(id);
                var mosqueDto = Mapper.Map<Mosque, MosqueDto>(mosque);
                return Ok(mosqueDto);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult FindByCity(int cityId)
        {
            try
            {
                var mosques = _mosqueRepo.FindByCity(cityId);
                var mosqueDtos = mosques.Select(m => Mapper.Map<Mosque, MosqueDto>(m)).ToList();
                return Ok(mosqueDtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region POST ACTIONS:
        [HttpPost]
        public IHttpActionResult Create(MosqueDto model)
        {
            try
            {
                var mosque = Mapper.Map<MosqueDto, Mosque>(model);
                mosque.CreationTime = DateTimeUtils.Now;
                mosque.LastUpdateTime = mosque.CreationTime;
                _mosqueRepo.AddWithSave(mosque);
                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region PUT ACTIONS:
        [HttpPut]
        public IHttpActionResult Update(MosqueDto model)
        {
            try
            {
                var newMosque = Mapper.Map<MosqueDto, Mosque>(model);
                _mosqueRepo.UpdateWidthSave(newMosque);

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region DELETE ACTIONS:
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                _mosqueRepo.RemoveWithSave(id);
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
