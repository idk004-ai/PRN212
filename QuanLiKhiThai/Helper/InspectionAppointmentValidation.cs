using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System.Windows;
using System.Transactions;

namespace QuanLiKhiThai.Context
{
    class InspectionAppointmentValidation
    {
        public static bool ValidatingScheduling(int vehicleId)
        {
            try
            {
                var inspectionRecordDAO = App.GetService<IInspectionRecordDAO>();

                // Check if the vehicle has any ongoing inspection (Testing status)
                InspectionRecord? currentRecord = inspectionRecordDAO.GetCurrentRecordByVehicleId(vehicleId);
                if (currentRecord != null && currentRecord.Result == Constants.RESULT_TESTING)
                {
                    // Get station info to display in the message
                    User? station = currentRecord.Station;
                    string stationInfo = station != null ? $" at {station.FullName}" : "";

                    string errorMessage = $"This vehicle is currently undergoing inspection{stationInfo}. " +
                                        $"Please wait for the inspection to be completed before scheduling a new appointment.";
                    LogValidationError(errorMessage, null);
                    MessageBox.Show(errorMessage, "Vehicle In Testing Process", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                InspectionRecord? lastRecord = inspectionRecordDAO.GetLastRecordByVehicleId(vehicleId);
                if (lastRecord == null)
                    return true;

                if (lastRecord.Result == Constants.RESULT_FAILED)
                    return true;

                if (lastRecord.Result == Constants.RESULT_PASSED)
                {
                    DateTime? lastInspectionDate = lastRecord.InspectionDate;

                    var inspectionAppointmentDAO = App.GetService<IInspectionAppointmentDAO>();

                    if (inspectionAppointmentDAO.HavePendingAppointment(vehicleId))
                    {
                        string warningMessage = "This vehicle already has a pending appointment. Please check your appointments list.";
                        LogValidationError(warningMessage, null);
                        MessageBox.Show(warningMessage, "Duplicate Appointment", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }

                    var vehicleDAO = App.GetService<IVehicleDAO>();
                    Vehicle? vehicle = vehicleDAO.GetById(vehicleId);
                    if (vehicle == null)
                    {
                        string errorMessage = "Vehicle not found.";
                        LogValidationError(errorMessage, null);
                        MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    int monthsUntilNextInspection = GetRequiredInspectionIntervalMonths(vehicle);
                    DateTime nextRequiredDate = lastInspectionDate.Value.AddMonths(monthsUntilNextInspection);

                    // Allow 30 days early inspection before the due date
                    DateTime earliestAllowedDate = nextRequiredDate.AddDays(-30);

                    if (DateTime.Now < earliestAllowedDate)
                    {
                        // calculate days until next allowed inspection
                        int daysUntilAllowed = (earliestAllowedDate - DateTime.Now).Days;

                        string warningMessage = $"The vehicle's previous inspection is still valid. Next inspection allowed in {daysUntilAllowed} days.";
                        LogValidationError(warningMessage, null);
                        MessageBox.Show(warningMessage, "Early Inspection", MessageBoxButton.OK, MessageBoxImage.Warning);

                        // Ask if they want to proceed
                        MessageBoxResult result = MessageBox.Show(
                            "Do you want to proceed with a special out-of-cycle inspection?\n" +
                            "This should only be done for vehicles with significant modifications or repairs.",
                            "Confirm Special Inspection",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            LogValidationError("User confirmed special out-of-cycle inspection.", null);
                        }

                        return result == MessageBoxResult.Yes;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogValidationError($"Error during scheduling validation: {ex.Message}", null, ex);
                MessageBox.Show($"An error occurred during validation: {ex.Message}",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool ValidateAssignment(List<InspectionAppointment> appointments, int vehicleId)
        {
            try
            {
                if (appointments.Count == 0)
                {
                    string errorMessage = "No inspection appointment found for this vehicle";
                    LogValidationError(errorMessage, null);
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var inspectionAppointmentDAO = App.GetService<IInspectionAppointmentDAO>();
                InspectionAppointment appointment = inspectionAppointmentDAO.GetLast(appointments);

                if (appointment == null)
                {
                    string errorMessage = "Failed to determine the latest appointment";
                    LogValidationError(errorMessage, null);
                    MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                if (appointments.Count == 0)
                    return ValidateFirstInspection(appointment);

                if (!ValidateDataConsistency(appointment))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                LogValidationError($"Error during assignment validation: {ex.Message}", null, ex);
                MessageBox.Show($"An error occurred during validation: {ex.Message}",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool ValidateFirstInspection(InspectionAppointment appointment)
        {
            try
            {
                var inspectionRecordDAO = App.GetService<IInspectionRecordDAO>();

                // Check whether the status of the first appointment is "Pending"
                if (appointment.Status != Constants.STATUS_PENDING)
                {
                    string errorMessage = "Cannot create assignment. The appointment is not in Pending status.";
                    LogValidationError(errorMessage, null);
                    MessageBox.Show(errorMessage, "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Check whether the record is already assigned
                if (inspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId) != null)
                {
                    string errorMessage = "Cannot create assignment. The vehicle is already assigned to another inspector.";
                    LogValidationError(errorMessage, null);
                    MessageBox.Show(errorMessage, "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogValidationError($"Error during first inspection validation: {ex.Message}", null, ex);
                MessageBox.Show($"An error occurred during validation: {ex.Message}",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private static int GetRequiredInspectionIntervalMonths(Vehicle? vehicle)
        {
            int vehicleAge = DateTime.Now.Year - vehicle.ManufactureYear;
            if (vehicleAge < 10)
            {
                return 24; // 2 years
            }
            else
            {
                return 12; // 1 year
            }
            // Additional rules can be added here
        }

        /// <summary>
        /// Logs a validation error message to the system log
        /// </summary>
        /// <param name="message">Error message to log</param>
        /// <param name="userId">Optional user ID, will use current user if null</param>
        /// <param name="ex">Optional exception object for additional details</param>
        private static void LogValidationError(string message, int? userId = null, Exception ex = null)
        {
            try
            {
                // Get current user ID or use a system account ID if no user is logged in
                int actualUserId = userId ?? UserContext.Current?.UserId ?? 1;

                Log validationLog = new Log
                {
                    UserId = actualUserId,
                    Action = $"Validation: {message}" +
                             (ex != null ? $" | Exception: {ex.GetType().Name}" : ""),
                    Timestamp = DateTime.Now
                };

                // Write to debug console for immediate visibility
                System.Diagnostics.Debug.WriteLine($"[VALIDATION] {validationLog.Timestamp}: {validationLog.Action}");

                // Try to persist the log to database
                try
                {
                    var logDAO = App.GetService<ILogDAO>();
                    if (logDAO != null)
                    {
                        // Using a transaction scope for logging
                        using (var logScope = new TransactionScope())
                        {
                            bool logSuccess = logDAO.Add(validationLog);
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
                    System.Diagnostics.Debug.WriteLine($"[CRITICAL] Failed to log validation error to database: {logEx.Message}");
                    System.Diagnostics.Debug.WriteLine($"Original validation message: {message}");
                }
            }
            catch (Exception)
            {
                // Last resort if everything fails - just write to debug output
                System.Diagnostics.Debug.WriteLine($"[CRITICAL] Complete logging failure in validation");
                System.Diagnostics.Debug.WriteLine($"Original validation message: {message}");
                if (ex != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Exception details: {ex}");
                }
            }
        }

        public static bool ValidateDataConsistency(InspectionAppointment appointment, string newStatus = null)
        {
            try
            {
                int currentUserId = UserContext.Current?.UserId ?? 1;

                if (appointment == null)
                {
                    string errorMessage = "Cannot validate data consistency for null appointment";
                    LogValidationError(errorMessage, currentUserId);
                    MessageBox.Show(errorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                var inspectionRecordDAO = App.GetService<IInspectionRecordDAO>();
                var record = inspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId);
                string status = newStatus ?? appointment.Status;

                // Pending/Cancelled appointments should not have records
                if ((status == Constants.STATUS_PENDING || status == Constants.STATUS_CANCELLED) && record != null)
                {
                    string errorMessage = $"Data inconsistency: {status} appointments should not have records. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                    LogValidationError(errorMessage, currentUserId);
                    MessageBox.Show(errorMessage, "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Assigned appointments should have records but not completed
                if (status == Constants.STATUS_ASSIGNED && (record?.Result == Constants.RESULT_PASSED || record?.Result == Constants.RESULT_FAILED))
                {
                    string errorMessage = $"Data inconsistency: Assigned appointments shouldn't have final inspection results yet. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                    LogValidationError(errorMessage, currentUserId);
                    MessageBox.Show(errorMessage, "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // Completed appointments should have records with final results
                if (status == Constants.STATUS_COMPLETED && (record == null || record.Result == Constants.RESULT_TESTING))
                {
                    string errorMessage = $"Data inconsistency: Completed appointments should have final inspection results. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                    if (record != null)
                        errorMessage += $", RecordResult: {record.Result}";
                    else
                        errorMessage += ", No record found";

                    LogValidationError(errorMessage, currentUserId);
                    MessageBox.Show(errorMessage, "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                int currentUserId = UserContext.Current?.UserId ?? 1;
                LogValidationError($"Error during data consistency validation: {ex.Message}", currentUserId, ex);
                MessageBox.Show($"An error occurred during validation: {ex.Message}",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
