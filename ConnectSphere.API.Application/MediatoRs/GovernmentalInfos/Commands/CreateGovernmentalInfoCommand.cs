using ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos.Responses;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.GovernmentalInfos.Commands
{
    public class CreateGovernmentalInfoCommand : IRequest<OperationResult<GovernmentalInfoResponseDto>>
    {
        public Guid PersonId { get; set; }
        public Guid CountryId { get; set; }
        public string? GovIdNumber { get; set; }
        public string? PassportNumber { get; set; }
        public string? CorrelationId { get; set; }
        public CreateGovernmentalInfoCommand(Guid personId, Guid countryId, string? govIdNumber, string? passportNumber, string? correlationId = null)
        {
            PersonId = personId;
            CountryId = countryId;
            GovIdNumber = govIdNumber;
            PassportNumber = passportNumber;
            CorrelationId = correlationId;
        }
    }
}
