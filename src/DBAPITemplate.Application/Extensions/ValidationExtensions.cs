using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAPITemplate.Application.Extensions
{
    public static class ValidationExtensions
    {
        public static async Task ValidateAndThrowCustomAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken)
        {
            var result = await validator.ValidateAsync(instance, cancellationToken);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
