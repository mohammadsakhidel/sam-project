using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Payment;
using SamAPI.Code.Utils;
using SamAPI.Resources;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Enums;
using SamUtils.Objects.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class PaymentController : ApiController
    {
        #region Fields:
        IPaymentRepo _paymentRepo;
        IPaymentService _paymentService;
        IConsolationRepo _consolationRepo;
        #endregion

        #region Ctors:
        public PaymentController(IPaymentRepo paymentRepo, IPaymentService paymentService,
                                 IConsolationRepo consolationRepo)
        {
            _paymentRepo = paymentRepo;
            _paymentService = paymentService;
            _consolationRepo = consolationRepo;
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
                dto.BankPageUrl = _paymentService.BankPageUrl;
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
        public IHttpActionResult Reverse()
        {
            try
            {
                var consolations = _consolationRepo.FindReversingConsolations();
                foreach (var c in consolations)
                {
                    var payment = _paymentRepo.Get(c.PaymentID);
                    var result = _paymentService.Reverse(payment.ReferenceCode);
                    if (result)
                    {
                        using (var ts = new TransactionScope())
                        {
                            payment.Status = PaymentStatus.reversed.ToString();
                            c.PaymentStatus = PaymentStatus.reversed.ToString();

                            _paymentRepo.Save();
                            _consolationRepo.Save();

                            ts.Complete();
                        }
                    }
                }

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
        public IHttpActionResult Verify(string id, string refcode)
        {
            try
            {
                #region find objects up update status:
                var payment = _paymentRepo.Get(id);
                if (payment == null)
                    return NotFound();

                Consolation consolation = null;
                if (payment.Type == PaymentType.consolation.ToString())
                {
                    consolation = _consolationRepo.FindByPaymentID(payment.ID);
                    if (consolation == null)
                        return NotFound();
                }
                #endregion

                using (var ts = new TransactionScope())
                {
                    #region update payment status:
                    payment.Status = PaymentStatus.verified.ToString();
                    payment.ReferenceCode = refcode;
                    _paymentRepo.Save();

                    consolation.PaymentStatus = PaymentStatus.verified.ToString();
                    _consolationRepo.Save();
                    #endregion

                    #region call psp verify service:
                    var res = _paymentService.Verify(payment.ID, refcode);
                    if (!res)
                        throw new PaymentException("Payment Verification Failed!");
                    #endregion

                    ts.Complete();
                }

                #region Send SMS To Customer:
                if (consolation?.Customer != null)
                {
                    string messageText = string.Format(SmsMessages.ConsolationCreationSms, consolation.TrackingNumber);
                    SmsUtil.Send(messageText, consolation.Customer.CellPhoneNumber);
                }
                #endregion

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPut]
        public IHttpActionResult VerifyPOS(PosPaymentVerificationDto model)
        {
            try
            {
                #region find consolation:
                var consolation = _consolationRepo.Get(model.ConsolationID);
                if (consolation == null)
                    return NotFound();
                #endregion

                #region update payment status:
                consolation.PaymentStatus = PaymentStatus.verified.ToString();
                consolation.ExtraData = model.PaymentData;
                _consolationRepo.Save();
                #endregion

                #region Send SMS To Customer:
                if (consolation?.Customer != null)
                {
                    string messageText = string.Format(SmsMessages.ConsolationCreationSms, consolation.TrackingNumber);
                    SmsUtil.Send(messageText, consolation.Customer.CellPhoneNumber);
                }
                #endregion

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