using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        internal static InspectionAppointment? GetAppointmentById(int appointmentId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(i => i.AppointmentId == appointmentId)
                    .FirstOrDefault();
            }
        }

        internal static List<InspectionAppointment>? GetAppointmentByVehicleAndStation(int vehicleId, int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(i => i.VehicleId == vehicleId && i.StationId == stationId)
                    .ToList();
            }
        }

        internal static bool UpdateAppointment(InspectionAppointment appointment)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionAppointments.Update(appointment);
                return db.SaveChanges() > 0;
            }
        }

        public static InspectionAppointment GetLastAppointment(List<InspectionAppointment> appointments)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return appointments.OrderByDescending(a => a.CreatedAt).First();
            }
        }

        public static bool HasPendingAppointment(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(i => i.VehicleId == vehicleId && i.Status == Constants.STATUS_PENDING)
                    .Count() > 0;
            }
        }


        public bool AssignInspector(
        InspectionRecord record,
        InspectionAppointment appointment,
        User inspector,
        string vehiclePlateNumber,
        int stationId,
        string stationFullName,
        Window windowToClose = null
        )
        {
            var operations = new Dictionary<string, Func<bool>>
        {
            { "add record", () => InspectionRecordDAO.AddInspectionRecord(record) },
            { "update appointment", () => UpdateAppointment(appointment) }
        };

            Log logEntry = new Log
            {
                UserId = stationId,
                Action = $"Assigned inspector {inspector.FullName} to vehicle {vehiclePlateNumber}",
                Timestamp = DateTime.Now
            };

            Notification notification = new Notification
            {
                UserId = stationId,
                Message = $"You have been assigned to inspect vehicle {vehiclePlateNumber} at {stationFullName}",
                SentDate = DateTime.Now,
                IsRead = false
            };

            string successMessage = $"Inspector {inspector.FullName} has been assigned to vehicle {vehiclePlateNumber}.";
            string errorMessage = "Failed to assign inspector to vehicle";

            bool result = TransactionHelper.ExecuteTransaction
                (operations,
                logEntry,
                notification,
                successMessage,
                errorMessage);

            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }

            return result;
        }
    }
}
