namespace ConnectSphere.API.Domain.Entities.Persons
{
    /// <summary>
    /// Represents a country entity that encapsulates a unique identifier and detailed information.
    /// </summary>
    /// <remarks>
    /// This class is used to store and manage a country’s identity and metadata, such as name, code,
    /// continent, capital, currency, and dialing code. Validation is enforced at creation and reconstruction.
    /// </remarks>
    public class Country
    {
        /// <summary>
        /// Gets the unique identifier for the country.
        /// </summary>
        public Guid CountryId { get; private set; }

        /// <summary>
        /// Gets or sets the detailed metadata of the country.
        /// </summary>
        /// <remarks>
        /// This includes properties like the country name, ISO code, continent, currency, and dialing code.
        /// </remarks>
        public CountryDetails Details { get; private set; }

        private Country(Guid countryId, CountryDetails countryDetails)
        {
            CountryId = countryId;
            Details = countryDetails;
        }

        /// <summary>
        /// Creates a new <see cref="Country"/> instance after validating the input values.
        /// </summary>
        /// <param name="countryId">The unique identifier for the country.</param>
        /// <param name="countryDetails">The detailed information about the country.</param>
        /// <returns>
        /// A new instance of <see cref="Country"/> with validated inputs.
        /// </returns>
        /// <remarks>
        /// Use this method when creating a new country that doesn't yet exist in the system.
        /// </remarks>
        /// <exception cref="CountryNotValidException">
        /// Thrown if <paramref name="countryId"/> is an empty GUID or if <paramref name="countryDetails"/> is null.
        /// </exception>
        public static Country Create(Guid countryId, CountryDetails countryDetails)
        {
            DomainValidator.ThrowIfEmptyGuid("countryId", countryId, new CountryNotValidException("CountryId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("countryDetails", countryDetails, new CountryNotValidException("CountryDetails cannot be null."));

            return new Country(countryId, countryDetails);
        }

        /// <summary>
        /// Reconstructs a <see cref="Country"/> instance from persisted values (e.g., from a database).
        /// </summary>
        /// <param name="countryId">The unique identifier for the country.</param>
        /// <param name="countryDetails">The existing detailed information of the country.</param>
        /// <returns>
        /// A reconstructed <see cref="Country"/> object using historical data.
        /// </returns>
        /// <remarks>
        /// This method is typically used to recreate domain entities from storage without triggering new creation logic.
        /// </remarks>
        /// <exception cref="CountryNotValidException">
        /// Thrown if <paramref name="countryId"/> is an empty GUID or if <paramref name="countryDetails"/> is null.
        /// </exception>
        public static Country Reconstruct(Guid countryId, CountryDetails countryDetails)
        {
            DomainValidator.ThrowIfEmptyGuid("countryId", countryId, new CountryNotValidException("CountryId cannot be empty."));
            DomainValidator.ThrowIfObjectNull("countryDetails", countryDetails, new CountryNotValidException("CountryDetails cannot be null."));

            return new Country(countryId, countryDetails);
        }
    }
}
