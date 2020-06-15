using FluentValidation;

namespace CloudScale.Contracts.Ping
{
    public class PingValidator : AbstractValidator<PingRequest>
    {
        public PingValidator()
        {
            RuleFor(m => m.Message)
                .NotEmpty();
        }
    }
}