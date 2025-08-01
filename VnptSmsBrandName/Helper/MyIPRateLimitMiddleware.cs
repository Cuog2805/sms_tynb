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
            $"Bạn đã vuợt quá giới hạn truy cập. Tối đa {rule.Limit} lần mỗi {rule.Period}. Vui lòng thử lại sau {retryAfter} giây.";
        string html = $@"
            <!DOCTYPE html>
            <html lang=""vi"">
            <head>
                <meta charset=""UTF-8"">
                <title>Thông báo giới hạn truy cập</title>
                <script type=""text/javascript"">
                    alert(""{message}"");
                    window.location.href = '/'; // Chuyển hướng về trang chủ hoặc trang mong muốn
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
