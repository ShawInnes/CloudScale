using CloudScale.Contracts;
using CloudScale.Contracts.Property;
using CloudScale.Data.Entities;

namespace CloudScale.Data.Repositories
{
    public interface ICloudScaleRepository
    {
        PagedList<Property> GetProperties(GetPropertiesRequest request);
    }
}