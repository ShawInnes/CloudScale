using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudScale.Contracts;
using CloudScale.Contracts.Property;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CloudScale.Api.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PropertyController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<GetPropertiesResponse>),StatusCodes.Status200OK)]
        public async Task<ActionResult<List<GetPropertiesResponse>>>
            GetProperties([FromQuery] GetPropertiesRequest request) =>
            Ok(await _mediator.Send(request));
    }
}