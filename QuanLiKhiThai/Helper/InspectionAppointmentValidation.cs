using QuanLiKhiThai.DAO;
using System.Windows;

namespace QuanLiKhiThai.Context
{
    class InspectionAppointmentValidation
    {

        public static bool ValidatingScheduling(int vehicleId)
        {
            InspectionRecord? lastRecord = InspectionRecordDAO.GetLastRecordByVehicleId(vehicleId);
            if (lastRecord == null)
                return true;

            if (lastRecord.Result == Constants.RESULT_FAILED)
                return true;

            if (lastRecord.Result == Constants.RESULT_PASSED)
            {
                DateTime? lastInspectionDate = lastRecord.InspectionDate;
                if (!lastInspectionDate.HasValue)
                {
                    MessageBox.Show("Previous inspection record is missing inspection date.",
                        "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true; // Allow inspection due to data inconsistency
                }

                if (InspectionAppointmentDAO.HasPendingAppointment(vehicleId))
                {
                    MessageBox.Show("This vehicle already has a pending appointment. Please check your appointments list.",
                        "Duplicate Appointment", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                Vehicle? vehicle = VehicleDAO.GetVehicleById(vehicleId);
                if (vehicle == null)
                {
                    MessageBox.Show("Vehicle not found.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

                    MessageBox.Show($"The vehicle's previous inspection is still valid. Next inspection allowed in {daysUntilAllowed} days.",
                        "Early Inspection", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Ask if they want to proceed
                    MessageBoxResult result = MessageBox.Show(
                        "Do you want to proceed with a special out-of-cycle inspection?\n" +
                        "This should only be done for vehicles with significant modifications or repairs.",
                        "Confirm Special Inspection",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    return result == MessageBoxResult.Yes;
                }
            }
            return true;
        }

        public static bool ValidateAssignment(List<InspectionAppointment> appointments, int vehicleId)
        {
            if (appointments.Count == 0)
            {
                MessageBox.Show("No inspection appointment found for this vehicle", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            InspectionAppointment appointment = InspectionAppointmentDAO.GetLastAppointment(appointments);
            if(appointments.Count == 0) 
                return ValidateFirstInspection(appointment);
            if (!ValidateDataConsistency(appointment))
                return false;

            return true;
        }

        public static bool ValidateFirstInspection(InspectionAppointment appointment)
        {
            // TODO1: Check whether the status of the first appointment is "Pending"
            if (appointment.Status != Constants.STATUS_PENDING)
            {
                MessageBox.Show("Cannot create assignment. The appointment is not in Pending status.",
                    "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // TODO2: Check whether the record is already assigned
            if (InspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId) != null)
            {
                MessageBox.Show("Cannot create assignment. The vehicle is already assigned to another inspector.",
                    "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true; 
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

        private static void LogValidationError(string mess, int? userId = null)
        {
            try
            {
                int actualUserId = userId ?? UserContext.Current.UserId;
                Log validationLog = new Log
                {
                    UserId = actualUserId,
                    Action = mess,
                    Timestamp = DateTime.Now
                };
                bool logSuccess = LogDAO.AddLog(validationLog);
                System.Diagnostics.Debug.WriteLine($"Validation error logged: {mess}");
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to log validation error: {e.Message}");
                System.Diagnostics.Debug.WriteLine($"Original message: {mess}");
            }
        }

        public static bool ValidateDataConsistency(InspectionAppointment appointment, string newStatus = null)
        {
            int currentUserId = UserContext.Current.UserId;
            if (appointment == null)
            {
                LogValidationError("Cannot validate data consistency for null appointment", currentUserId);
            }
            using (var db = new QuanLiKhiThaiContext())
            {
                var record = InspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId);
                string status = newStatus ?? appointment.Status;


                // TODO1: Pending/Cancelled appointments should not have records
                if ((status == Constants.STATUS_PENDING || status == Constants.STATUS_CANCELLED) && record != null)
                {
                     string err = $"Data inconsistency: {status} appointments should not have records. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                    LogValidationError(err, currentUserId); 
                    return false;
                }

                // TODO2: Assigned appointments should have records but not completed
                if (status == Constants.STATUS_ASSIGNED && (record?.Result == Constants.RESULT_PASSED || record?.Result == Constants.RESULT_FAILED))
                {
                    string err = $"Data inconsistency: Assigned appointments shouldn't have final inspection results yet. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                    LogValidationError(err, currentUserId);
                    return false;
                }

                // TODO3: Completed appointments should have records with final results
                if (status == Constants.STATUS_COMPLETED && (record == null || record.Result == Constants.RESULT_TESTING))
                {
                    string err = $"Data inconsistency: Completed appointments should have final inspection results. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                    if (record != null)
                        err += $", RecordResult: {record.Result}";
                    else
                        err += ", No record found";
                    LogValidationError(err, currentUserId);
                    return false;
                }
            }
            return true;
        }
    }
}
