using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Transactions;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class TemplatesController : ApiController
    {
        #region Fields:
        ITemplateRepo _templateRepo;
        IBlobRepo _blobRepo;
        IRemovedEntityRepo _removedEntityRepo;
        #endregion

        #region Ctors:
        public TemplatesController(ITemplateRepo templateRepo, IBlobRepo blobRepo, IRemovedEntityRepo removedEntityRepo)
        {
            _templateRepo = templateRepo;
            _blobRepo = blobRepo;
            _removedEntityRepo = removedEntityRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult All(bool onlyactives = true)
        {
            try
            {
                var templates = _templateRepo.GetAll(onlyactives);
                var dtos = templates
                    .OrderBy(t => t.Order)
                    .Select(t => Mapper.Map<Template, TemplateDto>(t))
                    .ToList();
                return Ok(dtos);
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
                var template = _templateRepo.Get(id);
                var dto = Mapper.Map<Template, TemplateDto>(template);
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
        public IHttpActionResult Create(TemplateDto model)
        {
            try
            {
                #region Prepare Background Image Blob:
                var now = DateTimeUtils.Now;
                var backgroundBytes = Convert.FromBase64String(model.BackgroundImageBase64);
                var backgroundBitmap = IOUtils.ByteArrayToBitmap(backgroundBytes);
                var resizer = new ImageResizer(backgroundBitmap.Width, backgroundBitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                var backgroundThumb = ImageUtils.GetThumbnailImage(backgroundBitmap, resizer.NewWidth, resizer.NewHeight);
                var backgroundBlob = new ImageBlob
                {
                    // blob:
                    ID = IDGenerator.GenerateImageID(),
                    Bytes = backgroundBytes,
                    CreationTime = now,
                    LastUpdateTime = now,
                    // imageblob:
                    ThumbImageBytes = IOUtils.BitmapToByteArray(backgroundThumb, ImageFormat.Jpeg),
                    ImageWidth = backgroundBitmap.Width,
                    ImageHeight = backgroundBitmap.Height
                };
                #endregion

                #region Prepare Template & Fields:
                var template = Mapper.Map<TemplateDto, Template>(model);
                template.BackgroundImageID = backgroundBlob.ID;
                template.CreationTime = DateTimeUtils.Now;
                template.LastUpdateTime = template.CreationTime;
                #endregion

                _templateRepo.AddWithSave(template, backgroundBlob);

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
        public IHttpActionResult Update(TemplateDto model)
        {
            try
            {
                #region Prepare Background Image Blob:
                ImageBlob backgroundBlob = null;
                if (!string.IsNullOrEmpty(model.BackgroundImageBase64))
                {
                    var now = DateTimeUtils.Now;
                    var backgroundBytes = Convert.FromBase64String(model.BackgroundImageBase64);
                    var backgroundBitmap = IOUtils.ByteArrayToBitmap(backgroundBytes);
                    var resizer = new ImageResizer(backgroundBitmap.Width, backgroundBitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                    var backgroundThumb = ImageUtils.GetThumbnailImage(backgroundBitmap, resizer.NewWidth, resizer.NewHeight);
                    backgroundBlob = new ImageBlob
                    {
                        // blob:
                        ID = IDGenerator.GenerateImageID(),
                        Bytes = backgroundBytes,
                        CreationTime = now,
                        LastUpdateTime = now,
                        // imageblob:
                        ThumbImageBytes = IOUtils.BitmapToByteArray(backgroundThumb, ImageFormat.Jpeg),
                        ImageWidth = backgroundBitmap.Width,
                        ImageHeight = backgroundBitmap.Height
                    };
                }
                #endregion

                var template = Mapper.Map<TemplateDto, Template>(model);
                _templateRepo.UpdateWithSave(template, backgroundBlob);

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
                var templateToDelete = _templateRepo.Get(id);
                if (templateToDelete == null)
                    return NotFound();

                #region Check if Delete is Allowed:
                if (_templateRepo.HasAnyConsolations(id))
                    return BadRequest();
                #endregion

                using (var ts = new TransactionScope())
                {
                    _templateRepo.RemoveWithSave(id);

                    #region add removed entity record:
                    var re = new RemovedEntity()
                    {
                        EntityID = templateToDelete.ID.ToString(),
                        EntityType = typeof(Template).Name,
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
    }
}
