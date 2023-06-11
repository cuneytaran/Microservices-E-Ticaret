using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;//rabbitmq ile iletişim kurmak için kullanıyoruz. tek microservice ile iletişim kurmak istiyorsak kullanıyoruz.

        public FakePaymentsController(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
        {
            //paymentDto ile ödeme işlemi gerçekleştir. Tek bir microservice ile iletişim kurmak istiyorsak kullanıyoruz.
            //kuyruk yolunu belirliyoruz.
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));//Uri=kuyruk ismi. yani veriler bu adrese gelecek ve kuyruğa eklenecek.

            //kuyruğa eklenecek verileri oluşturuyoruz.
            var createOrderMessageCommand = new CreateOrderMessageCommand();

            createOrderMessageCommand.BuyerId = paymentDto.Order.BuyerId;
            createOrderMessageCommand.Province = paymentDto.Order.Address.Province;
            createOrderMessageCommand.District = paymentDto.Order.Address.District;
            createOrderMessageCommand.Street = paymentDto.Order.Address.Street;
            createOrderMessageCommand.Line = paymentDto.Order.Address.Line;
            createOrderMessageCommand.ZipCode = paymentDto.Order.Address.ZipCode;

            paymentDto.Order.OrderItems.ForEach(x =>
            {
                createOrderMessageCommand.OrderItems.Add(new OrderItem //createOrderMessageCommand içerisindeki OrderItems listesine ekliyoruz.
                {
                    PictureUrl = x.PictureUrl,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName
                });
            });

            //2.yazım şekli
            //paymentDto.Order.OrderItems.ForEach(x =>
            //{
            //    var orderItem = new OrderItem
            //    {
            //        PictureUrl = x.PictureUrl,
            //        Price = x.Price,
            //        ProductId = x.ProductId,
            //        ProductName = x.ProductName
            //    };
            //    createOrderMessageCommand.OrderItems.Add(orderItem);
            //});

            await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);//rabbitmq ya mesajı gönderiyoruz.

            return CreateActionResultInstance(Shared.Dtos.Response<NoContent>.Success(200));
        }
    }
}