using AutoMapper;
using DentalClinicProject.Core.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinicProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected readonly IUnitOfWork work;

        public BaseController(IUnitOfWork work)
        {
            this.work = work;
        }
    }
}
