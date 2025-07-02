using ConnectSphere.API.Common.Events;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Events
{
    public record AddressAddedEvent(Guid PersonId, Guid AddressId, Guid AddressTypeId, AddressDetails Details, Guid CountryId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
