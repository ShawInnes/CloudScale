using FluentValidation;

namespace CloudScale.Contracts.Ping
{
    public class PingRequestValidator : AbstractValidator<PingRequest>
    {
        public PingRequestValidator()
        {
            RuleFor(m => m.Message)
                .NotEmpty();
        }
    }
}