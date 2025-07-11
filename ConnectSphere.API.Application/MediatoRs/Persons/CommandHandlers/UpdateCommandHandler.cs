using ConnectSphere.API.Application.MediatoRs.Persons.Commands;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.IRepositories;
using ConnectSphere.API.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.CommandHandlers
{
    public class UpdateCommandHandler : IRequestHandler<UpdateCommand, OperationResult<bool>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IAppLogger<UpdateCommandHandler> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly ISystemClocking _systemClocking;
        public UpdateCommandHandler(
            IPersonRepository personRepository,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger<UpdateCommandHandler> logger,
            ISystemClocking systemClocking,
            IErrorHandlingService errorHandlingService)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClocking = systemClocking ?? throw new ArgumentNullException(nameof(systemClocking));
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
        }

        public async Task<OperationResult<bool>> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("Starting handling UpdatePersonNameCommand process for PersonId: {PersonId}, NewName: {FirstName} {LastName}",
                request.PersonId, request.FirstName, request.LastName);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                // Fetch existing person
                var personResult = await _personRepository.GetByIdAsync(request.PersonId, request.CorrelationId, cancellationToken);
                if (!personResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve person for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", personResult.Errors.Select(e => e.Message)));
                    return OperationResult<bool>.Failure(personResult.Errors);
                }

                var nameResult = PersonName.Create(
                    request.FirstName,
                    request.MiddleName,
                    request.LastName,
                    request.Title,
                    request.Suffix);

                var person = personResult.Data;
                var updatedName = nameResult;

                var updatedingPerson = person!.UpdateName(updatedName, request.CorrelationId);
                if (!updatedingPerson.IsSuccess)
                {
                    _logger.LogWarning("Failed to update person name for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", updatedingPerson.Errors.Select(e => e.Message)));
                    return OperationResult<bool>.Failure(updatedingPerson.Errors);
                }

                var updatedPerson = updatedingPerson.Data!;

                _logger.LogInformation("Updating person name for PersonId: {PersonId}. New Name: {FirstName} {LastName}",
                    request.PersonId, request.FirstName, request.LastName);

                var updateResult = await _personRepository.UpdateAsync(updatedPerson, request.CorrelationId, cancellationToken);
                if (!updateResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to update person in repository for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonId, string.Join("; ", updateResult.Errors.Select(e => e.Message)));
                    return OperationResult<bool>.Failure(updateResult.Errors);
                }

                if (updatedPerson.DomainEvents.Any())
                {
                    _logger.LogInformation("Dispatching domain events for Person {PersonId}. CorrelationId: {CorrelationId}",
                        updatedPerson.PersonId, request.CorrelationId);
                    foreach (var domainEvent in updatedPerson.DomainEvents)
                    {
                        await _eventDispatcher.DispatchAsync(domainEvent, cancellationToken);
                    }
                }
                else
                {
                    _logger.LogDebug("No domain events to dispatch for Person {PersonId}. CorrelationId: {CorrelationId}",
                        updatedPerson.PersonId, request.CorrelationId);
                }

                // Clear domain events
                updatedPerson.ClearDomainEvents();

                _logger.LogInformation("Person name updated successfully for PersonId: {PersonId}. CorrelationId: {CorrelationId}",
                    request.PersonId, request.CorrelationId);
                return OperationResult<bool>.Success(true);

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("UpdatePersonNameCommand operation was canceled for PersonId: {PersonId}. Exception: {Exception}",
                    request.PersonId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<bool>(ex, request.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling UpdatePersonNameCommand for PersonId: {PersonId}. CorrelationId: {CorrelationId}",
                    request.PersonId, request.CorrelationId);
                return _errorHandlingService.HandleException<bool>(ex, request.CorrelationId);
            }
        }
    }
}
