using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI2.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSize = new[] { 5, 10, 15 };
        public RestaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if(!allowedPageSize.Contains(value))
                {
                    context.AddFailure("PageSize", $"PageSize must be in [{string.Join(",", allowedPageSize)}]");
                }
            });
        }
    }
}
