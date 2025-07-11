using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Aggregate;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.Entities.Persons;
using ConnectSphere.API.Domain.Exceptions;
using ConnectSphere.API.Domain.IRepositories;
using ConnectSphere.API.Domain.ValueObjects;
using ConnectSphere.API.Infrastructure.Data.DataWrapperFactory;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Infrastructure.Repositories
{
    public class PersonRepository : IPersonRepository
    {
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

        public async Task<OperationResult<Person>> GetByIdAsync(Guid personId, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting person fetch process for PersonId: {PersonId}", personId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetPersonById";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PersonId", personId);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: PersonId={PersonId}, CorrelationId={CorrelationId}",
                    personId, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                if (await reader.ReadAsync(cancellationToken))
                {
                    var nameResult = PersonName.Create(
                        reader.GetString(reader.GetOrdinal("FirstName")),
                        reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                        reader.GetString(reader.GetOrdinal("LastName")),
                        reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                        reader.IsDBNull(reader.GetOrdinal("Suffix")) ? null : reader.GetString(reader.GetOrdinal("Suffix")));
                   

                    var person = Person.Create(
                        reader.GetGuid(reader.GetOrdinal("PersonId")),
                        nameResult,
                        reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                        reader.GetBoolean(reader.GetOrdinal("IsDeleted")));
                    if (!person.IsSuccess)
                    {
                        _logger.LogWarning("Failed to create Person for PersonId: {PersonId}. Errors: {Errors}",
                            personId, string.Join("; ", person.Errors.Select(e => e.Message)));
                        return OperationResult<Person>.Failure(person.Errors);
                    }

                    _logger.LogInformation("Person fetched successfully for PersonId: {PersonId}", personId);
                    return OperationResult<Person>.Success(person.Data!);
                }

                _logger.LogWarning("Person not found for PersonId: {PersonId}", personId);
                return OperationResult<Person>.Failure(new Error(
                    ErrorCode.NotFound,
                    "NOT_FOUND",
                    $"Person with ID {personId} not found.",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person fetch operation was canceled for PersonId: {PersonId}. Exception: {Exception}", personId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<Person>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching person for PersonId: {PersonId}", personId);
                return _errorHandlingService.HandleException<Person>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching person for PersonId: {PersonId}", personId);
                return _errorHandlingService.HandleException<Person>(ex, correlationId);
            }
        }

        public async Task<OperationResult<int>> GetCountAsync(string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting person count fetch process");
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM [dbo].[Person] WHERE IsDeleted = 0";
                command.CommandType = System.Data.CommandType.Text;

                _logger.LogDebug("Executing count query");

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                var count = (int)await command.ExecuteScalarAsync(cancellationToken);

                _logger.LogInformation("Person count fetched successfully: {Count}", count);
                return OperationResult<int>.Success(count);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person count fetch operation was canceled. Exception: {Exception}", ex.Message);
                return _errorHandlingService.HandleCancelationToken<int>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching person count");
                return _errorHandlingService.HandleException<int>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching person count");
                return _errorHandlingService.HandleException<int>(ex, correlationId);
            }
        }

        public async Task<OperationResult<IReadOnlyList<Person>>> GetAllAsync(int pageNumber, int pageSize, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting fetch process for persons with PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
            try
            {
                if (pageNumber < 1 || pageSize < 1)
                {
                    _logger.LogWarning("Invalid pagination parameters: PageNumber={PageNumber}, PageSize={PageSize}", pageNumber, pageSize);
                    return OperationResult<IReadOnlyList<Person>>.Failure(new Error(
                        ErrorCode.INVALIDARGUMENT,
                        "Invalid Pagination Parameters",
                        $"PageNumber and PageSize must be greater than 0. Provided: PageNumber={pageNumber}, PageSize={pageSize}",
                        correlationId));
                }

                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_GetAllPersons";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PageNumber", pageNumber);
                command.AddParameter("@PageSize", pageSize);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                _logger.LogDebug("Parameters added to stored procedure: PageNumber={PageNumber}, PageSize={PageSize}, CorrelationId={CorrelationId}",
                    pageNumber, pageSize, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                using var reader = await command.ExecuteReaderAsync(cancellationToken);
                var persons = new List<Person>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    var nameResult = PersonName.Create(
                        /*reader.GetString(reader.GetOrdinal("FirstName"))*/null,
                        reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                        reader.GetString(reader.GetOrdinal("LastName")),
                        reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                        reader.IsDBNull(reader.GetOrdinal("Suffix")) ? null : reader.GetString(reader.GetOrdinal("Suffix")));
                    

                    var person = Person.Create(
                        reader.GetGuid(reader.GetOrdinal("PersonId")),
                        nameResult,
                        reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                        reader.GetBoolean(reader.GetOrdinal("IsDeleted")));
                    if (!person.IsSuccess)
                    {
                        _logger.LogWarning("Failed to create Person for PersonId: {PersonId}. Errors: {Errors}",
                            reader.GetGuid(reader.GetOrdinal("PersonId")), string.Join("; ", person.Errors.Select(e => e.Message)));
                        return OperationResult<IReadOnlyList<Person>>.Failure(person.Errors);
                    }

                    persons.Add(person.Data!);
                }

                //if (persons.Count == 0)
                //{
                //    _logger.LogWarning("No persons found for PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                //    return OperationResult<IReadOnlyList<Person>>.Failure(new Error(
                //        ErrorCode.NOTFOUND,
                //        "NOT_FOUND",
                //        $"No persons found for PageNumber: {pageNumber}, PageSize: {pageSize}",
                //        correlationId));
                //}

                _logger.LogInformation("Fetched {Count} persons successfully for PageNumber: {PageNumber}, PageSize: {PageSize}", persons.Count, pageNumber, pageSize);
                return OperationResult<IReadOnlyList<Person>>.Success(persons.AsReadOnly());
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Fetch persons operation was canceled for PageNumber: {PageNumber}, PageSize: {PageSize}. Exception: {Exception}", pageNumber, pageSize, ex.Message);
                return _errorHandlingService.HandleCancelationToken<IReadOnlyList<Person>>(ex, correlationId);
            }
            //catch(PersonNameNotValidException ex)
            //{
            //    _logger.LogError(ex, "Person Name not valid for PageNumber: {message}, ", ex.Message);
            //    return _errorHandlingService.HandleException<IReadOnlyList<Person>>(ex, correlationId);
            //}
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while fetching persons for PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return _errorHandlingService.HandleException<IReadOnlyList<Person>>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching persons for PageNumber: {PageNumber}, PageSize: {PageSize}", pageNumber, pageSize);
                return _errorHandlingService.HandleException<IReadOnlyList<Person>>(ex, correlationId);
            }
        }

        public async Task<OperationResult<bool>> DeleteAsync(Guid personId, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting person deletion process for PersonId: {PersonId}", personId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_DeletePerson";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PersonId", personId);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);
                command.AddOutputParameter("@RowsAffected", SqlDbType.Int);

                _logger.LogDebug("Parameters added to stored procedure: PersonId={PersonId}, CorrelationId={CorrelationId}",
                    personId, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                await command.ExecuteNonQueryAsync(cancellationToken);

                int rowsAffected = Convert.ToInt32(command.GetParameterValue("@RowsAffected"));

                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Person deleted successfully for PersonId: {PersonId}, RowsAffected: {RowsAffected}", personId, rowsAffected);
                    return OperationResult<bool>.Success(true);
                }

                _logger.LogWarning("Person: {PersonId} not found or already deleted ", personId);
                return OperationResult<bool>.Failure(new Error(
                    ErrorCode.NotFound,
                    "NOT_FOUND",
                    $"Person with ID {personId} not found or already deleted.",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person deletion operation was canceled for PersonId: {PersonId}. Exception: {Exception}", personId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<bool>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while deleting person for PersonId: {PersonId}", personId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting person for PersonId: {PersonId}", personId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
        }

        public async Task<OperationResult<bool>> UpdateNameAsync(Guid personId, PersonName newName, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting person name update process for PersonId: {PersonId}, NewName: {FirstName} {LastName}", personId, newName.FirstName, newName.LastName);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_UpdatePersonName";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PersonId", personId);
                command.AddParameter("@Title", newName.Title ?? (object)DBNull.Value);
                command.AddParameter("@FirstName", newName.FirstName);
                command.AddParameter("@MiddleName", newName.MiddleName ?? (object)DBNull.Value);
                command.AddParameter("@LastName", newName.LastName);
                command.AddParameter("@Suffix", newName.Suffix ?? (object)DBNull.Value);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                command.AddOutputParameter("@RowsAffected", SqlDbType.Int);

                _logger.LogDebug("Parameters added to stored procedure: PersonId={PersonId}, Title={Title}, FirstName={FirstName}, MiddleName={MiddleName}, LastName={LastName}, Suffix={Suffix}, CorrelationId={CorrelationId}",
                    personId, newName.Title, newName.FirstName, newName.MiddleName, newName.LastName, newName.Suffix, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                await command.ExecuteNonQueryAsync(cancellationToken);
                int rowsAffected = Convert.ToInt32(command.GetParameterValue("@RowsAffected"));
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Person name updated successfully for PersonId: {PersonId}, RowsAffected: {RowsAffected}", personId, rowsAffected);
                    return OperationResult<bool>.Success(true);
                }
                _logger.LogWarning("No rows affected while updating name for PersonId: {PersonId}. RowsAffected: {RowsAffected}", personId, rowsAffected);
                return OperationResult<bool>.Failure(new Error(
                    ErrorCode.RESOURCEUPDATEFAILED,
                    "Update Failed",
                    $"Failed to update name for PersonId {personId}. Rows affected: {rowsAffected}",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person name update operation was canceled for PersonId: {PersonId}. Exception: {Exception}", personId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<bool>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating person name for PersonId: {PersonId}", personId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating person name for PersonId: {PersonId}", personId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
        }

        public async Task<OperationResult<bool>> UpdateAsync(Person person, string? correlationId, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId ?? "N/A" } });
            _logger.LogInformation("Starting person name update process for PersonId: {PersonId}, NewName: {FirstName} {LastName}", person.PersonId, person.Name.FirstName, person.Name.LastName);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with database connection.");

                await using var connection = await _connectionFactory.CreateConnectionAsync(_connectionString, cancellationToken);

                using var command = connection.CreateCommand();
                command.CommandText = "SP_UpdatePersonName";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.AddParameter("@PersonId", person.PersonId);
                command.AddParameter("@Title", person.Name.Title ?? (object)DBNull.Value);
                command.AddParameter("@FirstName", person.Name.FirstName);
                command.AddParameter("@MiddleName", person.Name.MiddleName ?? (object)DBNull.Value);
                command.AddParameter("@LastName", person.Name.LastName);
                command.AddParameter("@Suffix", person.Name.Suffix ?? (object)DBNull.Value);
                command.AddParameter("@CorrelationId", correlationId ?? (object)DBNull.Value);

                command.AddOutputParameter("@RowsAffected", SqlDbType.Int);

                _logger.LogDebug("Parameters added to stored procedure: PersonId={PersonId}, Title={Title}, FirstName={FirstName}, MiddleName={MiddleName}, LastName={LastName}, Suffix={Suffix}, CorrelationId={CorrelationId}",
                    person.PersonId, person.Name.Title, person.Name.FirstName, person.Name.MiddleName, person.Name.LastName, person.Name.Suffix, correlationId);

                await connection.OpenAsync(cancellationToken);
                _logger.LogInformation("Database connection opened successfully.");

                await command.ExecuteNonQueryAsync(cancellationToken);
                int rowsAffected = Convert.ToInt32(command.GetParameterValue("@RowsAffected"));
                if (rowsAffected > 0)
                {
                    _logger.LogInformation("Person name updated successfully for PersonId: {PersonId}, RowsAffected: {RowsAffected}", person.PersonId, rowsAffected);
                    return OperationResult<bool>.Success(true);
                }
                _logger.LogWarning("No rows affected while updating name for PersonId: {PersonId}. RowsAffected: {RowsAffected}", person.PersonId, rowsAffected);
                return OperationResult<bool>.Failure(new Error(
                    ErrorCode.RESOURCEUPDATEFAILED,
                    "Update Failed",
                    $"Failed to update name for PersonId {person.PersonId}. Rows affected: {rowsAffected}",
                    correlationId));
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Person name update operation was canceled for PersonId: {PersonId}. Exception: {Exception}", person.PersonId, ex.Message);
                return _errorHandlingService.HandleCancelationToken<bool>(ex, correlationId);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating person name for PersonId: {PersonId}", person.PersonId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating person name for PersonId: {PersonId}", person.PersonId);
                return _errorHandlingService.HandleException<bool>(ex, correlationId);
            }
        }




        public Task<OperationResult<AddressType>> GetAddressTypeByIdAsync(Guid addressTypeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<Person>> GetByEmailAsync(Email email, CancellationToken cancellationToken)
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
 
    }
}
