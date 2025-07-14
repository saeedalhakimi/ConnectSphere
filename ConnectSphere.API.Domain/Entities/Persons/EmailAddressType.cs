namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a type or category for an email address (e.g., Personal, Work, Support).
    /// </summary>
    /// <remarks>
    /// This class includes the unique identifier, name, and an optional description for the email type.
    /// Useful for classifying and organizing email addresses within the domain.
    /// </remarks>
    public class EmailAddressType
    {
        private const int MaxNameLength = 50;

        /// <summary>
        /// Gets the unique identifier of the email address type.
        /// </summary>
        public Guid EmailAddressTypeId { get; private set; }

        /// <summary>
        /// Gets the name of the email address type (e.g., Work, Personal).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the optional description of the email address type.
        /// </summary>
        public string? Description { get; private set; }

        private EmailAddressType(Guid emailAddressTypeId, string name, string? description)
        {
            EmailAddressTypeId = emailAddressTypeId;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Creates a new <see cref="EmailAddressType"/> instance with validated input.
        /// </summary>
        /// <param name="emailAddressTypeId">The unique identifier for the email address type.</param>
        /// <param name="name">The name representing the email type (e.g., Personal, Work).</param>
        /// <param name="description">Optional description for the email type.</param>
        /// <returns>A new validated <see cref="EmailAddressType"/> instance.</returns>
        /// <remarks>
        /// Use this method when creating a new email address type from user input or application logic.
        /// </remarks>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if <paramref name="emailAddressTypeId"/> is empty, <paramref name="name"/> is null or too long, or if <paramref name="description"/> exceeds 200 characters.
        /// </exception>
        public static EmailAddressType Create(Guid emailAddressTypeId, string name, string? description)
        {
            DomainValidator.ThrowIfEmptyGuid("EmailAddressTypeId", emailAddressTypeId, new EmailAddressNotValidException("EmailAddressTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("Name", name, new EmailAddressNotValidException("Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", name, MaxNameLength, new EmailAddressNotValidException($"Name cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new EmailAddressNotValidException("Description cannot exceed 200 characters."));
            
            return new EmailAddressType(emailAddressTypeId, name, description);
        }

        /// <summary>
        /// Reconstructs an existing <see cref="EmailAddressType"/> from persisted storage (e.g., database).
        /// </summary>
        /// <param name="emailAddressTypeId">The unique identifier of the email address type.</param>
        /// <param name="name">The name of the email type.</param>
        /// <param name="description">Optional description of the email type.</param>
        /// <returns>The reconstructed <see cref="EmailAddressType"/> object.</returns>
        /// <remarks>
        /// This method is used for rehydrating domain entities without triggering domain events or business rules.
        /// </remarks>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if any required field is invalid.
        /// </exception>
        public static EmailAddressType Reconstruct(Guid emailAddressTypeId, string name, string? description)
        {
            DomainValidator.ThrowIfEmptyGuid("EmailAddressTypeId", emailAddressTypeId, new EmailAddressNotValidException("EmailAddressTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("Name", name, new EmailAddressNotValidException("Name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", name, MaxNameLength, new EmailAddressNotValidException($"Name cannot exceed {MaxNameLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("Description", description, 200, new EmailAddressNotValidException("Description cannot exceed 200 characters."));
            
            return new EmailAddressType(emailAddressTypeId, name, description);
        }

        /// <summary>
        /// Updates the name of the email address type.
        /// </summary>
        /// <param name="newName">The new name to assign.</param>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the <paramref name="newName"/> is null, empty, or exceeds allowed length.
        /// </exception>
        public void UpdateName(string newName)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("Name", newName, new EmailAddressNotValidException("New name cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("Name", newName, MaxNameLength, new EmailAddressNotValidException($"Name cannot exceed {MaxNameLength} characters."));
            Name = newName;
        }

        /// <summary>
        /// Updates the description of the email address type.
        /// </summary>
        /// <param name="newDescription">The new description value to assign (can be null).</param>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the <paramref name="newDescription"/> exceeds allowed length.
        /// </exception>
        public void UpdateDescription(string? newDescription)
        {
            DomainValidator.ThrowIfExceedsMaxLength("Description", newDescription, 200, new EmailAddressNotValidException("Description cannot exceed 200 characters."));
            Description = newDescription;
        }

        /// <summary>
        /// Returns a string representation of the email address type including name, ID, and description.
        /// </summary>
        /// <returns>A formatted string describing the email type.</returns>
        public override string ToString()
        {
            return $"{Name} ({EmailAddressTypeId}) - {Description}";
        }
    }
}
