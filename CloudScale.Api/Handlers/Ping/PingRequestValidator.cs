using FluentValidation;

namespace CloudScale.Api.Handlers.Ping
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