using System.Windows;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IInspectionAppointmentDAO : IServiceDAO<InspectionAppointment>
    {
        IEnumerable<InspectionAppointment> GetByVehicleAndStation(int vehicleId, int stationId);
        InspectionAppointment GetLast(List<InspectionAppointment> appointments);
        bool HavePendingAppointment(int vehicleId);
    }
}
