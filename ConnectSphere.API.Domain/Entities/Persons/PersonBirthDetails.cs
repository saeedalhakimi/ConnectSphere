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
    public class PersonBirthDetails
    {
        public Guid PersonBirthDetailsId { get; private set; }
        public Guid PersonId { get; private set; }
        public BirthDetails Details { get; private set; }
        public Guid CountryId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private PersonBirthDetails(Guid personBirthDetailsId, Guid personId, BirthDetails details, Guid countryId, DateTime createdAt, DateTime? updatedAt, bool isDeleted)
        {
            PersonBirthDetailsId = personBirthDetailsId;
            PersonId = personId;
            Details = details;
            CountryId = countryId;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }

        public static OperationResult<PersonBirthDetails> Create(Guid personBirthDetailsId, Guid personId, BirthDetails details, Guid countryId)
        {
            if (personBirthDetailsId == Guid.Empty)
                return OperationResult<PersonBirthDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonBirthDetailsId cannot be empty.");
            if (personId == Guid.Empty)
                return OperationResult<PersonBirthDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PersonId cannot be empty.");
            if (details == null)
                return OperationResult<PersonBirthDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "Birth details cannot be null.");
            if (countryId == Guid.Empty)
                return OperationResult<PersonBirthDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "CountryId cannot be empty.");

            return OperationResult<PersonBirthDetails>.Success(new PersonBirthDetails(personBirthDetailsId, personId, details, countryId, DateTime.UtcNow, null, false));
        }
    }
}
