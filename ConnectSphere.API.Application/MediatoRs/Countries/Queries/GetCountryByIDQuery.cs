using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Countries.Queries
{
    public class GetCountryByIDQuery : IRequest<OperationResult<CountryResponseDto>>
    {
        public Guid CountryId { get; set; }
        public string? CorrelationId { get; }

        public GetCountryByIDQuery(Guid countryId, string? correlationId)
        {
            CountryId = countryId;
            CorrelationId = correlationId; 
        }
    }
}
