using Asp.Versioning;
using ConnectSphere.API.Application.Contracts.PersonDtos.Requests;
using ConnectSphere.API.Application.MediatoRs.Persons.Commands;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.RUSTApi.Filters;
using ConnectSphere.API.RUSTApi.Routes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConnectSphere.API.RUSTApi.Controllers.V1.Person
{
    [ApiVersion("1.0")]
    [Route(ApiRoutes.BaseRoute)]
    [ApiController]
    public class PersonsController : BaseController<PersonsController>
    {
        private readonly IMediator _mediator;
        private readonly IAppLogger<PersonsController> _logger;
        private readonly ISystemClocking _systemClocks;
        public PersonsController(IAppLogger<PersonsController> logger, IMediator mediator, ISystemClocking systemClocking) : base(logger)
        {
            _logger = logger;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _systemClocks = systemClocking ?? throw new ArgumentNullException(nameof(systemClocking));
        }

        [HttpPost(ApiRoutes.PersonRoutes.CreatePerson, Name = "CreatePerson")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateModel]
        public async Task<IActionResult> CreatePerson([FromBody] CreatePersonDto dto, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Creating person with correlation ID: {CorrelationId}", correlationId);
            var command = new CreatePersonCommand(dto.Title, dto.FirstName, dto.MiddleName, dto.LastName, dto.Suffix, correlationId);
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null,"Failed to create person. Errors: {Errors}. CorrelationId: {CorrelationId}", 
                    string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Person created successfully. CorrelationId: {CorrelationId}", correlationId);
            return CreatedAtRoute("GetPersonById", new { personId = result.Data.PersonId }, result);
        }

        [HttpGet("{personId}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetPersonById(Guid personId)
        {
            // Placeholder: To be implemented later
            _logger.LogInformation("GetPersonById called for PersonId: {PersonId}", personId);
            return NotFound();
        }

    }
}
