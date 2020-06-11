using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudScale.Api.Handlers.Ping;
using CloudScale.Api.Handlers.Weather;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PingResponse>> Ping([FromBody] PingRequest request) =>
            Ok(await _mediator.Send(request));
    }
}