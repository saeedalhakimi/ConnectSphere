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
                CountryCode = country.CountryCode,
                Name = country.Name,
                Continent = country.Continent,
                Capital = country.Capital,
                CurrencyCode = country.CurrencyCode,
                CountryDialNumber = country.CountryDialNumber
            };
        }

    }
}
