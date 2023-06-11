using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.FakePayments;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    //API DEN MICROSERVISLERE GEÇERKEN YAPILACAK İŞLEMLER
    //1-configuration.development.json daki ayarları kontrol et.
    //2-appsetting.json da ayarlamayı yap. Order adında pathini ayarla
    //3-ServiceApiSettings.cs de order için ayarlamanı yap.
    //4-ServiceExtensions.cs de ayarlamanı yap

    public class OrderService : IOrderService
    {
        private readonly IPaymentService _paymentService;//ödeme için
        private readonly HttpClient _httpClient;//get ve post için
        private readonly IBasketService _basketService;//sepetteki datalar için
        private readonly ISharedIdentityService _sharedIdentityService;//clientid için

        public OrderService(IPaymentService paymentService, HttpClient httpClient, IBasketService basketService, ISharedIdentityService sharedIdentityService)
        {
            _paymentService = paymentService;
            _httpClient = httpClient;
            _basketService = basketService;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();//memory den sepetteki dataları alıyoruz

            var paymentInfoInput = new PaymentInfoInput()//ödeme işlemi
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice
            };
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);//ödeme gerçekleşti. true veya false dönecek. fakepayment bu.

            if (!responsePayment)
            {
                return new OrderCreatedViewModel() { Error = "Ödeme alınamadı", IsSuccessful = false };
            }

            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,//userid si
                Address = new AddressCreateInput { Province = checkoutInfoInput.Province, District = checkoutInfoInput.District, Street = checkoutInfoInput.Street, Line = checkoutInfoInput.Line, ZipCode = checkoutInfoInput.ZipCode },
            };

            basket.BasketItems.ForEach(x =>//sepetin içindekileri tek tek tanımlıyoruz.
            {
                var orderItem = new OrderItemCreateInput { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);//hazırlanan data json formatında gönderiliyor.

            if (!response.IsSuccessStatusCode)
            {
                return new OrderCreatedViewModel() { Error = "Sipariş oluşturulamadı", IsSuccessful = false };
            }

            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();//ReadFromJsonAsync=datayı json formatına çevriliyor.

            orderCreatedViewModel.Data.IsSuccessful = true;
            await _basketService.Delete();//ödeme işlemi gerçekleştikten sonra sepetim kısmı siliniyor.
            return orderCreatedViewModel.Data;
        }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");//"orders"= orders controlünden istek yapacak.

            return response.Data;
        }

        //RABBITMQ ile yapılan işlem ASSENKRON İLETİŞİM
        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput { Province = checkoutInfoInput.Province, District = checkoutInfoInput.District, Street = checkoutInfoInput.Street, Line = checkoutInfoInput.Line, ZipCode = checkoutInfoInput.ZipCode },
            };

            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };
                orderCreateInput.OrderItems.Add(orderItem);
            });

            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice,
                Order = orderCreateInput//yukarıdaki orderCreateInput ile paymentInfoInput birleştiriliyor.
            };

            //rabbitmq ya veri gönderiliyor.
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);

            if (!responsePayment)
            {
                return new OrderSuspendViewModel() { Error = "Ödeme alınamadı", IsSuccessful = false };
            }

            await _basketService.Delete();
            return new OrderSuspendViewModel() { IsSuccessful = true };
        }
    }
}