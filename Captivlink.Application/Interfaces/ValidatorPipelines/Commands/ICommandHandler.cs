using Captivlink.Application.Validators.Services;
using MediatR;

namespace Captivlink.Application.Interfaces.ValidatorPipelines.Commands
{
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, ValueResult<TResponse>>
       where TCommand : ICommand<TResponse>
    {

    }
}
