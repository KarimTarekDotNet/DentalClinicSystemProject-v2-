using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicProject.API.Controllers.Core
{
    public class AppointmentController : BaseController
    {
        public AppointmentController(IUnitOfWork work, IMapper mapper) : base(work, mapper) { }

        #region Admin Get Controllers

        [Authorize(Roles = "Admin")]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAppointments([FromQuery] PaginationParams param)
        {
            try
            {
                var result = await work.AppointmentRepository.GetAppointmentsWithDetailsAsync(param);
                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-doctor")]
        public async Task<IActionResult> GetDoctorAppointmentsAsync(int doctorId, [FromQuery] PaginationParams param)
        {
            try
            {
                var result = await work.AppointmentRepository.GetDoctorAppointmentsAsync(doctorId, param);

                if (result == null)
                    return NotFound(new { success = false, message = $"No appointments found for doctor with ID {doctorId}." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving doctor appointments.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-patient")]
        public async Task<IActionResult> GetPatientAppointmentsAsync(int patientId, [FromQuery] PaginationParams param)
        {
            try
            {
                var result = await work.AppointmentRepository.GetPatientAppointmentsAsync(patientId, param);

                if (result == null)
                    return NotFound(new { success = false, message = $"No appointments found for patient with ID {patientId}." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving patient appointments.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region My Appointments (Doctor / Patient)

        [Authorize(Roles = "Doctor")]
        [HttpGet("my-doctor")]
        public async Task<IActionResult> GetMyDoctorAppointments([FromQuery] PaginationParams param)
        {
            try
            {
                var doctorId = GetCurrentUid();

                var result = await work.AppointmentRepository
                    .GetDoctorAppointmentsAsync(int.Parse(doctorId!), param);

                if (result == null)
                    return NotFound(new { success = false, message = "No appointments found." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving doctor appointments.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my-patient")]
        public async Task<IActionResult> GetMyPatientAppointments([FromQuery] PaginationParams param)
        {
            try
            {
                var patientId = GetCurrentUid();

                var result = await work.AppointmentRepository
                    .GetPatientAppointmentsAsync(int.Parse(patientId!), param);

                if (result == null)
                    return NotFound(new { success = false, message = "No appointments found." });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving patient appointments.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Shared Get

        [Authorize(Roles = "Admin,Doctor,Patient")]
        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetAppointmentWithDetailsAsync([FromQuery] int id)
        {
            try
            {
                var result = await work.AppointmentRepository.GetAppointmentWithDetailsAsync(id);

                if (result == null)
                    return NotFound(new { success = false, message = $"No appointment found with ID {id}." });

                if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                {
                    if (result.PatientAppUserId != GetCurrentUid())
                        return Forbid();
                }

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving the appointment.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Edit Controllers

        [Authorize(Roles = "Admin,Patient")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateAppointmentAsync([FromBody] CreateAppointmentDTO dto)
        {
            try
            {
                var result = await work.AppointmentRepository.CreateAppointmentAsync(dto);

                if (result == null)
                    return BadRequest(new { success = false, message = "Failed to create the appointment." });

                if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                {
                    if (result.PatientAppUserId != GetCurrentUid())
                        return Forbid();
                }

                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while creating the appointment.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAppointmentAsync([FromBody] UpdateAppointmentDTO dto)
        {
            try
            {
                if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                {
                    var appointment = await work.AppointmentRepository
                        .GetAppointmentWithDetailsAsync(dto.Id);

                    if (appointment == null)
                        return NotFound(new { success = false, message = $"No appointment found with ID {dto.Id}." });

                    if (appointment.PatientAppUserId != GetCurrentUid())
                        return Forbid();
                }

                var result = await work.AppointmentRepository.UpdateAppointmentAsync(dto);

                if (!result.Success)
                    return BadRequest(new { success = false, message = result.Message });

                return Ok(new { success = true, message = result.Message, data = result.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while updating the appointment.",
                    error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Patient")]
        [HttpDelete("cancel")]
        public async Task<IActionResult> CancelAppointmentAsync([FromQuery] int id)
        {
            try
            {
                if (User.IsInRole("Patient") && !User.IsInRole("Admin"))
                {
                    var appointment = await work.AppointmentRepository
                        .GetAppointmentWithDetailsAsync(id);

                    if (appointment == null)
                        return NotFound(new { success = false, message = $"No appointment found with ID {id}." });

                    if (appointment.PatientAppUserId != GetCurrentUid())
                        return Forbid();
                }

                var result = await work.AppointmentRepository.CancelAppointmentAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = $"No appointment found with ID {id}." });

                return Ok(new { success = true, message = "Appointment cancelled successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while cancelling the appointment.",
                    error = ex.Message
                });
            }
        }

        #endregion

        #region Helpers

        private string? GetCurrentUid() =>
            User.FindFirst("uid")?.Value;

        #endregion
    }
}