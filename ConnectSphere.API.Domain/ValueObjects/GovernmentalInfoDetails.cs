using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.ValueObjects
{
    public record GovernmentalInfoDetails
    {
        private const int MaxGovIdNumberLength = 50; // Maximum length for a government ID number
        public string? GovIdNumber { get; init; }
        public string? PassportNumber { get; init; }

        private GovernmentalInfoDetails(string? govIdNumber, string? passportNumber)
        {
            GovIdNumber = govIdNumber;
            PassportNumber = passportNumber;
        }

        public static OperationResult<GovernmentalInfoDetails> Create(string? govIdNumber, string? passportNumber)
        {
            if (govIdNumber != null && string.IsNullOrWhiteSpace(govIdNumber))
                return OperationResult<GovernmentalInfoDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "GovIdNumber cannot be empty if provided.");
            if (govIdNumber != null && govIdNumber.Length > MaxGovIdNumberLength)
                return OperationResult<GovernmentalInfoDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"GovIdNumber cannot exceed {MaxGovIdNumberLength} characters.");
            if (passportNumber != null && string.IsNullOrWhiteSpace(passportNumber))
                return OperationResult<GovernmentalInfoDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", "PassportNumber cannot be empty if provided.");
            if (passportNumber != null && passportNumber.Length > MaxGovIdNumberLength)
                return OperationResult<GovernmentalInfoDetails>.Failure(ErrorCode.InvalidInput, "INVALID_INPUT", $"PassportNumber cannot exceed {MaxGovIdNumberLength} characters.");

            return OperationResult<GovernmentalInfoDetails>.Success(new GovernmentalInfoDetails(govIdNumber, passportNumber));
        }
    }

}
