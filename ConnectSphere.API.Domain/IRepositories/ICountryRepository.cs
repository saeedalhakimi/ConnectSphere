using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Domain.IRepositories
{
    public interface ICountryRepository
    {
        /// <summary>
        /// Retrieves all countries with pagination.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of countries per page.</param>
        /// <param name="correlationId">The correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing a paged response of countries or errors.</returns>
        Task <OperationResult<IReadOnlyList<Country>>> GetAllAsyncWithPagination(int pageNumber, int pageSize, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all countries without pagination.
        /// </summary>
        /// <param name="correlationId" >Correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing a list of countries or errors.</returns>
        Task<OperationResult<IReadOnlyList<Country>>> GetAllAsync(string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a country by its GUID.
        /// </summary>  
        /// <param name="countryId"> The GUID of the country to retrieve.</param>
        /// <param name="correlationId">Correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the country or errors.</returns>
        Task<OperationResult<Country>> GetByIdAsync(Guid countryId, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a country by its code.
        /// </summary>
        /// <param name="countryCode"> The code of the country to retrieve.</param>
        /// <param name="correlationId">Correlation ID for tracing the operation.</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the country or errors.</returns>
        Task<OperationResult<Country>> GetByCodeAsync(string countryCode, string? correlationId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a country by its name.
        /// </summary>
        /// <param name="name"> The name of the country to retrieve.</param>
        /// <param name="correlationId" >Correlation ID for tracing the operation.</param>  
        /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
        /// <returns>An OperationResult containing the country or errors.</returns>
        Task<OperationResult<Country>> GetByNameAsync(string name, string? correlationId, CancellationToken cancellationToken);
    }
}
