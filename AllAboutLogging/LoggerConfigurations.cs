﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace AllAboutLogging
{
    public static class LoggerConfigurations
    {
        //Method that will configure logger using appsettings.json file
        public static ILogger ConfigureLoggerFromConfiguration(IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.FromLogContext()//More on LogContext later
                .WriteTo.Console();//Write log data to console sink

            //Get the application insights key from the configuration
            var aiKey = configuration["ApplicationInsights:InstrumentationKey"];
            
            //If we have ai key and it is not development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != EnvironmentName.Development && 
                !string.IsNullOrEmpty(aiKey)) //If it is not Development environment 
            {
                //change the configuration to log info to the application insights user custom converter
                //Just an example to demonstrate how you can build logger configuration conditionally
                //This is just on of many ways not the only way 
                //Ignore the second parameter for now that is a custom converter
                // we will discuss in post about application insights
                //In short we can use it to edit logging before it get sent to Application insights for saving
                loggerConfiguration.WriteTo.ApplicationInsights(aiKey, new ConvertLogEventsToCustomTraceTelemetry());
            }
            //finally build the logger from the configuration
            //Assign the created logger to the Log Factory Logger instance
            //This will allows us to inject Serilog Logger into our code anywhere using Dependency injection 
            return loggerConfiguration.CreateLogger();
        }

        //Configure a simple file and console logger using Fluent api from serilog.
        public static ILogger ConfigureFileLoggerUsingCode()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console() //More on LogContext later
                .WriteTo.File("Logs.txt", LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//Override for framework libraries
                .MinimumLevel.Override("System", LogEventLevel.Warning);// Override to avoid logging system INF events
                
            return loggerConfiguration.CreateLogger();
        }
    }
}
