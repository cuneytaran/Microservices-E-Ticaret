using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.PhotoStocks;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    //appsettings.json için PhotoStock ayarı yapmayı unutma.
    // "PhotoStock": {
    //  "Path": "services/photostock/"
    //},
    //startup kısımını eklemeyi unutma. oda ServiceExtension  içinde tanımladık.
    public class PhotoStockService : IPhotoStockService
    {
        private readonly HttpClient _httpClient;//get,post işlemini asenkron olarak işlem yapmamızı sağlar.

        public PhotoStockService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DeletePhoto(string photoUrl)
        {
            var response = await _httpClient.DeleteAsync($"photos?photoUrl={photoUrl}");//photos controlüne gönderiyrouz.photoUrl olarak bir adres istiyor.
            return response.IsSuccessStatusCode;
        }

        public async Task<PhotoViewModel> UploadPhoto(IFormFile photo)
        {
            if (photo == null || photo.Length <= 0)
            {
                return null;
            }
            // örnek dosya ismi= 203802340234.jpg
            var randonFilename = $"{Guid.NewGuid().ToString()}{Path.GetExtension(photo.FileName)}";//Path.GetExtension=dosya uzantısını alıyoruz. nokta jpg şeklinde gelecek.

            using var ms = new MemoryStream();//MemoryStream= Kısa bir süre için bellekte tutulacak akım (stream) oluşturur.

            await photo.CopyToAsync(ms);//MemoryStream e kopyalama işlemi

            var multipartContent = new MultipartFormDataContent();//requestin body sine gönderiyoruz.çonkü postmanden bir conntent istiyor.

            multipartContent.Add(new ByteArrayContent(ms.ToArray()), "photo", randonFilename);//byte dönüştürüyoruz ve bir array contenti ekliyoruz. bu byte isim veriyoruz onuda photo ismini veriyrouz çünkü controller photo olarak bekliyor o yüzden isim aynı olmalı.

            var response = await _httpClient.PostAsync("photos", multipartContent);//post edilecek controller ismini veriyrouz.yani photos a. gönderilecek dosyayıda ekliyoruz.

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseSuccess = await response.Content.ReadFromJsonAsync<Response<PhotoViewModel>>();

            return responseSuccess.Data;
        }
    }
}