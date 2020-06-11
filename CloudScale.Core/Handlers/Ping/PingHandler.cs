using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace CloudScale.Core.Handlers.Ping
{
    public class PingHandler : IRequestHandler<PingRequest, PingResponse>
    {
        public Task<PingResponse> Handle(PingRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new PingResponse
            {
                Message = request.Message
            });
        }
    }
}