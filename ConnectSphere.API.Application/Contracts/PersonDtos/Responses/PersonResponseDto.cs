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
        string LastName,
        string? Suffix,
        Guid PersonTypeId, //turn this type name
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        bool IsDeleted);
}
