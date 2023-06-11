using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Baskets
{
    public class BasketItemViewModel
    {
        public int Quantity { get; set; } = 1;

        public string CourseId { get; set; }
        public string CourseName { get; set; }

        public decimal Price { get; set; }

        private decimal? DiscountAppliedPrice;

        public decimal GetCurrentPrice//indirimli fiyatı
        {
            get => DiscountAppliedPrice != null ? DiscountAppliedPrice.Value : Price;//indirimli fiyatı ver eğer indirim yoksa normal fiyatı ver.
        }

        public void AppliedDiscount(decimal discountPrice)
        {
            DiscountAppliedPrice = discountPrice;
        }
    }
}