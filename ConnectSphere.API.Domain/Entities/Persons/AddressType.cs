namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a classification or label for an address, such as "Home", "Work", or "Billing".
    /// </summary>
    /// <remarks>
    /// This entity holds a unique identifier, a name, and an optional description.
    /// Used to categorize addresses linked to a person or organization.
    /// </remarks>
    public class AddressType
    {
        /// <summary>
        /// The maximum number of characters allowed for the <see cref="Name"/> property.
        /// </summary>
        private const int MaxNameLength = 50;

        /// <summary>
        /// Gets the unique identifier of the address type.
        /// </summary>
        public Guid AddressTypeId { get; private set; }

        /// <summary>
        /// Gets the name of the address type (e.g., "Home", "Work").
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the optional description of the address type.
        /// </summary>
        public string? Description { get; private set; }


        private AddressType(Guid addressTypeId, string name, string? description)
        {
            AddressTypeId = addressTypeId;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Creates a new <see cref="AddressType"/> instance with validated input.
        /// </summary>
        /// <param name="addressTypeId">Unique identifier for the address type.</param>
        /// <param name="name">Name of the address type (required, max 50 characters).</param>
        /// <param name="description">Optional description (max 200 characters).</param>
        /// <returns>A new instance of <see cref="AddressType"/>.</returns>
        /// <remarks>
        /// Validates the ID, name, and description before creating the instance.
        /// </remarks>
        /// <exception cref="AddressTypeNotValidException">
        /// Thrown if any required field is invalid or violates length constraints.
        /// </exception>
        public static AddressType Create(Guid addressTypeId, string name, string? description)
        {
            DomainValidator.ThrowIfEmptyGuid("AddressTypeId", addressTypeId, new AddressTypeNotValidException("AddressTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("Name", name, new AddressTypeNotValidException("Type Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", name, MaxNameLength, new AddressTypeNotValidException($"TypeName cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new AddressTypeNotValidException("Description cannot exceed 200 characters."));

            return new AddressType(addressTypeId, name, description);
        }

        /// <summary>
        /// Reconstructs an <see cref="AddressType"/> instance, typically used when loading from a data store.
        /// </summary>
        /// <param name="addressTypeId">Unique identifier for the address type.</param>
        /// <param name="name">Name of the address type (required, max 50 characters).</param>
        /// <param name="description">Optional description (max 200 characters).</param>
        /// <returns>A reconstructed <see cref="AddressType"/> instance.</returns>
        /// <remarks>
        /// Performs the same validations as <see cref="Create"/> to ensure consistency.
        /// </remarks>
        /// <exception cref="AddressTypeNotValidException">
        /// Thrown if any parameter is invalid or violates length rules.
        /// </exception>
        public static AddressType Reconstruct(Guid addressTypeId, string name, string? description)
        {
            DomainValidator.ThrowIfEmptyGuid("AddressTypeId", addressTypeId, new AddressTypeNotValidException("AddressTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("Name", name, new AddressTypeNotValidException("Type Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", name, MaxNameLength, new AddressTypeNotValidException($"TypeName cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new AddressTypeNotValidException("Description cannot exceed 200 characters."));

            return new AddressType(addressTypeId, name, description);
        }

        /// <summary>
        /// Updates the <see cref="Name"/> of the address type.
        /// </summary>
        /// <param name="newName">The new name to set (must not be null, empty, or longer than 50 characters).</param>
        /// <exception cref="AddressTypeNotValidException">
        /// Thrown if <paramref name="newName"/> is invalid.
        /// </exception>
        public void UpdateName(string newName)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("Name", newName, new AddressTypeNotValidException("Type Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", newName, MaxNameLength, new AddressTypeNotValidException($"TypeName cannot exceed {MaxNameLength} characters."));
            Name = newName;
        }

        /// <summary>
        /// Updates the <see cref="Description"/> of the address type.
        /// </summary>
        /// <param name="description">The new description to set (max 200 characters).</param>
        /// <exception cref="AddressTypeNotValidException">
        /// Thrown if <paramref name="description"/> exceeds the maximum length.
        /// </exception>
        public void UpdateDescription(string description)
        {
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new AddressTypeNotValidException("Description cannot exceed 200 characters."));
            Description = description;
        }
    }
}
