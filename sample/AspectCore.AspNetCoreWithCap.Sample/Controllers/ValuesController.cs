using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AspectCore.AspNetCoreWithCap.Sample.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly ICapPublisher _publisher;

        private readonly IServiceProvider _serviceProvider;

        public ValuesController(ICapPublisher publisher, IServiceProvider serviceProvider)
        {
            _publisher = publisher;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// /api/values
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> TestGetAsync()
        {
            var test = _serviceProvider.GetService<ITestEventHandler>();
            await _publisher.PublishAsync("test", "隔壁老汪");
            return Ok();
        }
    }
}
