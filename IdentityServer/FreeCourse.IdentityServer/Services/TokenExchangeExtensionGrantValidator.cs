using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator//IExtensionGrantValidator = identityserver4 üzerinden gelen bir interface
    {
        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";//sadece token-exchange diyebilirsin ancak bu şekilde daha anlaşılır oluyor

        private readonly ITokenValidator _tokenValidator;//identity server 4 içerisindeki tokenvalidator interface'ini kullanacağız

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)//doğrulama işlemlerini burada yapacağız
        {
            var requestRaw = context.Request.Raw.ToString();

            var token = context.Request.Raw.Get("subject_token");//ham requestten subject tokeni alıyoruz

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidRequest, "token missing");
                return;
            }

            var tokenValidateResult = await _tokenValidator.ValidateAccessTokenAsync(token);//tokeni doğruluyoruz

            if (tokenValidateResult.IsError)//token doğru gelmiyorsa veya hatalıysa
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token invalid");

                return;
            }

            var subjectClaim = tokenValidateResult.Claims.FirstOrDefault(c => c.Type == "sub");//token doğruysa sub claimini alıyoruz. sub claimi olmadan token olmaz.yani kullanıcı id'si

            if (subjectClaim == null)
            {
                context.Result = new GrantValidationResult(IdentityServer4.Models.TokenRequestErrors.InvalidGrant, "token must contain sub value");

                return;
            }

            context.Result = new GrantValidationResult(subjectClaim.Value, "access_token", tokenValidateResult.Claims);//yeni tokenimizi oluşturup yenisini dönüyoruz.
            //startup tarafında bu grant type'ı tanımlamamız gerekiyor
            //builder.AddExtensionGrantValidator<TokenExchangeExtensionGrantValidator>();
            //Config.cs tarafında yeni bir client tanımlamamız gerekiyor.
            return;
        }
    }
}