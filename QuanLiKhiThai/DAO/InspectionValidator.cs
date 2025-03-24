using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;

namespace QuanLiKhiThai.DAO
{
    public class InspectionValidator : IInspectionValidator
    {

        private readonly Lazy<IInspectionRecordDAO> _inspectionRecordDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IUserDAO _userDAO;
        private readonly IValidationLogger _logger;
        private readonly InspectionRules _rules;

        public InspectionValidator(
            Lazy<IInspectionRecordDAO> inspectionRecordDAO,
            IInspectionAppointmentDAO inspectionAppointmentDAO,
            IVehicleDAO vehicleDAO,
            IUserDAO userDAO,
            IValidationLogger logger,
            InspectionRules rules
        )
        {
            _inspectionRecordDAO = inspectionRecordDAO;
            _inspectionAppointmentDAO = inspectionAppointmentDAO;
            _vehicleDAO = vehicleDAO;
            _userDAO = userDAO;
            _logger = logger;
            _rules = rules;
        }

        private static int GetRequiredInspectionIntervalMonths(Vehicle vehicle)
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

        public ValidationResult ValidateScheduling(int vehicleId, int stationId)
        {
            // Get vehicle and station
            Vehicle? vehicle = _vehicleDAO.GetById(vehicleId);
            if (vehicle == null)
            {
                string errorMessage = "Vehicle not found";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Vehicle not found",
                    ResultType = ValidationResultType.Error
                };
            }

            User? station = _userDAO.GetById(stationId);
            if (station == null)
            {
                string errorMessage = "Station not found";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Station not found",
                    ResultType = ValidationResultType.Error
                };
            }

            List<InspectionAppointment> appointments = _inspectionAppointmentDAO.GetByVehicleAndStation(vehicleId, stationId).ToList();
            bool hasPendingAppointment = appointments.Any(a => a.Status == Constants.STATUS_PENDING);
            if (hasPendingAppointment)
            {
                string errorMessage = "This vehicle has a pending appointment. Please wait for the current appointment to be completed inspection before scheduling a new one.";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Pending Appointment",
                    ResultType = ValidationResultType.Warning
                };
            }

            List<InspectionRecord> records = _inspectionRecordDAO.Value.GetRecordByVehicle(vehicleId).ToList();
            if (records.Count == 0)
            {
                return new ValidationResult { IsValid = true };
            }

            InspectionRecord? existTestingRecord = records.FirstOrDefault(r => r.Result == Constants.RESULT_TESTING);
            if (existTestingRecord != null)
            {
                User? existTestingStation = existTestingRecord.Station;
                string stationInfo = existTestingStation != null ? $" at {existTestingStation.FullName}" : "";

                string errorMessage = $"This vehicle is currently undergoing inspection{stationInfo}. " +
                                $"Please wait for the inspection to be completed before scheduling a new appointment.";
                _logger.LogValidationMessage(errorMessage);

                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Vehicle in Testing process",
                    ResultType = ValidationResultType.Warning
                };
            }

            InspectionRecord? lastRecord = records.OrderByDescending(r => r.InspectionDate).FirstOrDefault();
            if (lastRecord == null)
            {
                return new ValidationResult { IsValid = true };
            }
                
            if (lastRecord.Result == Constants.RESULT_FAILED)
            {
                return new ValidationResult { IsValid = true };
            }
            else // last record is PASSED
            {
                DateTime? lastInspectionDate = lastRecord.InspectionDate;

                int monthsUntilNextInspection = GetRequiredInspectionIntervalMonths(vehicle);
                DateTime nextRequiredDate = lastInspectionDate.Value.AddMonths(monthsUntilNextInspection);

                // Allow 30 days early inspection before the due date
                DateTime earliestAllowedDate = nextRequiredDate.AddDays(-30);

                if (DateTime.Now < earliestAllowedDate)
                {
                    // calculate days until next allowed inspection
                    int daysUntilAllowed = (earliestAllowedDate - DateTime.Now).Days;

                    string errorMessage = $"The vehicle's previous inspection is still valid. Next inspection allowed in {daysUntilAllowed} days.";
                    _logger.LogValidationMessage(errorMessage);
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = errorMessage,
                        Title = "Early Inspection",
                        ResultType = ValidationResultType.ConfirmationRequired
                    };
                }
            }

            return new ValidationResult { IsValid = true };
        }

        public ValidationResult ValidateAssignment(int appointmentId)
        {
            InspectionAppointment? appointment = _inspectionAppointmentDAO.GetById(appointmentId);
            if (appointment == null)
            {
                string errorMessage = "Appointment not found";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Appointment not found",
                    ResultType = ValidationResultType.Error
                };
            }

            if (appointment.Status != Constants.STATUS_PENDING)
            {
                string errorMessage = "This appointment is not pending. Assignment is not allowed.";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Invalid Appointment Status",
                    ResultType = ValidationResultType.Error
                };
            }

            InspectionRecord? record = _inspectionRecordDAO.Value.GetRecordByAppointment(appointmentId);
            if (record != null)
            {
                string errorMessage = "An inspector has already been assigned to this appointment.";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Inspector already assigned",
                    ResultType = ValidationResultType.Error
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public ValidationResult ValidateDataConsistency(int appointmentId, string? newStatus)
        {
            int currentUserId = UserContext.Current?.UserId ?? Constants.SYSTEM_USER_ID;
            InspectionAppointment? appointment = _inspectionAppointmentDAO.GetById(appointmentId);
            if (appointment == null)
            {
                string errorMessage = "Appointment not found";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Appointment not found",
                    ResultType = ValidationResultType.Error
                };
            }

            var record = _inspectionRecordDAO.Value.GetRecordByAppointment(appointmentId);
            string status = newStatus ?? appointment.Status;

            // Pending appointments should not have records and can be cancelled
            if (status == Constants.STATUS_PENDING && record != null && record.Result != Constants.RESULT_CANCELLED)
            {
                string errorMessage = "Data inconsistency: Pending appointments shouldn't have active inspection records. AppointmentId: " + appointment.AppointmentId;
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Data Consistency Error",
                    ResultType = ValidationResultType.Error
                };
            }

            // Cancelled appointments should have records with Result = CANCELLED
            if (status == Constants.STATUS_CANCELLED && (record == null || record.Result != Constants.RESULT_CANCELLED))
            {
                string errorMessage = "Data inconsistency: Cancelled appointments should have inspection records with Cancelled result. AppointmentId: " + appointment.AppointmentId;
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Data Consistency Error",
                    ResultType = ValidationResultType.Error
                };
            }

            // Assigned appointments should have records but not completed
            if (status == Constants.STATUS_ASSIGNED && (record?.Result == Constants.RESULT_PASSED || record?.Result == Constants.RESULT_FAILED))
            {
                string errorMessage = $"Data inconsistency: Assigned appointments shouldn't have final inspection results yet. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Data Consistency Error",
                    ResultType = ValidationResultType.Error
                };
            }

            // Completed appointments should have records with final results
            if (status == Constants.STATUS_COMPLETED && (record == null || record.Result == Constants.RESULT_TESTING || record.Result == Constants.RESULT_CANCELLED))
            {
                string errorMessage = $"Data inconsistency: Completed appointments should have final inspection results. AppointmentId: {appointment.AppointmentId}, Status: {status}";
                if (record != null)
                    errorMessage += $", RecordResult: {record.Result}";
                else
                    errorMessage += ", No record found";

                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Data Consistency Error",
                    ResultType = ValidationResultType.Error
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public ValidationResult ValidateDataConsistencyForRecordCancellation(int recordId)
        {
            InspectionRecord? record = _inspectionRecordDAO.Value.GetById(recordId
                );
            if (record == null || record.Appointment == null)
            {
                // Return appropriate error
                string errorMessage = "Appointment or Inspection Record not found. Inspector can not cancel inspection";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Appointment not found",
                    ResultType = ValidationResultType.Error
                };
            }

            InspectionAppointment appointment = record.Appointment;

            // Only allow ASSIGNED appointments to be cancelled
            if (appointment.Status != Constants.STATUS_ASSIGNED)
            {
                string errorMessage = $"Cannot cancel inspection with appointment'status: {appointment.Status}. Only Assigned appointments can be cancelled.";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Invalid Operation",
                    ResultType = ValidationResultType.Error
                };
            }

            // validate record status
            // Only allow records in TESTING state to be cancelled
            if (record.Result != Constants.RESULT_TESTING)
            {
                string errorMessage = $"Cannot cancel inspection with result {record.Result}. Only Testing inspections can be cancelled.";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Invalid Operation",
                    ResultType = ValidationResultType.Error
                };
            }

            return new ValidationResult { IsValid = true };
        }

        public ValidationResult ValidateDataConsistencyForAppointmentCancellation(int appointmentId)
        {
            InspectionAppointment? appointment = _inspectionAppointmentDAO.GetById(appointmentId);
            if (appointment == null)
            {
                string errorMessage = "Appointment not found. Cannot cancel appointment";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Appointment not found",
                    ResultType = ValidationResultType.Error
                };
            }

            // Only allow PENDING appointments to be cancelled
            if (appointment.Status != Constants.STATUS_PENDING)
            {
                string errorMessage = $"Cannot cancel appointment with status {appointment.Status}. Only Pending appointments can be cancelled.";
                _logger.LogValidationError(errorMessage);
                return new ValidationResult
                {
                    IsValid = false,
                    Message = errorMessage,
                    Title = "Invalid Operation",
                    ResultType = ValidationResultType.Error
                };
            }

            return new ValidationResult { IsValid = true };
        }
    }
}
