using ConnectSphere.API.Application.Contracts.CounteryDtos;
using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Application.MediatoRs.Countries.Queries;
using ConnectSphere.API.Application.Services;
using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.Domain.IRepositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectSphere.API.Application.MediatoRs.Countries.QueryHandlers
{
    public class GetCountryByCodeQueryHandler : IRequestHandler<GetCountryByCodeQuery, OperationResult<CountryResponseDto>>
    {
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IAppLogger<GetCountryByCodeQueryHandler> _logger;
        private readonly ICountryRepository _countryRepository;

        public GetCountryByCodeQueryHandler(
            IErrorHandlingService errorHandlingService,
            IAppLogger<GetCountryByCodeQueryHandler> logger,
            ICountryRepository countryRepository)
        {
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
        }
        public async Task<OperationResult<CountryResponseDto>> Handle(GetCountryByCodeQuery request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("Handling GetCountryByCodeQuery for CountryCode: {CountryCode}, CorrelationId: {CorrelationId}", request.CountryCode, request.CorrelationId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                var countryResult = await _countryRepository.GetByCodeAsync(request.CountryCode, request.CorrelationId, cancellationToken);
                if (!countryResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to fetch country for CountryCode: {CountryCode}. Errors: {Errors}",
                        request.CountryCode, string.Join("; ", countryResult.Errors.Select(e => e.Message)));
                    return OperationResult<CountryResponseDto>.Failure(
                        countryResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                var country = countryResult.Data!;
                var response = CountryMappers.ToResponseDto(country);

                _logger.LogInformation("GetCountryByCodeQuery handled successfully for CountryCode: {CountryCode}", request.CountryCode);
                return OperationResult<CountryResponseDto>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Operation was cancelled while handling GetCountryByCodeQuery for CountryCode: {CountryCode}, CorrelationId: {CorrelationId}",
                    request.CountryCode, request.CorrelationId);
                return OperationResult<CountryResponseDto>.Failure(new Error(
                    ErrorCode.OperationCancelled,
                    "OPERATION_CANCELLED",
                    "The operation was cancelled.",
                    request.CorrelationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetCountryByCodeQuery for CountryCode: {CountryCode}, CorrelationId: {CorrelationId}",
                    request.CountryCode, request.CorrelationId);
                return OperationResult<CountryResponseDto>.Failure(new Error(
                    ErrorCode.InternalServerError,
                    "INTERNAL_SERVER_ERROR",
                    "An unexpected error occurred while processing the request.",
                    request.CorrelationId));
            }
        }
    }
}
