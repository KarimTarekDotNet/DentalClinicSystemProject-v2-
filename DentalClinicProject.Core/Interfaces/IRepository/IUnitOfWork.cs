using DentalClinicProject.Core.Interfaces.IServices;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IUnitOfWork
    {
        // Repositories
        IAppointmentRepository AppointmentRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IProductRepository ProductRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IRateRepository RateRepository { get; }

        // Services
        ITokenService TokenService { get; }
        IRedisService RedisService { get; }
        IAuthService AuthService { get; }
        IEmailVerificationService EmailVerificationService { get; }
        IPhoneVerificationService PhoneVerificationService { get; }
        IExternalLoginService ExternalLoginService { get; }
    }
}