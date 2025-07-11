using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos.Responses
{
    public record GovernmentalInfoResponseDto
    {
        public Guid GovernmentalInfoId { get; set; }
        public Guid PersonId { get; set; }
        public Guid CountryId { get; set; }
        public string? GovIdNumber { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
