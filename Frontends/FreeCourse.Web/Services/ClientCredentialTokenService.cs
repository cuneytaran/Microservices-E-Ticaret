using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    //her istek yapıldığında token gönderme yapılacağı yer.

    //Nudged Package den IdentityModel.AspNetCore yükle. Bu sınıf kendi içinde implemente oluyor.

    //startup a interfaceleri eklemeyi unutma oda Extensions içinde duruyor.
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly ClientSettings _clientSettings;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;//eklemiş olduğumuz nugdget den  IClientAccessTokenCache ekliyoruz. 
        private readonly HttpClient _httpClient;

        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings, IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _clientSettings = clientSettings.Value;
            _clientAccessTokenCache = clientAccessTokenCache;
            _httpClient = httpClient;//client credential tanımlıyrouz.
        }

        public async Task<string> GetToken()
        {
            var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken");//catch üzerinde token varmı kontrolü yapıyoruz.

            if (currentToken != null)
            {
                return currentToken.AccessToken;//aynı token içinden Accesstokeni geri dönüyoruz.
            }

            //eğer catche de yoksa. tüm endpointlerin olduğu yere gidiyoruz.
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest //discovery bilgilerini aldık.
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var clientCredentialTokenRequest = new ClientCredentialsTokenRequest//client granttype ni alacağız.
            {
                ClientId = _clientSettings.WebClient.ClientId,
                ClientSecret = _clientSettings.WebClient.ClientSecret,
                Address = disco.TokenEndpoint
            };

            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);//Client Credentials Token ı gönderiyoruz.

            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn);//WebClientToken adında bir cookie içindeki bilgileri yani tokenları güncelleme işlemi.

            return newToken.AccessToken;//gelen access tokenıda geri döneceğiz.

        }
    }
}