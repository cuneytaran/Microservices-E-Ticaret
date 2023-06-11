using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Consumers
{
    public class CreateOrderMessageCommandConsumer : IConsumer<CreateOrderMessageCommand>//IConsumer MassTranssit den geliyor
    {
        //FreeCourse.Services.Order.Domain infrasturucture ve shared  dependencies den AddReferance dan eklemeyi unutma
        //nudget package manager console içinde MassTransit.AspNetCore ve MassTransit.RabbitMQ kurduk

        private readonly OrderDbContext _orderDbContext;

        public CreateOrderMessageCommandConsumer(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task Consume(ConsumeContext<CreateOrderMessageCommand> context)// consume edilecek mesajı belirtiyoruz
        {
            //rabbitmq den gelen mesajı consume etmemiz için FreeCourse.Services.Order.API startup içinde consumer ı register etmemiz gerekiyor!!!
            //appsettings.json da rabbitmq url i belirttik
            // "RabbitMQUrl": "localhost",

            //services.AddMassTransit(x =>
            //{
            //    x.AddConsumer<CreateOrderMessageCommandConsumer>();
            //    x.AddConsumer<CourseNameChangedEventConsumer>();
            //    // Default Port : 5672
            //    x.UsingRabbitMq((context, cfg) =>
            //    {
            //        cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
            //        {
            //            host.Username("guest");
            //            host.Password("guest");
            //        });

            //        cfg.ReceiveEndpoint("create-order-service", e =>
            //        {
            //            e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
            //        });
            //        cfg.ReceiveEndpoint("course-name-changed-event-order-service", e =>
            //        {
            //            e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
            //        });
            //    });
            //});

            //services.AddMassTransitHostedService();

            var newAddress = new Domain.OrderAggregate.Address(
                context.Message.Province, 
                context.Message.District, 
                context.Message.Street, 
                context.Message.ZipCode, 
                context.Message.Line
                );

            Domain.OrderAggregate.Order order = new Domain.OrderAggregate.Order(context.Message.BuyerId, newAddress);//alıcının id si ve adresi

            context.Message.OrderItems.ForEach(x =>
            {
                order.AddOrderItem(x.ProductId, x.ProductName, x.Price, x.PictureUrl);
            });

            await _orderDbContext.Orders.AddAsync(order);

            await _orderDbContext.SaveChangesAsync();
        }
    }
}