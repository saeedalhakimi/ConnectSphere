namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents an address associated with a person, including type, location details, and audit tracking.
    /// </summary>
    /// <remarks>
    /// The <see cref="Address"/> entity encapsulates location-specific data (e.g., address lines, city, country)
    /// and supports soft deletion, modification, and reconstruction from persisted data.
    /// It includes full validation to ensure domain consistency.
    /// </remarks>
    public class Address
    {
        /// <summary>Gets the unique identifier of the address.</summary>
        public Guid AddressId { get; private set; }

        /// <summary>Gets the identifier of the person to whom the address belongs.</summary>
        public Guid PersonId { get; private set; }

        /// <summary>Gets the identifier of the type of address (e.g., home, work).</summary>
        public Guid AddressTypeId { get; private set; }

        /// <summary>Gets the detailed address information.</summary>
        public AddressDetails Details { get; private set; }

        /// <summary>Gets the identifier of the country where the address is located.</summary>
        public Guid CountryId { get; private set; }

        /// <summary>Gets the timestamp of when the address was originally created.</summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>Gets the timestamp of the last update to the address, if any.</summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>Gets a value indicating whether the address has been soft-deleted.</summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        private Address(Guid addressId, Guid personId, Guid addressTypeId, AddressDetails details, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            AddressId = addressId;
            PersonId = personId;
            AddressTypeId = addressTypeId;
            Details = details;
            CountryId = countryId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Creates a new <see cref="Address"/> instance with validated inputs.
        /// </summary>
        /// <remarks>
        /// This factory method is used to create a new address within the domain model. 
        /// It validates that all required fields—<paramref name="addressId"/>, <paramref name="personId"/>, 
        /// <paramref name="addressTypeId"/>, <paramref name="countryId"/>, and <paramref name="details"/>—are valid and not empty or null.
        /// The method also sets the <see cref="CreatedAt"/> property to the current UTC time and initializes the <see cref="IsDeleted"/> flag to <c>false</c>.
        /// </remarks>
        /// <param name="addressId">Unique address identifier.</param>
        /// <param name="personId">Associated person identifier.</param>
        /// <param name="addressTypeId">Address type identifier (e.g., home, work).</param>
        /// <param name="details">Detailed address information.</param>
        /// <param name="countryId">Country where the address is located.</param>
        /// <returns>A new instance of <see cref="Address"/>.</returns>
        /// <exception cref="AddressNotValidException">
        /// Thrown if any required GUID is empty or if <paramref name="details"/> is null.
        /// </exception>
        public static Address Create(Guid addressId, Guid personId, Guid addressTypeId, AddressDetails details, Guid countryId)
        {
            DomainValidator.ThrowIfEmptyGuid("AddressId", addressId, new AddressNotValidException("Address ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new AddressNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("AddressTypeId", addressTypeId, new AddressNotValidException("Address Type ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new AddressNotValidException("Country ID cannot be empty."));
            DomainValidator.ThrowIfObjectNull("AddressDetails", details, new AddressNotValidException("Address details cannot be null."));

            return new Address(addressId, personId, addressTypeId, details, countryId, DateTime.UtcNow, null, false);
        }

        /// <summary>
        /// Reconstructs an <see cref="Address"/> instance from persisted data.
        /// </summary>
        /// <remarks>
        /// This method is intended for loading entities from a database or external store. 
        /// It bypasses the automatic creation timestamp and deletion state logic.
        /// </remarks>
        /// <param name="addressId">Address ID.</param>
        /// <param name="personId">Person ID.</param>
        /// <param name="addressTypeId">Address type ID.</param>
        /// <param name="details">Address details.</param>
        /// <param name="countryId">Country ID.</param>
        /// <param name="createdAt">Creation timestamp from database.</param>
        /// <param name="updatedAt">Last update timestamp from database.</param>
        /// <param name="isDeleted">Soft delete flag from database.</param>
        /// <returns>A reconstructed <see cref="Address"/> instance.</returns>
        public static Address Reconstruct(Guid addressId, Guid personId, Guid addressTypeId, AddressDetails details, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            DomainValidator.ThrowIfEmptyGuid("AddressId", addressId, new AddressNotValidException("Address ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new AddressNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("AddressTypeId", addressTypeId, new AddressNotValidException("Address Type ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new AddressNotValidException("Country ID cannot be empty."));
            DomainValidator.ThrowIfObjectNull("AddressDetails", details, new AddressNotValidException("Address details cannot be null."));

            return new Address(addressId, personId, addressTypeId, details, countryId, createdAt, updatedAt, isDeleted);
        }

        /// <summary>
        /// Updates the address details with a new value.
        /// </summary>
        /// <param name="newDetails">The new address details.</param>
        /// <exception cref="AddressNotValidException">
        /// Thrown if <paramref name="newDetails"/> is null or the address has been marked as deleted.
        /// </exception>
        public void UpdateDetails(AddressDetails newDetails)
        {
            DomainValidator.ThrowIfObjectNull("AddressDetails", newDetails, new AddressNotValidException("New address details cannot be null."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new AddressNotValidException("Operation not allowed: the address has been deleted."));
            Details = newDetails;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the address as deleted (soft delete).
        /// </summary>
        /// <exception cref="AddressNotValidException">Thrown if the address is already deleted.</exception>
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new AddressNotValidException("Address is already deleted.");
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Restores a previously deleted address.
        /// </summary>
        /// <exception cref="AddressNotValidException">Thrown if the address is not marked as deleted.</exception>
        public void Restore()
        {
            if (!IsDeleted)
                throw new AddressNotValidException("Address is not deleted.");
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Changes the associated country of the address.
        /// </summary>
        /// <param name="newCountryId">New country ID to assign.</param>
        /// <exception cref="AddressNotValidException">
        /// Thrown if <paramref name="newCountryId"/> is empty or the address is deleted.
        /// </exception>
        public void ChangeCountry(Guid newCountryId)
        {
            DomainValidator.ThrowIfEmptyGuid("CountryId", newCountryId, new AddressNotValidException("New Country ID cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new AddressNotValidException("Operation not allowed: the address has been deleted."));
            CountryId = newCountryId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Changes the address type (e.g., from Home to Office).
        /// </summary>
        /// <param name="newAddressTypeId">The new address type ID.</param>
        /// <exception cref="AddressNotValidException">
        /// Thrown if <paramref name="newAddressTypeId"/> is empty or the address is deleted.
        /// </exception>
        public void ChangeAddressType(Guid newAddressTypeId)
        {
            DomainValidator.ThrowIfEmptyGuid("AddressTypeId", newAddressTypeId, new AddressNotValidException("New Address Type ID cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new AddressNotValidException("Operation not allowed: the address has been deleted."));
            AddressTypeId = newAddressTypeId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="Address"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current address.</param>
        /// <returns>
        /// <c>true</c> if the specified object is an <see cref="Address"/> and has the same values for 
        /// all identity and value-based fields; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj is not Address other)
                return false;
            return AddressId == other.AddressId &&
                   PersonId == other.PersonId &&
                   AddressTypeId == other.AddressTypeId &&
                   CountryId == other.CountryId &&
                   CreatedAt == other.CreatedAt &&
                   UpdatedAt == other.UpdatedAt &&
                   IsDeleted == other.IsDeleted;
        }

        /// <summary>
        /// Serves as the default hash function for <see cref="Address"/>.
        /// </summary>
        /// <returns>
        /// A hash code that represents the current address instance, computed from its identifying and value fields.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(AddressId, PersonId, AddressTypeId, CountryId, CreatedAt, UpdatedAt, IsDeleted);
        }

        /// <summary>
        /// Determines whether two <see cref="Address"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Address"/> to compare.</param>
        /// <param name="right">The second <see cref="Address"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if both <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(Address? left, Address? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="Address"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Address"/> to compare.</param>
        /// <param name="right">The second <see cref="Address"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> differ; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(Address? left, Address? right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"AddressId: {AddressId}, PersonId: {PersonId}, AddressTypeId: {AddressTypeId}, CountryId: {CountryId}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, IsDeleted: {IsDeleted}";
        }
    }
}
