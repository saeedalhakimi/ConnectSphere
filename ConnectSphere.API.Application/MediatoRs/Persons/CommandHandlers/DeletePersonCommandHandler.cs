using ConnectSphere.API.Application.DomainRepositories.IRepositories;
using ConnectSphere.API.Application.MediatoRs.Persons.Commands;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.CommandHandlers
{
    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, OperationResult<bool>>
    {
        private readonly IPersonRepository _personRepository;
        private readonly IAppLogger<DeletePersonCommandHandler> _logger;
        private readonly IErrorHandlingService _errorHandlingService;

        public DeletePersonCommandHandler(
            IPersonRepository personRepository,
            IDomainEventDispatcher eventDispatcher,
            IAppLogger<DeletePersonCommandHandler> logger,
            ISystemClocking systemClocking,
            IErrorHandlingService errorHandlingService)
        {
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
        }
        public async Task<OperationResult<bool>> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("Starting handling DeletePersonCommand for PersonId: {PersonId}", request.PersonID);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                var deleteResult = await _personRepository.DeleteAsync(request.PersonID, request.CorrelationId, cancellationToken);
                if (!deleteResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to delete person for PersonId: {PersonId}. Errors: {Errors}",
                        request.PersonID, string.Join("; ", deleteResult.Errors.Select(e => e.Message)));
                    return OperationResult<bool>.Failure(deleteResult.Errors);
                }

                _logger.LogInformation("Person deleted successfully for PersonId: {PersonId}", request.PersonID);
                return OperationResult<bool>.Success(true);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person deletion operation was canceled for PersonId: {PersonId}. Exception: {Exception}",
                    request.PersonID, ex.Message);
                return _errorHandlingService.HandleCancelationToken<bool>(ex, request.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting person for PersonId: {PersonId}", request.PersonID);
                return _errorHandlingService.HandleException<bool>(ex, request.CorrelationId);
            }
        }
    }
}
