namespace Captivlink.Application.Interfaces.ValidatorPipelines.Commands
{
    public interface ICommand<TResponse> : IValidatedRequest<TResponse>
    {
    }
}
