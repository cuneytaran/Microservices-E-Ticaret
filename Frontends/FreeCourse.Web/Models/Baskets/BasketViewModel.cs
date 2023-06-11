using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Baskets
{
    public class BasketViewModel
    {
        public BasketViewModel()
        {
            _basketItems = new List<BasketItemViewModel>();
        }

        public string UserId { get; set; }

        public string DiscountCode { get; set; }

        public int? DiscountRate { get; set; }//indirimi ne
        private List<BasketItemViewModel> _basketItems;

        public List<BasketItemViewModel> BasketItems
        {
            get
            {
                if (HasDiscount)//indirim uygulanmışmı?
                {
                    //Örnek kurs fiyat 100 TL indirim %10
                    _basketItems.ForEach(x =>
                    {
                        var discountPrice = x.Price * ((decimal)DiscountRate.Value / 100);//indirim miktarı hesaplanıyor
                        x.AppliedDiscount(Math.Round(x.Price - discountPrice, 2));//tutar ile  indirimi çıkarıyoruz ve virgülden sonra en fazla iki karakter olsun
                    });
                }
                return _basketItems;
            }
            set
            {
                _basketItems = value;
            }
        }

        public decimal TotalPrice
        {
            get => _basketItems.Sum(x => x.GetCurrentPrice);//fiyatları topladık
        }

        public bool HasDiscount
        {
            get => !string.IsNullOrEmpty(DiscountCode) && DiscountRate.HasValue;//indirim kodu varsa true, indirim kodu yoksa false dönecek
        }

        public void CancelDiscount()
        {
            DiscountCode = null;
            DiscountRate = null;
        }

        public void ApplyDiscount(string code, int rate)
        {
            DiscountCode = code;
            DiscountRate = rate;
        }
    }
}