using AutoMapper;
using RamancoLibrary.Utilities;
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
    public class PaymentController : ApiController
    {
        #region Fields:
        IPaymentRepo _paymentRepo;
        #endregion

        #region Ctors:
        public PaymentController(IPaymentRepo paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult Find(string id)
        {
            try
            {
                var payment = _paymentRepo.Get(id);
                if (payment == null)
                    return NotFound();

                var dto = Mapper.Map<Payment, PaymentDto>(payment);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region POST:
        [HttpPost]
        public IHttpActionResult Create(PaymentDto dto)
        {
            try
            {
                var payment = Mapper.Map<PaymentDto, Payment>(dto);
                payment.CreationTime = DateTimeUtils.Now;
                payment.CreationTime = payment.CreationTime;

                _paymentRepo.AddWithSave(payment);

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
        public IHttpActionResult Update(PaymentDto dto)
        {
            try
            {
                var payment = _paymentRepo.Get(dto.ID);
                if (payment == null)
                    return NotFound();

                _paymentRepo.UpdateWithSave(payment);

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