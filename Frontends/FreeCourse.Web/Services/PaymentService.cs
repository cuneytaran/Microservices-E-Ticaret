using FreeCourse.Web.Models.FakePayments;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class PaymentService : IPaymentService
    {
        //appsettings den ServiceApiSettings den payment tanımla.
        //Model içindeki ServiceApiSettings.cs içini düzenle
        //startup yani Extensions tanımlamayı unutma.

        private readonly HttpClient _httpClient;//get ve post işlemi için

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ReceivePayment(PaymentInfoInput paymentInfoInput)
        {
            var response = await _httpClient.PostAsJsonAsync<PaymentInfoInput>("fakepayments", paymentInfoInput);//PostAsJsonAsync= datayı serialese yapsın. fakepayments=servicesdeki gidecek controller ismi.

            return response.IsSuccessStatusCode;
        }
    }
}