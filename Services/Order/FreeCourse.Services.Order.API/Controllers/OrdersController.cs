using FreeCourse.Services.Order.Application.Commands;
using FreeCourse.Services.Order.Application.Queries;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.API.Controllers
{
    //FreeCourse.Services.Order.Aplication  dependencies den AddReferance dan eklemeyi unutma
    //bu apimizi koruma almak için

    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : CustomBaseController
    {
        private readonly IMediator _mediator;//veri gönderme ve alma işlemini yapan kısım
        private readonly ISharedIdentityService _sharedIdentityService;//userid yi almak için
        //startup kısmına eklemeyi unutma services.AddScoped<ISharedIdentityService, SharedIdentityService>();

        public OrdersController(IMediator mediator, ISharedIdentityService sharedIdentityService) 
        {
            _mediator = mediator;
            _sharedIdentityService = sharedIdentityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var response = await _mediator.Send(new GetOrdersByUserIdQuery { UserId = _sharedIdentityService.GetUserId });//mediator üzerinden userid yi gönderiyoruz ve veri çekiyoruz

            return CreateActionResultInstance(response);
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrder(CreateOrderCommand createOrderCommand)
        {
            var response = await _mediator.Send(createOrderCommand);//data kaydetme işlemi yapıyoruz.

            return CreateActionResultInstance(response);
        }
    }
}