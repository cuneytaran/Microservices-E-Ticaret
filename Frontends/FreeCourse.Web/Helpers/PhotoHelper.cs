using FreeCourse.Web.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Helpers
{
    public class PhotoHelper
    {
        //startup içine eklemeyi unutma servisi
        //appsettings.json içindeki "PhotoStockUri": "http://localhost:5012", kullanacağız. o yüzden _serviceApiSettings kullanıyoruz.
        private readonly ServiceApiSettings _serviceApiSettings;

        public PhotoHelper(IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _serviceApiSettings = serviceApiSettings.Value;
        }

        public string GetPhotoStockUrl(string photoUrl)
        {
            return $"{_serviceApiSettings.PhotoStockUri}/photos/{photoUrl}";///photos/{photoUrl= photos klasörü içinde. http://localhost:5012/photos/deneme.jpg adresini gönderiyoruz.
        }
    }
}