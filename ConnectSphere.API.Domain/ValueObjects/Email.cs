namespace ConnectSphere.API.Domain.ValueObjects
{
    /// <summary>
    /// Represents a validated email address as a value object.
    /// </summary>
    /// <remarks>
    /// The <see cref="Email"/> record ensures that an email address is not null, not empty, 
    /// does not exceed the allowed maximum length (100 characters), and follows a valid email format.
    /// This type encapsulates the email as a strongly-typed domain primitive.
    /// </remarks>
    public record Email
    {
        private const int MaxLength = 100; // Maximum length for an email address

        /// <summary>
        /// Gets the string value of the email address.
        /// </summary>
        public string Value { get; init; }

        private Email(string value) => Value = value;

        /// <summary>
        /// Creates a new validated <see cref="Email"/> instance.
        /// </summary>
        /// <remarks>
        /// This factory method performs several validations on the provided email string:
        /// it must not be null or empty, must not exceed 100 characters, and must follow a valid email format.
        /// </remarks>
        /// <param name="email">The email address string to validate and encapsulate.</param>
        /// <returns>A new instance of <see cref="Email"/> if the input is valid.</returns>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the email is null, empty, too long, or does not match a valid email format.
        /// </exception>
        public static Email Create(string email)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("Email", email, new EmailAddressNotValidException("Email cannot be null or empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Email", email, MaxLength, new EmailAddressNotValidException($"Email cannot exceed {MaxLength} characters."));
            DomainValidator.ThrowIfInvalidEmailFormat(email, new EmailAddressNotValidException("Email format is invalid."));

            return new Email(email);
        }
    }
}
