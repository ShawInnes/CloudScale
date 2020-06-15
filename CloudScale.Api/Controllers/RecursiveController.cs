using System;
using System.Threading.Tasks;
using CloudScale.Contracts.Recursive;
using CloudScale.Contracts.Weather;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudScale.Api.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("[controller]")]
    public class RecursiveController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecursiveController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<WeatherResponse>> GetRecursive(RecursiveRequest request) =>
            Ok(await _mediator.Send(request));
    }
}