using Microsoft.AspNetCore.Http;

namespace DentalClinicProject.Infrastructure.Utilities
{
    public static class IpAddressHelper
    {
        public static string GetClientIpAddress(IHttpContextAccessor httpContextAccessor)
        {
			try
			{
                var httpcontext = httpContextAccessor.HttpContext;
                if (httpcontext == null)
                    return "Unknown";

                var forwardedFor = httpcontext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if(ips.Length > 0)
                    {
                        var ip = ips[0].Trim();
                        if (!string.IsNullOrEmpty(ip) && ip != "::1")
                            return ip;
                    }
                }
                var realIp = httpcontext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp) && realIp != "::1")
                    return realIp;

                var remoteIp = httpcontext.Connection.RemoteIpAddress?.ToString();

                if (remoteIp == "::1")
                    remoteIp = "127.0.0.1";

                return remoteIp ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
