using FluentValidation;

namespace CloudScale.Core.Handlers.Ping
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