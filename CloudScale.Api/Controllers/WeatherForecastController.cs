using System;
using System.Threading.Tasks;
using CloudScale.Contracts.Weather;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloudScale.Api.Controllers
{
    [ApiController]
    // [Authorize]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<WeatherResponse>> GetWeatherForecast() =>
            Ok(await _mediator.Send(new WeatherRequest()));
    }
}