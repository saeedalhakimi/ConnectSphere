using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.IRepositories
{
    public interface IGovernmentalInfoRepository
    {
        /// <summary>
        /// Creates a new GovernmentalInfo entity in Database.
        /// </summary>  
        ///<param name="governmentalInfo">The craeted GovernmentalInfo entity to save.</param>
        /// <param name="correlationId" >Optional correlation ID for tracking the request.</param>
        /// <param name="cancellationToken" >Cancellation token to cancel the operation.</param>
        /// <returns> A task that represents the asynchronous operation, containing the result of the operation.</returns>
        Task<OperationResult<bool>> CreateAsync(GovernmentalInfo governmentalInfo, string? correlationId, CancellationToken cancellationToken);
    }
}
