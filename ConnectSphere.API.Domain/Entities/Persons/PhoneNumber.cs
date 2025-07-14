namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a phone number entity linked to a person, including its type, country, and lifecycle state.
    /// </summary>
    /// <remarks>
    /// This entity captures essential metadata about a person's phone number:
    /// - <b>PhoneNumberTypeId</b>: Indicates the type of phone (e.g., mobile, home, work).
    /// - <b>CountryId</b>: Identifies the country associated with the number for localization or dialing purposes.
    /// - <b>CreatedAt</b> and <b>UpdatedAt</b>: Timestamps for auditing and tracking changes.
    /// - <b>IsDeleted</b>: Implements soft-deletion logic for historical retention without permanent removal.
    ///
    /// Use <see cref="Create"/> to instantiate a new phone number with automatic validation,
    /// or <see cref="Reconstruct"/> when restoring from persistence.
    /// </remarks>
    public class PhoneNumber
    {
        /// <summary>
        /// Gets the unique identifier for the phone number entry.
        /// </summary>
        public Guid PhoneNumberId { get; private set; }

        /// <summary>
        /// Gets the identifier of the person to whom this phone number belongs.
        /// </summary>
        public Guid PersonId { get; private set; }

        /// <summary>
        /// Gets the identifier for the phone number type (e.g., mobile, home, work).
        /// </summary>
        public Guid PhoneNumberTypeId { get; private set; }

        /// <summary>
        /// Gets the validated phone number value.
        /// </summary>
        public PhoneNumberValue Number { get; private set; }

        /// <summary>
        /// Gets the country identifier associated with this phone number.
        /// </summary>
        public Guid CountryId { get; private set; }

        /// <summary>
        /// Gets the timestamp when the phone number was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the timestamp of the last update made to this phone number, if any.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the phone number has been soft deleted.
        /// </summary>
        public bool IsDeleted { get; private set; }

        private PhoneNumber(Guid phoneNumberId, Guid personId, Guid phoneNumberTypeId, PhoneNumberValue number, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            PhoneNumberId = phoneNumberId;
            PersonId = personId;
            PhoneNumberTypeId = phoneNumberTypeId;
            Number = number;
            CountryId = countryId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PhoneNumber"/> with validated inputs.
        /// </summary>
        /// <param name="phoneNumberId">Unique identifier for the phone number.</param>
        /// <param name="personId">Associated person's identifier.</param>
        /// <param name="phoneNumberTypeId">Identifier of the phone number type.</param>
        /// <param name="number">Validated phone number value.</param>
        /// <param name="countryId">Associated country identifier.</param>
        /// <returns>A new <see cref="PhoneNumber"/> instance.</returns>
        /// <remarks>Validates required fields and sets creation timestamp.</remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if any input is invalid, such as an empty GUID or null number.
        /// </exception>
        public static PhoneNumber Create(Guid phoneNumberId, Guid personId, Guid phoneNumberTypeId, PhoneNumberValue number, Guid countryId)
        {
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberId", phoneNumberId, new PhoneNumberNotValidException("PhoneNumberId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new PhoneNumberNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberTypeId", phoneNumberTypeId, new PhoneNumberNotValidException("PhoneNumberTypeId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("PhoneNumberValue", number, new PhoneNumberNotValidException("Phone number cannot be null."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new PhoneNumberNotValidException("CountryId cannot be empty."));

            return new PhoneNumber(phoneNumberId, personId, phoneNumberTypeId, number, countryId, DateTime.UtcNow, null, false);
        }

        /// <summary>
        /// Reconstructs an existing <see cref="PhoneNumber"/> instance from persisted data.
        /// </summary>
        /// <remarks>
        /// This method is typically used when loading a <see cref="PhoneNumber"/> entity from a data source,
        /// such as a database. It bypasses domain rules related to object creation time and allows full control
        /// over setting the <paramref name="createdAt"/>, <paramref name="updatedAt"/>, and <paramref name="isDeleted"/> flags.
        /// All input parameters are validated to ensure data integrity.
        /// </remarks>
        /// <param name="phoneNumberId">The unique identifier of the phone number.</param>
        /// <param name="personId">The identifier of the associated person.</param>
        /// <param name="phoneNumberTypeId">The identifier of the phone number type (e.g., mobile, home).</param>
        /// <param name="number">The actual phone number value wrapped in a <see cref="PhoneNumberValue"/>.</param>
        /// <param name="countryId">The identifier of the associated country.</param>
        /// <param name="createdAt">The date and time the record was originally created.</param>
        /// <param name="updatedAt">The date and time the record was last updated, if any.</param>
        /// <param name="isDeleted">Indicates whether the record is marked as deleted (soft delete).</param>
        /// <returns>A fully reconstructed <see cref="PhoneNumber"/> entity.</returns>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if any of the required parameters are invalid (e.g., empty GUIDs or null values).
        /// </exception>
        public static PhoneNumber Reconstruct(Guid phoneNumberId, Guid personId, Guid phoneNumberTypeId, PhoneNumberValue number, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberId", phoneNumberId, new PhoneNumberNotValidException("PhoneNumberId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new PhoneNumberNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberTypeId", phoneNumberTypeId, new PhoneNumberNotValidException("PhoneNumberTypeId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("PhoneNumberValue", number, new PhoneNumberNotValidException("Phone number cannot be null."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new PhoneNumberNotValidException("CountryId cannot be empty."));

            return new PhoneNumber(phoneNumberId, personId, phoneNumberTypeId, number, countryId, createdAt, updatedAt, isDeleted);
        }

        /// <summary>
        /// Updates the current phone number value.
        /// </summary>
        /// <param name="newNumber">The new phone number value to set.</param>
        /// <remarks>
        /// This operation also updates the <see cref="UpdatedAt"/> timestamp. Cannot be called on a deleted phone number.
        /// </remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if <paramref name="newNumber"/> is null or the entity is marked as deleted.
        /// </exception>
        public void UpdateNumber(PhoneNumberValue newNumber)
        {
            DomainValidator.ThrowIfObjectNull("PhoneNumberValue", newNumber, new PhoneNumberNotValidException("Phone number cannot be null."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new PhoneNumberNotValidException("Operation not allowed: the phone number has been deleted."));
            Number = newNumber;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the phone number as deleted (soft delete).
        /// </summary>
        /// <remarks>
        /// Sets <see cref="IsDeleted"/> to <c>true</c> and updates the <see cref="UpdatedAt"/> timestamp.
        /// </remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if the phone number is already marked as deleted.
        /// </exception>
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new PhoneNumberNotValidException("Phone number is already deleted.");
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restores a previously deleted phone number.
        /// </summary>
        /// <remarks>
        /// Resets the <see cref="IsDeleted"/> flag to <c>false</c> and updates the <see cref="UpdatedAt"/> timestamp.
        /// </remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if the phone number is not currently marked as deleted.
        /// </exception>
        public void Restore()
        {
            if (!IsDeleted)
                throw new PhoneNumberNotValidException("Phone number is not deleted.");
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the phone number type associated with this entry.
        /// </summary>
        /// <param name="newPhoneNumberTypeId">The new phone number type identifier.</param>
        /// <remarks>
        /// This operation also updates the <see cref="UpdatedAt"/> timestamp. Cannot be called on a deleted phone number.
        /// </remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if <paramref name="newPhoneNumberTypeId"/> is empty or if the record is marked as deleted.
        /// </exception>
        public void UpdatePhoneNumberType(Guid newPhoneNumberTypeId)
        {
            DomainValidator.ThrowIfEmptyGuid("PhoneNumberTypeId", newPhoneNumberTypeId, new PhoneNumberNotValidException("PhoneNumberTypeId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new PhoneNumberNotValidException("Operation not allowed: the phone number has been deleted."));
            PhoneNumberTypeId = newPhoneNumberTypeId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the country associated with this phone number.
        /// </summary>
        /// <param name="newCountryId">The new country identifier.</param>
        /// <remarks>
        /// This operation also updates the <see cref="UpdatedAt"/> timestamp. Cannot be called on a deleted phone number.
        /// </remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if <paramref name="newCountryId"/> is empty or if the record is marked as deleted.
        /// </exception>
        public void UpdateCountry(Guid newCountryId)
        {
            DomainValidator.ThrowIfEmptyGuid("CountryId", newCountryId, new PhoneNumberNotValidException("CountryId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new PhoneNumberNotValidException("Operation not allowed: the phone number has been deleted."));
            CountryId = newCountryId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the person associated with this phone number.
        /// </summary>
        /// <param name="newPersonId">The new person identifier.</param>
        /// <remarks>
        /// This operation also updates the <see cref="UpdatedAt"/> timestamp. Cannot be called on a deleted phone number.
        /// </remarks>
        /// <exception cref="PhoneNumberNotValidException">
        /// Thrown if <paramref name="newPersonId"/> is empty or if the record is marked as deleted.
        /// </exception>
        public void UpdatePerson(Guid newPersonId)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonId", newPersonId, new PhoneNumberNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new PhoneNumberNotValidException("Operation not allowed: the phone number has been deleted."));
            PersonId = newPersonId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Generates a hash code for the current <see cref="PhoneNumber"/> instance.
        /// </summary>
        /// <returns>A hash code representing the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(PhoneNumberId, PersonId, PhoneNumberTypeId, Number, CountryId, CreatedAt, UpdatedAt, IsDeleted);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="PhoneNumber"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not PhoneNumber other)
                return false;
            return PhoneNumberId == other.PhoneNumberId &&
                   PersonId == other.PersonId &&
                   PhoneNumberTypeId == other.PhoneNumberTypeId &&
                   Number.Equals(other.Number) &&
                   CountryId == other.CountryId &&
                   CreatedAt == other.CreatedAt &&
                   UpdatedAt == other.UpdatedAt &&
                   IsDeleted == other.IsDeleted;
        }

        /// <summary>
        /// Determines whether two <see cref="PhoneNumber"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="PhoneNumber"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance to compare.</param>
        /// <param name="right">The second instance to compare.</param>
        /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string representation of the phone number with its type and country.
        /// </summary>
        /// <returns>A string describing the phone number, type, and country.</returns>
        public override string ToString()
        {
            return $"{Number.Number} ({PhoneNumberTypeId}) - {CountryId}";
        }
    }
}
