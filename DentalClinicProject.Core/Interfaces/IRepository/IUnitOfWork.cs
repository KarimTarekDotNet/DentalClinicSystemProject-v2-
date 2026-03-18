using DentalClinicProject.Core.Interfaces.IServices;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IUnitOfWork
    {
        // Repositories
        IAppointmentRepository AppointmentRepository { get; }
        ICartRepository CartRepository { get; }
        IProductRepository ProductRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IRateRepository RateRepository { get; }
        IAdminRepository AdminManager { get; }
        IProfileRepository ProfileRepository { get; }

        // Services
        ITokenService TokenService { get; }
        IRedisService RedisService { get; }
        IAuthService AuthService { get; }
        IEmailVerificationService EmailVerificationService { get; }
        IPhoneVerificationService PhoneVerificationService { get; }
        IExternalLoginService ExternalLoginService { get; }
    }
}