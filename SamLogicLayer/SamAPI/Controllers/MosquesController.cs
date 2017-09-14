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

        [HttpGet]
        public IHttpActionResult Search(string name = "", int provinceId = -1, int cityId = -1)
        {
            try
            {
                #region validate:
                if (provinceId <= 0 && cityId <= 0 && string.IsNullOrEmpty(name))
                    return Ok(Enumerable.Empty<MosqueDto>());
                #endregion

                var mosques = _mosqueRepo.Search(provinceId, cityId, name);
                var dtos = mosques.Select(m => Mapper.Map<Mosque, MosqueDto>(m)).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetLatests(int count = 5)
        {
            try
            {
                var mosques = _mosqueRepo.GetLatests(count);
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
                #region Prepare Image Blob:
                ImageBlob imageBlob = null;
                if (!string.IsNullOrEmpty(model.ImageBase64))
                {
                    var now = DateTimeUtils.Now;
                    var bytes = Convert.FromBase64String(model.ImageBase64);
                    var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                    var resizer = new ImageResizer(bitmap.Width, bitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                    var thumbBitmap = ImageUtils.GetThumbnailImage(bitmap, resizer.NewWidth, resizer.NewHeight);
                    imageBlob = new ImageBlob
                    {
                        // blob:
                        ID = IDGenerator.GenerateImageID(),
                        Bytes = bytes,
                        CreationTime = now,
                        LastUpdateTime = now,
                        // imageblob:
                        ThumbImageBytes = IOUtils.BitmapToByteArray(thumbBitmap, ImageFormat.Jpeg),
                        ImageWidth = bitmap.Width,
                        ImageHeight = bitmap.Height
                    };
                }
                #endregion

                var mosque = Mapper.Map<MosqueDto, Mosque>(model);
                mosque.CreationTime = DateTimeUtils.Now;
                mosque.LastUpdateTime = mosque.CreationTime;
                _mosqueRepo.AddWithSave(mosque, imageBlob);
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
                #region Prepare Background Image Blob:
                ImageBlob imageBlob = null;
                if (!string.IsNullOrEmpty(model.ImageBase64))
                {
                    var now = DateTimeUtils.Now;
                    var bytes = Convert.FromBase64String(model.ImageBase64);
                    var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                    var resizer = new ImageResizer(bitmap.Width, bitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                    var thumbBitmap = ImageUtils.GetThumbnailImage(bitmap, resizer.NewWidth, resizer.NewHeight);
                    imageBlob = new ImageBlob
                    {
                        // blob:
                        ID = IDGenerator.GenerateImageID(),
                        Bytes = bytes,
                        CreationTime = now,
                        LastUpdateTime = now,
                        // imageblob:
                        ThumbImageBytes = IOUtils.BitmapToByteArray(thumbBitmap, ImageFormat.Jpeg),
                        ImageWidth = bitmap.Width,
                        ImageHeight = bitmap.Height
                    };
                }
                #endregion

                var newMosque = Mapper.Map<MosqueDto, Mosque>(model);
                _mosqueRepo.UpdateWidthSave(newMosque, imageBlob);

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
                _mosqueRepo.RemoveAllDependencies(id);
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
