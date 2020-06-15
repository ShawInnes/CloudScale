using System.Text.Json.Serialization;
using MediatR;

namespace CloudScale.Contracts.Property
{
    public class GetPropertiesRequest : PagedRequest, IRequest<PagedList<GetPropertiesResponse>>
    {
        [JsonPropertyName("suburb")]
        public string Suburb { get; set; }
    }
}