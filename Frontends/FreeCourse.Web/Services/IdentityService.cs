using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class IdentityService : IIdentityService
    {
        //Nudget packages deki identityModel bizim identity servere istek yapabileceğimiz hazır sınıfılar sunar.
        //oath 2.0 yetkilendirme ile ilgili openid ise kimlik doğrulama ilgili bir protokoldür.
        //ÖNEMLİ... Eğerki kendi içinde login olacaksa yani identiy servere girtmeyecekse nudget packages den Microsotf.IdentitiyModel.Protocols.OpenIdConnetc kullanırız.
        //ÖNEMLİ... Eğerki identity nin kendi login sayfasına gidecek isek. Microsoft.AspnetCore.Authentication.OpenIdConnect kullanmamız gerekirdi.


        private readonly HttpClient _httpClient;//microservice istek yapacağımızdan dolayı bunu ekliyoruz.
        private readonly IHttpContextAccessor _httpContextAccessor;//cookie ye erişmek için  ekliyoruz.
        private readonly ClientSettings _clientSettings;//clientid ve client secret ihtiyacımız var bu yüzden tanımlıyoruz.IOptions üzeinden gelecek
        private readonly ServiceApiSettings _serviceApiSettings;

        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings, IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        //refresh token ile beraber yeni bir access token alma işlemi
        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            //http://localhost:5001/.well-known/openid-configuration identity nin sahip olduğu tüm endpointleri çağırıyoruz. 
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,//http://localhost:5001/ e istek yapacak
                Policy = new DiscoveryPolicy { RequireHttps = false }//RequireHttps= https i kaldırıyoruz. http yapıyoruz
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);//refresh token okuyoruz

            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);//refresh tokeni göndererek token bilgisini okuyoruz

            if (token.IsError)
            {
                return null;
            }

            var authenticationTokens = new List<AuthenticationToken>()
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value= DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();//cookiedeki bilgileri yenileriyle değiştireceğiz.

            var properties = authenticationResult.Properties;//içinen bilgilerini çekiyoruz
            properties.StoreTokens(authenticationTokens);//yeni bilgileri var olan cookie ile değiştiriyoruz.

            //
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.Principal, properties);//CookieAuthenticationDefaults.AuthenticationScheme= bunun içinde cookie yazıyor.istersen elle yazabilirsin.

            return token;
        }

        //LOGOUT İŞLEMİ. refresh tokeni kaldırma işlemi. logout yani. Not!! Access token revoke edemezsin. sadece refresh tokeni revoke yapabilirsin.
        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);//var olan refresh tokeni alıyoruz.

            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address = disco.RevocationEndpoint,//discoden gelen adres.
                Token = refreshToken,
                TokenTypeHint = "refresh_token"//refresh token gönderdiğimi belirtiyoruz.
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);//cookideki refresh tokeni siliyoruz.
        }

        //LOGİN İŞLEMİ
        public async Task<Response<bool>> SignIn(SigninInput signinInput)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,//http://localhost:5001/ e istek yapacak
                Policy = new DiscoveryPolicy { RequireHttps = false }//RequireHttps= https i kaldırıyoruz. http yapıyoruz
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var passwordTokenRequest = new PasswordTokenRequest//PasswordTokenRequest=Resource owner password un kısaltılmışı. yani identityden gelen bilgileri ayıklıyoruz.
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signinInput.Email,
                Password = signinInput.Password,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);//token verilerini alıyoruz.

            if (token.IsError)
            {
                var responseContent = await token.HttpResponse.Content.ReadAsStringAsync();//tokenden gelen response içini okuyoruz.

                var errorDto = JsonSerializer.Deserialize<ErrorDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });//hata varsa hatasını okumak için

                return Response<bool>.Fail(errorDto.Errors, 400);//hatalarını gösteren dönüş yapıyor
            }

            var userInfoRequest = new UserInfoRequest//UserInfoRequest=hazır
            {
                Token = token.AccessToken,//token bilgisi
                Address = disco.UserInfoEndpoint//nereye istek yapacak
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);//GetUserInfoAsync=get isteği yapıyoruz. bu hazır.

            if (userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");//AuthenticationScheme=cokies geliyor. Dikkatt!!! "name", "role" gelen cokie içinden name ve role leri oku.

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);//ClaimsPrincipal=cokie oluşturmak için bu gerekli. ve verileride üstten alıyoruz.

            var authenticationProperties = new AuthenticationProperties();//access token ve refresh tokeni tutuyor olacağız.

            authenticationProperties.StoreTokens(new List<AuthenticationToken>()//cokie içinde token tutma işlemi başlıyoruz.
            {
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.AccessToken,Value=token.AccessToken},//openid connect deki isimi kullanacağız.token içinden access tokeni aliyoruz
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.RefreshToken,Value=token.RefreshToken},//token üzerinen refresh tokenın valuesunu alıyrouz
                new AuthenticationToken{ Name=OpenIdConnectParameterNames.ExpiresIn,Value= DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}//token süresini süresini saniye cinsinden çevirip.InvariantCulture bilgisi olmadan stringe çeviriyoruz. yani formatlamayı engelliyoruz. 
            });

            authenticationProperties.IsPersistent = signinInput.IsRemember;//beni hatırla yı yapıyoruz. cookinin kalıcı olarak kalmasını sağlıyoruz. 60 gün ise 60 gün cookie kalıcı olacak.

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);//kullanıcı login oluşuyor ve cookie oluşuyor.

            return Response<bool>.Success(200);
        }
    }
}