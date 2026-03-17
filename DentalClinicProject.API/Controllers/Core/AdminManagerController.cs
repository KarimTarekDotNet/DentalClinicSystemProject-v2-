using DentalClinicProject.Core.DTOs.Auth;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicProject.API.Controllers.Core
{
    [Authorize]
    public class AdminManagerController : BaseController
    {
        public AdminManagerController(IUnitOfWork work) : base(work) { }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountFromAdmin(RegisterDTO dto)
        {
            try
            {
                var user = await work.AdminManager.CreateAccountFromAdminAsync(dto);
                return Ok(new { Message = "User created successfully.", UserId = user.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAccountFromAdmin(DeleteAccountFromAdminDTO dto)
        {
            try
            {
                var result = await work.AdminManager.DeleteAccountFromAdminAsync(dto);
                if (result)
                    return Ok(new { Message = "User deleted successfully." });
                else
                    return BadRequest(new { Message = "Failed to delete user." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAccountFromAdmin(UpdateAccountFromAdminDTO dto)
        {
            try
            {
                var user = await work.AdminManager.UpdateAccountFromAdminAsync(dto);
                return Ok(new { Message = "User updated successfully.", UserId = user.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
