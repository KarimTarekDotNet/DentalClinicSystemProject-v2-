using AutoMapper;
using DentalClinicProject.Core.Interfaces.IRepository;

namespace DentalClinicProject.API.Controllers
{
    public class RateController : BaseController
    {
        public RateController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}