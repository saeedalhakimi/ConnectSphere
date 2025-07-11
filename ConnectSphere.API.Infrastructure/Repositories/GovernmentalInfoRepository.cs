using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Aggregate;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.IRepositories;
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
    public class GovernmentalInfoRepository : IGovernmentalInfoRepository
    {
        private readonly IAppLogger<GovernmentalInfoRepository> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly string _connectionString;

        public GovernmentalInfoRepository(IConfiguration configuration,
            IErrorHandlingService errorHandlingService,
            IAppLogger<GovernmentalInfoRepository> logger,
            IDatabaseConnectionFactory connectionFactory,
            string connectionString = null)
        {
            _connectionString = connectionString ?? configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }
        public async Task<OperationResult<bool>> CreateAsync(GovernmentalInfo governmentalInfo, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting governmental info creation process for GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}",
                governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_CreateGovernmentalInfo";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@GovernmentalInfoId", governmentalInfo.GovernmentalInfoId);
                command.AddParameter("@PersonId", governmentalInfo.PersonId);
                command.AddParameter("@CountryId", governmentalInfo.CountryId);
                command.AddParameter("@GovIdNumber", (object?)governmentalInfo.Details.GovIdNumber ?? DBNull.Value);
                command.AddParameter("@PassportNumber", (object?)governmentalInfo.Details.PassportNumber ?? DBNull.Value);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: GovernmentalInfoId={GovernmentalInfoId}, PersonId={PersonId}, CountryId={CountryId}, GovIdNumber={GovIdNumber}, PassportNumber={PassportNumber}, CorrelationId={CorrelationId}",
                    governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId, governmentalInfo.Details.GovIdNumber!, governmentalInfo.Details.PassportNumber!, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                _logger.LogDebug("Stored procedure executed. rows affected: {rows}", rowsAffected);

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Governmental info created successfully for GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}",
                        governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId);
                    return OperationResult<bool>.Success(true);
                }
                else
                {
                    _logger.LogWarning("No rows affected while creating governmental info for GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}, Rowsaffected: {Rowsaffected}",
                        governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId, rowsAffected);
                    return OperationResult<bool>.Failure(ErrorCode.DatabaseError, "DATABASE_ERROR", "No rows affected while creating governmental info.", correlationId);
                }


            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Operation was cancelled for GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}",
                    governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId);
                return OperationResult<bool>.Failure(ErrorCode.OperationCancelled, "OPERATION_CANCELLED", "The operation was cancelled.", correlationId);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL error occurred while creating governmental info for GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}",
                    governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId);
                return _errorHandlingService.HandleException<bool>(sqlEx, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating governmental info for GovernmentalInfoId: {GovernmentalInfoId}, PersonId: {PersonId}, CountryId: {CountryId}",
                    governmentalInfo.GovernmentalInfoId, governmentalInfo.PersonId, governmentalInfo.CountryId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
        }
    }
}
