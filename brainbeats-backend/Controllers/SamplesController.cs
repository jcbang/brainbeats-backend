using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace brainbeats_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SamplesController : ControllerBase
    {
        private readonly Database db;

        public SamplesController(IConfiguration configuration)
        {
            db = new DatabaseContext(configuration).db;
        }

        [Route("create")]
        [HttpPost]
        public HttpResponseMessage Create()
        {
            Console.WriteLine(HttpContext.Request.Form.ToString());

            foreach (var formField in HttpContext.Request.Form)
            {
                Console.WriteLine(formField.Key);
                Console.WriteLine(formField.Value);
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}