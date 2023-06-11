using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    //API DEN MICROSERVISLERE GEÇERKEN YAPILACAK İŞLEMLER
    //1-configuration.development.json daki ayarları kontrol et.
    //2-appsetting.json da ayarlamayı yap. Order adında pathini ayarla
    //3-ServiceApiSettings.cs de order için ayarlamanı yap.
    //4-ServiceExtensions.cs de ayarlamanı yap

    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;//Sepetim servisi
        private readonly IOrderService _orderService;//Sipariş servisi

        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Checkout()
        {
            var basket = await _basketService.Get();//sepetteki dataları alıyoruz.

            ViewBag.basket = basket;
            return View(new CheckoutInfoInput());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
        {
            //1. yol senkron iletişim
            //  var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
            // 2.yol asenkron iletişim rabbitmq lu olan
            var orderSuspend = await _orderService.SuspendOrder(checkoutInfoInput);
            if (!orderSuspend.IsSuccessful)
            {
                var basket = await _basketService.Get();

                ViewBag.basket = basket;

                ViewBag.error = orderSuspend.Error;

                return View();
            }
            //1. yol senkron iletişim
            //  return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = orderStatus.OrderId });

            //2.yol asenkron iletişim rabbitmq lu olan
            return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = new Random().Next(1, 1000) });//ödemenin başarılı olduğunu durumunda dönen id olarak fake olarak id üretiyoruz.
        }

        public IActionResult SuccessfulCheckout(int orderId)//başarılı ise buraya yönlendirilecek.
        {
            ViewBag.orderId = orderId;
            return View();
        }

        public async Task<IActionResult> CheckoutHistory()
        {
            return View(await _orderService.GetOrder());
        }
    }
}