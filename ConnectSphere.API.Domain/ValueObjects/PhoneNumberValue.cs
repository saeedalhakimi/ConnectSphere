using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    /// <summary>
    /// Represents a validated phone number value object.
    /// </summary>
    /// <remarks>
    /// Encapsulates a phone number ensuring it meets basic validation rules such as being non-empty and within a defined length limit.
    /// </remarks>
    public record PhoneNumberValue
    {
        private const int MaxLength = 25; // Maximum length for a phone number

        /// <summary>
        /// Gets the validated phone number string.
        /// </summary>
        public string Number { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneNumberValue"/> class.
        /// </summary>
        /// <param name="number">The validated phone number.</param>
        private PhoneNumberValue(string number) => Number = number;

        /// <summary>
        /// Creates a new instance of <see cref="PhoneNumberValue"/> after validating the input.
        /// </summary>
        /// <remarks>
        /// Validates the provided phone number to ensure it is not null, not empty, and does not exceed the maximum allowed length.
        /// </remarks>
        /// <param name="number">The phone number string to validate and encapsulate.</param>
        /// <returns>A new validated <see cref="PhoneNumberValue"/> instance.</returns>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if the phone number is null, empty, or exceeds <c>25</c> characters.
        /// </exception>
        public static PhoneNumberValue Create(string number)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("PhoneNumber", number, new PhoneNumberNotValidException("Phone number cannot be null or empty."));
            DomainValidator.ThrowIfExceedsMaxLength("PhoneNumber", number, MaxLength, new PhoneNumberNotValidException($"Phone number cannot exceed {MaxLength} characters."));

            return new PhoneNumberValue(number);
        }
    }


}
