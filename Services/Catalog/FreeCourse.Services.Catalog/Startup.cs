using FreeCourse.Services.Catalog.Services;
using FreeCourse.Services.Catalog.Settings;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                // Default Port : 5672
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
                    {
                        host.Username("guest");
                        host.Password("guest");
                    });
                });
            });

            //token ile koruma altýna alýyoruz.
            //appsettings.json kýsýnda IdentityServerURL olarak tanýmlama yapmayý unutma
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityServerURL"];//bu microservise tokenin kimin daðýttýðýný belirtiyoruz.onuda appsettings.json kýsmýnda tanýmlýyoruz.token geldiðinde publickey ile doðrulama yapacak bunun adresini belirtiyoruz.
                options.Audience = "resource_catalog";//gelen tokenin içinde mutlaka aud vardý.ayný ismini burayada tanýmlýyoruz. resource_catalog identity service içindeki Config.cs den geliyor. 
                options.RequireHttpsMetadata = false;//https kullanmadýk. default olarak https bekler. bunu  kapatýyoruz.
            });

            //interfacelerin tanýmlandýðý yer
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICourseService, CourseService>();
            
            services.AddAutoMapper(typeof(Startup));//AutoMapper tanýmlanmasý. Startup denmesinin sebebi baþlangýçta tüm mapperleri tarar. Mapping klasörün içine GeneralMapping kuruldu.
            
            //Tüm controlleri Auhorize altýna almak için
            services.AddControllers(opt =>
            {
                opt.Filters.Add(new AuthorizeFilter());
            });

            services.Configure<DatabaseSettings>(Configuration.GetSection("DatabaseSettings"));//appsettins.json daki bilgileri DatabaseSettings interface içine dolduruyoruz.
           
            services.AddSingleton<IDatabaseSettings>(sp =>
            {
                return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            });//appsettings.json içinde yukarýda yakaladýðým verileri. interface içine value larýný doldur.Artýk IDatabaseSettings üzerinden baðlantý bilgilerini alabiliriz.

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FreeCourse.Services.Catalog", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FreeCourse.Services.Catalog v1"));
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}