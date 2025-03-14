
namespace QuanLiKhiThai.DAO.Interface
{
    public interface ILogDAO : IServiceDAO<Log>
    {
        IEnumerable<Log> GetLogSince(DateTime since);
        IEnumerable<Log> GetLogsByUser(int userId, int take = 100);
        IEnumerable<Log> SearchLog(string searchQuery, int take = 100);
        IEnumerable<Log> GetLogInDateRange(DateTime start, DateTime end);
    }
}
