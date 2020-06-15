using System.Threading;
using System.Threading.Tasks;
using CloudScale.ApiClient;
using CloudScale.Contracts.Recursive;
using CloudScale.Core;
using MediatR;

namespace CloudScale.Business.Recursive
{
    public class RecursiveHandler : IRequestHandler<RecursiveRequest, RecursiveResponse>
    {
        private readonly ICloudScaleClient _client;
        private readonly IBearerAccessor _bearerAccessor;

        public RecursiveHandler(ICloudScaleClient client, IBearerAccessor bearerAccessor)
        {
            _client = client;
            _bearerAccessor = bearerAccessor;
        }

        public Task<RecursiveResponse> Handle(RecursiveRequest request, CancellationToken cancellationToken)
        {
            if (request.CurrentDepth < request.MaxDepth)
            {
                return _client.GetRecursive(_bearerAccessor.Bearer, new RecursiveRequest
                {
                    CurrentDepth = request.CurrentDepth + 1,
                    MaxDepth = request.MaxDepth,
                });
            }

            return Task.FromResult(new RecursiveResponse
            {
                Message = $"Maxed it out @ {request.CurrentDepth}",
                CurrentDepth = request.CurrentDepth,
            });
        }
    }
}