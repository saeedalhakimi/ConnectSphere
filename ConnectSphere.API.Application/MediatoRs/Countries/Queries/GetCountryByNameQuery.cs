using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Countries.Queries
{
    public class GetCountryByNameQuery : IRequest<OperationResult<CountryResponseDto>>
    {
        public string Name { get; set; }
        public string? CorrelationId { get; set; }
        public GetCountryByNameQuery(string countryName, string? correlationId = null)
        {
            Name = countryName ?? throw new ArgumentNullException(nameof(countryName));
            CorrelationId = correlationId;
        }
    }
}
