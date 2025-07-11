using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.CounteryDtos.Responses
{
    public record CountryResponseDto
    {
        public Guid CountryId { get; init; }
        public string CountryCode { get; init; }
        public string Name { get; init; }
        public string? Continent { get; init; }
        public string? Capital { get; init; }
        public string? CurrencyCode { get; init; }
        public string? CountryDialNumber { get; init; }
    }
}
