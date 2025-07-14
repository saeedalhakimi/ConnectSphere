using ConnectSphere.API.Application.Contracts.PersonDtos;
using ConnectSphere.API.Application.Contracts.PersonDtos.Responses;
using ConnectSphere.API.Application.DomainRepositories.IRepositories;
using ConnectSphere.API.Application.MediatoRs.Persons.Queries;
using ConnectSphere.API.Application.Models;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Persons.QueryHandlers
{
    public class GetAllPersonsQueryHandler : IRequestHandler<GetAllPersonsQuery, OperationResult<PagedResponse<PersonResponseDto>>>
    {
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IAppLogger<GetAllPersonsQueryHandler> _logger;
        private readonly IPersonRepository _personRepository;
        public GetAllPersonsQueryHandler(
            IErrorHandlingService errorHandlingService,
            IAppLogger<GetAllPersonsQueryHandler> logger,
            IPersonRepository personRepository)
        {
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        }
        public async Task<OperationResult<PagedResponse<PersonResponseDto>>> Handle(GetAllPersonsQuery request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("handling GetAllPersonQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
            try
            {

                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                if (request.PageNumber < 1)
                {
                    _logger.LogWarning("Invalid page number: {PageNumber}", request.PageNumber);
                    return OperationResult<PagedResponse<PersonResponseDto>>.Failure(new Error(
                        ErrorCode.InvalidInput,
                        "INVALID_PAGE_NUMBER",
                        "Page number must be greater than or equal to 1",
                        request.CorrelationId));
                }

                if (request.PageSize < 1)
                {
                    _logger.LogWarning("Invalid page size: {PageSize}", request.PageSize);
                    return OperationResult<PagedResponse<PersonResponseDto>>.Failure(new Error(
                        ErrorCode.InvalidInput,
                        "INVALID_PAGE_SIZE",
                        "Page size must be greater than or equal to 1",
                        request.CorrelationId));
                }

                var totalCount = await _personRepository.GetCountAsync(request.CorrelationId, cancellationToken);
                if (!totalCount.IsSuccess)
                {
                    _logger.LogWarning("Failed to fetch total count of persons. Errors: {Errors}",
                         string.Join("; ", totalCount.Errors.Select(e => e.Message)));
                    return OperationResult<PagedResponse<PersonResponseDto>>.Failure(
                         totalCount.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                if (totalCount.Data == 0)
                {
                    _logger.LogInformation("No persons found. Returning empty response.");
                    return OperationResult<PagedResponse<PersonResponseDto>>.Success(new PagedResponse<PersonResponseDto>(
                        new List<PersonResponseDto>(),
                        request.PageNumber,
                        request.PageSize,
                        totalCount.Data));
                }

                var personsResult = await _personRepository.GetAllAsync(request.PageNumber, request.PageSize, request.CorrelationId, cancellationToken);
                if (!personsResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to fetch persons. Errors: {Errors}",
                         string.Join("; ", personsResult.Errors.Select(e => e.Message)));
                    return OperationResult<PagedResponse<PersonResponseDto>>.Failure(
                         personsResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                var persons = personsResult.Data!;
                var response = new PagedResponse<PersonResponseDto>(
                    persons.Select(PersonMappers.ToResponseDto).ToList(),
                    request.PageNumber,
                    request.PageSize,
                    totalCount.Data,
                    persons.Count);

                _logger.LogDebug("Persons fetched successfully. Total count: {TotalCount}, Page: {PageNumber}, Size: {PageSize}",
                    totalCount.Data, request.PageNumber, request.PageSize);
                _logger.LogInformation("Query handled successfully for GetAllPersonsQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
                return OperationResult<PagedResponse<PersonResponseDto>>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Operation was cancelled while handling GetAllPersonsQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
                return OperationResult<PagedResponse<PersonResponseDto>>.Failure(new Error(
                    ErrorCode.OperationCancelled,
                    "OPERATION_CANCELLED",
                    "The operation was cancelled.",
                    request.CorrelationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetAllPersonsQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
                return OperationResult<PagedResponse<PersonResponseDto>>.Failure(new Error(
                    ErrorCode.InternalServerError,
                    "INTERNAL_SERVER_ERROR",
                    "An unexpected error occurred while processing the request.",
                    request.CorrelationId));

            }
        }
    }
}
