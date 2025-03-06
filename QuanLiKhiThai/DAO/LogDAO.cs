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
    }
}
