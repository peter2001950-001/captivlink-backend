using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Interfaces.ValidatorPipelines;
using Captivlink.Application.Validators.Services;

namespace Captivlink.Application.Validators
{
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, ValueResult<TResponse>>
        where TRequest : class, IValidatedRequest<TResponse>
    {
        private readonly IValidator<TRequest> _validator;

        public ValidationBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public async Task<ValueResult<TResponse>> Handle(TRequest request, RequestHandlerDelegate<ValueResult<TResponse>> next, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid)
            {
                return await next();
            }

            return new(validationResult.Errors);
        }
    }
}
