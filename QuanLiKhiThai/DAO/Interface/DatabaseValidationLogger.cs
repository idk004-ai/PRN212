using QuanLiKhiThai.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace QuanLiKhiThai.DAO.Interface
{
    class DatabaseValidationLogger : IValidationLogger
    {
        private readonly ILogDAO _logDAO;

        public DatabaseValidationLogger(ILogDAO logDAO)
        {
            _logDAO = logDAO;
        }

        public void LogValidationError(string message, int? userId = null, Exception? ex = null)
        {
            int actualUserId = userId ?? UserContext.Current?.UserId ?? Constants.SYSTEM_USER_ID;

            Log validationLog = new Log
            {
                UserId = actualUserId,
                Action = $"Validation Error: {message}" + (ex != null ? $" | Exception: {ex.GetType().Name} - {ex.Message}" : ""),
                Timestamp = DateTime.Now
            };

            // Try to persist the log to database
            try
            {
                // Using a transaction scope for logging
                using (var logScope = new TransactionScope())
                {
                    bool logSuccess = _logDAO.Add(validationLog);
                    if (logSuccess)
                    {
                        logScope.Complete();
                    }
                }
            }
            catch (Exception logEx)
            {
                // If logging itself fails, at least output to debug console
                System.Diagnostics.Debug.WriteLine($"[CRITICAL] Failed to log validation error to database: {logEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Original validation message: {message}");
            }
        }

        public void LogValidationMessage(string message, int? userId = null)
        {
            // Get current user ID or use a system account ID if no user is logged in
            int actualUserId = userId ?? UserContext.Current?.UserId ?? Constants.SYSTEM_USER_ID;

            Log validationLog = new Log
            {
                UserId = actualUserId,
                Action = $"Validation: {message}",
                Timestamp = DateTime.Now
            };

            // Try to persist the log to database
            try
            {
                // Using a transaction scope for logging
                using (var logScope = new TransactionScope())
                {
                    bool logSuccess = _logDAO.Add(validationLog);
                    if (logSuccess)
                    {
                        logScope.Complete();
                    }
                }
            }
            catch (Exception logEx)
            {
                // If logging itself fails, at least output to debug console
                System.Diagnostics.Debug.WriteLine($"[CRITICAL] Failed to log validation error to database: {logEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Original validation message: {message}");
            }
        }
    }
}
