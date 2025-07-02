using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Application.MediatoRs.Persons.Commands;
using ConnectSphere.API.Domain.Aggregate;
using ConnectSphere.API.Domain.Common.Enums;
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
        private readonly IPersonRepository _personRepository;
        private readonly IMediator _mediator;
        public CreatePersonCommandHandler(IPersonRepository personRepository, IMediator mediator)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<OperationResult<PersonResponseDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Validate person type exists
                var personTypeResult = await _personRepository.GetPersonTypeByIdAsync(request.PersonTypeID, cancellationToken);
                if (!personTypeResult.IsSuccess)
                {
                    var errors = string.Join("; ", personTypeResult.Errors.Select(e => e.Message));// to use it for loggs later when impelemnted
                    return OperationResult<PersonResponseDto>.Failure(personTypeResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Create person name
                var nameResult = PersonName.Create(request.FirstName, request.MiddleName, request.LastName, request.Title, request.Suffix);
                if (!nameResult.IsSuccess)
                {
                    return OperationResult<PersonResponseDto>.Failure(
                        nameResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Create person
                var personResult = Person.Create(Guid.NewGuid(), nameResult.Data!, request.PersonTypeID);
                if (!personResult.IsSuccess)
                {
                    return OperationResult<PersonResponseDto>.Failure(
                        personResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Persist person
                var persistResult = await _personRepository.CreateAsync(personResult.Data!, request.CorrelationId, cancellationToken);
                if (!persistResult.IsSuccess)
                {
                    return OperationResult<PersonResponseDto>.Failure(
                        persistResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                // Publish domain events (e.g., PersonCreatedEvent) to notify other parts of the system
                // IMediator is used here to dispatch events to their handlers, decoupling the command handler
                foreach (var domainEvent in personResult.Data!.DomainEvents)
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                }

                // Clear domain events after publishing to prevent re-publishing
                personResult.Data.ClearDomainEvents();

                // Map to response DTO
                // Map to DTO
                var person = persistResult.Data;
                var responseDto = new PersonResponseDto(
                    person.PersonId,
                    person.Name.Title,
                    person.Name.FirstName,
                    person.Name.LastName,
                    person.Name.Suffix,
                    person.PersonTypeId,
                    person.CreatedAt,
                    person.UpdatedAt,
                    person.IsDeleted);

                return OperationResult<PersonResponseDto>.Success(responseDto);

            }
            catch (OperationCanceledException ex)
            {
                // Handle cancellation gracefully
                return OperationResult<PersonResponseDto>.Failure(new Error(ErrorCode.OperationCancelled, "Operation was cancelled.{ex.Message}", $"{ex.Source} - {ex.ToString()}.", request.CorrelationId));
            }
            catch (Exception ex)
            {
                return OperationResult<PersonResponseDto>.Failure(new Error(ErrorCode.InternalServerError, "An unexpected error occurred while creating the person.", $"{ex.Source} - {ex.ToString()}.", request.CorrelationId));
            }
        }
    }
}
