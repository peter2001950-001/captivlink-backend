using MediatR;

namespace Captivlink.Application.Interfaces.ValidatorPipelines.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse>
    {
    }
}