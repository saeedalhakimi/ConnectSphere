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
    public class PersonRepository : IPersonRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IAppLogger<PersonRepository> _logger;
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IDatabaseConnectionFactory _connectionFactory;
        private readonly ISystemClocking _systemClocking;
        private readonly string _connectionString;

        public PersonRepository(
           IConfiguration configuration,
           IAppLogger<PersonRepository> logger,
           IErrorHandlingService errorHandlingService,
           IDatabaseConnectionFactory connectionFactory,
           ISystemClocking systemClocking,
           string connectionString = null)
        {
            _connectionString = connectionString ?? configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _systemClocking = systemClocking ?? throw new ArgumentNullException(nameof(systemClocking));
        }
        public async Task<OperationResult<Person>> CreateAsync(Person person, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new System.Collections.Generic.Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting person creation process for PersonId: {PersonId}, Name: {FirstName} {LastName}",
                person.PersonId, person.Name.FirstName, person.Name.LastName);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_CreatePerson";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PersonId", person.PersonId);
                command.AddParameter("@FirstName", person.Name.FirstName);
                command.AddParameter("@MiddleName", person.Name.MiddleName ?? (object)DBNull.Value);
                command.AddParameter("@LastName", person.Name.LastName);
                command.AddParameter("@Title", person.Name.Title ?? (object)DBNull.Value);
                command.AddParameter("@Suffix", person.Name.Suffix ?? (object)DBNull.Value);
                command.AddParameter("@CreatedAt", person.CreatedAt);
                command.AddParameter("@UpdatedAt", person.UpdatedAt ?? (object)DBNull.Value);
                command.AddParameter("@IsDeleted", person.IsDeleted);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: PersonId={PersonId}, FirstName={FirstName}, MiddleName={MiddleName}, LastName={LastName}, Title={Title}, Suffix={Suffix}, CreatedAt={CreatedAt}, UpdatedAt={UpdatedAt}, IsDeleted={IsDeleted}, CorrelationId={CorrelationId}",
                    person.PersonId, person.Name.FirstName, person.Name.MiddleName, person.Name.LastName, person.Name.Title, person.Name.Suffix, person.CreatedAt, person.UpdatedAt, person.IsDeleted, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                int rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Person created successfully via SP_CreatePerson. Rows affected: {RowsAffected}, PersonId: {PersonId}", rowsAffected, person.PersonId);
                    return OperationResult<Person>.Success(person);
                }
                else
                {
                    _logger.LogWarning("Person creation failed. No rows affected. {RowsAffected}", rowsAffected);
                    return OperationResult<Person>.Failure(new Error(
                        ErrorCode.RESOURCECREATIONFAILED,
                        "Creation Failed",
                        $"Falied To Create Post -rows affected: {rowsAffected}"
                        ));
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person creation operation was canceled for PersonId: {PersonId}. Exception: {Exception}", person.PersonId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<Person>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating person for PersonId: {PersonId}", person.PersonId);
                return _errorHandlingService.HandleException<Person>(ex, correlationId);
            }

        }








        public Task<OperationResult<bool>> DeleteAsync(Guid personId, string? correlationId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<AddressType>> GetAddressTypeByIdAsync(Guid addressTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IEnumerable<Person>>> GetAllAsync(bool includeRelated, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Person>> GetByEmailAsync(Email email, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Person>> GetByIdAsync(Guid personId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Country>> GetCountryByIdAsync(Guid countryId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<EmailAddressType>> GetEmailAddressTypeByIdAsync(Guid emailAddressTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<PhoneNumberType>> GetPhoneNumberTypeByIdAsync(Guid phoneNumberTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Person>> UpdateAsync(Person person, string? correlationId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
