namespace ConnectSphere.API.Domain.Events
{
    public record AddressAddedEvent(Guid PersonId, Guid AddressId, Guid AddressTypeId, AddressDetails Details, Guid CountryId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
