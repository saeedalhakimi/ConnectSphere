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
    public class GovernmentalInfo
    {
        public Guid GovernmentalInfoId { get; private set; }
        public Guid PersonId { get; private set; }
        public Guid CountryId { get; private set; }
        public GovernmentalInfoDetails Details { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private GovernmentalInfo(Guid governmentalInfoId, Guid personId, Guid countryId, GovernmentalInfoDetails details, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            GovernmentalInfoId = governmentalInfoId;
            PersonId = personId;
            CountryId = countryId;
            Details = details;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        public static OperationResult<GovernmentalInfo> Create(Guid governmentalInfoId, Guid personId, Guid countryId, GovernmentalInfoDetails details)
        {
            if (governmentalInfoId == Guid.Empty)
                return OperationResult<GovernmentalInfo>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "GovernmentalInfoId cannot be empty.");
            if (personId == Guid.Empty)
                return OperationResult<GovernmentalInfo>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (countryId == Guid.Empty)
                return OperationResult<GovernmentalInfo>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "CountryId cannot be empty.");
            if (details == null)
                return OperationResult<GovernmentalInfo>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Governmental info details cannot be null.");

            return OperationResult<GovernmentalInfo>.Success(new GovernmentalInfo(governmentalInfoId, personId, countryId, details, DateTime.UtcNow, null, false));
        }
    }
}
