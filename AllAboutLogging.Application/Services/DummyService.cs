using AllAboutLogging.Application.Contracts;
using AllAboutLogging.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace AllAboutLogging.Application.Services
{
    public class DummyService : IDummyService
    {
        private ILogger<DummyService> _logger;
        public DummyService(ILogger<DummyService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Just a dummy method to return a list of cars.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<CarDto> GetLatestCars(short count =10)
        {
            _logger.LogInformation("Inside Service: User request to fetch the latest car");
            var range = Enumerable.Range(1,count);
            var cars = new List<CarDto>();
            foreach (var item in range)
            {
                cars.Add(new CarDto
                {
                    EngineNo = $"ENG1234{item}",
                    Make = $"Toyota{item}",
                    Model = $"{2014+item}",
                    Registration = $"M228{item}"
                });
            }
            //DO NOT LOG EVERYTHING ON PRODUCTION IT FOR DEMO PURPOSES
            _logger.LogInformation("Inside Service: GetLatestCars returned {@Cars}", cars);
            return cars;

        }

        

        public void DummyMethod()
        {
            _logger.LogInformation("Inside Service: User request to fetch the latest car");
            var random = new Random();


        }




    }
}
