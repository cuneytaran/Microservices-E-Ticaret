using FluentValidation.AspNetCore;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Extensions;
using FreeCourse.Web.Handler;
using FreeCourse.Web.Helpers;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Validators;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web
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
            services.Configure<ClientSettings>(Configuration.GetSection("ClientSettings"));
            services.Configure<ServiceApiSettings>(Configuration.GetSection("ServiceApiSettings"));
            services.AddHttpContextAccessor();
            services.AddAccessTokenManagement();
            services.AddSingleton<PhotoHelper>();
            services.AddScoped<ISharedIdentityService, SharedIdentityService>();

            services.AddScoped<ResourceOwnerPasswordTokenHandler>();//T�m handler ler servis olarak eklendi.
            services.AddScoped<ClientCredentialTokenHandler>();

            //Extensionslar�n tan�mland��� yer. middlewareler burada. T�m microservice STARTUPLARI BURADA
            services.AddHttpClientServices(Configuration);

            //cookie ekleme ve ayarlama i�lemi
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
             {
                 opts.LoginPath = "/Auth/SignIn";//login path
                 opts.ExpireTimeSpan = TimeSpan.FromDays(60);//cookie �mr�n� belirliyoruz. 60 g�nl�k �m�r verdik
                 opts.SlidingExpiration = true;//60 g�n i�inde girdik�e s�rekli uzas�nm�? Evet yapt�k
                 opts.Cookie.Name = "udemywebcookie";//cookie ismini verdik.
             });

            //fluent validator.CourseCreateInputValidator=ben sana bu validatoru vereyim. sen bu assemlbly i�indeki t�m validatorleri bul 
            services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CourseCreateInputValidator>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");//uygulama developlent d���nda �al��t���nda hata verdi�inde error sayfas�na gider.
                //bunu else i�inde tutuyoruz. ��nk� development esnas�nda bu hatay� g�reyim diye. canl�ya ��kt���nda zaten otomatik olarka logine f�rtalacak seni.
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            
        }
    }
}