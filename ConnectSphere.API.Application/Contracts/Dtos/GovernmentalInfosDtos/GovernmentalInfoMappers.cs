using ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos.Responses;
using ConnectSphere.API.Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.Contracts.Dtos.GovernmentalInfosDtos
{
    public static class GovernmentalInfoMappers
    {
        public static GovernmentalInfoResponseDto ToResponseDto(GovernmentalInfo governmentalInfo)
        {
            return new GovernmentalInfoResponseDto
            {
                GovernmentalInfoId = governmentalInfo.GovernmentalInfoId,
                PersonId = governmentalInfo.PersonId,
                CountryId = governmentalInfo.CountryId,
                GovIdNumber = governmentalInfo.Details.GovIdNumber,
                PassportNumber = governmentalInfo.Details.PassportNumber,
                CreatedAt = governmentalInfo.CreatedAt,
                UpdatedAt = governmentalInfo.UpdatedAt,
                IsDeleted = governmentalInfo.IsDeleted
            };
        }
    }
}
