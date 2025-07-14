namespace ConnectSphere.API.Domain.Events
{
    public record PhoneNumberAddedEvent(Guid PersonId, Guid PhoneNumberId, Guid PhoneNumberTypeId, PhoneNumberValue Number, Guid CountryId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
