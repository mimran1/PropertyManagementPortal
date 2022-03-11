using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace RentalPropertyManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //if (environment == EnvironmentName.Production)
            //{
            //    var configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.Production.json")
            //    .Build();

            //    Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();

            //    Log.Information("Production Application starting up");
            //    Log.Information("Writing to location: " + configuration.GetValue<string>("Serilog:WriteTo:1:Args:path"));

            //}
            //else
            //{
            //    var configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.Development.json")
            //    .Build();

            //    Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.Configuration(configuration)
            //    .CreateLogger();

            //    Log.Information("Development Application starting up");

            //    Log.Information("Writing to location: " + configuration.GetValue<string>("Serilog:WriteTo:1:Args:path"));
            //}

            //var configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.Production.json")
            //    .Build();

            //Log.Logger = new LoggerConfiguration()
            //.ReadFrom.Configuration(configuration)
            //.CreateLogger();

            //Log.Information("Production Application starting up");
            //Log.Information("Writing to location: " + configuration.GetValue<string>("Serilog:WriteTo:1:Args:path"));


            //******************************************************************************************************************

            //var configuration = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.Development.json")
            //    .Build();

            //Log.Logger = new LoggerConfiguration()
            //.ReadFrom.Configuration(configuration)
            //.CreateLogger();

            //Log.Information("Development Application starting up");

            //Log.Information("Writing to location: " + configuration.GetValue<string>("Serilog:WriteTo:1:Args:path"));


            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal("Application failed to start correctly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }


        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
