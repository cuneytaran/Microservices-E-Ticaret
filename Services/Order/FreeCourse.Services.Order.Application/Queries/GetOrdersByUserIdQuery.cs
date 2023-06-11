using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Shared.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Queries
{
    //FreeCourse.Services.Order.Domain infrasturucture ve shared  dependencies den AddReferance dan eklemeyi unutma
    //Nudget packages den MediatR.Extensions.Microsoft.DependencyInjection yükle 

    public class GetOrdersByUserIdQuery : IRequest<Response<List<OrderDto>>>//IRequest mediatR den geliyor.
    {
        public string UserId { get; set; }
    }
}