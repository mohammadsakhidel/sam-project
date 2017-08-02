﻿using SamUtils.Objects.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SamUtils.Utils
{
    public class HttpUtil
    {
        public static HttpClient CreateClient(params string[] args)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["api_host"]);
            return client;
        }

        public static void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpException(response);
        }
    }
}