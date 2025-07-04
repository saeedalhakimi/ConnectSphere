using ConnectSphere.API.Common.IClocking;
using ConnectSphere.API.Common.ILogging;
using Serilog.Context;

namespace ConnectSphere.API.RUSTApi.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAppLogger<CorrelationIdMiddleware> _logger;
        private readonly ISystemClocking _systemClocks;

        public CorrelationIdMiddleware(RequestDelegate next, IAppLogger<CorrelationIdMiddleware> logger, ISystemClocking systemClocks)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _systemClocks = systemClocks ?? throw new ArgumentNullException(nameof(systemClocks));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(correlationId) || !Guid.TryParse(correlationId, out _))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-Id"] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation("Request {Method} {Path} received at {Time}",
                    context.Request.Method, context.Request.Path, _systemClocks.UtcNow);
                await _next(context);
            }
        }
    }
}
