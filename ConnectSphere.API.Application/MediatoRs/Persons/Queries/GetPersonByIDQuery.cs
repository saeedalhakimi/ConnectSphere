using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.Queries
{
    public class GetPersonByIDQuery : IRequest<OperationResult<PersonResponseDto>>
    {
        public Guid PersonId { get; set; }
        public string? CorrelationId { get; set; }
        public GetPersonByIDQuery(Guid personID, string? correlationId)
        {
            PersonId = personID;
            CorrelationId = correlationId;
        }
    }
}
