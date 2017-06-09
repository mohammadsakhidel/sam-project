﻿using AutoMapper;
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

        #region Get Actions:
        [HttpGet]
        public IHttpActionResult GetUpdates(int mosqueId, DateTime? lastUpdatetime = (DateTime?)null)
        {
            try
            {
                var queryTime = DateTimeUtils.Now;
                var lastUpdateTimeValue = lastUpdatetime.HasValue ? lastUpdatetime.Value : DateTimeUtils.Now.AddMonths(-1);
                var consolations = _consolationRepo.GetUpdates(mosqueId, lastUpdateTimeValue, queryTime);
                var dtos = consolations.Select(c => Mapper.Map<Consolation, ConsolationDto>(c)).ToList();
                var updates = new ConsolationsUpdatePackDto
                {
                    QueryTime = queryTime,
                    Consolations = dtos
                };
                return Ok(updates);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
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
                consolation.CreationTime = DateTimeUtils.Now;
                consolation.Customer.IsMember = false;
                _consolationRepo.Add(consolation);
                _consolationRepo.Save();
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}
