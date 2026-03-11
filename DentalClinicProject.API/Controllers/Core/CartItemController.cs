using AutoMapper;
using DentalClinicProject.Core.Interfaces.IRepository;

namespace DentalClinicProject.API.Controllers.Core
{
    public class CartItemController : BaseController
    {
        public CartItemController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
    }
}
