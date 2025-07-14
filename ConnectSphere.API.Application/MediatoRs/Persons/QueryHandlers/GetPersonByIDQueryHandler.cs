using ConnectSphere.API.Application.Contracts.PersonDtos;
using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Application.DomainRepositories.IRepositories;
using ConnectSphere.API.Application.MediatoRs.Persons.CommandHandlers;
using ConnectSphere.API.Application.MediatoRs.Persons.Queries;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.QueryHandlers
{
    public class GetPersonByIDQueryHandler : IRequestHandler<GetPersonByIDQuery, OperationResult<PersonResponseDto>>
    {
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IAppLogger<CreatePersonCommandHandler> _logger;
        private readonly IPersonRepository _personRepository;
        public GetPersonByIDQueryHandler(
            IErrorHandlingService errorHandlingService,
            IAppLogger<CreatePersonCommandHandler> logger,
            IPersonRepository personRepository)
        {
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        }
        public async Task<OperationResult<PersonResponseDto>> Handle(GetPersonByIDQuery request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("handling GetPersonByIDQuery for PersonId: {PersonId}", request.PersonId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                var personResult = await _personRepository.GetByIdAsync(request.PersonId, request.CorrelationId, cancellationToken);
                if (!personResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to fetch person for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", personResult.Errors.Select(e => e.Message)));
                    return OperationResult<PersonResponseDto>.Failure(
                         personResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                var person = personResult.Data!;
                var response = PersonMappers.ToResponseDto(person);

                _logger.LogDebug("Person fetched successfully for PersonId: {PersonId}. Response: {@Response}",
                    request.PersonId, response);

                _logger.LogInformation("Query handled successfully for PersonId: {PersonId}", request.PersonId);
                return OperationResult<PersonResponseDto>.Success(response);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person fetch operation was canceled for PersonId: {PersonId}. Exception: {Exception}",
                    request.PersonId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<PersonResponseDto>(ex, request.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching person for PersonId: {PersonId}", request.PersonId);
                return _errorHandlingService.HandleException<PersonResponseDto>(ex, request.CorrelationId);
            }
        }
    }
}
