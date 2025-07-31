using AspNetCoreRateLimit;
using Microsoft.Extensions.Options;
using System.Text;

namespace VnptSmsBrandName.Helper;

public class MyIPRateLimitMiddleware : IpRateLimitMiddleware
{
    public MyIPRateLimitMiddleware(
        RequestDelegate next,
        IProcessingStrategy processingStrategy,
        IOptions<IpRateLimitOptions> options,
        IIpPolicyStore policyStore,
        IRateLimitConfiguration config,
        ILogger<IpRateLimitMiddleware> logger)
        : base(next, processingStrategy, options, policyStore, config, logger)
    {
    }

    public override Task ReturnQuotaExceededResponse(HttpContext context, RateLimitRule rule, string retryAfter)
    {
        return ReturnHtmlQuotaExceededResponse(context, rule, retryAfter);
    }

    private static Task ReturnHtmlQuotaExceededResponse(HttpContext context, RateLimitRule rule, string retryAfter)
    {
        string message =
            $"B?n d� vu?t qu� gi?i h?n truy c?p. T?i da {rule.Limit} l?n m?i {rule.Period}. Vui l�ng th? l?i sau {retryAfter} gi�y.";
        string html = $@"
            <!DOCTYPE html>
            <html lang=""vi"">
            <head>
                <meta charset=""UTF-8"">
                <title>Th�ng b�o gi?i h?n truy c?p</title>
                <script type=""text/javascript"">
                    alert(""{message}"");
                    window.location.href = '/'; // Chuy?n hu?ng v? trang ch? ho?c trang mong mu?n
                </script>
            </head>
            <body></body>
            </html>";

        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.Headers["Retry-After"] = retryAfter;
        return context.Response.WriteAsync(html, Encoding.UTF8);
    }
}
