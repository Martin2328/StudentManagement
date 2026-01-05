using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;

namespace StudentManagement.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private static readonly SemaphoreSlim _fileLock = new(1, 1);

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            _logger.LogInformation("Handling request {method} {path}", context.Request.Method, context.Request.Path);

            await _next(context);

            sw.Stop();
            _logger.LogInformation("Finished request {method} {path} responded {statusCode} in {elapsed} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);

            // Also write a simple log line to a text file under Logs/requests.txt
            try
            {
                var logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
                Directory.CreateDirectory(logDir);
                var logPath = Path.Combine(logDir, "requests.txt");

                var logLine = string.Format("{0:O} {1} {2} {3} {4}ms{5}",
                    DateTime.UtcNow,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds,
                    Environment.NewLine);

                await _fileLock.WaitAsync();
                try
                {
                    await File.AppendAllTextAsync(logPath, logLine);
                }
                finally
                {
                    _fileLock.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write request log to file");
                // swallow exceptions to avoid interfering with request processing
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
