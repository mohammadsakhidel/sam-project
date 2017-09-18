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
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class BannersController : ApiController
    {
        #region Fields:
        IBannerRepo _bannerRepo;
        #endregion

        #region Ctors:
        public BannersController(IBannerRepo bannerRepo)
        {
            _bannerRepo = bannerRepo;
        }
        #endregion

        #region GET:
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

                Banner banner = (Banner)Mapper.Map(hierarchy, typeof(BannerHierarchyDto), hierarchy.GetEntityType());
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

        #endregion

        #region DELETE:

        #endregion
    }
}
