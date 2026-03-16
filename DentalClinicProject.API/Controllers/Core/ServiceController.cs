using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicProject.API.Controllers.Core
{
    [Route("api/service")]
    public class ServiceController : BaseController
    {
        public ServiceController(IUnitOfWork work) : base(work) { }

        // ─── Queries (No Auth) ───────────────────────────────────────────────────

        [HttpGet("get-all")]
        public async Task<IActionResult> GetServices([FromQuery] PaginationParams param)
        {
            try
            {
                var services = await work.ServiceRepository.GetServicesAsync(param);
                if (!services.Items.Any())
                    return NotFound(new { success = false, message = "No services found." });

                return Ok(new { success = true, data = services });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetService([FromQuery] int id)
        {
            try
            {
                var service = await work.ServiceRepository.GetServiceByIdAsync(id);
                if (service is null)
                    return NotFound(new { success = false, message = "No service found with this id." });

                return Ok(new { success = true, data = service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("get-by-ids")]
        public async Task<IActionResult> GetServicesByIds([FromQuery] string ids)
        {
            try
            {
                var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id, out var x) ? x : (int?)null)
                    .Where(x => x.HasValue)
                    .Select(x => x!.Value)
                    .ToList();

                if (!idList.Any())
                    return BadRequest(new { success = false, message = "No valid IDs provided." });

                var services = await work.ServiceRepository.GetServicesByIdsAsync(idList);
                if (services is null || !services.Any())
                    return NotFound(new { success = false, message = "No services found for the provided IDs." });

                return Ok(new { success = true, data = services });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        // ─── Commands (Admin Only) ───────────────────────────────────────────────

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO dto)
        {
            try
            {
                var service = await work.ServiceRepository.CreateServiceAsync(dto);
                return Ok(new { success = true, message = "Service created successfully.", data = service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateService([FromBody] UpdateServiceDTO dto)
        {
            try
            {
                var service = await work.ServiceRepository.UpdateServiceAsync(dto);
                return Ok(new { success = true, message = "Service updated successfully.", data = service });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteService([FromQuery] int id)
        {
            try
            {
                var result = await work.ServiceRepository.DeleteServiceAsync(id);
                if (!result)
                    return NotFound(new { success = false, message = "No service found with this id." });

                return Ok(new { success = true, message = "Service deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }
    }
}