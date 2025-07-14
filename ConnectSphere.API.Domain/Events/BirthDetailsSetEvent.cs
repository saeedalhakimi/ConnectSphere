namespace ConnectSphere.API.Domain.Events
{
    public record BirthDetailsSetEvent(Guid PersonId, Guid PersonBirthDetailsId, BirthDetails Details, Guid CountryId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
