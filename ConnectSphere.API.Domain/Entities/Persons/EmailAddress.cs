using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.Entities.Persons
{
    public class EmailAddress
    {
        public Guid EmailAddressId { get; private set; }
        public Guid PersonId { get; private set; }
        public Guid EmailAddressTypeId { get; private set; }
        public Email Email { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private EmailAddress(Guid emailAddressId, Guid personId, Guid emailAddressTypeId, Email email, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            EmailAddressId = emailAddressId;
            PersonId = personId;
            EmailAddressTypeId = emailAddressTypeId;
            Email = email;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        public static OperationResult<EmailAddress> Create(Guid emailAddressId, Guid personId, Guid emailAddressTypeId, Email email)
        {
            if (emailAddressId == Guid.Empty)
                return OperationResult<EmailAddress>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "EmailAddressId cannot be empty.");
            if (personId == Guid.Empty)
                return OperationResult<EmailAddress>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (emailAddressTypeId == Guid.Empty)
                return OperationResult<EmailAddress>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "EmailAddressTypeId cannot be empty.");
            if (email == null)
                return OperationResult<EmailAddress>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Email cannot be null.");

            return OperationResult<EmailAddress>.Success(new EmailAddress(emailAddressId, personId, emailAddressTypeId, email, DateTime.UtcNow, null, false));
        }
    }
}
