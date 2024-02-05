using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;

namespace Captivlink.Application.Commons.Commands
{
    public abstract record BaseCommand<TResponse> : IBaseRequest, ICommand<TResponse>
    {
    }
}
