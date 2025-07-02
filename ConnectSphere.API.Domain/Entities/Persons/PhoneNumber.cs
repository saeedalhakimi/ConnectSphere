using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class PhoneNumber
    {
        public Guid PhoneNumberId { get; private set; }
        public Guid PersonId { get; private set; }
        public Guid PhoneNumberTypeId { get; private set; }
        public PhoneNumberValue Number { get; private set; }
        public Guid CountryId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private PhoneNumber(Guid phoneNumberId, Guid personId, Guid phoneNumberTypeId, PhoneNumberValue number, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            PhoneNumberId = phoneNumberId;
            PersonId = personId;
            PhoneNumberTypeId = phoneNumberTypeId;
            Number = number;
            CountryId = countryId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        public static OperationResult<PhoneNumber> Create(Guid phoneNumberId, Guid personId, Guid phoneNumberTypeId, PhoneNumberValue number, Guid countryId)
        {
            if (phoneNumberId == Guid.Empty)
                return OperationResult<PhoneNumber>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PhoneNumberId cannot be empty.");
            if (personId == Guid.Empty)
                return OperationResult<PhoneNumber>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (phoneNumberTypeId == Guid.Empty)
                return OperationResult<PhoneNumber>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PhoneNumberTypeId cannot be empty.");
            if (number == null)
                return OperationResult<PhoneNumber>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Phone number cannot be null.");
            if (countryId == Guid.Empty)
                return OperationResult<PhoneNumber>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "CountryId cannot be empty.");

            return OperationResult<PhoneNumber>.Success(new PhoneNumber(phoneNumberId, personId, phoneNumberTypeId, number, countryId, DateTime.UtcNow, null, false));
        }
    }
}
