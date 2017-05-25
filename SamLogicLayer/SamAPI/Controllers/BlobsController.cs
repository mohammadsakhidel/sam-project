using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.Entities.Blobs;
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
        public HttpResponseMessage GetImage(string id)
        {
            try
            {
                var blob = _blobRepo.Get(id);
                if (blob == null || !(blob is ImageBlob))
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(blob.Bytes);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception(ExceptionManager.GetProperApiMessage(ex)));
            }
        }
        #endregion
    }
}
