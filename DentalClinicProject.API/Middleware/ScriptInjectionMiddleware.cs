using Ganss.Xss;
using Microsoft.Extensions.Primitives;
using System.Text;

namespace DentalClinicProject.API.Middleware
{
    public class ScriptInjectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HtmlSanitizer _sanitizer;

        public ScriptInjectionMiddleware(RequestDelegate next, HtmlSanitizer sanitizer)
        {
            _next = next;
            _sanitizer = sanitizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            // 1. Body clean
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                var cleanBody = _sanitizer.Sanitize(body);
                context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(cleanBody));
            }
            context.Request.Body.Position = 0;

            // 2. Query String clean
            var queryDict = context.Request.Query.ToDictionary(k => k.Key,
            k => new StringValues(_sanitizer.Sanitize(k.Value!)));
            context.Request.Query = new QueryCollection(queryDict);

            // 3. Headers clean
            foreach (var header in context.Request.Headers.Keys.ToList())
            {
                context.Request.Headers[header] = _sanitizer.Sanitize(context.Request.Headers[header]!);
            }

            await _next(context);
        }
    }
}