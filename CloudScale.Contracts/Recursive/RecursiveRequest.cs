using MediatR;

namespace CloudScale.Contracts.Recursive
{
    public class RecursiveRequest : IRequest<RecursiveResponse>
    {
        public int MaxDepth { get; set; } = 3;
        public int CurrentDepth { get; set; } = 0;
    }
}