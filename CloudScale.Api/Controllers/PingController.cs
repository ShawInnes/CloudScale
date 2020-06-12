using System;
using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloudScale.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PingController> _logger;

        public PingController(IMediator mediator, ILogger<PingController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PingResponse>> Ping([FromBody] PingRequest request) =>
            Ok(await _mediator.Send(request));
    }
}