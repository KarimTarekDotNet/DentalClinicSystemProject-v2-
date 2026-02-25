using DentalClinicProject.Core.Interfaces.IServices;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IUnitOfWork
    {
        // Services
        ITokenService TokenService { get; }
        IRedisService RedisService { get; }
        IAuthService AuthService { get; }
        IEmailVerificationService EmailVerificationService { get; }
        IPhoneVerificationService PhoneVerificationService { get; }
    }
}