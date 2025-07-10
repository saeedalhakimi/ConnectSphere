using Asp.Versioning;
using ConnectSphere.API.Application.Contracts.PersonDtos.Requests;
using ConnectSphere.API.Application.MediatoRs.Persons.Commands;
using ConnectSphere.API.Application.MediatoRs.Persons.Queries;
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
                _logger.LogError(null, "Failed to create person. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Person created successfully. CorrelationId: {CorrelationId}", correlationId);
            return CreatedAtRoute("GetPersonById", new { personId = result.Data.PersonId }, result);
        }

        [HttpGet(ApiRoutes.PersonRoutes.PersonById, Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateGuid("personId")]
        public async Task<IActionResult> GetPersonById([FromRoute] string personId, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Getting person with ID: {personId} and CorrelationId: {CorrelationId}", personId, correlationId);

            var query = new GetPersonByIDQuery(Guid.Parse(personId), correlationId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to get person by ID: {PersonId}. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    personId, string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Person retrieved successfully for ID: {PersonId}. CorrelationId: {CorrelationId}", personId, correlationId);
            return Ok(result);
        }

        [HttpGet(ApiRoutes.PersonRoutes.GetPersons,Name = "GetPersons")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersons([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Retrieving all persons with CorrelationId: {CorrelationId}", correlationId);
            var query = new GetAllPersonsQuery(pageNumber, pageSize, correlationId);
            var result = await _mediator.Send(query, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to retrieve persons. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Persons retrieved successfully. CorrelationId: {CorrelationId}", correlationId);
            return Ok(result);
        }

        [HttpPut(ApiRoutes.PersonRoutes.UpdatePerson, Name = "UpdatePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateGuid("personId")]
        [ValidateModel]
        public async Task<IActionResult> UpdatePerson([FromRoute] string personId, [FromBody] UpdatePersonDto dto, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Updating person with ID: {PersonId} and CorrelationId: {CorrelationId}", personId, correlationId);
            var command = new UpdateCommand(Guid.Parse(personId), dto.Title, dto.FirstName, dto.MiddleName, dto.LastName, dto.Suffix, correlationId);
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to update person with ID: {PersonId}. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    personId, string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Person with ID: {PersonId} updated successfully. CorrelationId: {CorrelationId}", personId, correlationId);
            return Ok(result);
        }

        [HttpDelete(ApiRoutes.PersonRoutes.DeletePerson, Name = "DeletePerson")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ValidateGuid("personId")]
        public async Task<IActionResult> DeletePerson([FromRoute] string personId, CancellationToken cancellationToken)
        {
            var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } });
            _logger.LogRequest(HttpContext.Request.Method, HttpContext.Request.Path, correlationId);
            _logger.LogInformation("Deleting person with ID: {PersonId} and CorrelationId: {CorrelationId}", personId, correlationId);
            var command = new DeletePersonCommand(Guid.Parse(personId), correlationId);
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess)
            {
                _logger.LogError(null, "Failed to delete person with ID: {PersonId}. Errors: {Errors}. CorrelationId: {CorrelationId}",
                    personId, string.Join(", ", result.Errors.Select(e => e.Message)), correlationId);
                return HandleResult(result, correlationId);
            }
            _logger.LogInformation("Person with ID: {PersonId} deleted successfully. CorrelationId: {CorrelationId}", personId, correlationId);
            return NoContent();
        }
    }
}
