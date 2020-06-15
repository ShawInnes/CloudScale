using System.Collections.Generic;
using System.Linq;
using CloudScale.Contracts;
using CloudScale.Contracts.Property;
using CloudScale.Core;
using CloudScale.Data.Entities;

namespace CloudScale.Data.Repositories
{
    public class CloudScaleRepository : ICloudScaleRepository
    {
        private readonly CloudScaleDbContext _dbContext;

        public CloudScaleRepository(CloudScaleDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedList<Property> GetProperties(GetPropertiesRequest request)
        {
            return PagedList<Property>.ToPagedList(_dbContext.Properties.OrderBy(p => p.Id), request.PageNumber,
                request.PageSize);
        }
    }
}