using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IIdentityService
    {
        //Nudget Packages den IdentityModel li yükle Client Library olanı ekle.
        //Add Referanceden Shared dahil et
        Task<Response<bool>> SignIn(SigninInput signinInput);

        Task<TokenResponse> GetAccessTokenByRefreshToken();//TokenResponse ise IdentityModel den hazır geliyor.

        Task RevokeRefreshToken();
    }
}