namespace ConnectSphere.API.Domain.ValueObjects
{
    /// <summary>
    /// Represents a structured postal address, including optional second address line and postal code.
    /// </summary>
    /// <remarks>
    /// This value object encapsulates address-related information with validation rules applied during creation.
    /// Required fields include <c>AddressLine1</c> and <c>City</c>. Optional fields include <c>AddressLine2</c> and <c>PostalCode</c>.
    /// Each field is validated against maximum length constraints.
    /// </remarks>
    public record AddressDetails
    {
        /// <summary>
        /// The maximum allowed length for address lines and city fields.
        /// </summary>
        private const int MaxLength = 100;

        /// <summary>
        /// The maximum allowed length for postal code.
        /// </summary>
        private const int PostalCodeMax = 20;

        /// <summary>
        /// Gets the primary address line. This field is required.
        /// </summary>
        public string AddressLine1 { get; init; }

        /// <summary>
        /// Gets the secondary address line (e.g., apartment or suite number). This field is optional.
        /// </summary>
        public string? AddressLine2 { get; init; }

        /// <summary>
        /// Gets the city associated with the address. This field is required.
        /// </summary>
        public string City { get; init; }

        /// <summary>
        /// Gets the postal or ZIP code. This field is optional.
        /// </summary>
        public string? PostalCode { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressDetails"/> record.
        /// </summary>
        /// <param name="addressLine1">The main address line. Required and must not exceed maximum length.</param>
        /// <param name="addressLine2">The secondary address line. Optional and must not exceed maximum length.</param>
        /// <param name="city">The city. Required and must not exceed maximum length.</param>
        /// <param name="postalCode">The postal code. Optional and must not exceed maximum length.</param>
        private AddressDetails(string addressLine1, string? addressLine2, string city, string? postalCode)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            City = city;
            PostalCode = postalCode;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AddressDetails"/> with validated address fields.
        /// </summary>
        /// <remarks>
        /// This factory method applies validation to ensure the address components are not null (where required)
        /// and that none of the fields exceed their defined maximum lengths. Optional values are allowed to be null or empty.
        /// </remarks>
        /// <param name="addressLine1">The primary address line. Required and must not exceed maximum length.</param>
        /// <param name="addressLine2">The secondary address line. Optional and must not exceed maximum length.</param>
        /// <param name="city">The city name. Required and must not exceed maximum length.</param>
        /// <param name="postalCode">The postal or ZIP code. Optional and must not exceed the defined length limit.</param>
        /// <returns>A validated instance of <see cref="AddressDetails"/>.</returns>
        /// <exception cref="AddressNotValidException">
        /// Thrown if required fields are null/empty or if any value exceeds the allowed maximum length.
        /// </exception>
        public static AddressDetails Create(string addressLine1, string? addressLine2, string city, string? postalCode)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("AddressLine 1", addressLine1, new AddressNotValidException("AddressLine 1 cannot be null or empty."));
            DomainValidator.ThrowIfExceedsMaxLength("AddressLine 1", addressLine1, MaxLength, new AddressNotValidException($"AddressLine 1 cannot exceed {MaxLength} characters."));

            DomainValidator.ThrowIfExceedsMaxLength("AddressLine 2", addressLine2, MaxLength, new AddressNotValidException($"AddressLine 2 cannot exceed {MaxLength} characters."));

            DomainValidator.ThrowIfStringNullOrEmpty("City", city, new AddressNotValidException("City cannot be null or empty."));
            DomainValidator.ThrowIfExceedsMaxLength("City", city, MaxLength, new AddressNotValidException($"City cannot exceed {MaxLength} characters."));

            DomainValidator.ThrowIfExceedsMaxLength("PostalCode", postalCode, PostalCodeMax, new AddressNotValidException($"PostalCode cannot exceed {PostalCodeMax} characters."));

            return new AddressDetails(addressLine1.Trim(), addressLine2?.Trim(), city.Trim(), postalCode?.Trim());
        }
    }

}
