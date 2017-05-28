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
    public class ObitsController : ApiController
    {
        #region Fields:
        IObitRepo _obitRepo;
        #endregion

        #region Ctors:
        public ObitsController(IObitRepo obitRepo)
        {
            _obitRepo = obitRepo;
        }
        #endregion

        #region GET ACTIONS:
        [HttpGet]
        public IHttpActionResult GetObits(int mosqueId, DateTime date)
        {
            try
            {
                Thread.Sleep(1000);
                var obits = _obitRepo.Get(mosqueId, date);
                var dtos = obits.Select(o => Mapper.Map<Obit, ObitDto>(o));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }

        [HttpGet]
        public IHttpActionResult GetHoldings(int mosqueId, DateTime date)
        {
            try
            {
                Thread.Sleep(1000);
                var holdings = _obitRepo.GetHoldings(mosqueId, date);
                var dtos = holdings.Select(o => Mapper.Map<ObitHolding, ObitHoldingDto>(o, opt => opt.AfterMap((src, dest) =>
                {
                    dest.Obit = Mapper.Map<Obit, ObitDto>(src.Obit);
                })));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion

        #region POST ACTIONS:
        [HttpPost]
        public IHttpActionResult Create(ObitDto model)
        {
            try
            {
                Thread.Sleep(2000);
                var obit = Mapper.Map<ObitDto, Obit>(model);
                obit.CreationTime = DateTimeUtils.Now;
                _obitRepo.AddWithSave(obit);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion

        #region PUT ACTIONS:
        [HttpPut]
        public IHttpActionResult Update(ObitDto model)
        {
            try
            {
                Thread.Sleep(2000);

                var newObit = Mapper.Map<ObitDto, Obit>(model);
                _obitRepo.UpdateWithSave(newObit);

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
