namespace API_BookingHotel.MiddlewareCustom
{
    public class RequestLoggingMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString();
            var time = DateTime.UtcNow;
            var endpoint = context.GetEndpoint()?.DisplayName;

            _logger.LogInformation(
                "Request info | IP: {IP} | Time: {Time} | Endpoint: {Endpoint}",
                ip, time, endpoint);

            await _next(context);
        }

    }
}
