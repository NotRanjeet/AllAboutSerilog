﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Channel; 
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace AllAboutLogging
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        public static void Main(string[] args)
        {
            Log.Logger = LoggerConfigurations.ConfigureLoggerForApplicationInsights(Configuration);
            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();


        
       
    }

    public class ConvertLogEventsToCustomTraceTelemetry : TraceTelemetryConverter
    {
        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            // first create a default TraceTelemetry using the sink's default logic
            // .. but without the log level, and (rendered) message (template) included in the Properties
            foreach (ITelemetry telemetry in base.Convert(logEvent, formatProvider))
            {
                // then go ahead and post-process the telemetry's context to contain the user id as desired
                if (logEvent.Properties.ContainsKey("UserId"))
                {
                    telemetry.Context.User.Id = logEvent.Properties["UserId"].ToString();
                }

                // post-process the telemetry's context to contain the operation id
                if (logEvent.Properties.ContainsKey("operation_Id"))
                {
                    telemetry.Context.Operation.Id = logEvent.Properties["operation_Id"].ToString();
                }

                // post-process the telemetry's context to contain the operation parent id
                if (logEvent.Properties.ContainsKey("operation_parentId"))
                {
                    telemetry.Context.Operation.ParentId = logEvent.Properties["operation_parentId"].ToString();
                }

                // typecast to ISupportProperties so you can manipulate the properties as desired
                ISupportProperties propTelematry = (ISupportProperties) telemetry;

                // find redundent properties
                var removeProps = new[] {"UserId", "operation_parentId", "operation_Id"};
                removeProps = removeProps.Where(prop => propTelematry.Properties.ContainsKey(prop)).ToArray();

                foreach (var prop in removeProps)
                {
                    // remove redundent properties
                    propTelematry.Properties.Remove(prop);
                }

                yield return telemetry;
            }
        }
    }
}
