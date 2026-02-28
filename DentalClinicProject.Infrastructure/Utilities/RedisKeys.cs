namespace DentalClinicProject.Infrastructure.Utilities
{
    public static class RedisKeys
    {
        // User related keys
        public static string UserById(string userId) => $"User:Id:{userId}";
        public static string UserByEmail(string email) => $"User:Email:{email}";
        public static string UserByUsername(string username) => $"User:Username:{username}";
        public static string UserByPhone(string phone) => $"User:Phone:{phone}";
        
        // Verification codes
        public static string EmailVerificationCode(string email, string code) => $"{email}:Code:{code}";
        public static string ActiveEmailVerificationCode(string email) => $"ActiveVerification:Email:{email}";
        public static string PhoneVerificationCode(string phone) => $"user:{phone}";
        
        // Pending verification session (temporary token for resend operations)
        public static string PendingVerificationSession(string sessionToken) => $"PendingVerification:Session:{sessionToken}";
        public static string PendingVerificationByUserId(string userId) => $"PendingVerification:UserId:{userId}";
        
        // Tokens
        public static string RefreshToken(string userId, string ipAddress) => $"{userId}:RefreshToken:{ipAddress}";
        public static string BlacklistedToken(string token) => $"blacklist:{token}";
        public static string BlacklistedAccessToken(string userId, string accessToken) => $"{userId}:Blacklist:{accessToken}";
        
        // Rate limiting
        public static string RateLimitEmail(string email) => $"User:{email}";
        public static string RateLimitPhone(string phone) => $"User:{phone}";
    }
}
