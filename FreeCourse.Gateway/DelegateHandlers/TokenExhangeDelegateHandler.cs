using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Gateway.DelegateHandlers
{
    public class TokenExhangeDelegateHandler : DelegatingHandler//http den miras alıyoruz
    {
        private readonly HttpClient _httpClient;//httpclient get set yapacağız. 
        private readonly IConfiguration _configuration;

        private string _accessToken;//global bir access Token tanımıyoruz. çünkü her requestte tokeni değiştirmek istiyoruz. bu yüzden her requestte tokeni alıp değiştireceğiz.

        //appsettings.json da 
        //"ClientId": "TokenExhangeClient",
        //"ClientSecret": "secret",
        //tanımlıyoruz. bu bilgiler config.cs deki client ile aynı olmalı

        public TokenExhangeDelegateHandler(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        private async Task<string> GetToken(string requestToken)
        {
            if (!string.IsNullOrEmpty(_accessToken))//eğer token varsa tekrar token istemiyoruz. çünkü tokeni her istediğimizde identityserver'a istek atmak istemiyoruz. bu yüzden tokeni bir değişkende tutuyoruz.
            {
                return _accessToken;
            }

            //Nudget Packages den IdentityModel yükledik.GetDiscoveryDocumentAsync tanımladık.
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest //identitydeki tüm endpointleri alıyoruz
            {
                Address = _configuration["IdentityServerURL"],//appsettings.json daki "IdentityServerURL": "http://localhost:5001", adresini buradan alacağını belirtiyoruz
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            TokenExchangeTokenRequest tokenExchangeTokenRequest = new TokenExchangeTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = _configuration["ClientId"],//appsettings.jsondaki "ClientId": "TokenExhangeClient", adresini buradan alacağını belirtiyoruz
                ClientSecret = _configuration["ClientSecret"],//appsettings.jsondaki ClientSecret içini belirtiyoruz
                GrantType = _configuration["TokenGrantType"],//appsettings.jsondaki TokenGrantType içini belirtiyoruz
                SubjectToken = requestToken,//bu metodu çağıran requestin yeni tokenini alıyoruz.subject tokeni requestinden elde ediyoruz
                SubjectTokenType = "urn:ietf:params:oauth:token-type:access-token",//tipini access token olarak belirtiyoruz
                Scope = "openid discount_fullpermission payment_fullpermission"//bu tokenin hangi scopelara sahip olacağını belirtiyoruz.
            };

            var tokenResponse = await _httpClient.RequestTokenExchangeTokenAsync(tokenExchangeTokenRequest);//Yeni tokeni alıyoruz.

            if (tokenResponse.IsError)
            {
                throw tokenResponse.Exception;
            }

            _accessToken = tokenResponse.AccessToken;

            return _accessToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var requestToken = request.Headers.Authorization.Parameter;//mevcut olan tokeni alıyoruz

            var newToken = await GetToken(requestToken);//yeni tokeni alıyoruz

            request.SetBearerToken(newToken);//SetBearerToken=identity modeldengeliyor. ve yeni tokeni set ediyoruz.

            return await base.SendAsync(request, cancellationToken);
            //Startup tarafında eklemeyi unutma.
            //services.AddOcelot().AddDelegatingHandler<TokenExhangeDelegateHandler>(); şeklinde değiştir.
            //delegenin nezaman devreye gireceğinide belirtmemiz gerekiyor.Bu yüzden 
            //services.AddHttpClient<TokenExhangeDelegateHandler>(); ekliyoruz. en üstte yerleştir.
            //configuration.development.json da 
            //"DelegatingHandlers": [ "TokenExhangeDelegateHandler" ], ekliyoruz. bunu discount ve payment içinde yapacağız.
        }
    }
}