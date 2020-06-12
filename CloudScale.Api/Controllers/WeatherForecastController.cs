using System;
using System.Threading.Tasks;
using CloudScale.Contracts.Weather;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CloudScale.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IMediator mediator, ILogger<WeatherForecastController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<WeatherResponse>> GetWeatherForecast() =>
            Ok(await _mediator.Send(new WeatherRequest()));
    }
}