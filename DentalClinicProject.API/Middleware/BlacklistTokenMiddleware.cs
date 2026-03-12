using DentalClinicProject.Core.Interfaces.IServices;
using DentalClinicProject.Infrastructure.Utilities;
using System.IdentityModel.Tokens.Jwt;

namespace DentalClinicProject.API.Middleware
{
    public class BlacklistTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedisService _redisService;

        public BlacklistTokenMiddleware(RequestDelegate next, IRedisService redisService)
        {
            _next = next;
            _redisService = redisService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"]
                               .ToString().Replace("Bearer ", "").Trim();

            if (!string.IsNullOrEmpty(token))
            {
                // استخرج userId من الـ Token مباشرة بدون validation
                var handler = new JwtSecurityTokenHandler();
                string userId = "";

                if (handler.CanReadToken(token))
                {
                    var jwtToken = handler.ReadJwtToken(token);
                    userId = jwtToken.Claims
                                     .FirstOrDefault(c => c.Type == "sub" || c.Type == "nameid")
                                     ?.Value ?? "";
                }

                if (!string.IsNullOrEmpty(userId))
                {
                    var blacklistKey = RedisKeys.BlacklistedAccessToken(userId, token);
                    var isBlacklisted = await _redisService.GetAsync(blacklistKey);

                    if (!string.IsNullOrEmpty(isBlacklisted))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsJsonAsync(new { message = "Token has been revoked" });
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
