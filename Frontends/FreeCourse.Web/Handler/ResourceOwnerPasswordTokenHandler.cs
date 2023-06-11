using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    //NOT: BURDAKİ İŞLEMLERİ AKTİF HALE GETİRMEK İÇİN STARTUP DA TANIMLAMAN GEREKİYOR.biz bir kod kalabalığı olmasın diye
    //Extensions klasörün içindeki ServiceExtension clasın içine tanımladık.
    //startup da ise services.AddHttpClientServices(Configuration); bu şekilde tanımladık.
    //yani IUserService içinde bir istek başlatıldığında ilk aşağıdakini çalıştır yaptık

     //services.AddHttpClient<IUserService, UserService>(opt => {
     //opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
     //}).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

//intercepter operasyonu. yani her işlemde burasının araya girmesi.
public class ResourceOwnerPasswordTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;
        private readonly ILogger<ResourceOwnerPasswordTokenHandler> _logger;

        public ResourceOwnerPasswordTokenHandler(IHttpContextAccessor httpContextAccessor, IIdentityService identityService, ILogger<ResourceOwnerPasswordTokenHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _logger = logger;
        }

        //her istek geldiğinde burası araya girecek ve çalıştırılacak.
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);//access tokeni okuyoruz.

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);//gelen access tokeni gelen requestin headerine ekliyoruz.

            var response = await base.SendAsync(request, cancellationToken);//elimdeki requesti cancellationToken e gönderiyoruz.

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)// eğer 401 geldiyse. access tokenin ömrü dolmuş demektir.
            {
                var tokenResponse = await _identityService.GetAccessTokenByRefreshToken();//refresh token ile beraber yeni bir access token alıyoruz.

                if (tokenResponse != null)//eğer bu metot hiç çalışmıyorsa 
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken);//bu sefer access token ise tokenResponse.AccessToken den gelecek.

                    response = await base.SendAsync(request, cancellationToken);
                }
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)//buraya kadar geliyorsa artık refresh tokenin ömrü doldumuş demektir.
            {
                throw new UnAuthorizeException();//hata fırlatıp. kullanıcıyı login ekranına döndüreceğiz. tüm hataları yakalayan hazır middleware kullanacağız.
            }

            return response;
        }
    }
}