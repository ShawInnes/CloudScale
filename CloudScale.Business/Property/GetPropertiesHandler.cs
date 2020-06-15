using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CloudScale.Contracts;
using CloudScale.Contracts.Property;
using CloudScale.Core;
using CloudScale.Data;
using CloudScale.Data.Repositories;
using MediatR;

namespace CloudScale.Business.Property
{
    public class GetPropertiesHandler : IRequestHandler<GetPropertiesRequest, PagedList<GetPropertiesResponse>>
    {
        private readonly ICloudScaleRepository _repository;

        public GetPropertiesHandler(ICloudScaleRepository repository)
        {
            _repository = repository;
        }

        public Task<PagedList<GetPropertiesResponse>> Handle(GetPropertiesRequest request,
            CancellationToken cancellationToken)
        {
            var properties = _repository.GetProperties(request);

            var result = new PagedList<GetPropertiesResponse>(properties.Select(p => new GetPropertiesResponse
                {
                    Id = p.Id
                }).ToList(),
                properties.TotalCount, properties.CurrentPage, properties.PageSize);

            return Task.FromResult(result);
        }
    }
}