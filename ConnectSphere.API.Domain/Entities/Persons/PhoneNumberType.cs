using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class PhoneNumberType
    {
        private const int MaxNameLength = 50;
        public Guid PhoneNumberTypeId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private PhoneNumberType(Guid phoneNumberTypeId, string name, string? description)
        {
            PhoneNumberTypeId = phoneNumberTypeId;
            Name = name;
            Description = description;
        }

        public static OperationResult<PhoneNumberType> Create(Guid phoneNumberTypeId, string name, string? description)
        {
            if (phoneNumberTypeId == Guid.Empty)
                return OperationResult<PhoneNumberType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PhoneNumberTypeId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<PhoneNumberType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Name cannot be empty.");
            if (name.Length > MaxNameLength)
                return OperationResult<PhoneNumberType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"Name cannot exceed {MaxNameLength} characters.");
            if (description != null && description.Length > 200)
                return OperationResult<PhoneNumberType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Description cannot exceed 200 characters.");
            
            return OperationResult<PhoneNumberType>.Success(new PhoneNumberType(phoneNumberTypeId, name, description));
        }
    }
}
