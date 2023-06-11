using FreeCourse.Services.PhotoStock.Dtos;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Services.PhotoStock.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : CustomBaseController
    {
        //Dependencies den AppReferance ile Shared i referans ver...
        //token ile kontrol etmek için NugetPackageden Microsoft.AspNetCore.Authentication.JwtBearer yükle ve startup kısmına
        //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        //{
        //options.Authority = Configuration["IdentityServerURL"];
        //options.Audience = "resource_catalog";
        //options.RequireHttpsMetadata = false;
        //});
        // ve
        //app.UseAuthentication(); ekle

        [HttpPost]
        public async Task<IActionResult> PhotoSave(IFormFile photo, CancellationToken cancellationToken) //CancellationToken=fotoğraf geldiğinde. işlemi sondarırsak fotoğrafı kaydetmesin. kullanıcı fotoğrafı kaydederken vazgeçerse veya tarayıcı kapatırsa işlemi durduracak.
        {
            //NOT!!!= ASENKRON BİR İŞLEMİ SADECE HATA FIRLATARAK SONLANDIRABİLİRSİNİZ....

            if (photo != null && photo.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photo.FileName); //yolunu belirliyoruz.

                using var stream = new FileStream(path, FileMode.Create);
                await photo.CopyToAsync(stream, cancellationToken);//fotoğraf kopyalanıyor.tarayıcı kapatılırsa cancellation ile hataya düşecek ve duracak

                //http://www.photostock.api.com/photos/deneme.jpg
                var returnPath = photo.FileName;//fotoğraf kaydedildikten sonra dönüş yolunu gösteriyor.ve dosya ismi

                PhotoDto photoDto = new() { Url = returnPath };//modele dolduruluyor.

                return CreateActionResultInstance(Response<PhotoDto>.Success(photoDto, 200));
            }

            return CreateActionResultInstance(Response<PhotoDto>.Fail("photo is empty", 400));
        }


        [HttpDelete]
        public IActionResult PhotoDelete(string photoUrl)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/photos", photoUrl);//silinecek pathi alıyoruz.
            if (!System.IO.File.Exists(path))//statik metot kullanıyoruz. bunu startup da tanımladık. oraya bak.
            {
                return CreateActionResultInstance(Response<NoContent>.Fail("photo not found", 404));
            }

            System.IO.File.Delete(path);//silme işlemi yapıyoruz.

            return CreateActionResultInstance(Response<NoContent>.Success(204));
        }
    }
}