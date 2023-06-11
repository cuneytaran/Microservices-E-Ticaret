using FluentValidation;
using FreeCourse.Web.Models.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Validators
{
    //nuget packages den FluentValidation.AspNetCore kur
    //startup tarafında eklemeyi unutma. services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CourseCreateInputValidator>());
    public class CourseCreateInputValidator : AbstractValidator<CourseCreateInput>//CourseCreateInput= modeldeki CourseCreateInput classına bağlanacak
    {
        public CourseCreateInputValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("isim alanı boş olamaz");//boş olursa WithMessage ile uyarı yazsın.
            RuleFor(x => x.Description).NotEmpty().WithMessage("açıklama alanı boş olamaz");
            RuleFor(x => x.Feature.Duration).InclusiveBetween(1, int.MaxValue).WithMessage("süre alanı boş olamaz");//InclusiveBetween= 1 saat ile max değeri arası olmalı

            RuleFor(x => x.Price).NotEmpty().WithMessage("fiyat alanı boş olamaz").ScalePrecision(2, 6).WithMessage("hatalı para formatı");//ScalePrecision=virgülden önce 4 karakter, virgülden sonra 2 karakter kullanıbilirsin  $$$$.$$  1234.00
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("kategori alanı seçiniz");
        }
    }
}