using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    class InspectionAppointmentDAO : IInspectionAppointmentDAO
    {
        public bool Add(InspectionAppointment entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionAppointments.Add(entity);
                return db.SaveChanges() > 0;
            }
        }


        public bool Delete(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var entity = db.InspectionAppointments.Find(id);
                if (entity != null)
                {
                    db.InspectionAppointments.Remove(entity);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }

        public IEnumerable<InspectionAppointment> GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments.ToList();
            }
        }

        public InspectionAppointment? GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments.Find(id);
            }
        }

        public IEnumerable<InspectionAppointment> GetByVehicleAndStation(int vehicleId, int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(a => a.VehicleId == vehicleId && a.StationId == stationId)
                    .ToList();
            }
        }

        public bool HavePendingAppointment(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments.Any(a => a.VehicleId == vehicleId && a.Status == "Pending");
            }
        }

        public bool Update(InspectionAppointment entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Update(entity);
                return db.SaveChanges() > 0;
            }
        }

        public bool CancelAppointment(
            InspectionAppointment appointment, 
            UserContext station, 
            string plateNumber,
            string fullName,
            string reason,
            Window? windowToClose = null)
        {

            var operations = new Dictionary<string, Func<bool>>
            {
                {
                    "Cancel appointment",
                    () =>
                    {
                        appointment.Status = Constants.STATUS_CANCELLED;
                        return Update(appointment);
                    }
                }
            };

            Log logEntry = new Log
            {
                UserId = station.UserId,
                Action = $"Cancelled appointment for vehicle {plateNumber}",
                Timestamp = DateTime.Now
            };


            string successMessage = $"Appointment for vehicle {plateNumber} has been cancelled at {station}.";
            string errorMessage = "Failed to cancel inspection";

            bool result = TransactionHelper.ExecuteTransaction(
                operations,
                logEntry,
                notification: null,
                successMessage,
                errorMessage
            );

            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }

            return result;
        }
    }
}
