namespace ConnectSphere.API.Domain.Events
{
    public record PersonCreatedEvent(Guid PersonId, PersonName Name, string? CorrelationId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
