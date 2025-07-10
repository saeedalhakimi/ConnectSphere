using ConnectSphere.API.Common.Events;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static OperationResult<Person> Create(Guid personId, PersonName name, string? correlationId = null)
        {
            if (personId == Guid.Empty)
                return OperationResult<Person>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (name == null)
                return OperationResult<Person>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Name cannot be null.");
           
            var person = new Person(personId, name, DateTime.UtcNow, null, false);
            person._domainEvents.Add(new Events.PersonCreatedEvent(personId, name, correlationId));
            return OperationResult<Person>.Success(person);
        }

        public static OperationResult<Person> Create(Guid personId, PersonName name, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            if (personId == Guid.Empty)
                return OperationResult<Person>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (name == null)
                return OperationResult<Person>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Name cannot be null.");

            var person = new Person(personId, name, createdAt, updatedAt, isDeleted);
            return OperationResult<Person>.Success(person);
        }

        public OperationResult<Person> UpdateName(PersonName newName, string? correlationId = null)
        {
            if (newName == null)
                return OperationResult<Person>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "New name cannot be null.", correlationId);
            if (IsDeleted)
                return OperationResult<Person>.Failure(ErrorCode.InvalidOperation, "INVALID_OPERATION", "Cannot update a deleted person.", correlationId);

            Name = newName;
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.PersonNameUpdatedEvent(PersonId, newName, correlationId));
            return OperationResult<Person>.Success(this);
        }
        public OperationResult<Address> AddAddress(Address address)
        {
            if (address == null || address.PersonId != PersonId)
                return OperationResult<Address>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Invalid address or person mismatch.");
            if (_addresses.Any(a => a.AddressId == address.AddressId))
                return OperationResult<Address>.Failure(ErrorCode.ConflictError, "CONFLICT_ERROR", "Address already exists.");

            _addresses.Add(address);
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.AddressAddedEvent(PersonId, address.AddressId, address.AddressTypeId, address.Details, address.CountryId));
            return OperationResult<Address>.Success(address);
        }

        public OperationResult<PhoneNumber> AddPhoneNumber(PhoneNumber phoneNumber)
        {
            if (phoneNumber == null || phoneNumber.PersonId != PersonId)
                return OperationResult<PhoneNumber>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Invalid phone number or person mismatch.");
            if (_phoneNumbers.Any(p => p.PhoneNumberId == phoneNumber.PhoneNumberId))
                return OperationResult<PhoneNumber>.Failure(ErrorCode.ConflictError, "CONFLICT_ERROR", "Phone number already exists.");

            _phoneNumbers.Add(phoneNumber);
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.PhoneNumberAddedEvent(PersonId, phoneNumber.PhoneNumberId, phoneNumber.PhoneNumberTypeId, phoneNumber.Number, phoneNumber.CountryId));
            return OperationResult<PhoneNumber>.Success(phoneNumber);
        }

        public OperationResult<EmailAddress> AddEmailAddress(EmailAddress emailAddress)
        {
            if (emailAddress == null || emailAddress.PersonId != PersonId)
                return OperationResult<EmailAddress>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Invalid email address or person mismatch.");
            if (_emailAddresses.Any(e => e.EmailAddressId == emailAddress.EmailAddressId))
                return OperationResult<EmailAddress>.Failure(ErrorCode.ConflictError, "CONFLICT_ERROR", "Email address already exists.");

            _emailAddresses.Add(emailAddress);
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.EmailAddressAddedEvent(PersonId, emailAddress.EmailAddressId, emailAddress.EmailAddressTypeId, emailAddress.Email));
            return OperationResult<EmailAddress>.Success(emailAddress);
        }

        public OperationResult<GovernmentalInfo> AddGovernmentalInfo(GovernmentalInfo governmentalInfo)
        {
            if (governmentalInfo == null || governmentalInfo.PersonId != PersonId)
                return OperationResult<GovernmentalInfo>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Invalid governmental info or person mismatch.");
            if (_governmentalInfos.Any(g => g.GovernmentalInfoId == governmentalInfo.GovernmentalInfoId))
                return OperationResult<GovernmentalInfo>.Failure(ErrorCode.ConflictError, "CONFLICT_ERROR", "Governmental info already exists.");

            _governmentalInfos.Add(governmentalInfo);
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.GovernmentalInfoAddedEvent(PersonId, governmentalInfo.GovernmentalInfoId, governmentalInfo.CountryId, governmentalInfo.Details));
            return OperationResult<GovernmentalInfo>.Success(governmentalInfo);
        }

        public OperationResult<PersonBirthDetails> SetBirthDetails(PersonBirthDetails birthDetails)
        {
            if (birthDetails == null || birthDetails.PersonId != PersonId)
                return OperationResult<PersonBirthDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Invalid birth details or person mismatch.");
            if (_birthDetails != null)
                return OperationResult<PersonBirthDetails>.Failure(ErrorCode.ConflictError, "CONFLICT_ERROR", "Birth details already set.");

            _birthDetails = birthDetails;
            UpdatedAt = DateTime.UtcNow;
            _domainEvents.Add(new Events.BirthDetailsSetEvent(PersonId, birthDetails.PersonBirthDetailsId, birthDetails.Details, birthDetails.CountryId));
            return OperationResult<PersonBirthDetails>.Success(birthDetails);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
