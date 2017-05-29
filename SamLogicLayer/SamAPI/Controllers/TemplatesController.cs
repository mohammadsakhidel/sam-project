using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities.Blobs;
using SamModels.Entities.Core;
using SamUtils.Constants;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class TemplatesController : ApiController
    {
        #region Fields:
        ITemplateRepo _templateRepo;
        #endregion

        #region Ctors:
        public TemplatesController(ITemplateRepo templateRepo)
        {
            this._templateRepo = templateRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult All()
        {
            try
            {
                System.Threading.Thread.Sleep(1000);
                var templates = _templateRepo.GetAll();
                var dtos = templates.Select(t => Mapper.Map<Template, TemplateDto>(t)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
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
                var backgroundBytes = Convert.FromBase64String(model.BackgroundImageBase64);
                var backgroundBitmap = IOUtils.ByteArrayToBitmap(backgroundBytes);
                var resizer = new ImageResizer(backgroundBitmap.Width, backgroundBitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                var backgroundThumb = ImageUtils.GetThumbnailImage(backgroundBitmap, resizer.NewWidth, resizer.NewHeight);
                var backgroundBlob = new ImageBlob
                {
                    // blob:
                    ID = IDGenerator.GenerateImageID(),
                    Bytes = backgroundBytes,
                    CreationTime = DateTimeUtils.Now,
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
                template.Creator = "Dummy User";
                #endregion

                _templateRepo.AddWithSave(template, backgroundBlob);

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion

        #region PUT:
        [HttpPut]
        public IHttpActionResult Update(TemplateDto model)
        {
            return Ok();
        }
        #endregion

        #region DELETE:
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                System.Threading.Thread.Sleep(2000);
                _templateRepo.RemoveAllDependencies(id);
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
