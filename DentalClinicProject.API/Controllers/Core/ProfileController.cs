using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers.Core
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ProfileController(IUnitOfWork work) : base(work) { }

        private string? GetCurrentUid() =>
            User.FindFirst("uid")?.Value ??
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpGet("get-profile")]
        public async Task<ActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUid();
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Unauthorized." });

                var profile = await work.ProfileRepository.GetProfileAsync(userId);

                if (profile == null)
                    return NotFound(new { success = false, message = "Profile not found." });

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred.",
                    error = ex.Message
                });
            }
        }

        [HttpPut("update-profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateAccountDTO user)
        {
            try
            {
                var userId = GetCurrentUid();
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Unauthorized." });

                var result = await work.ProfileRepository.UpdateProfileAsync(userId, user);

                if (!result.Succeeded)
                    return BadRequest(new
                    {
                        success = false,
                        message = "Update failed.",
                        errors = result.Errors.Select(e => e.Description)
                    });

                return Ok(new
                {
                    success = true,
                    message = "Profile updated successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred.",
                    error = ex.Message
                });
            }
        }

        [HttpDelete("delete-profile")]
        public async Task<ActionResult> DeleteProfile()
        {
            try
            {
                var userId = GetCurrentUid();
                if (userId == null)
                    return Unauthorized(new { success = false, message = "Unauthorized." });

                var result = await work.ProfileRepository.DeleteProfileAsync(userId);

                if (!result)
                    return NotFound(new { success = false, message = "Profile not found." });

                return Ok(new
                {
                    success = true,
                    message = "Profile deleted successfully."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred.",
                    error = ex.Message
                });
            }
        }
    }
}