using FluentValidation;
using FreeCourse.Web.Models.Discounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Validators
{
    //nuget packages den FluentValidation.AspNetCore kur
    //startup tarafında eklemeyi unutma. services.AddControllersWithViews().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CourseCreateInputValidator>());
    public class DiscountApplyInputValidator : AbstractValidator<DiscountApplyInput>//DiscountApplyInput= modeldeki DiscountApplyInput classına bağlanacak
    {
        public DiscountApplyInputValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("indirim kupon alanı boş olamaz");
        }
    }
}