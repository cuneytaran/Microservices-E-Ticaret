using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Application.Mapping
{
    //FreeCourse.Services.Order.Domain infrasturucture ve shared  dependencies den AddReferance dan eklemeyi unutma
    //Nuget Packages den AutoMapper  yükle. core api mvc olsaydı AutoMaper.Extensions.Microsoft.DpendencyInjection yüklesek olurdu. Ama burası class library olduğu için kendimiz tanımlayacağız.
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomMapping>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => lazy.Value;//bunu çağırdığımda burası çalışmaya başlayacak.
    }
}