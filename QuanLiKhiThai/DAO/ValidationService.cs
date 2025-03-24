

using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;

namespace QuanLiKhiThai.DAO
{
    /// <summary>
    /// Service that combines validation logic with notifications
    /// </summary>
    public class ValidationService
    {
        private readonly IInspectionValidator _validator;
        private readonly IValidationNotifier _notifier;

        public ValidationService(IInspectionValidator validator, IValidationNotifier notifier)
        {
            _validator = validator;
            _notifier = notifier;
        }

        /// <summary>
        /// Validates scheduling and shows appropriate notifications
        /// </summary>
        /// <param name="vehicleId">Vehicle ID to validate</param>
        /// <param name="stationId">Station ID for validation</param>
        /// <returns>True if validation passes or user confirms special case, false otherwise</returns>
        public bool ValidateScheduling(int vehicleId, int stationId)
        {
            ValidationResult result = _validator.ValidateScheduling(vehicleId, stationId);
            return _notifier.ProcessValidationResult(result);
        }


        /// <summary>
        /// Validates assignment and shows appropriate notifications
        /// </summary>
        public bool ValidateAssignment(int appointmentId)
        {
            ValidationResult result = _validator.ValidateAssignment(appointmentId);
            return _notifier.ProcessValidationResult(result);
        }

        /// <summary>
        /// Validates data consistency and shows appropriate notifications
        /// </summary>
        public bool ValidateDataConsistency(int appointmentId, string? newStatus = null)
        {
            ValidationResult result = _validator.ValidateDataConsistency(appointmentId, newStatus);
            return _notifier.ProcessValidationResult(result);
        }

        public bool ValidateDataConsistencyForRecordCancellation(int recordId)
        {
            ValidationResult result = _validator.ValidateDataConsistencyForRecordCancellation(recordId);
            return _notifier.ProcessValidationResult(result);
        }

        public bool ValidateDataConsistencyForAppointmentCancellation(int appointmentId)
        {
            ValidationResult result = _validator.ValidateDataConsistencyForAppointmentCancellation(appointmentId);
            return _notifier.ProcessValidationResult(result);
        }
    }
}
