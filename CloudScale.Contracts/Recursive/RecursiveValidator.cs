using FluentValidation;

namespace CloudScale.Contracts.Recursive
{
    public class RecursiveValidator : AbstractValidator<RecursiveRequest>
    {
        public RecursiveValidator()
        {
            RuleFor(p => p.CurrentDepth)
                .GreaterThan(0)
                .LessThanOrEqualTo(10);
        }
    }
}