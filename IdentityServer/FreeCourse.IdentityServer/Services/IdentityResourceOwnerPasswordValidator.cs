using FreeCourse.IdentityServer.Models;
using IdentityModel;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.IdentityServer.Services
{
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator //IResourceOwnerPasswordValidator hazır geliyor
    {
        //IdentityResourceOwnerPasswordValidator startup tarafında tanıtmayı unutma. builder.AddResourceOwnerValidator<IdentityResourceOwnerPasswordValidator>();
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityResourceOwnerPasswordValidator(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var existUser = await _userManager.FindByEmailAsync(context.UserName);//böyle bir kullanıcı varmı kontrolü. kullanıcıdan userName gelecek ama ben email göndereceğim login işlemi için.

            if (existUser == null)
            {
                var errors = new Dictionary<string, object>();//Dictionary key ve value tanımlıyoruz.
                errors.Add("errors", new List<string> { "Email veya şifreniz yanlış" });//response clasına benzettik.aynı şekilde hata vermesi için
                context.Result.CustomResponse = errors;

                return;
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(existUser, context.Password);//username olarak biz email göndereceğiz.şuan email daha çok kullanıldığı için.

            if (passwordCheck == false)
            {
                var errors = new Dictionary<string, object>();
                errors.Add("errors", new List<string> { "Email veya şifreniz yanlış" });
                context.Result.CustomResponse = errors;

                return;
            }
            //herşey doğru ise. kullanıcı var ise
            context.Result = new GrantValidationResult(existUser.Id.ToString(), OidcConstants.AuthenticationMethods.Password);//user id sini ve akış tipini belirtiyoruz.
        }
    }
}