using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;

namespace Captivlink.Application.Commons.Queries
{
    public abstract class BaseQuery<TResponse> : IBaseRequest, IValidatedQuery<TResponse>
    {
    }
}
