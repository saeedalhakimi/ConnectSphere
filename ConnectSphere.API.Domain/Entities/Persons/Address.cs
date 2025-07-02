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
    public class Address
    {
        public Guid AddressId { get; private set; }
        public Guid PersonId { get; private set; }
        public Guid AddressTypeId { get; private set; }
        public AddressDetails Details { get; private set; }
        public Guid CountryId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Address(Guid addressId, Guid personId, Guid addressTypeId, AddressDetails details, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            AddressId = addressId;
            PersonId = personId;
            AddressTypeId = addressTypeId;
            Details = details;
            CountryId = countryId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        public static OperationResult<Address> Create(Guid addressId, Guid personId, Guid addressTypeId, AddressDetails details, Guid countryId)
        {
            if (addressId == Guid.Empty)
                return OperationResult<Address>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "AddressId cannot be empty.");
            if (personId == Guid.Empty)
                return OperationResult<Address>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (addressTypeId == Guid.Empty)
                return OperationResult<Address>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "AddressTypeId cannot be empty.");
            if (countryId == Guid.Empty)
                return OperationResult<Address>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "CountryId cannot be empty.");
            if (details == null)
                return OperationResult<Address>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Address details cannot be null.");

            return OperationResult<Address>.Success(new Address(addressId, personId, addressTypeId, details, countryId, DateTime.UtcNow, null, false));
        }
    }
}
