using DentalClinicProject.Core.Interfaces.Logging;
using Microsoft.Extensions.Logging;

namespace DentalClinicProject.Infrastructure.Logging
{
    /// <summary>
    /// Production-ready structured logger.
    /// All messages follow: [Class] | Operation | Detail | Context
    /// </summary>
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly ILogger<T> _logger;
        private readonly string _className;

        public AppLogger(ILogger<T> logger)
        {
            _logger = logger;
            _className = typeof(T).Name;
        }

        // ─── Info ────────────────────────────────────────────────────────────────

        public void LogOperationStarted(string operation, object? context = null)
        {
            if (context is not null)
                _logger.LogInformation("[{Class}] ▶ {Operation} started | Context: {@Context}", _className, operation, context);
            else
                _logger.LogInformation("[{Class}] ▶ {Operation} started", _className, operation);
        }

        public void LogOperationCompleted(string operation, object? context = null)
        {
            if (context is not null)
                _logger.LogInformation("[{Class}] ✔ {Operation} completed successfully | Context: {@Context}", _className, operation, context);
            else
                _logger.LogInformation("[{Class}] ✔ {Operation} completed successfully", _className, operation);
        }

        // ─── Warning ─────────────────────────────────────────────────────────────

        public void LogNotFound(string entityName, object identifier)
        {
            _logger.LogWarning("[{Class}] ⚠ {Entity} not found | Identifier: {Id}", _className, entityName, identifier);
        }

        public void LogEmptyResult(string operation, object? context = null)
        {
            if (context is not null)
                _logger.LogWarning("[{Class}] ⚠ {Operation} returned no results | Context: {@Context}", _className, operation, context);
            else
                _logger.LogWarning("[{Class}] ⚠ {Operation} returned no results", _className, operation);
        }

        public void LogUnauthorizedAccess(string operation, string userId)
        {
            _logger.LogWarning("[{Class}] 🚫 Unauthorized access attempt | Operation: {Operation} | UserId: {UserId}", _className, operation, userId);
        }

        public void LogBusinessRuleViolation(string operation, string reason)
        {
            _logger.LogWarning("[{Class}] ⛔ Business rule violated | Operation: {Operation} | Reason: {Reason}", _className, operation, reason);
        }

        // ─── Error ───────────────────────────────────────────────────────────────

        public void LogException(string operation, Exception ex, object? context = null)
        {
            if (context is not null)
                _logger.LogError(ex, "[{Class}] ✖ Unhandled exception | Operation: {Operation} | Context: {@Context} | Message: {Message}", _className, operation, context, ex.Message);
            else
                _logger.LogError(ex, "[{Class}] ✖ Unhandled exception | Operation: {Operation} | Message: {Message}", _className, operation, ex.Message);
        }

        public void LogValidationError(string operation, string details)
        {
            _logger.LogError("[{Class}] ✖ Validation failed | Operation: {Operation} | Details: {Details}", _className, operation, details);
        }
    }
}