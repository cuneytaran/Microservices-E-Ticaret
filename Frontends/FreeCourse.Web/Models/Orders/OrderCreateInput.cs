using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Orders
{
    public class OrderCreateInput
    {
        public OrderCreateInput()
        {
            OrderItems = new List<OrderItemCreateInput>();//Listenin boş bir constructurunu oluşturuz ki hata vermesin ilk çalışma anında
        }

        public string BuyerId { get; set; }

        public List<OrderItemCreateInput> OrderItems { get; set; }

        public AddressCreateInput Address { get; set; }
    }
}