public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public TokenAuthenticationMiddleware(
        RequestDelegate next,
        ILogger<TokenAuthenticationMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(
                "Authorization",
                out var authHeader))
        {
            await RejectRequest(
                context,
                "Missing Authorization header.");

            return;
        }

        var authorization = authHeader.ToString();

        if (!authorization.StartsWith(
                "Bearer ",
                StringComparison.OrdinalIgnoreCase))
        {
            await RejectRequest(
                context,
                "Invalid Authorization scheme.");

            return;
        }

        var token = authorization["Bearer ".Length..].Trim();

        if (string.IsNullOrWhiteSpace(token))
        {
            await RejectRequest(
                context,
                "Bearer token is empty.");

            return;
        }

        var validToken = _configuration["ApiToken"];

        if (string.IsNullOrWhiteSpace(validToken))
        {
            // This is a server configuration problem,
            // not an authentication failure from the client.
            throw new InvalidOperationException(
                "API authentication token is not configured.");
        }

        if (!string.Equals(
                token,
                validToken,
                StringComparison.Ordinal))
        {
            await RejectRequest(
                context,
                "Invalid authentication token.");

            return;
        }

        await _next(context);
    }

    private async Task RejectRequest(
        HttpContext context,
        string reason)
    {
        _logger.LogWarning(
            "Unauthorized request: {Method} {Path}. Reason: {Reason}. Status: {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            reason,
            StatusCodes.Status401Unauthorized);

        context.Response.StatusCode =
            StatusCodes.Status401Unauthorized;

        await context.Response.WriteAsJsonAsync(new
        {
            error = "Unauthorized."
        });
    }
}

public static class TokenAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenAuthentication(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenAuthenticationMiddleware>();
    }
}