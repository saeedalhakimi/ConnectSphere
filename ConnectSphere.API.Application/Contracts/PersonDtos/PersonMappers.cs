using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.PersonDtos
{
    public static class PersonMappers
    {
        public static PersonResponseDto ToResponseDto(this Domain.Aggregate.Person person)
        {
            if (person == null) throw new ArgumentNullException(nameof(person));
            return new PersonResponseDto(
                person.PersonId,
                person.Name.Title,
                person.Name.FirstName,
                person.Name.MiddleName,
                person.Name.LastName,
                person.Name.Suffix,
                person.CreatedAt,
                person.UpdatedAt,
                person.IsDeleted);
        }
    }
}
