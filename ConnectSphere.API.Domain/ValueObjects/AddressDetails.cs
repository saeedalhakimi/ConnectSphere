using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    public record AddressDetails
    {
        private const int MaxLength = 100; // Maximum length for address lines
        public string AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string City { get; init; }
        public string? PostalCode { get; init; }

        private AddressDetails(string addressLine1, string? addressLine2, string city, string? postalCode)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            PostalCode = postalCode;
        }

        public static OperationResult<AddressDetails> Create(string addressLine1, string? addressLine2, string city, string? postalCode)
        {
            if (string.IsNullOrWhiteSpace(addressLine1))
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "AddressLine1 cannot be empty.");
            if (addressLine1.Length > MaxLength)
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT",  $"AddressLine1 cannot exceed {MaxLength} characters.");
            if (addressLine2 != null && addressLine2.Length > MaxLength)
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"AddressLine2 cannot exceed {MaxLength} characters.");
            if (string.IsNullOrWhiteSpace(city))
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "City cannot be empty.");
            if (city.Length > MaxLength)
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"City cannot exceed {MaxLength} characters.");
            if (postalCode != null && string.IsNullOrWhiteSpace(postalCode))
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PostalCode cannot be empty if provided.");
            if (postalCode != null && postalCode.Length > 20)
                return OperationResult<AddressDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"PostalCode cannot exceed {20} characters.");
            return OperationResult<AddressDetails>.Success(new AddressDetails(addressLine1, addressLine2, city, postalCode));
        }
    }
}
