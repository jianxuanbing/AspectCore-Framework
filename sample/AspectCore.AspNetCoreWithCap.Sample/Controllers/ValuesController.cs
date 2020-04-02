using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace AspectCore.AspNetCoreWithCap.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ICapPublisher _publisher;

        public ValuesController(ICapPublisher publisher)
        {
            _publisher = publisher;
        }

        /// <summary>
        /// /api/values
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestGetAsync()
        {
            await _publisher.PublishAsync("test", "隔壁老汪");
            return Ok();
        }
    }
}
