using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class PersonType
    {
        private const int MaxNameLength = 50;
        public Guid PersonTypeId { get; private set; }
        public string Name { get; private set; }
        private PersonType(Guid personTypeId, string name)
        {
            PersonTypeId = personTypeId;
            Name = name;
        }

        public static OperationResult<PersonType> Create(Guid personTypeId, string name)
        {
            if (personTypeId == Guid.Empty)
                return OperationResult<PersonType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonTypeId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<PersonType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonTypeName cannot be empty.");
            if (name.Length > MaxNameLength)
                return OperationResult<PersonType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"PersonTypeName cannot exceed {MaxNameLength} characters.");

            return OperationResult<PersonType>.Success(new PersonType(personTypeId, name));
        }
    }
}
