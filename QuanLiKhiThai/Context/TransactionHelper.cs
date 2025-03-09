
using QuanLiKhiThai.DAO;
using System.Transactions;
using System.Windows;

class TransactionHelper
{
    /// <summary>
    /// Executes a set of database operations within a transaction scope, with optional logging and notifications
    /// </summary>
    /// <param name="operations"></param>
    /// <param name="logEntry"></param>
    /// <param name="notification"></param>
    /// <param name="successMessage"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static bool ExecuteTransaction(
    Dictionary<string, Func<bool>> operations,
    Log logEntry = null,
    Notification notification = null,
    string successMessage = null,
    string errorMessage = "Transaction failed"
    )
    {
        bool success = false;

        try
        {
            using (var transactionScope = new TransactionScope())
            {
                bool allOperationsSuccess = true;
                string failedOperation = null;

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
                            if (!LogDAO.AddLog(logEntry))
                            {
                                allOperationsSuccess = false;
                                failedOperation = "Logging";
                            }
                        }
                        catch (Exception logEx)
                        {
                            allOperationsSuccess = false;
                            failedOperation = $"Logging - Error: {logEx.Message}";
                        }
                    }

                    // Send notification if notification is provided
                    if (notification != null && allOperationsSuccess)
                    {
                        try
                        {
                            if (!NotificationDAO.AddNotification(notification))
                            {
                                allOperationsSuccess = false;
                                failedOperation = "Notification";
                            }
                        }
                        catch (Exception notifEx)
                        {
                            allOperationsSuccess = false;
                            failedOperation = $"Notification - Error: {notifEx.Message}";
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
                    MessageBox.Show(detailedError, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            // Hiển thị chi tiết lỗi
            MessageBox.Show($"{errorMessage}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        return success;
    }
}

