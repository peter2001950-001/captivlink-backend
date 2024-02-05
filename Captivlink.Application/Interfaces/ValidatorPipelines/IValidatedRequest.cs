using Captivlink.Application.Validators.Services;
using MediatR;

namespace Captivlink.Application.Interfaces.ValidatorPipelines
{
    public interface IValidatedRequest<TResponse> : IRequest<ValueResult<TResponse>>
    {
    }
}
