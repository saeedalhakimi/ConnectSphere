using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.Commands
{
    public class UpdateCommand : IRequest<OperationResult<bool>>
    {
        public Guid PersonId { get; set; }
        public string? Title { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Suffix { get; set; }

        public string? CorrelationId { get; set; }

        public UpdateCommand(Guid personId, string? title, string firstName, string? middleName, string lastName, string? suffix, string? correlationId)
        {
            PersonId = personId;
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Suffix = suffix;
            CorrelationId = correlationId;
        }
    }
}
