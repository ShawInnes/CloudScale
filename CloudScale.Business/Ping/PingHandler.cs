using System.Threading;
using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
using MediatR;

namespace CloudScale.Business.Ping
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