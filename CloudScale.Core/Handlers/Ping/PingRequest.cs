using System.ComponentModel.DataAnnotations;
using MediatR;

namespace CloudScale.Core.Handlers.Ping
{
    public class PingRequest : IRequest<PingResponse>
    {
        [Required]
        public string Message { get; set; }
    }
}