namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a type or classification for a person (e.g., Employee, Student, Customer).
    /// </summary>
    /// <remarks>
    /// Each person type is uniquely identified by a GUID and has a descriptive name. 
    /// Useful for categorizing individuals in a domain-driven application.
    /// </remarks>
    public class PersonType
    {
        private const int MaxNameLength = 50;

        /// <summary>
        /// Gets the unique identifier for the person type.
        /// </summary>
        public Guid PersonTypeId { get; private set; }

        /// <summary>
        /// Gets the name of the person type (e.g., Employee, Student).
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonType"/> class.
        /// Private constructor used internally by factory methods.
        /// </summary>
        /// <param name="personTypeId">The unique identifier of the person type.</param>
        /// <param name="name">The name of the person type.</param>
        private PersonType(Guid personTypeId, string name)
        {
            PersonTypeId = personTypeId;
            Name = name;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PersonType"/> with validation.
        /// </summary>
        /// <param name="personTypeId">The unique identifier of the person type.</param>
        /// <param name="name">The name of the person type (e.g., Student, Admin).</param>
        /// <returns>A new valid <see cref="PersonType"/> instance.</returns>
        /// <remarks>
        /// Use this method to create a new PersonType from user input or business logic.
        /// </remarks>
        /// <exception cref="PersonTypeNotValidException">
        /// Thrown if <paramref name="personTypeId"/> is empty, or if <paramref name="name"/> is null, empty, or exceeds the maximum length.
        /// </exception>
        public static PersonType Create(Guid personTypeId, string name)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonTypeId", personTypeId, new PersonTypeNotValidException("PersonTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("PersonTypeName", name, new PersonTypeNotValidException("PersonTypeName cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("PersonTypeName", name, MaxNameLength, new PersonTypeNotValidException($"PersonTypeName cannot exceed {MaxNameLength} characters."));

            return new PersonType(personTypeId, name);
        }

        /// <summary>
        /// Reconstructs an existing <see cref="PersonType"/> instance from persisted data.
        /// </summary>
        /// <param name="personTypeId">The unique identifier of the person type.</param>
        /// <param name="name">The name of the person type.</param>
        /// <returns>A rehydrated <see cref="PersonType"/> instance.</returns>
        /// <remarks>
        /// Use this method when mapping from database or storage.
        /// </remarks>
        /// <exception cref="PersonTypeNotValidException">
        /// Thrown if input values are invalid.
        /// </exception>
        public static PersonType Reconstruct(Guid personTypeId, string name)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonTypeId", personTypeId, new PersonTypeNotValidException("PersonTypeId cannot be empty."));
            DomainValidator.ThrowIfStringNullOrEmpty("PersonTypeName", name, new PersonTypeNotValidException("PersonTypeName cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("PersonTypeName", name, MaxNameLength, new PersonTypeNotValidException($"PersonTypeName cannot exceed {MaxNameLength} characters."));
            return new PersonType(personTypeId, name);
        }

        /// <summary>
        /// Updates the name of the person type.
        /// </summary>
        /// <param name="newName">The new name to assign.</param>
        /// <exception cref="PersonTypeNotValidException">
        /// Thrown if the new name is null, empty, or exceeds the maximum allowed length.
        /// </exception>
        public void UpdateName(string newName)
        {
            DomainValidator.ThrowIfStringNullOrEmpty("PersonTypeName", newName, new PersonTypeNotValidException("PersonTypeName cannot be empty."));
            DomainValidator.ThrowIfExceedsMaxLength("PersonTypeName", newName, MaxNameLength, new PersonTypeNotValidException($"PersonTypeName cannot exceed {MaxNameLength} characters."));

            Name = newName;
        }

        /// <summary>
        /// Returns a string representation of the person type, including its ID and name.
        /// </summary>
        /// <returns>A formatted string.</returns>
        public override string ToString()
        {
            return $"{nameof(PersonTypeId)}: {PersonTypeId}, {nameof(Name)}: {Name}";
        }
    }
}
