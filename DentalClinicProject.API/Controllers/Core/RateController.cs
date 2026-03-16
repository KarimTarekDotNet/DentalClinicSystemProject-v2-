using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers
{
    [Route("api/rate")]
    public class RateController : BaseController
    {
        public RateController(IUnitOfWork work) : base(work) { }


        private string GetUserId() => User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        // ─── Generic ────────────────────────────────────────────────────────────

        [HttpGet("get-all")]
        public async Task<ActionResult> GetAllRates(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var rates = await work.RateRepository.GetRatesWithDetailsAsync(pageNumber, pageSize);
                if (!rates.Items.Any())
                    return NotFound(new { success = false, message = "No rates found." });

                return Ok(new { success = true, data = rates });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("by-id")]
        public async Task<ActionResult> GetRateById([FromQuery] int id)
        {
            try
            {
                var rate = await work.RateRepository.GetRateWithDetailsAsync(id);
                if (rate is null)
                    return NotFound(new { success = false, message = "No rate found with this id." });

                return Ok(new { success = true, data = rate });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        // ─── Doctor Rates ────────────────────────────────────────────────────────

        [HttpGet("doctor/average")]
        public async Task<ActionResult> GetDoctorAverageRating([FromQuery] int doctorId)
        {
            try
            {
                var result = await work.RateRepository.GetDoctorAverageRatingAsync(doctorId);
                if (result is null)
                    return NotFound(new { success = false, message = "Doctor not found or has no approved rates." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("doctor/all")]
        public async Task<ActionResult> GetDoctorRates([FromQuery] int doctorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await work.RateRepository.GetDoctorRatesAsync(doctorId, pageNumber, pageSize);
                if (!result.Items.Any())
                    return NotFound(new { success = false, message = "No rates found for this doctor." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("doctor/by-id")]
        public async Task<ActionResult> GetDoctorRateById([FromQuery] int id)
        {
            try
            {
                var result = await work.RateRepository.GetDoctorRateAsync(id);
                if (result is null)
                    return NotFound(new { success = false, message = "Doctor rate not found." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("doctor/create")]
        public async Task<ActionResult> CreateDoctorRate([FromBody] CreateDoctorRateDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await work.RateRepository.CreateDoctorRateAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("doctor/update")]
        public async Task<ActionResult> UpdateDoctorRate([FromBody] UpdateRateDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await work.RateRepository.UpdateDoctorRateAsync(userId, dto);
                return Ok(new { success = true, data = result });
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

        [Authorize]
        [HttpDelete("doctor/delete")]
        public async Task<ActionResult> DeleteDoctorRate([FromQuery] int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await work.RateRepository.DeleteDoctorRateAsync(userId, id);
                if (!success)
                    return NotFound(new { success = false, message = "Doctor rate not found." });

                return Ok(new { success = true, message = "Doctor rate deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        // ─── Product Rates ───────────────────────────────────────────────────────

        [HttpGet("product/average")]
        public async Task<ActionResult> GetProductAverageRating([FromQuery] int productId)
        {
            try
            {
                var result = await work.RateRepository.GetProductAverageRatingAsync(productId);
                if (result is null)
                    return NotFound(new { success = false, message = "Product not found or has no rates." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("product/all")]
        public async Task<ActionResult> GetProductRates([FromQuery] int productId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await work.RateRepository.GetProductRatesAsync(productId, pageNumber, pageSize);
                if (!result.Items.Any())
                    return NotFound(new { success = false, message = "No rates found for this product." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("product/by-id")]
        public async Task<ActionResult> GetProductRateById([FromQuery] int id)
        {
            try
            {
                var result = await work.RateRepository.GetProductRateAsync(id);
                if (result is null)
                    return NotFound(new { success = false, message = "Product rate not found." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("product/create")]
        public async Task<ActionResult> CreateProductRate([FromBody] CreateProductRateDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await work.RateRepository.CreateProductRateAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("product/update")]
        public async Task<ActionResult> UpdateProductRate([FromBody] UpdateRateDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await work.RateRepository.UpdateProductRateAsync(userId, dto);
                return Ok(new { success = true, data = result });
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

        [Authorize]
        [HttpDelete("product/delete")]
        public async Task<ActionResult> DeleteProductRate([FromQuery] int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await work.RateRepository.DeleteProductRateAsync(userId, id);
                if (!success)
                    return NotFound(new { success = false, message = "Product rate not found." });

                return Ok(new { success = true, message = "Product rate deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        // ─── Appointment Rates ───────────────────────────────────────────────────

        [HttpGet("appointment/by-appointment")]
        public async Task<ActionResult> GetRateByAppointment([FromQuery] int appointmentId)
        {
            try
            {
                var result = await work.RateRepository.GetRateByAppointmentAsync(appointmentId);
                if (result is null)
                    return NotFound(new { success = false, message = "No rate found for this appointment." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [HttpGet("appointment/by-id")]
        public async Task<ActionResult> GetAppointmentRateById([FromQuery] int id)
        {
            try
            {
                var result = await work.RateRepository.GetAppointmentRateAsync(id);
                if (result is null)
                    return NotFound(new { success = false, message = "Appointment rate not found." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("appointment/create")]
        public async Task<ActionResult> CreateAppointmentRate([FromBody] CreateApponitmentRateDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await work.RateRepository.CreateAppointmentRateAsync(userId, dto);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("appointment/update")]
        public async Task<ActionResult> UpdateAppointmentRate([FromBody] UpdateRateDTO dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await work.RateRepository.UpdateAppointmentRateAsync(userId, dto);
                return Ok(new { success = true, data = result });
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

        [Authorize]
        [HttpDelete("appointment/delete")]
        public async Task<ActionResult> DeleteAppointmentRate([FromQuery] int id)
        {
            try
            {
                var userId = GetUserId();
                var success = await work.RateRepository.DeleteAppointmentRateAsync(userId, id);
                if (!success)
                    return NotFound(new { success = false, message = "Appointment rate not found." });

                return Ok(new { success = true, message = "Appointment rate deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred.", error = ex.Message });
            }
        }
    }
}