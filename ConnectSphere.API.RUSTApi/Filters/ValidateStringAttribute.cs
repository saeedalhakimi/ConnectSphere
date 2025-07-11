using ConnectSphere.API.RUSTApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConnectSphere.API.RUSTApi.Filters
{
    public class ValidateStringAttribute : ActionFilterAttribute
    {
        private readonly string _key;

        public ValidateStringAttribute(string key)
        {
            _key = key ?? throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue(_key, out var value) || string.IsNullOrWhiteSpace(value?.ToString()))
            {
                var errorResponse = new ErrorResponse
                {
                    StatusCode = 400,
                    StatusPhrase = "Bad Request",
                    Timestamp = DateTime.UtcNow,
                    Path = context.HttpContext.Request.Path,
                    Method = context.HttpContext.Request.Method,
                    Detail = $"The parameter '{_key}' is required and cannot be null, empty, or whitespace.",
                    CorrelationId = context.HttpContext.TraceIdentifier
                };

                errorResponse.Errors.Add($": The parameter '{_key}' is missing or empty.");

                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }
    }
}
