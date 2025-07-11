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
    public class GetAllCountriesQuery : IRequest<OperationResult<IReadOnlyList<CountryResponseDto>>>
    {
        public string? CorrelationId { get; }

        public GetAllCountriesQuery(string? correlationId)
        {
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        }


    }
}
