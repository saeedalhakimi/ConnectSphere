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
    public class GetCountryByCodeQuery : IRequest<OperationResult<CountryResponseDto>>
    {
        public string CountryCode { get; set; }
        public string? CorrelationId { get; set; }

        public GetCountryByCodeQuery(string countryCode, string? correlationId)
        {
            CountryCode = countryCode;
            CorrelationId = correlationId;
        }
    }
}
