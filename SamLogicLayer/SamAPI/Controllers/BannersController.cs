using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities;
using SamUtils.Constants;
using SamUtils.Enums;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class BannersController : ApiController
    {
        #region Fields:
        IBannerRepo _bannerRepo;
        IRemovedEntityRepo _removedEntityRepo;
        #endregion

        #region Ctors:
        public BannersController(IBannerRepo bannerRepo, IRemovedEntityRepo removedEntityRepo)
        {
            _bannerRepo = bannerRepo;
            _removedEntityRepo = removedEntityRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult Find(int id)
        {
            try
            {
                var banner = _bannerRepo.Get(id);
                if (banner == null)
                    return NotFound();

                var dto = Mapper.Map(banner, banner.GetType(), typeof(BannerHierarchyDto), opts =>
                {
                    opts.AfterMap((src, dst) =>
                    {
                        var hierarchy = (BannerHierarchyDto)dst;
                        if (src is ObitBanner)
                        {
                            var cbanner = (ObitBanner)src;
                            hierarchy.MosqueID = cbanner.Obit.MosqueID;
                            hierarchy.CityID = cbanner.Obit.Mosque.CityID;
                        }
                        else if (src is MosqueBanner)
                        {
                            var cbanner = (MosqueBanner)src;
                            hierarchy.CityID = cbanner.Mosque.CityID;
                        }
                    });
                });

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult GetLatests(int count = 20)
        {
            try
            {
                var entities = _bannerRepo.GetLatests(count);
                var dtos = entities.Select(e => Mapper.Map(e, e.GetType(), typeof(BannerHierarchyDto)));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpGet]
        public IHttpActionResult FindByType(string bannerType, int count = 20)
        {
            try
            {
                var type = BannerHierarchyDto.GetEntityType(bannerType);
                var entities = _bannerRepo.FindByType(type, count);
                var dtos = entities.Select(e => Mapper.Map(e, e.GetType(), typeof(BannerHierarchyDto)));
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion

        #region POST:
        [HttpPost]
        public IHttpActionResult Create(BannerHierarchyDto hierarchy)
        {
            try
            {
                #region Prepare Image Blob:
                var now = DateTimeUtils.Now;
                var bytes = Convert.FromBase64String(hierarchy.ImageBase64);
                var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                var resizer = new ImageResizer(bitmap.Width, bitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                var thumb = ImageUtils.GetThumbnailImage(bitmap, resizer.NewWidth, resizer.NewHeight);
                var blob = new ImageBlob
                {
                    // blob:
                    ID = IDGenerator.GenerateImageID(),
                    Bytes = bytes,
                    CreationTime = now,
                    LastUpdateTime = now,
                    // imageblob:
                    ThumbImageBytes = IOUtils.BitmapToByteArray(thumb, ImageFormat.Jpeg),
                    ImageWidth = bitmap.Width,
                    ImageHeight = bitmap.Height
                };
                #endregion

                var banner = (Banner)Mapper.Map(hierarchy, typeof(BannerHierarchyDto), hierarchy.GetEntityType());
                banner.CreationTime = DateTimeUtils.Now;
                banner.LastUpdateTime = banner.CreationTime;

                _bannerRepo.AddWithSave(banner, blob);

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
        public IHttpActionResult Update(BannerHierarchyDto hierarchy)
        {
            try
            {
                #region Prepare Background Image Blob:
                ImageBlob blob = null;
                if (!string.IsNullOrEmpty(hierarchy.ImageBase64))
                {
                    var now = DateTimeUtils.Now;
                    var bytes = Convert.FromBase64String(hierarchy.ImageBase64);
                    var bitmap = IOUtils.ByteArrayToBitmap(bytes);
                    var resizer = new ImageResizer(bitmap.Width, bitmap.Height, ResizeType.LongerFix, Values.thumbnail_size);
                    var thumb = ImageUtils.GetThumbnailImage(bitmap, resizer.NewWidth, resizer.NewHeight);
                    blob = new ImageBlob
                    {
                        // blob:
                        ID = IDGenerator.GenerateImageID(),
                        Bytes = bytes,
                        CreationTime = now,
                        LastUpdateTime = now,
                        // imageblob:
                        ThumbImageBytes = IOUtils.BitmapToByteArray(thumb, ImageFormat.Jpeg),
                        ImageWidth = bitmap.Width,
                        ImageHeight = bitmap.Height
                    };
                }
                #endregion

                var banner = (Banner)Mapper.Map(hierarchy, typeof(BannerHierarchyDto), hierarchy.GetEntityType());
                _bannerRepo.UpdateWithSave(banner, blob);

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
                var bannerToDelete = _bannerRepo.Get(id);
                if (bannerToDelete == null)
                    return NotFound();

                DeleteBanner(bannerToDelete);

                return Ok();
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }

        [HttpPut]
        public IHttpActionResult DeleteAll([FromBody]int[] ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var bannerToDelete = _bannerRepo.Get(id);
                    if (bannerToDelete != null)
                    {
                        DeleteBanner(bannerToDelete);
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

        #region Private Methods:
        private void DeleteBanner(Banner bannerToDelete)
        {
            using (var ts = new TransactionScope())
            {
                _bannerRepo.RemoveWithSave(bannerToDelete.ID);

                #region add removed entity record:
                var re = new RemovedEntity()
                {
                    EntityID = bannerToDelete.ID.ToString(),
                    EntityType = typeof(Banner).Name,
                    RemovingTime = DateTimeUtils.Now
                };
                _removedEntityRepo.AddWithSave(re);
                #endregion

                ts.Complete();
            }
        }
        #endregion
    }
}
