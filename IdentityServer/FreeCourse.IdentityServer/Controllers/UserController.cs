using FreeCourse.IdentityServer.Dtos;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;

namespace FreeCourse.IdentityServer.Controllers
{

    //startup ConfigureServices içine services.AddLocalApiAuthentication(); app.UseAuthorization(); ekle 
    [Authorize(LocalApi.PolicyName)]//bununla koruma altına alıyoruz.Gelen policyName içinde IdentityServerApi yazısını bekliyor.
    [Route("api/[controller]/[action]")] //http://localhost:5001/api/user/signup
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager; //UserManager hazır geliyor.

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        //Kullanıcının kayıt olması gereken controller burası
        [HttpPost]
        public async Task<IActionResult> SignUp(SignupDto signupDto)
        {
            var user = new ApplicationUser
            {
                UserName = signupDto.UserName,
                Email = signupDto.Email,
                City = signupDto.City
            };

            var result = await _userManager.CreateAsync(user, signupDto.Password);//user ve password geliyor

            if (!result.Succeeded)
            {
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));// return Response yapabilmek için dependencies den AddReferance dan FreeCourse.Shared eklemeyi unutma
            }

            return NoContent(); //hata yoksa geriye birşey dönmesin.
        }

        //token ile user bilgilerini alma
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);//id sini göre bilgiyi getiriyoruz.

            if (userIdClaim == null) return BadRequest();

            var user = await _userManager.FindByIdAsync(userIdClaim.Value);

            if (user == null) return BadRequest();

            return Ok(new { Id = user.Id, UserName = user.UserName, Email = user.Email, City = user.City });
        }
    }
}