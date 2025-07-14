using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Services
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IAppLogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(IAppLogger<DomainEventDispatcher> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            if (domainEvent == null)
            {
                _logger.LogWarning("Attempted to dispatch null domain event.");
                return;
            }

            if (domainEvent is PersonCreatedEvent personCreatedEvent)
            {
                _logger.LogInformation("Handling PersonCreatedEvent for PersonId: {PersonId}, Name: {FirstName} {LastName}, CorrelationId: {CorrelationId}",
                    personCreatedEvent.PersonId, personCreatedEvent.Name.FirstName, personCreatedEvent.Name.LastName, personCreatedEvent.CorrelationId);
                // Add custom handling logic here (e.g., send email, queue message, etc.)
                await Task.CompletedTask; // Placeholder for async handling
            }
            else
            {
                _logger.LogDebug("Unhandled domain event type: {EventType}, EventId: {EventId}", domainEvent.GetType().Name, domainEvent.EventId);
            }
        }
    }
}
