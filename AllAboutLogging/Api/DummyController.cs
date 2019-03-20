using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AllAboutLogging.Application.Contracts;
using AllAboutLogging.Application.DTOs;
using AllAboutLogging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using ILogger = Serilog.ILogger;


namespace AllAboutLogging.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DummyController : ControllerBase
    {
        private readonly ILogger<DummyController> _logger;
        private IDummyService _dummyService;
        public DummyController(IDummyService dummyService, ILogger<DummyController> logger)
        {
            _logger = logger;
            _dummyService = dummyService;
        }

        //Dummy action to return a list of dummy cars.
        [HttpGet]
        public ActionResult GetCars()
        {
            _logger.LogInformation("User requested to fetch the Cities list");
            var items = GetCarsList().ToList();
            _logger.LogInformation($"Returning {items.Count} cars as a result");
            return Ok(items);
        }

        //Dummy action to return a single car 
        [HttpGet]
        [Route("favorite")]
        public ActionResult GetFavoriteCar()
        {
            _logger.LogInformation("User request to fetch the Favorite car");
            var car = new CarDto()
            {
                EngineNo = "Eng987",
                Model = "TT",
                Make = "Audi",
                Registration = "R1234"
            };
            _logger.LogInformation($"Returning car with RegistrationNo {car.Registration} and CarInfo {car}");
            _logger.LogInformation("Returning car with RegistrationNo:{RegistrationNo} and {@CarInfo}", car.Registration, car);
            return Ok(car);
        }


        [HttpGet]
        [Route("latest")]
        public ActionResult GetLatestCar()
        {
            using (LogContext.PushProperty("RequestId", Activity.Current.Id))
            {
                _logger.LogInformation("Inside Controller: User request to fetch the Favorite car");
                var cars = _dummyService.GetLatestCars(10);
                return Ok(cars);
            }
        }
        

        private IEnumerable<CarDto> GetCarsList(short count =10)
        {
            var range = Enumerable.Range(1,count);
            foreach (var item in range)
            {
                yield return new CarDto
                {
                    EngineNo = $"ENG1234{item}",
                    Make = $"Toyota{item}",
                    Model = $"{2014+item}",
                    Registration = $"M228{item}"
                };
            }
        }
    }
}