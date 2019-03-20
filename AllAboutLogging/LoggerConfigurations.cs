using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllAboutLogging.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace AllAboutLogging
{
    public static class LoggerConfigurations
    {
        //Method that will configure logger using appsettings.json file and combination of Code
        //This is an example that shows mix and match of both configuration approaches.
        public static ILogger ConfigureLoggerForApplicationInsights(IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()//More on LogContext later
                .WriteTo.Console();//Write log data to console sink

            //Get the application insights key from the configuration
            var aiKey = configuration["ApplicationInsights:InstrumentationKey"];
            
            //If we have ai key and it is not staging in Ideal scenario it can be Development
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != EnvironmentName.Staging && 
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


        //Configure simple file and console logger but also enable rolling files 
        public static ILogger ConfigureFileLoggerWithRollingFilesUsingCode()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console() //More on LogContext later
                //fileSizeLimitBytes: We instruct that Limit each log file to 10 MB
                //rollOnFileSizeLimit: We instruct to create new file when the size is exceeded 10MB
                //retainedFileCountLimit: We instruct to keep maximum of 20 logs files
                .WriteTo.File("Logs.txt", LogEventLevel.Debug, fileSizeLimitBytes: 10485760, rollOnFileSizeLimit: true, retainedFileCountLimit: 20)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//Override for framework libraries
                .MinimumLevel.Override("System", LogEventLevel.Warning);// Override to avoid logging system INF events
                
            return loggerConfiguration.CreateLogger();
        }

        //Configure simple file and console logger but also enable rolling files 
        public static ILogger ConfigureLoggerWithDestructure()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WriteTo.Console() //More on LogContext later
                //fileSizeLimitBytes: We instruct that Limit each log file to 10 MB
                //rollOnFileSizeLimit: We instruct to create new file when the size is exceeded 10MB
                //retainedFileCountLimit: We instruct to keep maximum of 20 logs files
                .WriteTo.File("Logs.txt", LogEventLevel.Debug, fileSizeLimitBytes: 10485760, rollOnFileSizeLimit: true, retainedFileCountLimit: 20)
                .Destructure.ByTransforming<LoginModel>(login => new {login.Email, login.RememberMe})// We tell serilog to skip Password while serializing
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)//Override for framework libraries
                .MinimumLevel.Override("System", LogEventLevel.Warning);// Override to avoid logging system INF events
                
            return loggerConfiguration.CreateLogger();
        }


    }
}
