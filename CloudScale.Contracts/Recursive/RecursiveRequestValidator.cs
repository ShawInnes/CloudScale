using FluentValidation;

namespace CloudScale.Contracts.Recursive
{
    public class RecursiveRequestValidator : AbstractValidator<RecursiveRequest>
    {
        public RecursiveRequestValidator()
        {
            RuleFor(p => p.CurrentDepth)
                .GreaterThan(0)
                .LessThanOrEqualTo(10);
        }
    }
}