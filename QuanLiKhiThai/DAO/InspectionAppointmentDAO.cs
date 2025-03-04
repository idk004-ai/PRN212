using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    class InspectionAppointmentDAO
    {
        public static bool AddInspectionAppointment(InspectionAppointment inspectionAppointment)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionAppointments.Add(inspectionAppointment);
                return db.SaveChanges() > 0;
            }
        }
    }
}
