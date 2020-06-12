using System.Threading;
using System.Threading.Tasks;
using CloudScale.ApiClient;
using CloudScale.Contracts.Recursive;
using MediatR;

namespace CloudScale.Business
{
    public class RecursiveHandler : IRequestHandler<RecursiveRequest, RecursiveResponse>
    {
        private readonly ICloudScaleClient _client;

        public RecursiveHandler(ICloudScaleClient client)
        {
            _client = client;
        }

        public Task<RecursiveResponse> Handle(RecursiveRequest request, CancellationToken cancellationToken)
        {
            // if (request.CurrentDepth < request.MaxDepth)
            // {
            //     return _client.GetRecursive(new RecursiveRequest
            //     {
            //         CurrentDepth = request.CurrentDepth + 1,
            //         MaxDepth = request.MaxDepth
            //     });
            // }

            return Task.FromResult(new RecursiveResponse
            {
                Message = $"Maxed it out @ {request.CurrentDepth}",
                CurrentDepth = request.CurrentDepth,
            });
        }
    }
}