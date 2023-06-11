using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Services.Basket.Settings;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Controllers
{
    //token ile kontrol etmek için NugetPackageden Microsoft.AspNetCore.Authentication.JwtBearer yükle ve startup kısmına
    //burdaki startup DİĞERLERİNDEN FARKLI!!!
    //var requireAuthorizePolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
    //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    //{
    //options.Authority = Configuration["IdentityServerURL"];
    //options.Audience = "resource_basket";
    //options.RequireHttpsMetadata = false;
    //});
     //services.AddControllers(opt =>
     //{
     //opt.Filters.Add(new AuthorizeFilter(requireAuthorizePolicy));
     //});
    // ve
    //app.UseAuthentication(); ekle
    //Dependencies den AddReferance kısmından Sharedi referans vermeyi unutma
    //Properties klasöründeki lauchSettings.json da ayarlamayı yap.
    //NudgetPackages den StackExchange.Redis yükle
    //appsettings.json içinde redise nasıl bağlanacağını belirledik.
    //settings klasörün içinde redis ayarları yaptık.
    //startup tarafında redisi tanımlama yaptım.
    //services.Configure<RedisSettings>(Configuration.GetSection("RedisSettings"));            
    //services.AddSingleton<RedisService>(sp =>
    //{
    //var redisSettings = sp.GetRequiredService<IOptions<RedisSettings>>().Value;
    //var redis = new RedisService(redisSettings.Host, redisSettings.Port);
    //redis.Connect();
    //return redis;
    //});
    //userid leri okumak için shared dan. startup kısmına services.AddHttpContextAccessor(); eklememiz gerekiyor.shared klasöründeki token içindeki claim içindeki userid yi okumak için bunu yapıyoruz.

[Route("api/[controller]")]
    [ApiController]
    public class BasketsController : CustomBaseController
    {
        //bu iki servis startupda tanımlama yapmayı unutma.
        private readonly IBasketService _basketService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public BasketsController(IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _basketService = basketService;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBasket()
        {            
            return CreateActionResultInstance(await _basketService.GetBasket(_sharedIdentityService.GetUserId));
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdateBasket(BasketDto basketDto)
        {
            basketDto.UserId = _sharedIdentityService.GetUserId;
            var response = await _basketService.SaveOrUpdate(basketDto);

            return CreateActionResultInstance(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBasket()

        {
            return CreateActionResultInstance(await _basketService.Delete(_sharedIdentityService.GetUserId));
        }
    }
}