using ConnectSphere.API.Common.Events;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Events
{
    public record GovernmentalInfoAddedEvent(Guid PersonId, Guid GovernmentalInfoId, Guid CountryId, GovernmentalInfoDetails Details) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
