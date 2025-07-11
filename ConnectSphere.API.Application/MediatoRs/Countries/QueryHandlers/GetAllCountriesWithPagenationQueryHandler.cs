using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Application.MediatoRs.Countries.Queries;
using ConnectSphere.API.Application.Models;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Countries.QueryHandlers
{
    public class GetAllCountriesWithPagenationQueryHandler : IRequestHandler<GetAllCountriesWithPagenationQuery, OperationResult<PagedResponse<CountryResponseDto>>>
    {
        public async Task<OperationResult<PagedResponse<CountryResponseDto>>> Handle(GetAllCountriesWithPagenationQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
