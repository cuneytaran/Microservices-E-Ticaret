using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Mapping
{
    public class GeneralMapping : Profile//Profile miras alıyor. Reflection.
    {
        public GeneralMapping()
        {
            CreateMap<Course, CourseDto>().ReverseMap();//Course dan bir CourseDto nesnesine döntüşme.ReverseMap terside olur.
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Feature, FeatureDto>().ReverseMap();

            CreateMap<Course, CourseCreateDto>().ReverseMap();
            CreateMap<Course, CourseUpdateDto>().ReverseMap();
        }
    }
}