using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Application.Models;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.Queries
{
    public class GetAllPersonsQuery : IRequest<OperationResult<PagedResponse<PersonResponseDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string CorrelationId { get; set; }
        public GetAllPersonsQuery(int pageNumber = 1, int pageSize = 10, string correlationId = "")
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            CorrelationId = correlationId;
        }
    }
}
