using System;
using System.Collections.Generic;
using System.Text;

namespace FreeCourse.Shared.Messages
{
    //masstransit RABBITMQ için burada modelleri tanımlıyoruz
    //https://hub.docker.com/_/rabbitmq adresinden rabbitmq yükleyebiliriz.

    //***FakePayment için rabitmq yükleme adımları***
    //Web içinde OrderService içinde CreateOrderMessageCommand.cs oluşturduk
    //Services Klasörün içindeki FakePayment projesinin Models içinde OrderDto.cs oluşturduk
    //içine Ordes.cs inin içindeki get ve setleri yazdık. Yani gönderilecek olan get setleri yazdık.
    //PaymentDto içine az önce oluşturduğumuz OrderDto.cs içindeki classın get set tini tanımladık.

    //Services içindeki FakePayment içinde nudget package manager console içinde MassTransit.AspNetCore ve MassTransit.RabbitMQ kurduk
    //appsttings.json içine "RabbitMQUrl": "localhost", tanımladık. burdaki localhost dockerize ettiğimizde burdaki ismimiz değişecek.envairament olarak ezdiğimde hangi isim verdiysem o isim gelecek.
    //FakePayment içindeki Startup.cs içinde

    //  services.AddMassTransit(x =>
    //{
    //    // Default Port : 5672
    //    x.UsingRabbitMq((context, cfg) =>
    //    {
    //        cfg.Host(Configuration["RabbitMQUrl"], "/", host =>// appsettings.json içindeki RabbitMQUrl mizi okuyoruz. host olarak  username ve password u tanımlıyoruz.
    //        {
    //            host.Username("guest");//default gelen kullanıcı adı
    //            host.Password("guest");//default gelen şifre
    //        });
    //    });
    //});
    //services.AddMassTransitHostedService();

    //rabbitmq da iki tane port belirleyeceğiz. biri bizim için biri de rabbitmq için. bizim için olan portu 5672 olarak belirledik. rabbitmq için olan portu ise 15672 olarak belirledik.    
    //cmd ye  docker run -d -p 15672:15672 -p 5672:5672 --name rabbitmqcontainer rabbitmq:3.8.14-management komutuyla rabbitmq yu yükle
    //http://localhost:15672/ adresinden bağlan ve kullanıcı adı ve şifre olarak guest yaz

    //rabbitmq içindeki queue ları göndermek için FreeCourse.Web.Services içindeki OrderService.cs içindeki SuspendOrder içindeki kodu yaz
    //rabbitmq ya veriyi gönderiyoruz. var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput); //paymentInfoInput= içindeki OrderId yi gönderiyoruz.
    //services içindeki FakePayment içindeki FakePaymentsController içindeki ReceivePayment içindeki kodu yazdık. rabbitmq ya veriyi gönderdik.

    //FreeCourse.Web.Controllers içindeki OrderController içindeki Checkout içindeki kodu yazdık. rabbitmq ya veriyi gönderdik.
    //gelen sonuca göre artık işlemlerini devam ettirebirsin. örnek: sipariş oluşturulduysa rabbitmq ya gönderildi. ödeme gerçekleştirildi. bir sonuç geldi. ve işlemler devam ettirme gibi düşünebilirsin.

    //NOT: Send - Command  ve Publish - Event arasındaki farkı anlamak için
    //Send - Command : Bir işi yapmak için bir komut göndeririz. Örneğin bir sipariş oluşturmak için bir komut göndeririz.
    //Publish - Event : Bir şeyin gerçekleştiğini diğer servislere bildirmek için kullanırız. Örneğin bir sipariş oluşturulduğunda diğer servislere bildirmek için kullanırız.

    public class CreateOrderMessageCommand
    {
        public CreateOrderMessageCommand()
        {
            OrderItems = new List<OrderItem>();// ilk oluştuğunda null gelmemesi için. boş bir liste oluşturduk.
        }

        public string BuyerId { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public string Province { get; set; }

        public string District { get; set; }

        public string Street { get; set; }

        public string ZipCode { get; set; }

        public string Line { get; set; }
    }

    public class OrderItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }
    }
}