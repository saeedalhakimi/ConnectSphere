using Asp.Versioning;
using ConnectSphere.API.Application.MediatoRs.Countries.Queries;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.RUSTApi.Controllers.V1.Person;
using ConnectSphere.API.RUSTApi.Filters;
using ConnectSphere.API.RUSTApi.Routes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConnectSphere.API.RUSTApi.Controllers.V1.Country
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.BaseRoute)]
    [ApiController]
    public class CountriesController : BaseController<CountriesController>
    {
        private readonly IMediator _mediator;
        private readonly IAppLogger<CountriesController> _logger;

        public CountriesController(IMediator mediator, IAppLogger<CountriesController> logger) : base(logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }

        [HttpGet(Name = "GetAllCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCountries(CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Retrieving all countries with correlation ID: {CorrelationId}", correlationId);
            var query = new GetAllCountriesQuery(correlationId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to retrieve countries. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Countries retrieved successfully. CorrelationId: {CorrelationId}", correlationId);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.CountryRoutes.GetByCountryID, Name = "GetByCountryID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateGuid("countryId")]
        public async Task<IActionResult> GetByCountryID([FromRoute] string countryId, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Retrieving country by ID: {CountryId} with correlation ID: {CorrelationId}", countryId, correlationId);

            var query = new GetCountryByIDQuery(Guid.Parse(countryId), correlationId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to retrieve country by ID: {CountryId}. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    countryId, string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Country retrieved successfully by ID: {CountryId}. CorrelationId: {CorrelationId}", countryId, correlationId);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.CountryRoutes.GetByCountryCode, Name = "GetByCountryCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateString("countryCode")]
        public async Task<IActionResult> GetByCountryCode([FromRoute] string countryCode, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Retrieving country by code: {CountryCode} with correlation ID: {CorrelationId}", countryCode, correlationId);

            var query = new GetCountryByCodeQuery(countryCode, correlationId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to retrieve country by code: {CountryCode}. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    countryCode, string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Country retrieved successfully by code: {CountryCode}. CorrelationId: {CorrelationId}", countryCode, correlationId);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.CountryRoutes.GetByCountryName, Name = "GetByCountryName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateString("name")]
        public async Task<IActionResult> GetByCountryName([FromRoute] string name, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Retrieving country by name: {Name} with correlation ID: {CorrelationId}", name, correlationId);

            var query = new GetCountryByNameQuery(name, correlationId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to retrieve country by name: {Name}. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    name, string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Country retrieved successfully by name: {Name}. CorrelationId: {CorrelationId}", name, correlationId);
            return Ok(result);
        }
    }
}
