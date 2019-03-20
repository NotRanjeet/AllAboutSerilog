using System;
using System.Collections.Generic;
using System.Text;
using AllAboutLogging.Application.DTOs;

namespace AllAboutLogging.Application.Contracts
{
    public interface IDummyService
    {
        List<CarDto> GetLatestCars();
    }
}
