namespace ConnectSphere.API.Domain.Aggregate
{
    public class Person
    {
        public Guid PersonId { get; private set; }
        public PersonName Name { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        private readonly List<Address> _addresses = new();
        private readonly List<PhoneNumber> _phoneNumbers = new();
        private readonly List<EmailAddress> _emailAddresses = new();
        private readonly List<GovernmentalInfo> _governmentalInfos = new();
        private PersonBirthDetails? _birthDetails;
        private readonly List<IDomainEvent> _domainEvents = new();

        public IReadOnlyList<Address> Addresses => _addresses.AsReadOnly();
        public IReadOnlyList<PhoneNumber> PhoneNumbers => _phoneNumbers.AsReadOnly();
        public IReadOnlyList<EmailAddress> EmailAddresses => _emailAddresses.AsReadOnly();
        public IReadOnlyList<GovernmentalInfo> GovernmentalInfos => _governmentalInfos.AsReadOnly();
        public PersonBirthDetails? BirthDetails => _birthDetails;
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private Person(Guid personId, PersonName name, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            PersonId = personId;
            Name = name;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }
        public static Person Create(Guid personId, PersonName name, string? correlationId = null)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new PersonNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfObjectNull("PersonName", name, new PersonNotValidException("Person Name cannot be null."));

            var person = new Person(personId, name, DateTime.UtcNow, null, false);
            person._domainEvents.Add(new Events.PersonCreatedEvent(personId, name, correlationId));
            return person;
        }
        public static Person Reconstruct(Guid personId, PersonName name, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new PersonNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfObjectNull("PersonName", name, new PersonNotValidException("Person Name cannot be null."));

            var person = new Person(personId, name, createdAt, updatedAt, isDeleted);
            return person;
        }
        public void UpdateName(PersonName newName, string? correlationId = null)
        {
            DomainValidator.ThrowIfObjectNull("PersonName", newName, new PersonNotValidException("Person new name cannot be null."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new PersonNotValidException("Operation not allowed: the person has been deleted."));

            Name = newName;
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.PersonNameUpdatedEvent(PersonId, newName, correlationId));
        }
        public void AddGovernmentalInfo(GovernmentalInfo info)
        {
            DomainValidator.ThrowIfDeleted(IsDeleted, new PersonNotValidException("Operation not allowed: the person has been deleted."));
            DomainValidator.ThrowIfMismatch("GovernmentalInfo", info, PersonId, new GovernmentalInfoNotValidException("Governmental info belongs to a different person."));
            DomainValidator.ThrowIfDuplicate("Governmental info", info.GovernmentalInfoId, _governmentalInfos, g => g.GovernmentalInfoId, new GovernmentalInfoNotValidException("Governmental info already exists."));

            _governmentalInfos.Add(info);
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new Events.GovernmentalInfoAddedEvent(
                PersonId, info.GovernmentalInfoId, info.CountryId, info.Details));
        }
        public void AddAddress(Address address)
        {
            DomainValidator.ThrowIfDeleted(IsDeleted, new PersonNotValidException("Operation not allowed: the person has been deleted."));
            DomainValidator.ThrowIfDuplicate("Address", address.AddressId, _addresses, a => a.AddressId, new PersonNotValidException("Address already exists."));
            DomainValidator.ThrowIfMismatch("Address", address, PersonId, new PersonNotValidException("Address belongs to a different person."));

            _addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new Events.AddressAddedEvent(
                PersonId, address.AddressId, address.AddressTypeId, address.Details, address.CountryId));
        }
        public void AddPhoneNumber(PhoneNumber phoneNumber)
        {
            DomainValidator.ThrowIfDeleted(IsDeleted, new PersonNotValidException("Operation not allowed: the person has been deleted."));
            DomainValidator.ThrowIfDuplicate("Phone number", phoneNumber.PhoneNumberId, _phoneNumbers, p => p.PhoneNumberId, new PersonNotValidException("Phone number already exists."));
            DomainValidator.ThrowIfMismatch("PhoneNumber", phoneNumber, PersonId, new PersonNotValidException("Phone number belongs to a different person."));
           
            _phoneNumbers.Add(phoneNumber);
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new Events.PhoneNumberAddedEvent(
                PersonId, phoneNumber.PhoneNumberId, phoneNumber.PhoneNumberTypeId, phoneNumber.Number, phoneNumber.CountryId));
        }
        public void AddEmailAddress(EmailAddress emailAddress)
        {
            DomainValidator.ThrowIfDeleted(IsDeleted, new PersonNotValidException("Operation not allowed: the person has been deleted."));
            DomainValidator.ThrowIfDuplicate("Email address", emailAddress.EmailAddressId, _emailAddresses, e => e.EmailAddressId, new PersonNotValidException("Email address already exists."));
            DomainValidator.ThrowIfMismatch("EmailAddress", emailAddress, PersonId, new PersonNotValidException("Email address belongs to a different person."));
            
            _emailAddresses.Add(emailAddress);
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new Events.EmailAddressAddedEvent(
                PersonId, emailAddress.EmailAddressId, emailAddress.EmailAddressTypeId, emailAddress.Email));
        }
        public void SetBirthDetails(PersonBirthDetails birthDetails)
        {
            DomainValidator.ThrowIfDeleted(IsDeleted, new PersonNotValidException("Operation not allowed: the person has been deleted."));
            DomainValidator.ThrowIfMismatch("BirthDetails", birthDetails, PersonId, new PersonNotValidException("Birth details belong to a different person."));
            
            if (_birthDetails != null)
                throw new PersonNotValidException("Birth details already set.");

            _birthDetails = birthDetails;
            UpdatedAt = DateTime.UtcNow;

            _domainEvents.Add(new Events.BirthDetailsSetEvent(
                PersonId, birthDetails.PersonBirthDetailsId, birthDetails.Details, birthDetails.CountryId));
        }
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
