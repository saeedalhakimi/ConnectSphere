using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos.Requests
{
    public record CreateGovernmentalInfoDto
    {
        [Required(ErrorMessage = "Country Id is required")]
        public Guid CountryId { get; init; }
        public string? GovIdNumber { get; init; }
        public string? PassportNumber { get; init; }
    }
}
