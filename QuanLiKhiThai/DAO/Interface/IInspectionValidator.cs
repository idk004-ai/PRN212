using QuanLiKhiThai.Helper;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IInspectionValidator
    {
        /// <summary>
        /// <p>Check if the vehicle is currently in testing process</p>
        /// <para>
        /// Process flow:
        /// <list type="number">
        /// <item>Check if the vehicle has a record with result = TESTING</item>
        /// <item>If yes, inform the user that the vehicle is currently in testing process</item>
        /// <item>If not, check if the vehicle has a pending appointment, if yes, warning user</item>
        /// <item>If not, check if the vehicle has more than one recorded inspection</item>
        /// <item>If no prior inspections, return true</item>
        /// <item>If yes, check if the last inspection has result = FAILED, if yes, return true</item>
        /// <item>If not, calculate the time between the last inspection and the current time</item>
        /// <item>If this is an early inspection, require confirmation from user to wait for the next inspection or start now</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="vehicleId">ID of the vehicle to validate</param>
        /// <param name="stationId">ID of the station to validate</param>
        /// <returns>Validation result indicating if scheduling is allowed</returns>
        ValidationResult ValidateScheduling(int vehicleId, int stationId);

        /// <summary>
        /// Validates the data consistency of an appointment, optionally with a new status
        /// </summary>
        /// <param name="appointmentId">The id of appointment to validate</param>
        /// <param name="newStatus">Optional new status to validate</param>
        /// <returns>Validation result indicating if data is consistent</returns>
        ValidationResult ValidateDataConsistency(int appointmentId, string? newStatus = null);

        /// <summary>
        /// Validates the data consistency for inspection record cancellation
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        ValidationResult ValidateDataConsistencyForRecordCancellation(int recordId);
        /// <summary>
        /// Validates the data consistency for appointment cancellation
        /// </summary>
        /// <param name="appointmentId"></param>
        /// <returns></returns>
        ValidationResult ValidateDataConsistencyForAppointmentCancellation(int appointmentId);
    }
}
