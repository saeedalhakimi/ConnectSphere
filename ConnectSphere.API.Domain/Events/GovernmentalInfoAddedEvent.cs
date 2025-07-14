namespace ConnectSphere.API.Domain.Events
{
    public record GovernmentalInfoAddedEvent(Guid PersonId, Guid GovernmentalInfoId, Guid CountryId, GovernmentalInfoDetails Details) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
