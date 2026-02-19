using DentalClinicProject.Core.Interfaces.IServices;

namespace DentalClinicProject.Core.Interfaces.IRepository
{
    public interface IUnitOfWork
    {
        public ITokenService TokenService { get; }
        public IRedisService RedisService { get; }
        public IAuthService AuthService { get; }
    }
}
