using AutoMapper;
using DentalClinicProject.Core.DTOs.Core.Create;
using DentalClinicProject.Core.DTOs.Core.Update;
using DentalClinicProject.Core.Helpers;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DentalClinicProject.API.Controllers.Core
{
    public class AppointmentController : BaseController
    {
        public AppointmentController(IUnitOfWork work, IMapper mapper) : base(work, mapper) { }

        #region Get Controllers
        [HttpGet("get-appointments")]
        public async Task<IActionResult> GetAppointments([FromQuery] PaginationParams param)
        {
            try
            {
                var result = await work.AppointmentRepository.GetAppointmentsWithDetailsAsync(param);
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }
        [HttpGet("get-appointments-with-doctorId")]
        public async Task<IActionResult> GetDoctorAppointmentsAsync(int doctorId, [FromQuery] PaginationParams param)
        {
            try
            {
                var result = await work.AppointmentRepository.GetDoctorAppointmentsAsync(doctorId, param);
                if (result == null)
                    return NotFound("Appointment or doctor not found");

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }

        [HttpGet("get-appointments-with-patientId")]
        public async Task<IActionResult> GetPatientAppointmentsAsync(int patientId, PaginationParams param)
        {
            try
            {
                var result = await work.AppointmentRepository.GetPatientAppointmentsAsync(patientId, param);
                if (result == null)
                    return NotFound("Appointment or doctor not found");

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetAppointmentWithDetailsAsync([FromQuery] int id)
        {
            try
            {
                var result = await work.AppointmentRepository.GetAppointmentWithDetailsAsync(id);

                if (result == null)
                    return NotFound("Appointment not found");

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }
        #endregion

        #region Edit Controllers

        [HttpPost("create-appiontment")]
        public async Task<IActionResult> CreateAppointmentAsync(CreateAppointmentDTO dto, int patientId)
        {
            try
            {
                var result = await work.AppointmentRepository.CreateAppointmentAsync(dto, patientId);

                if (result == null)
                    return BadRequest("Appointment not created successfully");

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }

        [HttpPut("update-appiontment")]
        public async Task<IActionResult> UpdateAppointmentAsync(int id, UpdateAppointmentDTO dto)
        {
            try
            {
                var result = await work.AppointmentRepository.UpdateAppointmentAsync(id, dto);

                if (!result)
                    return BadRequest("Appointment not created successfully");

                var updatedAppointment = await work.AppointmentRepository.GetAppointmentWithDetailsAsync(id);
                return Ok(new
                {
                    success = true,
                    data = updatedAppointment
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }
        [HttpDelete("cancel-appiontment")]
        public async Task<IActionResult> CancelAppointmentAsync(int id)
        {
            try
            {
                var result = await work.AppointmentRepository.CancelAppointmentAsync(id);

                if (!result)
                    return BadRequest("Appointment not created successfully");

                return Ok(new
                {
                    success = true,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving appointments",
                    error = ex.Message
                });
            }
        }

        #endregion
    }
}