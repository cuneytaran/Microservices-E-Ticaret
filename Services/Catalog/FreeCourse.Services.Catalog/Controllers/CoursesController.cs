using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Services;
using FreeCourse.Shared.ControllerBases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Controllers
{
    //Mongo db için nuget paketten MongoDB.Driver kur
    //Dependencies den AddReferance kısmından Sharedi referans vermeyi unutma
    //Properties klasöründeki lauchSettings.json da ayarlamayı yap.
    //token ile kontrol etmek için NugetPackageden Microsoft.AspNetCore.Authentication.JwtBearer yükle ve startup kısmına 
    //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    //{
    //options.Authority = Configuration["IdentityServerURL"];
    //options.Audience = "resource_catalog";
    //options.RequireHttpsMetadata = false;
    //});
    // ve
    //app.UseAuthentication(); ekle

    //Tüm controlleri kontrol altına almak yani Authorize için startup kısmına şunu yazıyoruz. 
    //services.AddControllers(opt =>
    //{
    //    opt.Filters.Add(new AuthorizeFilter());
    //});

    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : CustomBaseController //return response tanımlamak için bunu araya giriyoruz.shared klasöründen faydalanacağız.drum kodunu otomatik döndürmek için..
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseService.GetAllAsync();

            return CreateActionResultInstance(response); // return CreateActionResultInstance(response); bunu shared klasöründe ControllerBase de tanımladığımız return geliyor.
                                                         // metot içinden kendisi otomatik çıkaracak, 404 mü 200 mü vs.. hepsi artık otomatik dönecek.
        }

        //[HttpGet] şeklinde tanımlarsak. istek şu şekilde olmalıyldı. //courses?id=5
        //[HttpGet("{id}")] bu şekilde tanımlarsak. //courses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _courseService.GetByIdAsync(id);

            return CreateActionResultInstance(response);
        }

        //api/courses/getallbyuserid/5
        //[Route("/api/[controller]/GetAllByUserId/{userId}")] birden fazla get olduğu ve parametre aldığı için bunları ayırmak gerekiyor. 
        [HttpGet]
        [Route("/api/[controller]/GetAllByUserId/{userId}")]
        public async Task<IActionResult> GetAllByUserId(string userId)
        {
            var response = await _courseService.GetAllByUserIdAsync(userId);

            return CreateActionResultInstance(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseCreateDto courseCreateDto)
        {
            var response = await _courseService.CreateAsync(courseCreateDto);

            return CreateActionResultInstance(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update(CourseUpdateDto courseUpdateDto)
        {
            var response = await _courseService.UpdateAsync(courseUpdateDto);

            return CreateActionResultInstance(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _courseService.DeleteAsync(id);

            return CreateActionResultInstance(response);
        }
    }
}