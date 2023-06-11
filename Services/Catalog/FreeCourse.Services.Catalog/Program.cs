using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ocelot ile baðlantý kurmak için deðiþiklik yaptýk.
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var categoryService = serviceProvider.GetRequiredService<ICategoryService>();//var categoryService ve ICategoryService deðiþtirilecek

                if (!categoryService.GetAllAsync().Result.Data.Any())//eðer dizi boþ geliyorsa iki tane kategory ekle
                {
                    categoryService.CreateAsync(new CategoryDto { Name = "Asp.net Core Kursu" }).Wait();//projene uygun servis eklemeyi yap
                    categoryService.CreateAsync(new CategoryDto { Name = "Asp.net Core API Kursu" }).Wait();
                }
            }

            host.Run();
        }
        // ocelot ile baðlantý kurmak için deðiþiklik yaptýk end...

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}