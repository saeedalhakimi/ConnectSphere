namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a person's email address, including metadata such as its type and creation history.
    /// </summary>
    /// <remarks>
    /// The <see cref="EmailAddress"/> entity links an email address to a person and supports
    /// operations such as updating the email, changing the address type, soft deletion, and restoration.
    /// </remarks>
    public class EmailAddress
    {
        /// <summary>
        /// Gets the unique identifier of the email address.
        /// </summary>
        public Guid EmailAddressId { get; private set; }

        /// <summary>
        /// Gets the identifier of the associated person.
        /// </summary>
        public Guid PersonId { get; private set; }

        /// <summary>
        /// Gets the identifier representing the type of email address (e.g., personal, work).
        /// </summary>
        public Guid EmailAddressTypeId { get; private set; }

        /// <summary>
        /// Gets the validated email value object.
        /// </summary>
        public Email Email { get; private set; }

        /// <summary>
        /// Gets the UTC timestamp when the email address was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the UTC timestamp of the most recent update, if any.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the email address has been marked as deleted.
        /// </summary>
        public bool IsDeleted { get; private set; }


        private EmailAddress(Guid emailAddressId, Guid personId, Guid emailAddressTypeId, Email email, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            EmailAddressId = emailAddressId;
            PersonId = personId;
            EmailAddressTypeId = emailAddressTypeId;
            Email = email;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Creates a new <see cref="EmailAddress"/> instance with the specified identifiers and email.
        /// </summary>
        /// <param name="emailAddressId">Unique identifier of the email address.</param>
        /// <param name="personId">Identifier of the person the email belongs to.</param>
        /// <param name="emailAddressTypeId">Type identifier of the email address.</param>
        /// <param name="email">The email value object.</param>
        /// <returns>A new <see cref="EmailAddress"/> instance.</returns>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if any required value is null or invalid.
        /// </exception>
        public static EmailAddress Create(Guid emailAddressId, Guid personId, Guid emailAddressTypeId, Email email)
        {
            DomainValidator.ThrowIfEmptyGuid("EmailAddressId", emailAddressId, new EmailAddressNotValidException("EmailAddressId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new EmailAddressNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("EmailAddressTypeId", emailAddressTypeId, new EmailAddressNotValidException("EmailAddressTypeId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("Email", email, new EmailAddressNotValidException("Email cannot be null."));

            return new EmailAddress(emailAddressId, personId, emailAddressTypeId, email, DateTime.UtcNow, null, false);
        }

        /// <summary>
        /// Reconstructs an existing <see cref="EmailAddress"/> instance from persisted data.
        /// </summary>
        /// <param name="emailAddressId">The email address ID.</param>
        /// <param name="personId">The person ID.</param>
        /// <param name="emailAddressTypeId">The email address type ID.</param>
        /// <param name="email">The email value object.</param>
        /// <param name="createdAt">The original creation timestamp.</param>
        /// <param name="updatedAt">The last update timestamp.</param>
        /// <param name="isDeleted">Whether the email is marked as deleted.</param>
        /// <returns>An existing <see cref="EmailAddress"/> entity.</returns>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if any provided argument is invalid.
        /// </exception>
        public static EmailAddress Reconstruct(Guid emailAddressId, Guid personId, Guid emailAddressTypeId, Email email, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            DomainValidator.ThrowIfEmptyGuid("EmailAddressId", emailAddressId, new EmailAddressNotValidException("EmailAddressId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new EmailAddressNotValidException("PersonId cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("EmailAddressTypeId", emailAddressTypeId, new EmailAddressNotValidException("EmailAddressTypeId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("Email", email, new EmailAddressNotValidException("Email cannot be null."));

            return new EmailAddress(emailAddressId, personId, emailAddressTypeId, email, createdAt, updatedAt, isDeleted);
        }

        /// <summary>
        /// Updates the email value with a new validated <see cref="Email"/>.
        /// </summary>
        /// <param name="newEmail">The new email to set.</param>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the email is null or the entity is marked as deleted.
        /// </exception>
        public void UpdateEmail(Email newEmail)
        {
            DomainValidator.ThrowIfObjectNull("Email", newEmail, new EmailAddressNotValidException("New email cannot be null."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new EmailAddressNotValidException("Operation not allowed: the email address has been deleted."));
            Email = newEmail;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the type of the email address.
        /// </summary>
        /// <param name="newEmailAddressTypeId">The new email address type ID.</param>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the type ID is invalid or the entity is deleted.
        /// </exception>
        public void UpdateEmailAddressType(Guid newEmailAddressTypeId)
        {
            DomainValidator.ThrowIfEmptyGuid("EmailAddressTypeId", newEmailAddressTypeId, new EmailAddressNotValidException("New EmailAddressTypeId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new EmailAddressNotValidException("Operation not allowed: the email address has been deleted."));
            EmailAddressTypeId = newEmailAddressTypeId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the associated person ID.
        /// </summary>
        /// <param name="newPersonId">The new person ID.</param>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the person ID is invalid or the entity is deleted.
        /// </exception>
        public void UpdatePersonId(Guid newPersonId)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonId", newPersonId, new EmailAddressNotValidException("New PersonId cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new EmailAddressNotValidException("Operation not allowed: the email address has been deleted."));
            PersonId = newPersonId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the email address as deleted.
        /// </summary>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the email address is already marked as deleted.
        /// </exception>
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new EmailAddressNotValidException("Email address is already deleted.");
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restores a previously deleted email address.
        /// </summary>
        /// <exception cref="EmailAddressNotValidException">
        /// Thrown if the email address is not currently deleted.
        /// </exception>
        public void Restore()
        {
            if (!IsDeleted)
                throw new EmailAddressNotValidException("Email address is not deleted.");
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="EmailAddress"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is EmailAddress other)
            {
                return EmailAddressId == other.EmailAddressId &&
                       PersonId == other.PersonId &&
                       EmailAddressTypeId == other.EmailAddressTypeId &&
                       Email.Equals(other.Email) &&
                       CreatedAt == other.CreatedAt &&
                       UpdatedAt == other.UpdatedAt &&
                       IsDeleted == other.IsDeleted;
            }
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>An integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(EmailAddressId, PersonId, EmailAddressTypeId, Email, CreatedAt, UpdatedAt, IsDeleted);
        }

        /// <summary>
        /// Determines whether two <see cref="EmailAddress"/> instances are equal.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><c>true</c> if the instances are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(EmailAddress? left, EmailAddress? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="EmailAddress"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first instance.</param>
        /// <param name="right">The second instance.</param>
        /// <returns><c>true</c> if the instances are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(EmailAddress? left, EmailAddress? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Returns a string representation of the email address and its identifier.
        /// </summary>
        /// <returns>A string in the format "email (ID)".</returns>
        public override string ToString()
        {
            return $"{Email} ({EmailAddressId})";
        }
    }
}
