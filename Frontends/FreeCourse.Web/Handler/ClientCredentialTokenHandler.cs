using FreeCourse.Web.Exceptions;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Handler
{
    public class ClientCredentialTokenHandler : DelegatingHandler//DelegatingHandler delegeyi miras olarak alıyoruz.
    {
        private readonly IClientCredentialTokenService _clientCredentialTokenService;// token servisinden tokel elde etmemize yarar.

        public ClientCredentialTokenHandler(IClientCredentialTokenService clientCredentialTokenService)
        {
            _clientCredentialTokenService = clientCredentialTokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)//HttpRequestMessage=header da token göndermek için kullanacağız.
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _clientCredentialTokenService.GetToken());//headerdeki gettoken geçiyoruz. requestin headerine tokeni ekliyoruz.

            var response = await base.SendAsync(request, cancellationToken);//request ve cancellationToken token gönderiyoruz.

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)//authorize değilse hata versin.
            {
                throw new UnAuthorizeException();
            }

            return response;
        }
    }
}