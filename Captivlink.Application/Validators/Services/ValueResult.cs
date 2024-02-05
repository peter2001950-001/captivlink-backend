using FluentValidation.Results;

namespace Captivlink.Application.Validators.Services
{
    public class ValueResult<T> : ValidationResult
    {
        public ValueResult(T value)
        {
            Value = value;
        }

        public ValueResult(IEnumerable<ValidationFailure> failures) : base(failures)
        {
        }

        public ValueResult(params ValidationFailure[] failures) : base(failures)
        {
        }

        public T Value { get; }
    }

    public class ValueResult : ValidationResult
    {
        public ValueResult()
        {

        }

        public ValueResult(IEnumerable<ValidationFailure> failures) : base(failures)
        {

        }
        public ValueResult(params ValidationFailure[] failures) : base(failures)
        {
        }
    }
}
