﻿using AutoMapper;
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

        #region GET:
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
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
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
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}
