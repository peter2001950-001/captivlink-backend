using Captivlink.Application.Validators.Services;
using MediatR;

namespace Captivlink.Application.Interfaces.ValidatorPipelines.Queries
{
    public interface IValidatedQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, ValueResult<TResponse>>
    where TQuery : IValidatedQuery<TResponse>
    {
    }
}
