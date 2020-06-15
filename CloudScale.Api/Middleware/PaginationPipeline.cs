using System;
using System.Threading;
using System.Threading.Tasks;
using CloudScale.Contracts;
using CloudScale.Core;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

namespace CloudScale.Api.Middleware
{
    public class PaginationPipeline<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _accessor;

        public PaginationPipeline(IHttpContextAccessor accessor)
        {
            _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (response is IPagedList pagedResponse)
            {
                _accessor.HttpContext.Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(new
                {
                    pagedResponse.TotalCount,
                    pagedResponse.PageSize,
                    pagedResponse.CurrentPage,
                    pagedResponse.TotalPages,
                    pagedResponse.HasNext,
                    pagedResponse.HasPrevious
                }));
            }
        }
    }
}