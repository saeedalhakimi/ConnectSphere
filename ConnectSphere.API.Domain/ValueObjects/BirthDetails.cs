namespace ConnectSphere.API.Domain.ValueObjects
{
    /// <summary>
    /// Represents the birth-related information of a person, including birth date and optional birth city.
    /// </summary>
    /// <remarks>
    /// This value object encapsulates a person's date of birth and the city where they were born. 
    /// It enforces domain rules such as disallowing future birth dates, excessively old birth dates,
    /// and limiting the length of the birth city name.
    /// </remarks>
    public record BirthDetails
    {
        private const int MaxBirthCityLength = 100;

        /// <summary>
        /// Gets the person's birth date. Must not be in the future or unrealistically old (before 1900).
        /// </summary>
        public DateTime BirthDate { get; init; }

        /// <summary>
        /// Gets the city where the person was born. This field is optional and limited to 100 characters if provided.
        /// </summary>
        public string? BirthCity { get; init; }


        private BirthDetails(DateTime birthDate, string? birthCity)
        {
            BirthDate = birthDate;
            BirthCity = birthCity;
        }

        /// <summary>
        /// Creates a new instance of <see cref="BirthDetails"/> with validated birth information.
        /// </summary>
        /// <remarks>
        /// This factory method validates the provided birth date to ensure it is not in the future and not earlier than the year 1900.
        /// It also validates that the optional birth city name does not exceed the allowed maximum length of 100 characters.
        /// </remarks>
        /// <param name="birthDate">The date of birth. Cannot be in the future or before 1900.</param>
        /// <param name="birthCity">The name of the city where the person was born. Optional and limited to 100 characters.</param>
        /// <returns>A validated <see cref="BirthDetails"/> instance.</returns>
        /// <exception cref="BirthDetailsNotValidException">
        /// Thrown if the birth date is invalid or the birth city exceeds the maximum length.
        /// </exception>
        public static BirthDetails Create(DateTime birthDate, string? birthCity)
        {
            
            DomainValidator.ThrowIfBirthDateInFuture(birthDate, new BirthDetailsNotValidException("Birth date cannot be in the future."));
            DomainValidator.ThrowIfBirthDateTooOld(birthDate, new DateTime(1900, 1, 1), new BirthDetailsNotValidException("Birth date cannot be before 1900."));
            DomainValidator.ThrowIfExceedsMaxLength("BirthCity", birthCity, MaxBirthCityLength, new BirthDetailsNotValidException("Birth city cannot exceed 100 characters."));

            return new BirthDetails(birthDate, birthCity);
        }
    }
}
