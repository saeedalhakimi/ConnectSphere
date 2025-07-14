namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a governmental information record associated with a person and a country, 
    /// including identification details such as government ID and passport number.
    /// </summary>
    /// <remarks>
    /// This class encapsulates governmental identification data for a person, scoped by country. 
    /// It includes validation to ensure that required values such as GUIDs and the details object are properly provided.
    /// </remarks>
    public class GovernmentalInfo
    {
        /// <summary>
        /// Gets the unique identifier of the governmental information record.
        /// </summary>
        public Guid GovernmentalInfoId { get; private set; }

        /// <summary>
        /// Gets the identifier of the person to whom this governmental information belongs.
        /// </summary>
        public Guid PersonId { get; private set; }

        /// <summary>
        /// Gets the identifier of the country that issued the governmental information.
        /// </summary>
        public Guid CountryId { get; private set; }

        /// <summary>
        /// Gets the detailed governmental identification information, such as government ID and passport number.
        /// </summary>
        public GovernmentalInfoDetails Details { get; private set; }

        /// <summary>
        /// Gets the timestamp indicating when this record was created.
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets the timestamp indicating when this record was last updated, if any.
        /// </summary>
        public DateTime? UpdatedAt { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this record has been soft-deleted.
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GovernmentalInfo"/> class.
        /// </summary>
        /// <param name="governmentalInfoId">The unique identifier of the governmental info record.</param>
        /// <param name="personId">The identifier of the person associated with the info.</param>
        /// <param name="countryId">The identifier of the issuing country.</param>
        /// <param name="details">The detailed governmental identification information.</param>
        /// <param name="createdAt">The creation timestamp.</param>
        /// <param name="updatedAt">The last update timestamp, if any.</param>
        /// <param name="isDeleted">Indicates whether the record is soft-deleted.</param>
        private GovernmentalInfo(Guid governmentalInfoId, Guid personId, Guid countryId, GovernmentalInfoDetails details, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            GovernmentalInfoId = governmentalInfoId;
            PersonId = personId;
            CountryId = countryId;
            Details = details;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        /// <summary>
        /// Creates a new <see cref="GovernmentalInfo"/> instance with the specified identifiers and details.
        /// </summary>
        /// <remarks>
        /// This factory method ensures all required identifiers are valid non-empty GUIDs, 
        /// and that the provided details object is not null. The creation timestamp is set to <see cref="DateTime.UtcNow"/>.
        /// </remarks>
        /// <param name="governmentalInfoId">The unique identifier for the new record.</param>
        /// <param name="personId">The identifier of the person to associate with this record.</param>
        /// <param name="countryId">The identifier of the issuing country.</param>
        /// <param name="details">The detailed governmental identification information.</param>
        /// <returns>A fully initialized instance of <see cref="GovernmentalInfo"/>.</returns>
        /// <exception cref="GovernmentalInfoNotValidException">
        /// Thrown if any of the provided GUIDs are empty, or if the <paramref name="details"/> object is null.
        /// </exception>
        public static GovernmentalInfo Create(Guid governmentalInfoId, Guid personId, Guid countryId, GovernmentalInfoDetails details)
        {
            DomainValidator.ThrowIfEmptyGuid("GovernmentalInfoId", governmentalInfoId, new GovernmentalInfoNotValidException("Governmental Information ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new GovernmentalInfoNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new GovernmentalInfoNotValidException("Country ID cannot be empty."));
            DomainValidator.ThrowIfObjectNull("GovernmentalInfoDetails", details, new GovernmentalInfoNotValidException("Governmental Information details cannot be null."));

            return new GovernmentalInfo(governmentalInfoId, personId, countryId, details, DateTime.UtcNow, null, false);
        }

        /// <summary>
        /// Reconstructs an instance of <see cref="GovernmentalInfo"/> from persisted database values.
        /// </summary>
        /// <remarks>
        /// This method is used specifically when loading data from a data store (e.g., a database),
        /// bypassing domain creation timestamps and flags by accepting all persisted values directly. 
        /// It performs basic validation to ensure the integrity of critical fields before reconstruction.
        /// </remarks>
        /// <param name="governmentalInfoId">The unique identifier of the governmental information record.</param>
        /// <param name="personId">The identifier of the associated person.</param>
        /// <param name="countryId">The identifier of the issuing country.</param>
        /// <param name="governmentalInfoDetails">The detailed governmental information.</param>
        /// <param name="createdAt">The original creation timestamp of the record.</param>
        /// <param name="updatedAt">The timestamp of the last update, if any.</param>
        /// <param name="isDeleted">Indicates whether the record was soft-deleted in the database.</param>
        /// <returns>An instance of <see cref="GovernmentalInfo"/> reflecting the persisted state.</returns>
        /// <exception cref="GovernmentalInfoNotValidException">
        /// Thrown if any required GUIDs are empty or if the <paramref name="governmentalInfoDetails"/> object is null.
        /// </exception>

        public static GovernmentalInfo Reconstruct(Guid governmentalInfoId, Guid personId, Guid countryId, GovernmentalInfoDetails governmentalInfoDetails, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            DomainValidator.ThrowIfEmptyGuid("GovernmentalInfoId", governmentalInfoId, new GovernmentalInfoNotValidException("Governmental Information ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("PersonId", personId, new GovernmentalInfoNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfEmptyGuid("CountryId", countryId, new GovernmentalInfoNotValidException("Country ID cannot be empty."));
            DomainValidator.ThrowIfObjectNull("GovernmentalInfoDetails", governmentalInfoDetails, new GovernmentalInfoNotValidException("Governmental Information details cannot be null."));
            
            return new GovernmentalInfo(governmentalInfoId, personId, countryId, governmentalInfoDetails, createdAt, updatedAt, isDeleted);
        }

        /// <summary>
        /// Updates the governmental information details and issuing country.
        /// </summary>
        /// <remarks>
        /// This method replaces the existing <see cref="Details"/> and <see cref="CountryId"/> with the new values 
        /// and sets the <see cref="UpdatedAt"/> timestamp to the current UTC time.
        /// </remarks>
        /// <param name="newCountryId">The new country ID to associate with this record.</param>
        /// <param name="newDetails">The updated governmental information details.</param>
        /// <exception cref="GovernmentalInfoNotValidException">
        /// Thrown if the provided <paramref name="newCountryId"/> is an empty GUID or if <paramref name="newDetails"/> is null.
        /// </exception>
        public void Update(Guid newCountryId, GovernmentalInfoDetails newDetails)
        {
            DomainValidator.ThrowIfEmptyGuid("CountryId", newCountryId,
                new GovernmentalInfoNotValidException("Country ID cannot be empty."));

            DomainValidator.ThrowIfObjectNull("GovernmentalInfoDetails", newDetails,
                new GovernmentalInfoNotValidException("GovernmentalInfo details cannot be null."));

            CountryId = newCountryId;
            Details = newDetails;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Changes the associated person to the governmental information.
        /// </summary>
        /// <param name="newPersonId">New person ID to assign.</param>
        /// <exception cref="GovernmentalInfoNotValidException">
        /// Thrown if <paramref name="newPersonId"/> is empty or the info is deleted.
        /// </exception>
        public void ChangePerson(Guid newPersonId)
        {
            DomainValidator.ThrowIfEmptyGuid("PersonId", newPersonId,new GovernmentalInfoNotValidException("Person ID cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new GovernmentalInfoNotValidException("Operation not allowed: the governmental info has been deleted."));
            PersonId = newPersonId;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Changes the associated country that issued the governmental information.
        /// </summary>
        /// <param name="newCountryId">New country ID to assign.</param>
        /// <exception cref="GovernmentalInfoNotValidException">
        /// Thrown if <paramref name="newCountryId"/> is empty or the info is deleted.
        /// </exception>
        public void ChangeCountry(Guid newCountryId)
        {
            DomainValidator.ThrowIfEmptyGuid("CountryId", newCountryId,new GovernmentalInfoNotValidException("Country ID cannot be empty."));
            DomainValidator.ThrowIfDeleted(IsDeleted, new GovernmentalInfoNotValidException("Operation not allowed: the governmental info has been deleted."));
            CountryId = newCountryId;
            UpdatedAt = DateTime.UtcNow;
        }



        /// <summary>
        /// Marks the Governmental Info as deleted (soft delete).
        /// </summary>
        /// <exception cref="GovernmentalInfoNotValidException">Thrown if the info is already deleted.</exception>
        public void MarkAsDeleted()
        {
            if (IsDeleted)
                throw new GovernmentalInfoNotValidException("Address is already deleted.");
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }


        /// <summary>
        /// Restores a previously deleted info.
        /// </summary>
        /// <exception cref="GovernmentalInfoNotValidException">Thrown if the info is not marked as deleted.</exception>
        public void Restore()
        {
            if (!IsDeleted)
                throw new GovernmentalInfoNotValidException("Address is not deleted.");
            IsDeleted = false;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="GovernmentalInfo"/> instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current info.</param>
        /// <returns>
        /// <c>true</c> if the specified object is an <see cref="GovernmentalInfo"/> and has the same values for 
        /// all identity and value-based fields; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj is not GovernmentalInfo other)
                return false;
            return GovernmentalInfoId == other.GovernmentalInfoId &&
                   PersonId == other.PersonId &&
                   CountryId == other.CountryId &&
                   Details.Equals(other.Details) &&
                   CreatedAt == other.CreatedAt &&
                   UpdatedAt == other.UpdatedAt &&
                   IsDeleted == other.IsDeleted;
        }

        /// <summary>
        /// Serves as the default hash function for <see cref="GovernmentalInfo"/>.
        /// </summary>
        /// <returns>
        /// A hash code that represents the current info instance, computed from its identifying and value fields.
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(GovernmentalInfoId, PersonId, CountryId, Details, CreatedAt, UpdatedAt, IsDeleted);
        }

        /// <summary>
        /// Determines whether two <see cref="GovernmentalInfo"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="GovernmentalInfo"/> to compare.</param>
        /// <param name="right">The second <see cref="GovernmentalInfo"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if both <paramref name="left"/> and <paramref name="right"/> are equal; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(GovernmentalInfo? left, GovernmentalInfo? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="GovernmentalInfo"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="GovernmentalInfo"/> to compare.</param>
        /// <param name="right">The second <see cref="GovernmentalInfo"/> to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="left"/> and <paramref name="right"/> differ; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(GovernmentalInfo? left, GovernmentalInfo? right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}, IsDeleted: {IsDeleted}, Details: {Details}";
        }
    }

}
