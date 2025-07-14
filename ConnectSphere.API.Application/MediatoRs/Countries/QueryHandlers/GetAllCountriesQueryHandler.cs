using ConnectSphere.API.Application.Contracts.CounteryDtos;
using ConnectSphere.API.Application.Contracts.CounteryDtos.Responses;
using ConnectSphere.API.Application.DomainRepositories.IRepositories;
using ConnectSphere.API.Application.MediatoRs.Countries.Queries;
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

namespace ConnectSphere.API.Application.MediatoRs.Countries.QueryHandlers
{
    public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, OperationResult<IReadOnlyList<CountryResponseDto>>>
    {
        private readonly IErrorHandlingService _errorHandlingService;
        private readonly IAppLogger<GetAllCountriesQueryHandler> _logger;
        private readonly ICountryRepository _countryRepository;

        public GetAllCountriesQueryHandler(
            IErrorHandlingService errorHandlingService,
            IAppLogger<GetAllCountriesQueryHandler> logger,
            ICountryRepository countryRepository)
        {
            _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _countryRepository = countryRepository ?? throw new ArgumentNullException(nameof(countryRepository));
        }
        public async Task<OperationResult<IReadOnlyList<CountryResponseDto>>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            using var scope = _logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", request.CorrelationId ?? "N/A" } });
            _logger.LogInformation("Handling GetAllCountriesQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                _logger.LogDebug("Cancellation token checked. Proceeding with repository call.");

                var countriesResult = await _countryRepository.GetAllAsync(request.CorrelationId, cancellationToken);
                if (!countriesResult.IsSuccess)
                {
                    _logger.LogWarning("Failed to fetch countries. Errors: {Errors}",
                        string.Join("; ", countriesResult.Errors.Select(e => e.Message)));
                    return OperationResult<IReadOnlyList<CountryResponseDto>>.Failure(
                        countriesResult.Errors.Select(e => new Error(e.Code, e.Message, e.Details, request.CorrelationId)).ToList());
                }

                var countries = countriesResult.Data!;
                var response = countries.Select(CountryMappers.ToResponseDto).ToList().AsReadOnly();

                _logger.LogDebug("Countries fetched successfully. Total count: {TotalCount}", countries.Count);
                _logger.LogInformation("Query handled successfully for GetAllCountriesQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
                return OperationResult<IReadOnlyList<CountryResponseDto>>.Success(response);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Operation was cancelled while handling GetAllCountriesQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
                return OperationResult<IReadOnlyList<CountryResponseDto>>.Failure(new Error(
                    ErrorCode.OperationCancelled,
                    "OPERATION_CANCELLED",
                    "The operation was cancelled.",
                    request.CorrelationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling GetAllCountriesQuery with CorrelationId: {CorrelationId}", request.CorrelationId);
                return OperationResult<IReadOnlyList<CountryResponseDto>>.Failure(new Error(
                    ErrorCode.InternalServerError,
                    "INTERNAL_SERVER_ERROR",
                    "An unexpected error occurred while processing the request.",
                    request.CorrelationId));
            }
        }
    }
}
