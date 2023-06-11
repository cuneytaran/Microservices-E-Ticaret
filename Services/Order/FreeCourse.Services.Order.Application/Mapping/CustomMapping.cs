using AutoMapper;
using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Services.Order.Domain.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Mapping
{
    //FreeCourse.Services.Order.Domain infrasturucture ve shared  dependencies den AddReferance dan eklemeyi unutma
    //Nuget Packages den AutoMapper  yükle. core api mvc olsaydı AutoMaper.Extensions.Microsoft.DpendencyInjection yüklesek olurdu. Ama burası class library olduğu için kendimiz tanımlayacağız.
    internal class CustomMapping : Profile//Profile mapper dan miras alıyor
    {
        public CustomMapping()
        {
            CreateMap<Domain.OrderAggregate.Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}