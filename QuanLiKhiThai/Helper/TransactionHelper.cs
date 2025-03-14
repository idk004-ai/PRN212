using QuanLiKhiThai;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System.Transactions;
using System.Windows;

class TransactionHelper
{
    /// <summary>
    /// Executes a set of database operations within a transaction scope, with optional logging and notifications
    /// </summary>
    /// <param name="operations">Dictionary of operations to execute within the transaction</param>
    /// <param name="logEntry">Optional log entry to record on success</param>
    /// <param name="notification">Optional notification to send on success</param>
    /// <param name="successMessage">Optional message to display on success</param>
    /// <param name="errorMessage">Message to display on error (defaults to "Transaction failed")</param>
    /// <returns>True if transaction was successful, false otherwise</returns>
    public static bool ExecuteTransaction(
        Dictionary<string, Func<bool>> operations,
        Log? logEntry = null,
        Notification? notification = null,
        string? successMessage = null,
        string errorMessage = "Transaction failed"
    )
    {
        bool success = false;

        try
        {
            using (var transactionScope = new TransactionScope())
            {
                bool allOperationsSuccess = true;
                string? failedOperation = null;

                // Execute all operations
                foreach (var operation in operations)
                {
                    try
                    {
                        if (!operation.Value())
                        {
                            allOperationsSuccess = false;
                            failedOperation = operation.Key;
                            break;
                        }
                    }
                    catch (Exception opEx)
                    {
                        allOperationsSuccess = false;
                        failedOperation = $"{operation.Key} - Error: {opEx.Message}";
                        LogError($"Operation error in '{operation.Key}': {opEx.Message}", opEx);
                        break;
                    }
                }

                // If all operations are successful, commit the transaction
                if (allOperationsSuccess)
                {
                    // Log the action if log entry is provided
                    if (logEntry != null)
                    {
                        try
                        {
                            // Get ILogDAO from DI container
                            var logDAO = App.GetService<ILogDAO>();
                            if (!logDAO.Add(logEntry))
                            {
                                allOperationsSuccess = false;
                                failedOperation = "Logging";
                                LogError("Failed to add log entry to database", null);
                            }
                        }
                        catch (Exception logEx)
                        {
                            allOperationsSuccess = false;
                            failedOperation = $"Logging - Error: {logEx.Message}";
                            LogError($"Exception during logging: {logEx.Message}", logEx);
                        }
                    }

                    // Send notification if notification is provided
                    if (notification != null && allOperationsSuccess)
                    {
                        try
                        {
                            var notificationDAO = App.GetService<INotificationDAO>();
                            if (!notificationDAO.Add(notification))
                            {
                                allOperationsSuccess = false;
                                failedOperation = "Notification";
                                LogError("Failed to add notification to database", null);
                            }
                        }
                        catch (Exception notifEx)
                        {
                            allOperationsSuccess = false;
                            failedOperation = $"Notification - Error: {notifEx.Message}";
                            LogError($"Exception during notification: {notifEx.Message}", notifEx);
                        }
                    }

                    if (allOperationsSuccess)
                    {
                        transactionScope.Complete();
                        success = true;

                        if (!string.IsNullOrEmpty(successMessage))
                        {
                            MessageBox.Show(successMessage, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }

                if (!allOperationsSuccess)
                {
                    string detailedError = $"{errorMessage}: Failed at operation '{failedOperation}'";
                    LogError(detailedError, null);
                    MessageBox.Show(detailedError, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            string fullErrorMessage = $"{errorMessage}: {ex.Message}";
            LogError(fullErrorMessage, ex);
            MessageBox.Show(fullErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        return success;
    }

    /// <summary>
    /// Logs an error message to the system log
    /// </summary>
    /// <param name="message">Error message to log</param>
    /// <param name="ex">Optional exception object for additional details</param>
    private static void LogError(string message, Exception? ex)
    {
        try
        {
            // Get current user ID or use a system account ID if no user is logged in
            int userId = UserContext.Current?.UserId ?? 1; // 1 could be your system account ID

            Log errorLog = new Log
            {
                UserId = userId,
                Action = $"Transaction Error: {message}" +
                         (ex != null ? $" | Exception: {ex.GetType().Name}" : ""),
                Timestamp = DateTime.Now
            };

            // Write to debug console for immediate visibility
            System.Diagnostics.Debug.WriteLine($"[ERROR] {errorLog.Timestamp}: {errorLog.Action}");

            // Try to persist the log to database outside of the failed transaction
            try
            {
                var logDAO = App.GetService<ILogDAO>();
                if (logDAO != null)
                {
                    // Using a new transaction scope for just the logging
                    using (var logScope = new TransactionScope())
                    {
                        bool logSuccess = logDAO.Add(errorLog);
                        if (logSuccess)
                        {
                            logScope.Complete();
                        }
                    }
                }
            }
            catch (Exception logEx)
            {
                // If logging itself fails, at least output to debug console
                System.Diagnostics.Debug.WriteLine($"[CRITICAL] Failed to log error to database: {logEx.Message}");
                System.Diagnostics.Debug.WriteLine($"Original error: {message}");
            }
        }
        catch (Exception)
        {
            // Last resort if everything fails - just write to debug output
            System.Diagnostics.Debug.WriteLine($"[CRITICAL] Complete logging failure");
            System.Diagnostics.Debug.WriteLine($"Original error: {message}");
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"Exception details: {ex}");
            }
        }
    }
}
