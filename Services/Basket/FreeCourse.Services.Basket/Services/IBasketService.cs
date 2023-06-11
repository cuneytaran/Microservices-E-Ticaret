using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Services
{
    public interface IBasketService
    {
        Task<Response<BasketDto>> GetBasket(string userId);

        Task<Response<bool>> SaveOrUpdate(BasketDto basketDto);//redis ekleme ve güncellemeyi aynı anda yapacağız. Yoksa oluşturacak varsa update yapacak.

        Task<Response<bool>> Delete(string userId);
    }
}