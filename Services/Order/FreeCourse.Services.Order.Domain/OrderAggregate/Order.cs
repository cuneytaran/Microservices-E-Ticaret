using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    //FreeCourse.Services.Order.Domain.Core  dependencies den AddReferance dan eklemeyi unutma
    //Nudget packages den MediatR.Extensions.Microsoft.DependencyInjection yükle 

    //EF Core features
    // -- Owned Types
    // -- Shadow Property
    // -- Backing Field
    public class Order : Entity, IAggregateRoot
    {
        public DateTime CreatedDate { get; private set; }

        public Address Address { get; private set; }//Owned Types denir. ister içindekileri tüm tanımlamaları sütün olarak tanımlayabilir.

        public string BuyerId { get; private set; }

        private readonly List<OrderItem> _orderItems;

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;//Backing Field=get ve set yoksa field denir. Kimse orderitem eklemesin benim metodum üzerinden eklesin.Dış dünyadan alabilsin ancak set edemesin.
        //burada bir kapsülleme işlemi yaptık._orderItems=EF Core bunununla doldursun.

        public Order()//Order constructur üretiyoruz.
        {
        }

        public Order(string buyerId, Address address)//Order parametreli constructur üretiyoruz.
        {
            _orderItems = new List<OrderItem>();
            CreatedDate = DateTime.Now;
            BuyerId = buyerId;
            Address = address;
        }

        public void AddOrderItem(string productId, string productName, decimal price, string pictureUrl)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == productId);

            if (!existProduct)
            {
                var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);

                _orderItems.Add(newOrderItem);
            }
        }

        public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
    }
}