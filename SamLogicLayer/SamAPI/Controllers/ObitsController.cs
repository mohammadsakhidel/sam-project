using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamAPI.Resources;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Utils;
using SmsLib.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Hosting;
using System.Web.Http;
using Unity.WebApi;
using Microsoft.Practices.Unity;
using System.Drawing;
using SamUtils.Enums;
using SamUtils.Objects.Utilities;

namespace SamAPI.Controllers
{
    public class ObitsController : ApiController
    {
        #region Fields:
        IObitRepo _obitRepo;
        IMosqueRepo _mosqueRepo;
        IRemovedEntityRepo _removedEntityRepo;
        #endregion

        #region Ctors:
        public ObitsController(IObitRepo obitRepo, IMosqueRepo mosqueRepo,
            IRemovedEntityRepo removedEntityRepo, IConsolationRepo consolationRepo)
        {
            _obitRepo = obitRepo;
            _mosqueRepo = mosqueRepo;
            _removedEntityRepo = removedEntityRepo;
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
        public IHttpActionResult GetHenceForwardObits(int mosqueId)
        {
            try
            {
                var obits = _obitRepo.GetHenceForwardObits(mosqueId);
                var dtos = obits.Select(o => Mapper.Map<Obit, ObitDto>(o));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult Search(string query)
        {
            try
            {
                var obits = _obitRepo.Search(query, DateTimeUtils.Now);
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
        public IHttpActionResult GetLatests(int count = 10)
        {
            try
            {
                var obits = _obitRepo.GetLatests(count);
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

        [HttpGet]
        public IHttpActionResult Find(string trackingNumber)
        {
            try
            {
                var obit = _obitRepo.FindByTrackingNumber(trackingNumber);
                if (obit == null)
                    return NotFound();

                var dto = Mapper.Map<Obit, ObitDto>(obit);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var obit = _obitRepo.Get(id);
                if (obit == null)
                    return NotFound();

                var dto = Mapper.Map<Obit, ObitDto>(obit);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> GetGif(int id)
        {
            try
            {
                var obit = _obitRepo.Get(id);
                if (obit == null || !obit.Consolations.Any())
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                #region generate conoslation bitmaps:
                var verified = PaymentStatus.verified.ToString();
                var confirmed = ConsolationStatus.confirmed.ToString();
                var displayed = ConsolationStatus.displayed.ToString();

                var consolations = obit.Consolations
                    .Where(c => c.PaymentStatus == verified && (c.Status == confirmed || c.Status == displayed))
                    .ToList();

                if (!consolations.Any())
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                var consolationBitmaps = new List<Bitmap>(consolations.Count());
                foreach (var c in consolations)
                {
                    var consolationsController = UnityConfig.Container.Resolve<ConsolationsController>();
                    var response = consolationsController.GetPreview(c.ID, thumb: false);
                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var bitmap = new Bitmap(stream);
                        consolationBitmaps.Add(bitmap);
                    }
                }
                #endregion

                #region generate obit gif:
                var gifWidth = consolationBitmaps.Select(b => b.Width).Max();
                var gifHeight = consolationBitmaps.Select(b => b.Height).Max();
                var gifBytes = ImageUtils.GenerateGif(consolationBitmaps.ToArray(), gifWidth, gifHeight, delayInSeconds: 3);
                #endregion

                return GifResponse(gifBytes);
            }
            catch (Exception ex)
            {
                return ExceptionManager.GetExceptionResponse(this, ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetDeceasedPeople()
        {
            try
            {
                var obits = _obitRepo.GetLastObitsWithDeceasedId(40)
                    .Distinct(new LambdaComparer<Obit>((a, b) => a.DeceasedIdentifier == b.DeceasedIdentifier))
                    .OrderByDescending(o => o.ID);

                var kvPairs = obits.Select(o => new KeyValuePair<string, string>(o.DeceasedIdentifier, o.Title)).ToList();

                return Ok(kvPairs);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetFutureRelatedObits(int obitId)
        {
            try
            {
                var obits = _obitRepo.GetFutureRelatedObits(obitId);
                var dtos = obits.Select(o => Mapper.Map<Obit, ObitDto>(o));
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
                obit.TrackingNumber = IDGenerator.GenerateObitTrackingNumber();
                obit.CreationTime = DateTimeUtils.Now;
                obit.LastUpdateTime = obit.CreationTime;
                _obitRepo.AddWithSave(obit);

                #region Send SMS To Owner:
                string messageText = string.Format(SmsMessages.ObitCreationSms, obit.Title, obit.TrackingNumber);
                SmsUtil.Send(messageText, obit.OwnerCellPhone);
                #endregion

                return Ok(obit.TrackingNumber);
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
                var obitToDelete = _obitRepo.Get(id);
                if (obitToDelete == null)
                    return NotFound();

                using (var ts = new TransactionScope())
                {
                    _obitRepo.RemoveWithSave(obitToDelete);

                    #region add removed entity record:
                    var re = new RemovedEntity()
                    {
                        EntityID = obitToDelete.ID.ToString(),
                        EntityType = typeof(Obit).Name,
                        RemovingTime = DateTimeUtils.Now
                    };
                    _removedEntityRepo.AddWithSave(re);
                    #endregion

                    ts.Complete();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region Methods:
        private HttpResponseMessage GifResponse(byte[] imageBytes)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(imageBytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/gif");
            return result;
        }
        #endregion
    }
}
