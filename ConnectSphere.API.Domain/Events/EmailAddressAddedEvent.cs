namespace ConnectSphere.API.Domain.Events
{
    public record EmailAddressAddedEvent(Guid PersonId, Guid EmailAddressId, Guid EmailAddressTypeId, Email Email) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
