using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Application.MediatoRs.Persons.Commands;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Aggregate;
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
    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, OperationResult<PersonResponseDto>>
    {
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IDomainEventDispatcher _eventDispatcher;
        private readonly IAppLogger<CreatePersonCommandHandler> _logger;
        private readonly ISystemClocking _systemClocking;
        private readonly IPersonRepository _personRepository;

        public CreatePersonCommandHandler(
            IErrorHandlingService errorHandlingService,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger<CreatePersonCommandHandler> logger,
            ISystemClocking systemClocking,
            IPersonRepository personRepository)
        {
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClocking = systemClocking ?? throw new ArgumentNullException(nameof(systemClocking));
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        }
        public async Task<OperationResult<PersonResponseDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new System.Collections.Generic.Dictionary<string, object> { { "CorrelationId", request.CorrelationId } });
            _logger.LogInformation("Handling CreatePersonCommand for {FirstName} {LastName}. CorrelationId: {CorrelationId}",
                request.FirstName, request.LastName, request.CorrelationId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Reconstruct person name
                _logger.LogInformation("Creating PersonName. CorrelationId: {CorrelationId}", request.CorrelationId);
                var nameResult = PersonName.Create(request.FirstName, request.MiddleName, request.LastName, request.Title, request.Suffix);
               

                // Reconstruct person
                _logger.LogInformation("Creating Person entity. CorrelationId: {CorrelationId}", request.CorrelationId);
                var personResult = Person.Create(Guid.NewGuid(), nameResult!);
               

                // Persist person
                _logger.LogInformation("Persisting Person to repository. CorrelationId: {CorrelationId}", request.CorrelationId);
                var persistResult = await _personRepository.CreateAsync(personResult, request.CorrelationId, cancellationToken);
                if (!persistResult.IsSuccess)
                {
                    _logger.LogWarning("Person persistence failed: {Errors}. CorrelationId: {CorrelationId}",
                        string.Join("; ", persistResult.Errors.Select(e => e.Message)), request.CorrelationId);
                    return OperationResult<PersonResponseDto>.Failure(
                        persistResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Dispatch domain events
                var person = persistResult.Data!;
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

                // Clear domain events
                personResult.ClearDomainEvents();
                _logger.LogInformation("Cleared domain events for Person {PersonId}. CorrelationId: {CorrelationId}",
                    personResult.PersonId, request.CorrelationId);

                // Map to response DTO
                var responseDto = new PersonResponseDto(
                    person.PersonId,
                    person.Name.Title,
                    person.Name.FirstName,
                    person.Name.MiddleName,
                    person.Name.LastName,
                    person.Name.Suffix,
                    person.CreatedAt,
                    person.UpdatedAt,
                    person.IsDeleted);

                _logger.LogInformation("Person created successfully with ID {PersonId}. CorrelationId: {CorrelationId}",
                    person.PersonId, request.CorrelationId);
                return OperationResult<PersonResponseDto>.Success(responseDto);
            }
            catch (OperationCanceledException ex)
            {
                return _errorHandlingService.HandleCancelationToken<PersonResponseDto>(ex, request.CorrelationId);
            }
            catch (Exception ex)
            {
                return _errorHandlingService.HandleException<PersonResponseDto>(ex, request.CorrelationId);
            }
        }
    }
}
