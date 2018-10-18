﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LZZ.DEV.Watch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args,
                new ConfigurationBuilder()
                    .AddJsonFile($"appsettings.json")
                    .Build()
            ).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfigurationRoot hostingConfig) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls(hostingConfig["Server.urls"])
                .UseStartup<Startup>();
    }
}