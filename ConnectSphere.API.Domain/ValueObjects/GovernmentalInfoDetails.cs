namespace ConnectSphere.API.Domain.ValueObjects
{
    /// <summary>
    /// Represents detailed governmental information, including optional government ID and passport numbers.
    /// </summary>
    /// <remarks>
    /// This record is used to encapsulate governmental identification details, such as a government
    /// ID number and a passport number. Both fields are optional and can be null or empty. If provided,
    /// they must adhere to specific validation rules, such as maximum length constraints.
    /// </remarks>
    public record GovernmentalInfoDetails
    {
        /// <summary>
        /// The maximum allowed length for government ID and passport numbers.
        /// </summary>
        private const int MaxLength = 50;

        /// <summary>
        /// Gets the government ID number. This value is optional and may be null or empty.
        /// </summary>
        public string? GovIdNumber { get; init; }

        /// <summary>
        /// Gets the passport number. This value is optional and may be null or empty.
        /// </summary>
        public string? PassportNumber { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GovernmentalInfoDetails"/> record.
        /// </summary>
        /// <param name="govIdNumber">Optional government ID number. Can be null or empty.</param>
        /// <param name="passportNumber">Optional passport number. Can be null or empty.</param>
        private GovernmentalInfoDetails(string? govIdNumber, string? passportNumber)
        {
            GovIdNumber = govIdNumber;
            PassportNumber = passportNumber;
        }

        /// <summary>
        /// Creates a new instance of <see cref="GovernmentalInfoDetails"/>.
        /// </summary>
        /// <remarks>
        /// This factory method ensures that any provided government ID or passport number adheres to validation rules, 
        /// including maximum length constraints. If either value is supplied and fails validation, an exception is thrown.
        /// </remarks>
        /// <param name="govIdNumber">Optional government ID number. Can be null, but if provided, must not exceed the maximum length.</param>
        /// <param name="passportNumber">Optional passport number. Can be null, but if provided, must not exceed the maximum length.</param>
        /// <returns>A new instance of <see cref="GovernmentalInfoDetails"/> containing the provided values.</returns>
        /// <exception cref="GovernmentalInfoNotValidException">
        /// Thrown when either <paramref name="govIdNumber"/> or <paramref name="passportNumber"/> exceeds the allowed length.
        /// </exception>
        public static GovernmentalInfoDetails Create(string? govIdNumber, string? passportNumber)
        {
            DomainValidator.ThrowIfExceedsMaxLength("GovIdNumber", govIdNumber, MaxLength,
                new GovernmentalInfoNotValidException($"If provided, Government Id Number cannot exceed {MaxLength} characters."));
            DomainValidator.ThrowIfExceedsMaxLength("PassportNumber", passportNumber, MaxLength,
                new GovernmentalInfoNotValidException($"If provided, Passport Number cannot be empty or exceed {MaxLength} characters."));

            return new GovernmentalInfoDetails(govIdNumber?.Trim(), passportNumber?.Trim());
        }
    }
}
