using MediatR;

namespace Captivlink.Application.Interfaces.ValidatorPipelines.Queries
{
    public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
       where TQuery : IQuery<TResponse>
    {
    }
}
