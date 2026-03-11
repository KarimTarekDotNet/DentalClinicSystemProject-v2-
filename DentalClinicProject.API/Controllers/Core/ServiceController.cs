using AutoMapper;
using DentalClinicProject.Core.Interfaces.IRepository;

namespace DentalClinicProject.API.Controllers.Core
{
    public class ServiceController : BaseController
    {
        public ServiceController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}