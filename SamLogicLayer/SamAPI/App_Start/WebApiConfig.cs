using AutoMapper;
using SamModels.DTOs;
using SamModels.Entities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper.Configuration;
using SamAPI.App_Start;

namespace SamAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            AutoMapperConfig.InitializeMapper();

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
