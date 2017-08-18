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
        IMosqueRepo _mosqueRepo;
        #endregion

        #region Ctors:
        public ObitsController(IObitRepo obitRepo, IMosqueRepo mosqueRepo)
        {
            _obitRepo = obitRepo;
            _mosqueRepo = mosqueRepo;
        }
        #endregion

        #region GET ACTIONS:
        [HttpGet]
        public IHttpActionResult GetAllObits(int mosqueId)
        {
            try
            {
                var obits = _obitRepo.GetAllFromDate(mosqueId, DateTimeUtils.Now);
                var dtos = obits.Select(o => Mapper.Map<Obit, ObitDto>(o));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetObits(int mosqueId, DateTime date)
        {
            try
            {
                var obits = _obitRepo.Get(mosqueId, date);
                var dtos = obits.Select(o => Mapper.Map<Obit, ObitDto>(o));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetHoldings(int mosqueId, DateTime date)
        {
            try
            {
                var holdings = _obitRepo.GetHoldings(mosqueId, date);
                var dtos = holdings.Select(o => Mapper.Map<ObitHolding, ObitHoldingDto>(o, opt => opt.AfterMap((src, dest) =>
                {
                    dest.Obit = Mapper.Map<Obit, ObitDto>(src.Obit);
                    dest.SaloonName = _mosqueRepo.FindSaloon(mosqueId, src.SaloonID)?.Name;
                })));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region POST ACTIONS:
        [HttpPost]
        public IHttpActionResult Create(ObitDto model)
        {
            try
            {
                var obit = Mapper.Map<ObitDto, Obit>(model);
                obit.CreationTime = DateTimeUtils.Now;
                obit.LastUpdateTime = obit.CreationTime;
                _obitRepo.AddWithSave(obit);
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
        public IHttpActionResult Update(ObitDto model)
        {
            try
            {
                var newObit = Mapper.Map<ObitDto, Obit>(model);
                _obitRepo.UpdateWithSave(newObit);

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
                _obitRepo.RemoveWithSave(id);

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
