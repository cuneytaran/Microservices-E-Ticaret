using FreeCourse.Services.Order.Application.Consumers;
using FreeCourse.Services.Order.Infrastructure;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Consumers
{
    public class CourseNameChangedEventConsumer : IConsumer<CourseNameChangedEvent>//IConsumer= MassTransit kütüphanesinden geliyor. CourseNameChangedEvent= Shared içindeki eventten geliyor.
    {
        //FreeCourse.Services.Order.Domain infrasturucture ve shared  dependencies den AddReferance dan eklemeyi unutma
        //Nudget Packages manager console içinde MassTransit.AspNetCore ve MassTransit.RabbitMQ kurduk
        //startup tarafında ayar yapmamız gerekiyor.
        //services.AddMassTransit(x =>
        //    {
        //        x.AddConsumer<CreateOrderMessageCommandConsumer>();
        //        x.AddConsumer<CourseNameChangedEventConsumer>();
        //        // Default Port : 5672
        //        x.UsingRabbitMq((context, cfg) =>
        //        {
        //            cfg.Host(Configuration["RabbitMQUrl"], "/", host =>
        //            {
        //                host.Username("guest");
        //                host.Password("guest");
        //            });

        //            cfg.ReceiveEndpoint("create-order-service", e =>//rabbitmq da kuyruğun adresini belirliyoruz
        //            {
        //                e.ConfigureConsumer<CreateOrderMessageCommandConsumer>(context);
        //            });
        //      cfg.ReceiveEndpoint("course-name-changed-event-order-service", e =>
        //    {
        //        e.ConfigureConsumer<CourseNameChangedEventConsumer>(context);
        //    });
        //                    });
        //                });

        //services.AddMassTransitHostedService();

private readonly OrderDbContext _orderDbContext;

        public CourseNameChangedEventConsumer(OrderDbContext orderDbContext)//OrderDbContext 
        {
            _orderDbContext = orderDbContext;
        }

        public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
        {
            var orderItems = await _orderDbContext.OrderItems.Where(x => x.ProductId == context.Message.CourseId).ToListAsync();

            orderItems.ForEach(x =>
            {
                x.UpdateOrderItem(context.Message.UpdatedName, x.PictureUrl, x.Price);//memoryde güncellendi
            });

            await _orderDbContext.SaveChangesAsync();//veritabanında güncellendi
        }
    }
}