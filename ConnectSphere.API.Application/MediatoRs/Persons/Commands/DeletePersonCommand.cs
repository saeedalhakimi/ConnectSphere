using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.Commands
{
    public class DeletePersonCommand : IRequest<OperationResult<bool>>
    {
        public Guid PersonID { get; set; }
        public string? CorrelationId { get; set; }
        public DeletePersonCommand(Guid personID, string? correlationId)
        {
            PersonID = personID;
            CorrelationId = correlationId;
        }
    }
}
