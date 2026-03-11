using AutoMapper;
using DentalClinicProject.Core.Interfaces.IRepository;

namespace DentalClinicProject.API.Controllers.Core
{
    public class ProductController : BaseController
    {
        public ProductController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}