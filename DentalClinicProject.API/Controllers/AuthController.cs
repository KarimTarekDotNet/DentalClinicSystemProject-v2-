using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicProject.API.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO dto)
        {
            var response = await work.AuthService.Register(dto);
            return StatusCode(response.StatusCode, response);
        }
    }
}