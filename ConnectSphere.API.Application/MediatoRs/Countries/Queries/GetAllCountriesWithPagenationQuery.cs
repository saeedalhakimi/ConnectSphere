using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Application.Models;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Countries.Queries
{
    public class GetAllCountriesWithPagenationQuery : IRequest<OperationResult<PagedResponse<CountryResponseDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? CorrelationId { get; }
        public GetAllCountriesWithPagenationQuery(int pageNumber, int pageSize, string? correlationId)
        {
            PageNumber = pageNumber > 0 ? pageNumber : 1; // Ensure page number is at least 1
            PageSize = pageSize > 0 ? pageSize : 10; // Ensure page size is at least 1
            CorrelationId = correlationId ?? throw new ArgumentNullException(nameof(correlationId));
        }
    }
}
