using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class AddressType
    {
        private const int MaxNameLength = 50;
        public Guid AddressTypeId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private AddressType(Guid addressTypeId, string name, string? description)
        {
            AddressTypeId = addressTypeId;
            Name = name;
            Description = description;
        }

        public static OperationResult<AddressType> Create(Guid addressTypeId, string name, string? description)
        {
            if (addressTypeId == Guid.Empty)
                return OperationResult<AddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "AddressTypeId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<AddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Name cannot be empty.");
            if (name.Length > MaxNameLength)
                return OperationResult<AddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"Name cannot exceed {MaxNameLength} characters.");
            if (description != null && description.Length > 200)
                return OperationResult<AddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"Description cannot exceed 200 characters.");
            
            return OperationResult<AddressType>.Success(new AddressType(addressTypeId, name, description));
        }
    }
}
