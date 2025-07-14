using ConnectSphere.API.Application.DomainRepositories.IRepositories;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.ValueObjects;
using ConnectSphere.API.Infrastructure.Data.DataWrapperFactory;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IAppLogger<CountryRepository> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly ISystemClocking _systemClocking;
        private readonly string _connectionString;

        public CountryRepository(IConfiguration configuration,
            IAppLogger<CountryRepository> logger,
            IErrorHandlingService errorHandlingService,
            IDatabaseConnectionFactory connectionFactory,
            ISystemClocking systemClocking,
            string connectionString = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _systemClocking = systemClocking ?? throw new ArgumentNullException(nameof(systemClocking));
            _connectionString = connectionString ?? configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<OperationResult<IReadOnlyList<Country>>> GetAllAsync(string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting fetch process for all countries");
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetAllCountriesNoPagination";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: CorrelationId={CorrelationId}", correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var countries = new List<Country>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    var countryDetails = CountryDetails.Create(
                        reader.GetString(reader.GetOrdinal("CountryCode")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.IsDBNull(reader.GetOrdinal("Continent")) ? null : reader.GetString(reader.GetOrdinal("Continent")),
                        reader.IsDBNull(reader.GetOrdinal("Capital")) ? null : reader.GetString(reader.GetOrdinal("Capital")),
                        reader.IsDBNull(reader.GetOrdinal("CurrencyCode")) ? null : reader.GetString(reader.GetOrdinal("CurrencyCode")),
                        reader.IsDBNull(reader.GetOrdinal("CountryDialNumber")) ? null : reader.GetString(reader.GetOrdinal("CountryDialNumber")));

                    var country = Country.Reconstruct(
                        reader.GetGuid(reader.GetOrdinal("CountryId")), countryDetails);

                    countries.Add(country);
                }

                _logger.LogInformation("Fetched {Count} countries successfully", countries.Count);
                return OperationResult<IReadOnlyList<Country>>.Success(countries.AsReadOnly());
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Fetch countries operation was canceled. Exception: {Exception}", ex.Message);
                return _errorHandlingService.HandleCancelationToken<IReadOnlyList<Country>>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching countries");
                return _errorHandlingService.HandleException<IReadOnlyList<Country>>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching countries");
                return _errorHandlingService.HandleException<IReadOnlyList<Country>>(ex, correlationId);
            }
        }
        public async Task<OperationResult<IReadOnlyList<Country>>> GetAllAsyncWithPagination(int pageNumber, int pageSize, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting fetch process for countries with PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetAllCountries";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PageNumber", pageNumber);
                command.AddParameter("@PageSize", pageSize);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: PageNumber={PageNumber}, PageSize={PageSize}, CorrelationId={CorrelationId}",
                    pageNumber, pageSize, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var countries = new List<Country>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    var countryDetails = CountryDetails.Create(
                        reader.GetString(reader.GetOrdinal("CountryCode")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.IsDBNull(reader.GetOrdinal("Continent")) ? null : reader.GetString(reader.GetOrdinal("Continent")),
                        reader.IsDBNull(reader.GetOrdinal("Capital")) ? null : reader.GetString(reader.GetOrdinal("Capital")),
                        reader.IsDBNull(reader.GetOrdinal("CurrencyCode")) ? null : reader.GetString(reader.GetOrdinal("CurrencyCode")),
                        reader.IsDBNull(reader.GetOrdinal("CountryDialNumber")) ? null : reader.GetString(reader.GetOrdinal("CountryDialNumber")));

                    var country = Country.Reconstruct(
                        reader.GetGuid(reader.GetOrdinal("CountryId")), countryDetails);

                    countries.Add(country);
                }

                _logger.LogInformation("Fetched {Count} countries successfully for PageNumber: {PageNumber}, PageSize: {PageSize}", countries.Count, pageNumber, pageSize);
                return OperationResult<IReadOnlyList<Country>>.Success(countries.AsReadOnly());
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Fetch countries operation was canceled for PageNumber: {PageNumber}, PageSize: {PageSize}. Exception: {Exception}", pageNumber, pageSize, ex.Message);
                return _errorHandlingService.HandleCancelationToken<IReadOnlyList<Country>>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching countries for PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return _errorHandlingService.HandleException<IReadOnlyList<Country>>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching countries for PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return _errorHandlingService.HandleException<IReadOnlyList<Country>>(ex, correlationId);
            }
        }
        public async Task<OperationResult<Country>> GetByCodeAsync(string countryCode, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting country fetch process for CountryCode: {CountryCode}", countryCode);
            try
            {
                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    _logger.LogWarning("Invalid CountryCode: {CountryCode}", countryCode);
                    return OperationResult<Country>.Failure(new Error(
                        ErrorCode.InvalidInput,
                        "INVALID_INPUT",
                        "CountryCode cannot be empty.",
                        correlationId));
                }

                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetCountryByCode";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@CountryCode", countryCode);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: CountryCode={CountryCode}, CorrelationId={CorrelationId}",
                    countryCode, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    var countryDetails = CountryDetails.Create(
                        reader.GetString(reader.GetOrdinal("CountryCode")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.IsDBNull(reader.GetOrdinal("Continent")) ? null : reader.GetString(reader.GetOrdinal("Continent")),
                        reader.IsDBNull(reader.GetOrdinal("Capital")) ? null : reader.GetString(reader.GetOrdinal("Capital")),
                        reader.IsDBNull(reader.GetOrdinal("CurrencyCode")) ? null : reader.GetString(reader.GetOrdinal("CurrencyCode")),
                        reader.IsDBNull(reader.GetOrdinal("CountryDialNumber")) ? null : reader.GetString(reader.GetOrdinal("CountryDialNumber")));
                   
                    var countryResult = Country.Reconstruct(
                        reader.GetGuid(reader.GetOrdinal("CountryId")), countryDetails);

                    _logger.LogInformation("Country fetched successfully for CountryCode: {CountryCode}", countryCode);
                    return OperationResult<Country>.Success(countryResult);
                }

                _logger.LogWarning("Country not found for CountryCode: {CountryCode}", countryCode);
                return OperationResult<Country>.Failure(new Error(
                    ErrorCode.NotFound,
                    "NOT_FOUND",
                    $"Country with Code {countryCode} not found.",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Country fetch operation was canceled for CountryCode: {CountryCode}. Exception: {Exception}", countryCode, ex.Message);
                return _errorHandlingService.HandleCancelationToken<Country>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching country for CountryCode: {CountryCode}", countryCode);
                return _errorHandlingService.HandleException<Country>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching country for CountryCode: {CountryCode}", countryCode);
                return _errorHandlingService.HandleException<Country>(ex, correlationId);
            }
        }
        public async Task<OperationResult<Country>> GetByIdAsync(Guid countryId, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting country fetch process for CountryId: {CountryId}", countryId);
            try
            {
                if (countryId == Guid.Empty)
                {
                    _logger.LogWarning("Invalid CountryId: {CountryId}", countryId);
                    return OperationResult<Country>.Failure(new Error(
                        ErrorCode.InvalidInput,
                        "INVALID_INPUT",
                        "CountryId cannot be empty.",
                        correlationId));
                }

                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetCountryById";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@CountryId", countryId);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: CountryId={CountryId}, CorrelationId={CorrelationId}",
                    countryId, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    var countryDetails = CountryDetails.Create(
                        reader.GetString(reader.GetOrdinal("CountryCode")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.IsDBNull(reader.GetOrdinal("Continent")) ? null : reader.GetString(reader.GetOrdinal("Continent")),
                        reader.IsDBNull(reader.GetOrdinal("Capital")) ? null : reader.GetString(reader.GetOrdinal("Capital")),
                        reader.IsDBNull(reader.GetOrdinal("CurrencyCode")) ? null : reader.GetString(reader.GetOrdinal("CurrencyCode")),
                        reader.IsDBNull(reader.GetOrdinal("CountryDialNumber")) ? null : reader.GetString(reader.GetOrdinal("CountryDialNumber")));

                    var countryResult = Country.Reconstruct(
                        reader.GetGuid(reader.GetOrdinal("CountryId")), countryDetails);

                    _logger.LogInformation("Country fetched successfully for CountryId: {CountryId}", countryId);
                    return OperationResult<Country>.Success(countryResult);
                }

                _logger.LogWarning("Country not found for CountryId: {CountryId}", countryId);
                return OperationResult<Country>.Failure(new Error(
                    ErrorCode.NotFound,
                    "NOT_FOUND",
                    $"Country with ID {countryId} not found.",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Country fetch operation was canceled for CountryId: {CountryId}. Exception: {Exception}", countryId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<Country>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching country for CountryId: {CountryId}", countryId);
                return _errorHandlingService.HandleException<Country>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching country for CountryId: {CountryId}", countryId);
                return _errorHandlingService.HandleException<Country>(ex, correlationId);
            }
        }
        public async Task<OperationResult<Country>> GetByNameAsync(string name, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting country fetch process for Name: {Name}", name);
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Invalid Country Name: {Name}", name);
                    return OperationResult<Country>.Failure(new Error(
                        ErrorCode.InvalidInput,
                        "INVALID_INPUT",
                        "Country Name cannot be empty.",
                        correlationId));
                }

                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetCountryByName";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@Name", name);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: Name={Name}, CorrelationId={CorrelationId}",
                    name, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    var countryDetails = CountryDetails.Create(
                        reader.GetString(reader.GetOrdinal("CountryCode")),
                        reader.GetString(reader.GetOrdinal("Name")),
                        reader.IsDBNull(reader.GetOrdinal("Continent")) ? null : reader.GetString(reader.GetOrdinal("Continent")),
                        reader.IsDBNull(reader.GetOrdinal("Capital")) ? null : reader.GetString(reader.GetOrdinal("Capital")),
                        reader.IsDBNull(reader.GetOrdinal("CurrencyCode")) ? null : reader.GetString(reader.GetOrdinal("CurrencyCode")),
                        reader.IsDBNull(reader.GetOrdinal("CountryDialNumber")) ? null : reader.GetString(reader.GetOrdinal("CountryDialNumber")));

                    var country = Country.Reconstruct(
                        reader.GetGuid(reader.GetOrdinal("CountryId")), countryDetails);

                    _logger.LogInformation("Country fetched successfully for Name: {Name}", name);
                    return OperationResult<Country>.Success(country);
                }
                _logger.LogWarning("Country not found for Name: {Name}", name);
                return OperationResult<Country>.Failure(new Error(
                    ErrorCode.NotFound,
                    "NOT_FOUND",
                    $"Country with Name {name} not found.",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Country fetch operation was canceled for Name: {Name}. Exception: {Exception}", name, ex.Message);
                return _errorHandlingService.HandleCancelationToken<Country>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching country for Name: {Name}", name);
                return _errorHandlingService.HandleException<Country>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching country for Name: {Name}", name);
                return _errorHandlingService.HandleException<Country>(ex, correlationId);
            }
        }
    }
}
