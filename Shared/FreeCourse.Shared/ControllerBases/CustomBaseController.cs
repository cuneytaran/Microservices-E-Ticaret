using FreeCourse.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.ControllerBases
{
    //Microservisler içindeki ortak yapıları tanımlama için shared kullanıyoruz.

    public class CustomBaseController : ControllerBase //controllerden miras alamayız. controllerbase library almak için frameworku almak lazım.
                                                       //FreeCourse.Shared projesine sağ tıkla Edit Project File ile içine giriyoruz kendimiz ellek ekliyoruz frameworku.
                                                       //<FrameworkReference Include="Microsoft.AspNetCore.App" /> ekliyoruz.
                                                       //artık bu framework kütüpanenin içindeki tüm şeyleri erişebilir.bunun içinde ControllerBase de var.
    {
        public IActionResult CreateActionResultInstance<T>(Response<T> response) //hepsinde kullanacağımız ortak metot tanımlıyoruz.
        {
            return new ObjectResult(response) //dönüş tipini herşeyi dönebilirim. dönüş olarak status deki kodu dönecek. artık response dan 404 geldiyse 404 dönecek. return badrequst vs.. yazmaya gerek kalmayacak.
            {
                StatusCode = response.StatusCode
            };
        }
    }
}