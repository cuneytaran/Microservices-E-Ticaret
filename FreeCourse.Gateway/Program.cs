using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>//OCELOT tan�mlamak i�in buray� d�zenledik.
            {
                config.AddJsonFile($"configuration.{hostingContext.HostingEnvironment.EnvironmentName.ToLower()}.json").AddEnvironmentVariables();//DEVELOPER M� YOKSA ENV�ROMENT ARYIRIMI BURADA YAPIYORUZ. $"configuration=senin olu�turdu�un configuration.development.json ve configuration.production.json dosyalar�na ula�mak i�in
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}