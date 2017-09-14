using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class BlobsController : ApiController
    {
        #region Fields:
        IBlobRepo _blobRepo;
        #endregion

        #region Ctors:
        public BlobsController(IBlobRepo blobRepo)
        {
            _blobRepo = blobRepo;
        }
        #endregion

        #region GET:
        [HttpGet]
        public IHttpActionResult GetImage(string id, bool? thumb = false)
        {
            try
            {
                var blob = _blobRepo.Get(id);
                if (blob == null || !(blob is ImageBlob))
                    return NotFound();

                var imgBlob = (ImageBlob)blob;
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(thumb.HasValue && thumb.Value ? imgBlob.ThumbImageBytes : imgBlob.Bytes);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return ResponseMessage(result);
            }
            catch (Exception ex)
            {
                return ResponseMessage(ExceptionManager.GetExceptionResponse(this, ex));
            }
        }
        #endregion
    }
}
