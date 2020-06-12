using System;
using System.Threading.Tasks;
using CloudScale.Contracts.Ping;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloudScale.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PingController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PingResponse>> Ping([FromBody] PingRequest request) =>
            Ok(await _mediator.Send(request));
    }
}