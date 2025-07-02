using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class Country
    {
        public Guid CountryId { get; private set; }
        public string CountryCode { get; private set; }
        public string Name { get; private set; }
        public string? Continent { get; private set; }
        public string? Capital { get; private set; }
        public string? CurrencyCode { get; private set; }
        public string? CountryDialNumber { get; private set; }

        private Country(Guid countryId, string countryCode, string name, string? continent, string? capital, string? currencyCode, string? countryDialNumber)
        {
            CountryId = countryId;
            CountryCode = countryCode;
            Name = name;
            Continent = continent;
            Capital = capital;
            CurrencyCode = currencyCode;
            CountryDialNumber = countryDialNumber;
        }

        public static OperationResult<Country> Create(Guid countryId, string countryCode, string name, string? continent, string? capital, string? currencyCode, string? countryDialNumber)
        {
            if (countryId == Guid.Empty)
                return OperationResult<Country>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "CountryId cannot be empty.");
            if (string.IsNullOrWhiteSpace(countryCode))
                return OperationResult<Country>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "CountryCode cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<Country>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Name cannot be empty.");

            return OperationResult<Country>.Success(new Country(countryId, countryCode, name, continent, capital, currencyCode, countryDialNumber));
        }
    }
}
