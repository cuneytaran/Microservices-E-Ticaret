using FreeCourse.Web.Models.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    //Extensions kısmına yani startup a eklemeyi unutma
    //appsettings.json a Discount eklemeyi unutma
    //Gateway deki configuration.development.json da da tanımlamayı unutma
    public interface IDiscountService
    {
        Task<DiscountViewModel> GetDiscount(string discountCode);
    }
}