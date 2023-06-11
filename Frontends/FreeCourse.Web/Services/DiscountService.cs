using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Discounts;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    //Extensions kısmına yani startup a eklemeyi unutma
    //appsettings.json a Discount eklemeyi unutma
    //Gateway deki configuration.development.json da da tanımlamayı unutma
    //models kısmında ServiceApiSettings.cs yi eklemeyi unutma.

    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _httpClient;//post ve get işlemlerini yapıyoruz.

        public DiscountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DiscountViewModel> GetDiscount(string discountCode)
        {
            //[controller]/[action]/{code}

            var response = await _httpClient.GetAsync($"discounts/GetByCode/{discountCode}");//istek yapacağımız adresi ekliyoruz.

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var discount = await response.Content.ReadFromJsonAsync<Response<DiscountViewModel>>();

            return discount.Data;
        }
    }
}