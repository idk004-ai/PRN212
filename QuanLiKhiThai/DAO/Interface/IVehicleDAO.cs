namespace QuanLiKhiThai.DAO.Interface
{
    public interface IVehicleDAO : IServiceDAO<Vehicle>
    {
        IEnumerable<Vehicle> GetVehicleByOwnerId(int ownerId);
        IEnumerable<Vehicle> GetVehicleWithPendingStatus(int stationId);
        Vehicle? GetByPlateNumber(string plateNumber);
    }
}
