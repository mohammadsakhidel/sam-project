using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SamAPI.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult OK()
        {
            return Ok(new { Message = "Successfully Tested!" });
        }
    }
}
