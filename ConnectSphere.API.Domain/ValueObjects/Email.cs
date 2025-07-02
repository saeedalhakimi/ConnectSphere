using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    public record Email
    {
        private const int MaxLength = 100; // Maximum length for an email address
        public string Value { get; init; }

        private Email(string value) => Value = value;

        public static OperationResult<Email> Create(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return OperationResult<Email>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Email cannot be empty.");
            if (email.Length > MaxLength)
                return OperationResult<Email>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"Email cannot exceed {MaxLength} characters.");
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return OperationResult<Email>.Failure(ErrorCode.InvalidData, "INVALID_INPUT", "Invalid email format.");

            return OperationResult<Email>.Success(new Email(email));
        }
    }
}
