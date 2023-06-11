using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeCourse.Shared.Services
{
    //token içinden userid sini otomatik almak için yapıyoruz
    public class SharedIdentityService : ISharedIdentityService
    {
        //IHttpContextAccessor özel bir interface dir.bunu tanımlayabilmek için startup kısmına eklemek gerekiyor.

        private IHttpContextAccessor _httpContextAccessor; //IHttpContextAccessor=jsonwebtoken lerin içindeki claimler httpcontext içine eklenir bunu içini okumak için ekliyoruz.

        public SharedIdentityService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId => _httpContextAccessor.HttpContext.User.FindFirst("sub").Value;//bu userid yi heryerde kullanabilmek için. her servisin startubunu servislerin içine yazman gerekiyor.bunun içinde o servisin startubunu yazman gerekiyor.
        //veya
        //public string GetUserId => _httpContextAccessor.HttpContext.User.Claims.Where(x=>x.Type=="sub").FirstOrDefault().Value;
    }
}