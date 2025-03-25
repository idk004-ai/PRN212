namespace QuanLiKhiThai.DAO.Interface
{
    public interface IStationInspectorDAO : IServiceDAO<StationInspector>
    {
        IEnumerable<StationInspector> GetByStationId(int stationId);
        StationInspector? GetByInspectorId(int inspectorId);
        StationInspector? GetByStationAndInspectorId(int stationId, int inspectorId);
    }
}
