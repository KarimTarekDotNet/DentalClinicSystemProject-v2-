namespace DentalClinicProject.API.Middleware
{
    public class CookieToHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieToHeaderMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("accessToken") &&
                !context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Cookies["accessToken"];
                context.Request.Headers!.Append("Authorization", $"Bearer {token}");
            }

            await _next(context);
        }
    }
}
