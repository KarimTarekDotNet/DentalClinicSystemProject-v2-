namespace DentalClinicProject.Core.Interfaces.Logging
{
    /// <summary>
    /// Custom application logger — wraps ILogger with structured, consistent log messages.
    /// </summary>
    public interface IAppLogger<T>
    {
        // ─── Info ────────────────────────────────────────────────────────────────
        void LogOperationStarted(string operation, object? context = null);
        void LogOperationCompleted(string operation, object? context = null);

        // ─── Warning ─────────────────────────────────────────────────────────────
        void LogNotFound(string entityName, object identifier);
        void LogEmptyResult(string operation, object? context = null);
        void LogUnauthorizedAccess(string operation, string userId);
        void LogBusinessRuleViolation(string operation, string reason);

        // ─── Error ───────────────────────────────────────────────────────────────
        void LogException(string operation, Exception ex, object? context = null);
        void LogValidationError(string operation, string details);
    }
}