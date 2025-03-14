namespace QuanLiKhiThai.DAO.Interface
{
    public interface IStationInspectorDAO : IServiceDAO<StationInspector>
    {
        IEnumerable<StationInspector> GetByStationId(int stationId);
        StationInspector? GetByStationAndInspectorId(int stationId, int inspectorId);
    }
}
