using FluentValidation;

namespace Captivlink.Application.Commons
{
    public class BaseValidator<T>: AbstractValidator<T> where T : IBaseRequest
    {
    }
}
