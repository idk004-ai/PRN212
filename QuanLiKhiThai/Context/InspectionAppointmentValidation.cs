using QuanLiKhiThai.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLiKhiThai.Context
{
    class InspectionAppointmentValidation
    {

        public static bool ValidateAssignment(List<InspectionAppointment> appointments, int vehicleId)
        {
            if (appointments.Count == 0)
            {
                MessageBox.Show("No inspection appointment found for this vehicle", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (appointments.Count == 1)
            {
                ValidateFirstInspection(appointments[0]);
            }
            else
            {
                InspectionAppointment appointmentId = InspectionAppointmentDAO.GetLastAppointment(appointments);
                if (!ValidateReInspection(vehicleId, appointmentId.AppointmentId) || !ValidateDataConsistency(appointmentId))
                    return false;
            }
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

        public static bool ValidateReInspection(int vehicleId, int appointmentId)
        {
            InspectionRecord? lastRecord = InspectionRecordDAO.GetLastRecordByVehicleId(vehicleId);
            InspectionAppointment? appointment = InspectionAppointmentDAO.GetAppointmentById(appointmentId);
            if (appointment == null)
            {
                MessageBox.Show("Current appointment not found.",
                    "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (lastRecord.Result == Constants.RESULT_FAILED)
            {
                return true;
            }

            if (lastRecord.Result == Constants.RESULT_PASSED)
            {
                DateTime? lastInspectionDate = lastRecord.InspectionDate;
                if (!lastInspectionDate.HasValue)
                {
                    MessageBox.Show("Previous inspection record is missing inspection date.",
                        "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return true; // Allow inspection due to data inconsistency
                }
                // TODO: Calculate the time period on vehicle type
                Vehicle? vehicle = VehicleDAO.GetVehicleById(vehicleId);
                int monthsUntilNextInspection = GetRequiredInspectionIntervalMonths(vehicle);

                DateTime nextRequiredDate = lastInspectionDate.Value.AddMonths(monthsUntilNextInspection);

                // Allow 30 days early inspection before the due date
                DateTime earliestAllowedDate = nextRequiredDate.AddDays(-30);

                if ( DateTime.Now < earliestAllowedDate) 
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

        public static bool ValidateDataConsistency(InspectionAppointment appointment, string newStatus = null)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var record = InspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId);

                string status = newStatus ?? appointment.Status;

                // TODO1: Pending/Cancelled appointments should not have records
                if ((status == Constants.STATUS_PENDING || status == Constants.STATUS_CANCELLED) && record != null)
                {
                        MessageBox.Show("Data inconsistency: Pending/Cancelled appointments should not have records.",
                            "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                }

                // TODO2: Assigned appointments should have records but not completed
                if (status == Constants.STATUS_ASSIGNED && (record?.Result == Constants.RESULT_PASSED || record?.Result == Constants.RESULT_FAILED))
                {
                    MessageBox.Show("Data inconsistency: Assigned or Confirmed appointments shouldn't have final inspection results yet",
                       "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                // TODO3: Completed appointments should have records with final results
                if (status == Constants.STATUS_COMPLETED && (record == null || record.Result == Constants.RESULT_TESTING))
                {
                    MessageBox.Show("Data inconsistency: Completed appointments should have final inspection results",
                        "Data Consistency Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;
        }
    }
}
