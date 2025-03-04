using QuanLiKhiThai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    class InspectionRecordDAO
    {
        public static bool AddInspectionRecord(InspectionRecord inspectionRecord)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Add(inspectionRecord);
                return db.SaveChanges() > 0;
            }
        }
    }
}
