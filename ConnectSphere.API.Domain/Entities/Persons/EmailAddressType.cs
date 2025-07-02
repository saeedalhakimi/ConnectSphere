using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class EmailAddressType
    {
        private const int MaxNameLength = 50;
        public Guid EmailAddressTypeId { get; private set; }
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private EmailAddressType(Guid emailAddressTypeId, string name, string? description)
        {
            EmailAddressTypeId = emailAddressTypeId;
            Name = name;
            Description = description;
        }

        public static OperationResult<EmailAddressType> Create(Guid emailAddressTypeId, string name, string? description)
        {
            if (emailAddressTypeId == Guid.Empty)
                return OperationResult<EmailAddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "EmailAddressTypeId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name))
                return OperationResult<EmailAddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Name cannot be empty.");
            if (name.Length > MaxNameLength)
                return OperationResult<EmailAddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"Name cannot exceed {MaxNameLength} characters.");
            if (description != null && description.Length > 200)
                return OperationResult<EmailAddressType>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Description cannot exceed 200 characters.");
            
            return OperationResult<EmailAddressType>.Success(new EmailAddressType(emailAddressTypeId, name, description));
        }
    }
}
