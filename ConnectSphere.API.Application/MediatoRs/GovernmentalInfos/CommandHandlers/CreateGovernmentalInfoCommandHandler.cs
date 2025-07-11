using ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos;
using ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos.Responses;
using ConnectSphere.API.Application.MediatoRs.GovernmentalInfos.Commands;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.IRepositories;
using ConnectSphere.API.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.GovernmentalInfos.CommandHandlers
{
    public class CreateGovernmentalInfoCommandHandler : IRequestHandler<CreateGovernmentalInfoCommand, OperationResult<GovernmentalInfoResponseDto>>
    {
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IAppLogger<CreateGovernmentalInfoCommandHandler> _logger;
        private readonly IGovernmentalInfoRepository _governmentalInfoRepository;
        private readonly IPersonRepository _personRepository;

        public CreateGovernmentalInfoCommandHandler(
            IErrorHandlingService errorHandlingService,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger<CreateGovernmentalInfoCommandHandler> logger,
            IGovernmentalInfoRepository governmentalInfoRepository,
            IPersonRepository personRepository)
        {
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _governmentalInfoRepository = governmentalInfoRepository ?? throw new ArgumentNullException(nameof(governmentalInfoRepository));
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        }
        public async Task<OperationResult<GovernmentalInfoResponseDto>> Handle(CreateGovernmentalInfoCommand request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("Handling CreateGovernmentalInfoCommand for PersonId: {PersonId}, CountryId: {CountryId}, CorrelationId: {CorrelationId}",
                request.PersonId, request.CountryId, request.CorrelationId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                // Validate Person exists
                var personResult = await _personRepository.GetByIdAsync(request.PersonId, request.CorrelationId, cancellationToken);
                if (!personResult.IsSuccess)
                {
                    _logger.LogWarning("Person not found for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", personResult.Errors.Select(e => e.Message)));
                    return OperationResult<GovernmentalInfoResponseDto>.Failure(
                        personResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }
                var person = personResult.Data;

                // Create GovernmentalInfoDetails
                var detailsResult = GovernmentalInfoDetails.Create(request.GovIdNumber, request.PassportNumber);
                if (!detailsResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create GovernmentalInfoDetails for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", detailsResult.Errors.Select(e => e.Message)));
                    return OperationResult<GovernmentalInfoResponseDto>.Failure(
                        detailsResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Create GovernmentalInfo
                var governmentalInfoResult = GovernmentalInfo.Create(Guid.NewGuid(), request.PersonId, request.CountryId, detailsResult.Data!);
                if (!governmentalInfoResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to create GovernmentalInfo for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", governmentalInfoResult.Errors.Select(e => e.Message)));
                    return OperationResult<GovernmentalInfoResponseDto>.Failure(
                        governmentalInfoResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                var governmentalInfo = governmentalInfoResult.Data!;

                var addResult = person.AddGovernmentalInfo(governmentalInfo);
                if (!addResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to add GovernmentalInfo to Person for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", addResult.Errors.Select(e => e.Message)));
                    return OperationResult<GovernmentalInfoResponseDto>.Failure(
                        addResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Dispatch domain events
                if (person.DomainEvents.Any())
                {
                    _logger.LogInformation("Dispatching domain events for Person {PersonId}. CorrelationId: {CorrelationId}",
                        person.PersonId, request.CorrelationId);
                    foreach (var domainEvent in person.DomainEvents)
                    {
                        await _eventDispatcher.DispatchAsync(domainEvent, cancellationToken);
                    }
                }
                else
                {
                    _logger.LogDebug("No domain events to dispatch for Person {PersonId}. CorrelationId: {CorrelationId}",
                        person.PersonId, request.CorrelationId);
                }
                person.ClearDomainEvents();

                // persist to database
                var createResult = await _governmentalInfoRepository.CreateAsync(governmentalInfo, request.CorrelationId, cancellationToken);
                if (!createResult.IsSuccess)
                {
                    _logger.LogError(null,"Failed to create GovernmentalInfo in repository for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", createResult.Errors.Select(e => e.Message)));
                    return OperationResult<GovernmentalInfoResponseDto>.Failure(
                        createResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                var response = GovernmentalInfoMappers.ToResponseDto(governmentalInfo);
                _logger.LogInformation("GovernmentalInfo created successfully for GovernmentalInfoId: {GovernmentalInfoId}", governmentalInfo.GovernmentalInfoId);
                return OperationResult<GovernmentalInfoResponseDto>.Success(response);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Operation was cancelled while handling CreateGovernmentalInfoCommand for PersonId: {PersonId}, CorrelationId: {CorrelationId}",
                    request.PersonId, request.CorrelationId);
                return OperationResult<GovernmentalInfoResponseDto>.Failure(new Error(
                    ErrorCode.OperationCancelled,
                    "OPERATION_CANCELLED",
                    "The operation was cancelled.",
                    request.CorrelationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling CreateGovernmentalInfoCommand for PersonId: {PersonId}, CorrelationId: {CorrelationId}",
                    request.PersonId, request.CorrelationId);
                return OperationResult<GovernmentalInfoResponseDto>.Failure(new Error(
                    ErrorCode.InternalServerError,
                    "INTERNAL_SERVER_ERROR",
                    "An unexpected error occurred while processing the request.",
                    request.CorrelationId));
            }
        }
    }
}
