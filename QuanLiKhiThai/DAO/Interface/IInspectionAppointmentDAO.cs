using QuanLiKhiThai.Context;
using System.Windows;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IInspectionAppointmentDAO : IServiceDAO<InspectionAppointment>
    {
        IEnumerable<InspectionAppointment> GetByVehicleAndStation(int vehicleId, int stationId);
        bool HavePendingAppointment(int vehicleId);
        /// <summary>
        /// Cancel an appointment from station.
        /// WARNING: This method does not check for validation, because this method must ensure that station have sent the cancellation mail to user then update the appointment status.
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="station"></param>
        /// <param name="plateNumber"></param>
        /// <param name="fullName"></param>
        /// <param name="reason"></param>
        /// <param name="windowToClose"></param>
        /// <returns></returns>
        bool CancelAppointment(
            InspectionAppointment appointment,
            UserContext station,
            string plateNumber,
            string fullName,
            string reason,
            Window? windowToClose = null);
    }
}
