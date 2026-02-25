using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : BaseController
    {
        public AuthController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO dto)
        {
            var response = await work.AuthService.Register(dto);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO dto)
        {
            var response = await work.EmailVerificationService.VerifyEmailAsync(dto.Email, dto.Code);

            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("verify-phone")]
        [Authorize]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneDTO dto)
        {
            var userId = User.FindFirst("uid")?.Value ??
                         User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await work.PhoneVerificationService.VerifyPhoneAsync(userId, dto.Code);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("resend-phone-code")]
        public async Task<IActionResult> ResendPhoneCode()
        {
            var userId = User.FindFirst("uid")?.Value ??
                         User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await work.PhoneVerificationService.ResendPhoneVerificationCodeAsync(userId);

            if (!result)
                return BadRequest(new { message = "Failed to resend verification code. Please try again later or check rate limit." });

            return Ok(new { message = "Verification code sent successfully" });
        }

        [HttpPost("resend-email-code")]
        public async Task<IActionResult> ResendEmailCode()
        {
            var userId = User.FindFirst("uid")?.Value ??
                         User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await work.EmailVerificationService.ResendEmailVerificationCodeAsync(userId);

            if (!result)
                return BadRequest(new { message = "Failed to resend verification code. Please try again later or check rate limit." });

            return Ok(new { message = "Verification code sent successfully" });
        }
    }
}