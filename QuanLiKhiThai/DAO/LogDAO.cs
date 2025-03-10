using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    class LogDAO
    {
        internal static bool AddLog(Log newLog)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Logs.Add(newLog);
                return db.SaveChanges() > 0;
            }
        }

        public static List<Log> GetLogsSince(DateTime since)
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

        public static List<Log> GetLogsByUser(int userId, int take = 100)
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

        public static List<Log> SearchLogs(string searchTerm, int take = 100)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new List<Log>();
            }

            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Logs
                    .Include(l => l.User)
                    .Where(l => l.Action.Contains(searchTerm))
                    .OrderByDescending(l => l.Timestamp)
                    .Take(take)
                    .ToList();
            }
        }

        public static List<Log> GetLogsInDateRange(DateTime start, DateTime end)
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
    }
}
