using AutoMapper;
using RamancoLibrary.Utilities;
using SamAPI.Code.Utils;
using SamDataAccess.Repos.Interfaces;
using SamModels.DTOs;
using SamModels.Entities.Core;
using SamUtils.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace SamAPI.Code.Utils
{
    public class ExceptionManager
    {
        public static HttpResponseMessage GetExceptionResponse(ApiController apiController, Exception ex)
        {
            return apiController.Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        }
    }
}