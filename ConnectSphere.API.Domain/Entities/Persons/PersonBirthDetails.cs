namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents the birth details of a person, including birth date, birth city, country, and audit metadata.
    /// </summary>
    /// <remarks>
    /// The <see cref="PersonBirthDetails"/> entity models a person's birth-related information in a normalized structure. 
    /// It supports soft deletion, auditing, and mutation operations, while enforcing domain-level validation through controlled creation and update methods.
    /// </remarks>
    public class PersonBirthDetails
    {
        /// <summary>Gets the unique identifier for the person's birth details record.</summary>
        public Guid PersonBirthDetailsId { get; private set; }

        /// <summary>Gets the identifier of the person associated with the birth details.</summary>
        public Guid PersonId { get; private set; }

        /// <summary>Gets the structured birth details including date and optional city.</summary>
        public BirthDetails Details { get; private set; }

        /// <summary>Gets the identifier of the country where the person was born.</summary>
        public Guid CountryId { get; private set; }

        /// <summary>Gets the timestamp when this record was created.</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Gets the timestamp when this record was last updated, if applicable.</summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>Indicates whether this record has been soft-deleted.</summary>
        public bool IsDeleted { get; private set; }


        private PersonBirthDetails(Guid personBirthDetailsId, Guid personId, BirthDetails details, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            PersonBirthDetailsId = personBirthDetailsId;
            PersonId = personId;
            Details = details;
            CountryId = countryId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Creates a new <see cref="PersonBirthDetails"/> instance with validated inputs.
        /// </summary>
        /// <remarks>
        /// This factory method ensures all required fields are non-empty and valid. It sets the creation timestamp and initializes the deletion flag to <c>false</c>.
        /// </remarks>
        /// <param name="personBirthDetailsId">Unique identifier for the birth details record.</param>
        /// <param name="personId">The ID of the person this birth data belongs to.</param>
        /// <param name="details">The validated birth details.</param>
        /// <param name="countryId">The country where the birth occurred.</param>
        /// <returns>A new instance of <see cref="PersonBirthDetails"/>.</returns>
        /// <exception cref="BirthDetailsNotValidException">Thrown if any required fields are invalid or null.</exception>
        public static PersonBirthDetails Create(Guid personBirthDetailsId, Guid personId, BirthDetails details, Guid countryId)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonBirthDetailsId", personBirthDetailsId, new BirthDetailsNotValidException("PersonBirthDetailsId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new BirthDetailsNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("BirthDetails", details, new BirthDetailsNotValidException("Birth details cannot be null."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new BirthDetailsNotValidException("CountryId cannot be empty."));

            return new PersonBirthDetails(personBirthDetailsId, personId, details, countryId, DateTime.UtcNow, null, false);
        }

        /// <summary>
        /// Reconstructs a <see cref="PersonBirthDetails"/> instance from persisted data.
        /// </summary>
        /// <remarks>
        /// This method is used when loading existing records from a data store. It bypasses domain logic like default timestamps or soft deletion flags.
        /// </remarks>
        /// <param name="personBirthDetailsId">ID of the birth details record.</param>
        /// <param name="personId">ID of the associated person.</param>
        /// <param name="details">Validated birth details.</param>
        /// <param name="countryId">Country ID where the birth occurred.</param>
        /// <param name="createdAt">Timestamp of when the record was originally created.</param>
        /// <param name="updatedAt">Timestamp of the last update, if applicable.</param>
        /// <param name="isDeleted">Soft deletion flag from the database.</param>
        /// <returns>A reconstructed <see cref="PersonBirthDetails"/> instance.</returns>
        /// /// <exception cref="BirthDetailsNotValidException">Thrown if any required fields are invalid or null.</exception>
        public static PersonBirthDetails Reconstruct(Guid personBirthDetailsId, Guid personId, BirthDetails details, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonBirthDetailsId", personBirthDetailsId, new BirthDetailsNotValidException("PersonBirthDetailsId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new BirthDetailsNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("BirthDetails", details, new BirthDetailsNotValidException("Birth details cannot be null."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new BirthDetailsNotValidException("CountryId cannot be empty."));
            
            return new PersonBirthDetails(personBirthDetailsId, personId, details, countryId, createdAt, updatedAt, isDeleted);
        }

        /// <summary>
        /// Updates the birth details and country.
        /// </summary>
        /// <param name="newDetails">The new birth details.</param>
        /// <param name="countryId">The new country ID.</param>
        /// <exception cref="BirthDetailsNotValidException">
        /// Thrown if any value is invalid or the record is marked as deleted.
        /// </exception>
        public void UpdateDetails(BirthDetails newDetails, Guid countryId)
        {
            DomainValidator.ThrowIfObjectNull("BirthDetails", newDetails, new BirthDetailsNotValidException("New birth details cannot be null."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new BirthDetailsNotValidException("CountryId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new BirthDetailsNotValidException("Operation not allowed: the person birth details have been deleted."));
            Details = newDetails;
            CountryId = countryId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the current record as soft-deleted.
        /// </summary>
        /// <exception cref="BirthDetailsNotValidException">Thrown if the record is already deleted.</exception>
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new BirthDetailsNotValidException("Operation not allowed: the person birth details have already been deleted.");
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restores a previously deleted record.
        /// </summary>
        /// <exception cref="BirthDetailsNotValidException">Thrown if the record is not marked as deleted.</exception>
        public void Restore()
        {
            if (!IsDeleted)
                throw new BirthDetailsNotValidException("Operation not allowed: the person birth details are not deleted.");
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the country ID of the birth details.
        /// </summary>
        /// <param name="countryId">New country ID to set.</param>
        /// <exception cref="BirthDetailsNotValidException">Thrown if the country ID is empty or the record is deleted.</exception>
        public void UpdateCountry(Guid countryId)
        {
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new BirthDetailsNotValidException("CountryId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new BirthDetailsNotValidException("Operation not allowed: the person birth details have been deleted."));
            CountryId = countryId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the associated person ID of the record.
        /// </summary>
        /// <param name="personId">New person ID to set.</param>
        /// <exception cref="BirthDetailsNotValidException">Thrown if the person ID is empty or the record is deleted.</exception>
        public void UpdatePersonId(Guid personId)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new BirthDetailsNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new BirthDetailsNotValidException("Operation not allowed: the person birth details have been deleted."));
            PersonId = personId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="PersonBirthDetails"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not PersonBirthDetails other)
                return false;
            return PersonBirthDetailsId == other.PersonBirthDetailsId &&
                   PersonId == other.PersonId &&
                   Details.Equals(other.Details) &&
                   CountryId == other.CountryId &&
                   CreatedAt == other.CreatedAt &&
                   UpdatedAt == other.UpdatedAt &&
                   IsDeleted == other.IsDeleted;
        }

        /// <summary>
        /// Returns a hash code for the current <see cref="PersonBirthDetails"/> instance.
        /// </summary>
        /// <returns>A hash code based on identifying and value fields.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(PersonBirthDetailsId, PersonId, Details, CountryId, CreatedAt, UpdatedAt, IsDeleted);
        }

        /// <summary>
        /// Determines whether two <see cref="PersonBirthDetails"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(PersonBirthDetails? left, PersonBirthDetails? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="PersonBirthDetails"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if the instances differ; otherwise, <c>false</c>.</returns>
        public static bool operator !=(PersonBirthDetails? left, PersonBirthDetails? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string representation of the <see cref="PersonBirthDetails"/> instance.
        /// </summary>
        /// <returns>A string containing key property values for display or logging purposes.</returns>
        public override string ToString()
        {
            return $"PersonBirthDetailsId: {PersonBirthDetailsId}, PersonId: {PersonId}, BirthDate: {Details.BirthDate}, BirthCity: {Details.BirthCity}, CountryId: {CountryId}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, IsDeleted: {IsDeleted}";
        }
    }
}
