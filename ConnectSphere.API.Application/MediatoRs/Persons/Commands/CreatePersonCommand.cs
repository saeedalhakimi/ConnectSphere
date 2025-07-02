using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.Commands
{
    public class CreatePersonCommand : IRequest<OperationResult<PersonResponseDto>>
    {
        public string? Title { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; } 
        public string LastName { get; set; }
        public string? Suffix { get; set; }
        public Guid PersonTypeID { get; set; }
        public string CorrelationId { get; set; }

        public CreatePersonCommand(string? title, string firstName, string? middleName, string lastName, string? suffix, Guid personTypeID, string correlationId)
        {
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Suffix = suffix;
            PersonTypeID = personTypeID;
            CorrelationId = correlationId;
        }
    }
}
