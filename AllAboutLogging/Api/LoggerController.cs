using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllAboutLogging.Application.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace AllAboutLogging.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        private readonly List<string> _users = new List<string>
        {
            "James",
            "Smith",
            "Marshal",
            "Robin",
            "Russel",
            "Neil",
            "Nathan"
        };
        private readonly ILogger<DummyController> _logger;
        private IDummyService _dummyService;
        public LoggerController(IDummyService dummyService, ILogger<DummyController> logger)
        {
            _logger = logger;
            _dummyService = dummyService;
        }


        //Dummy action to generate random log entries
        [HttpGet]
        [Route("generate")]
        public ActionResult GenerateLogEntry()
        {
            var random = new Random();
            //Select some random user
            var index = random.Next(0, _users.Count);
            //Add some random duration to process request
            var duration = random.Next(50, 2000);
            _logger.LogInformation("Processed Request for {User} in {Duration}milliseconds", _users[index], duration);
            return Ok("Logged Successfully");

        }
    }
}