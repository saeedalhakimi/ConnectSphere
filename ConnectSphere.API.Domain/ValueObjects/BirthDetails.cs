using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    public record BirthDetails
    {
        public DateTime BirthDate { get; init; }
        public string? BirthCity { get; init; }

        private BirthDetails(DateTime birthDate, string? birthCity)
        {
            BirthDate = birthDate;
            BirthCity = birthCity;
        }

        public static OperationResult<BirthDetails> Create(DateTime birthDate, string? birthCity)
        {
            if (birthDate > DateTime.UtcNow)
                return OperationResult<BirthDetails>.Failure(ErrorCode.InvalidData, "Birth date cannot be in the future.");
            if (birthDate < new DateTime(1900, 1, 1))
                return OperationResult<BirthDetails>.Failure(ErrorCode.InvalidData, "Birth date cannot be before 1900.");
            if (birthCity != null && birthCity.Length > 100)
                return OperationResult<BirthDetails>.Failure(ErrorCode.InvalidInput, "Birth city cannot exceed 100 characters.");

            return OperationResult<BirthDetails>.Success(new BirthDetails(birthDate, birthCity));
        }
    }
}
