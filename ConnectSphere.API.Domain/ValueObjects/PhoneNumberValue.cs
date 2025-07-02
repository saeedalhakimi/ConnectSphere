using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    public record PhoneNumberValue
    {
        private const int MaxLength = 25; // Maximum length for a phone number
        public string Number { get; init; }

        private PhoneNumberValue(string number) => Number = number;

        public static OperationResult<PhoneNumberValue> Create(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return OperationResult<PhoneNumberValue>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Phone number cannot be empty.");
            if (number.Length > MaxLength)
                return OperationResult<PhoneNumberValue>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"Phone number cannot exceed {MaxLength} characters.");
            return OperationResult<PhoneNumberValue>.Success(new PhoneNumberValue(number));
        }
    }
}
