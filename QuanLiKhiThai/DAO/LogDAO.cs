using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.DAO.Interface;

namespace QuanLiKhiThai.DAO
{
    public class LogDAO : ILogDAO
    {
        bool IServiceDAO<Log>.Add(Log log)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Logs.Add(log);
                return db.SaveChanges() > 0;
            }
        }

        bool IServiceDAO<Log>.Delete(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var log = db.Logs.Find(id);
                if (log != null)
                {
                    db.Logs.Remove(log);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }

        IEnumerable<Log> IServiceDAO<Log>.GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs.Include(l => l.User).ToList();
            }
        }

        Log? IServiceDAO<Log>.GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs.Include(l => l.User).FirstOrDefault(l => l.LogId == id);
            }
        }

        IEnumerable<Log> ILogDAO.GetLogInDateRange(DateTime start, DateTime end)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs
                    .Include(l => l.User)
                    .Where(l => l.Timestamp >= start && l.Timestamp <= end)
                    .OrderByDescending(l => l.Timestamp)
                    .ToList();
            }
        }

        IEnumerable<Log> ILogDAO.GetLogsByUser(int userId, int take)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs
                    .Include(l => l.User)
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.Timestamp)
                    .Take(take)
                    .ToList();
            }
        }

        IEnumerable<Log> ILogDAO.GetLogSince(DateTime since)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs
                    .Include(l => l.User)
                    .Where(l => l.Timestamp > since)
                    .OrderByDescending(l => l.Timestamp)
                    .Distinct()
                    .ToList();
            }
        }

        IEnumerable<Log> ILogDAO.SearchLog(string searchQuery, int take)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return new List<Log>();
            }

            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs
                    .Include(l => l.User)
                    .Where(l => l.Action.Contains(searchQuery))
                    .OrderByDescending(l => l.Timestamp)
                    .Take(take)
                    .ToList();
            }
        }

        bool IServiceDAO<Log>.Update(Log entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Logs.Update(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}
