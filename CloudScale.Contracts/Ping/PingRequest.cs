using System.ComponentModel.DataAnnotations;
using MediatR;

namespace CloudScale.Contracts.Ping
{
    public class PingRequest : IRequest<PingResponse>
    {
        [Required]
        public string Message { get; set; }
    }
}