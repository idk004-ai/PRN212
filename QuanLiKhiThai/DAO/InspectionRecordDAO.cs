using Microsoft.EntityFrameworkCore;
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

        internal static InspectionRecord? GetLastRecordByVehicleId(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.VehicleId == vehicleId)
                    .OrderByDescending(i => i.InspectionDate)
                    .FirstOrDefault();
            }
        }

        public static InspectionRecord? GetCurrentRecordByVehicleId(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Include(r => r.Inspector)
                    .Include(r => r.Appointment)
                    .Where(r => r.VehicleId == vehicleId && r.Result == Constants.RESULT_TESTING)
                    .OrderByDescending(r => r.InspectionDate)
                    .FirstOrDefault();
            }
        }


        internal static InspectionRecord? GetRecordByAppointment(int appointmentId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.AppointmentId == appointmentId)
                    .FirstOrDefault();
            }
        }
    }
}
