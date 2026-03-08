using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Enum;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    public class ExternalLoginController : BaseController
    {
        public ExternalLoginController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("signin-google")]
        public IActionResult SignInGoogle([FromQuery] string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "ExternalLogin", new { returnUrl });
            var properties = work.ExternalLoginService.ConfigureExternalAuthenticationProperties(Provider.Google, redirectUrl!);
            return Challenge(properties, Provider.Google.ToString());
        }

        [HttpGet("external-callback")]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery] string? returnUrl = null)
        {
            try
            {
                var info = await HttpContext.AuthenticateAsync();
                if (info?.Principal == null)
                {
                    return BadRequest(new { message = "External authentication failed" });
                }

                var provider = info.Properties?.Items[".AuthScheme"] ?? "Unknown";
                var providerKey = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

                if (string.IsNullOrWhiteSpace(providerKey) || string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new { message = "Required information not provided by external provider" });
                }

                var response = await work.ExternalLoginService.ExternalSignInAsync(
                    provider, providerKey, email, firstName, lastName);

                if (!response.Success)
                    return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

                // Redirect to frontend with token
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect($"{returnUrl}?token={response.Data?.Token}&refreshToken={response.Data?.RefreshToken}");
                }

                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during external login", error = ex.Message });
            }
        }

        [HttpPost("signin-external")]
        public async Task<IActionResult> SignInExternal([FromBody] ExternalLoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Provider) ||
                string.IsNullOrWhiteSpace(request.ProviderKey) || string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { message = "Invalid external login data" });
            }

            var response = await work.ExternalLoginService.ExternalSignInAsync(
                request.Provider,
                request.ProviderKey,
                request.Email,
                request.FirstName ?? "",
                request.LastName ?? "");

            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, response);
        }
    }
}
