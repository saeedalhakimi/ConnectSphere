namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a classification for a phone number (e.g., Mobile, Home, Work).
    /// </summary>
    /// <remarks>
    /// Each phone number type has a unique identifier, a name, and an optional description.
    /// Used to categorize phone numbers in a contact management or person profile system.
    /// </remarks>
    public class PhoneNumberType
    {
        private const int MaxNameLength = 50;
        /// <summary>
        /// Gets the unique identifier of the phone number type.
        /// </summary>
        public Guid PhoneNumberTypeId { get; private set; }

        /// <summary>
        /// Gets the name of the phone number type (e.g., Mobile, Home).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the optional description of the phone number type.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhoneNumberType"/> class.
        /// Private constructor used internally by factory methods.
        /// </summary>
        /// <param name="phoneNumberTypeId">The unique ID of the phone number type.</param>
        /// <param name="name">The name of the phone number type.</param>
        /// <param name="description">The optional description of the phone number type.</param>
        private PhoneNumberType(Guid phoneNumberTypeId, string name, string? description)
        {
            PhoneNumberTypeId = phoneNumberTypeId;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PhoneNumberType"/> with validation.
        /// </summary>
        /// <param name="phoneNumberTypeId">The unique identifier of the phone number type.</param>
        /// <param name="name">The name of the phone number type.</param>
        /// <param name="description">The optional description of the phone number type.</param>
        /// <returns>A validated <see cref="PhoneNumberType"/> instance.</returns>
        /// <exception cref="PhoneNumberTypeNotValideException">
        /// Thrown when input data is null, empty, or exceeds allowed limits.
        /// </exception>
        public static PhoneNumberType Create(Guid phoneNumberTypeId, string name, string? description)
        {
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberTypeId", phoneNumberTypeId, new PhoneNumberTypeNotValideException("PhoneNumberTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("Name", name, new PhoneNumberTypeNotValideException("Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", name, MaxNameLength, new PhoneNumberTypeNotValideException($"Name cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new PhoneNumberTypeNotValideException("Description cannot exceed 200 characters."));

            return new PhoneNumberType(phoneNumberTypeId, name, description);
        }

        /// <summary>
        /// Reconstructs a <see cref="PhoneNumberType"/> from stored data.
        /// </summary>
        /// <param name="phoneNumberTypeId">The ID of the phone number type.</param>
        /// <param name="name">The name of the phone number type.</param>
        /// <param name="description">The optional description.</param>
        /// <returns>A rehydrated <see cref="PhoneNumberType"/> instance.</returns>
        /// <exception cref="PhoneNumberTypeNotValideException">
        /// Thrown if any values are invalid.
        /// </exception>
        public static PhoneNumberType Reconstruct(Guid phoneNumberTypeId, string name, string? description)
        {
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberTypeId", phoneNumberTypeId, new PhoneNumberTypeNotValideException("PhoneNumberTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("Name", name, new PhoneNumberTypeNotValideException("Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", name, MaxNameLength, new PhoneNumberTypeNotValideException($"Name cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new PhoneNumberTypeNotValideException("Description cannot exceed 200 characters."));
            return new PhoneNumberType(phoneNumberTypeId, name, description);
        }

        /// <summary>
        /// Updates the name and optionally the description of the phone number type.
        /// </summary>
        /// <param name="newName">The new name of the phone number type.</param>
        /// <param name="description">Optional new description.</param>
        /// <exception cref="PhoneNumberTypeNotValideException">
        /// Thrown if the name or description is invalid.
        /// </exception>
        public void UpdateName(string newName, string? description = null)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("Name", newName, new PhoneNumberTypeNotValideException("New name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", newName, MaxNameLength, new PhoneNumberTypeNotValideException($"New name cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new PhoneNumberTypeNotValideException("Description cannot exceed 200 characters."));
            Name = newName;
            Description = description;
        }

        /// <summary>
        /// Updates only the description of the phone number type.
        /// </summary>
        /// <param name="description">The new description.</param>
        /// <exception cref="PhoneNumberTypeNotValideException">
        /// Thrown if the description exceeds the allowed length.
        /// </exception>
        public void UpdateDescription(string? description)
        {
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new PhoneNumberTypeNotValideException("Description cannot exceed 200 characters."));
            Description = description;
        }

        /// <summary>
        /// Returns a string representation of the phone number type including name, ID, and description.
        /// </summary>
        /// <returns>A formatted string.</returns>
        public override string ToString()
        {
            return $"{Name} ({PhoneNumberTypeId}) - {Description}";
        }
    }
}
