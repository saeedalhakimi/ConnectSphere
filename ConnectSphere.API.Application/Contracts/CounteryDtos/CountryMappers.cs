using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.CounteryDtos
{
    public static class CountryMappers
    {
        public static CountryResponseDto ToResponseDto(Country country)
        {
            return new CountryResponseDto
            {
                CountryId = country.CountryId,
                CountryCode = country.Details.CountryCode,
                Name = country.Details.Name,
                Continent = country.Details.Continent,
                Capital = country.Details.Capital,
                CurrencyCode = country.Details.CurrencyCode,
                CountryDialNumber = country.Details.CountryDialNumber
            };
        }

    }
}
