using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public string UserId { get; set; }

        public string DiscountCode { get; set; }//indirim kodu

        public int? DiscountRate { get; set; }//
        public List<BasketItemDto> basketItems { get; set; }//ürünler yani satın alınan kurslar

        public decimal TotalPrice//fiyatı
        {
            get => basketItems.Sum(x => x.Price * x.Quantity);//burada linq işlemi yapabiliyoruz.
        }
    }
}