using ConnectSphere.API.Common.ILogging;
using ConnectSphere.API.Domain.Common.Enums;
using ConnectSphere.API.Domain.Common.Models;
using ConnectSphere.API.RUSTApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConnectSphere.API.RUSTApi.Controllers.V1
{
    public class BaseController<T> : Controller
    {
        private readonly IAppLogger<T> _logger;
        public BaseController(IAppLogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Dictionary for status code mappings
        private static readonly Dictionary<ErrorCode, (int StatusCode, string StatusPhrase)> StatusMappings = new()
        {
            { ErrorCode.UnknownError, (500, "Internal Server Error") },
            { ErrorCode.NotFound, (404, "Not Found") },
            { ErrorCode.InvalidInput, (400, "Bad Request") },
            { ErrorCode.OperationCancelled, (499, "Client Closed Request") },
            { ErrorCode.InternalServerError, (500, "Internal Server Error") },
            { ErrorCode.ConflictError, (409, "Conflict") },
            { ErrorCode.Unauthorized, (401, "Unauthorized") },
            { ErrorCode.BadRequest, (400, "Bad Request") }
        };

        /// <summary>
        /// Handles an <see cref="OperationResult{T}"/> and returns an appropriate IActionResult.
        /// </summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        /// <param name="result">The operation result to handle.</param>
        /// <returns>An <see cref="IActionResult"/> representing the response.</returns>
        protected IActionResult HandleResult<TResult>(OperationResult<TResult> result, string correlationId)
        {
            try
            {
                if (result == null)
                {
                    _logger.LogError(null, "Operation result is null. CorrelationId: {CorrelationId}", correlationId);
                    return CreateErrorResponse(
                        ErrorCode.UnknownError,
                        new List<string> { "Operation result is null." },
                        new List<string> { "The server received an invalid operation result." },
                        correlationId);
                }

                if (result.IsSuccess)
                {
                    _logger.LogInformation("Operation succeeded. CorrelationId: {CorrelationId}", correlationId);
                    return result.Data == null ? NoContent() : Ok(result.Data);
                }

                if (!result.Errors.Any())
                {
                    _logger.LogError(null, "No errors provided in failed operation result. CorrelationId: {CorrelationId}", correlationId);
                    return CreateErrorResponse(
                        ErrorCode.UnknownError,
                        new List<string> { "An unknown error occurred." },
                        new List<string> { "No error details provided." },
                        correlationId);
                }

                var errorMessages = result.Errors.Select(e => e.Message).ToList();
                var errorDetails = result.Errors.Select(e => e.Details).ToList();
                var errorCodes = result.Errors.Select(e => e.Code.ToString()).ToList();
                var correlationID = result.Errors.FirstOrDefault()?.CorrelationId ?? correlationId;

                _logger.LogError(null, "Operation failed with errors: {Errors}. CorrelationId: {CorrelationId}", string.Join(", ", errorMessages), correlationID);

                var errorCode = result.Errors.FirstOrDefault()?.Code ?? ErrorCode.UnknownError;
                var (statusCode, statusPhrase) = StatusMappings.GetValueOrDefault(errorCode, (500, "Internal Server Error"));

                var apiError = new ErrorResponse
                {
                    Timestamp = result.Timestamp != default ? result.Timestamp : DateTime.UtcNow,
                    CorrelationId = correlationID,
                    Errors = errorMessages,
                    ErrorsDetails = errorDetails,
                    ErrorCodes = errorCodes,
                    StatusCode = statusCode,
                    StatusPhrase = statusPhrase,
                    Path = HttpContext.Request.Path,
                    Method = HttpContext.Request.Method,
                    Detail = $"An error occurred while processing the request: {statusPhrase}"
                };

                return StatusCode(apiError.StatusCode, apiError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while handling operation result. CorrelationId: {CorrelationId}",
                    correlationId);

                return CreateErrorResponse(
                    ErrorCode.InternalServerError,
                    new List<string> { "An unexpected error occurred." },
                    new List<string> { ex.Message },
                    correlationId);
            }
        }

        private IActionResult CreateErrorResponse(ErrorCode errorCode, List<string> errors, List<string> errorDetails, string? correlationId)
        {
            var (statusCode, statusPhrase) = StatusMappings.GetValueOrDefault(errorCode, (500, "Internal Server Error"));
            var apiError = new ErrorResponse
            {
                Timestamp = DateTime.UtcNow,
                CorrelationId = correlationId ?? HttpContext.TraceIdentifier,
                Errors = errors,
                ErrorsDetails = errorDetails,
                ErrorCodes = new List<string> { errorCode.ToString() },
                StatusCode = statusCode,
                StatusPhrase = statusPhrase,
                Path = HttpContext.Request.Path,
                Method = HttpContext.Request.Method,
                Detail = $"An error occurred while processing the request: {statusPhrase}"
            };

            return StatusCode(apiError.StatusCode, apiError);
        }
    }
}
