using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

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
                return BadRequest(new { errors = response.Errors });

            return StatusCode(response.StatusCode, response);
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok("Hello");
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO dto)
        {
            var response = await work.AuthService.VerifyEmail(dto.Email, dto.Code);

            return StatusCode(response.StatusCode, response);
        }
    }
}