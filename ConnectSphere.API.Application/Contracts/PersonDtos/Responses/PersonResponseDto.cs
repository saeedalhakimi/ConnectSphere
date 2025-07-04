using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.PersonDtos.Responses
{
    public record PersonResponseDto(
        Guid PersonId,
        string? Title,
        string FirstName,
        string? MiddleName,
        string LastName,
        string? Suffix,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        bool IsDeleted);
}
