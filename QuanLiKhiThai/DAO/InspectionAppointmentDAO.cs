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
        bool IServiceDAO<InspectionAppointment>.Add(InspectionAppointment entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionAppointments.Add(entity);
                return db.SaveChanges() > 0;
            }
        }


        bool IServiceDAO<InspectionAppointment>.Delete(int id)
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

        IEnumerable<InspectionAppointment> IServiceDAO<InspectionAppointment>.GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments.ToList();
            }
        }

        InspectionAppointment? IServiceDAO<InspectionAppointment>.GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments.Find(id);
            }
        }

        IEnumerable<InspectionAppointment> IInspectionAppointmentDAO.GetByVehicleAndStation(int vehicleId, int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(a => a.VehicleId == vehicleId && a.StationId == stationId)
                    .ToList();
            }
        }

        InspectionAppointment IInspectionAppointmentDAO.GetLast(List<InspectionAppointment> appointments)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return appointments.OrderByDescending(a => a.CreatedAt).FirstOrDefault();
            }
        }

        bool IInspectionAppointmentDAO.HavePendingAppointment(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments.Any(a => a.VehicleId == vehicleId && a.Status == "Pending");
            }
        }

        bool IServiceDAO<InspectionAppointment>.Update(InspectionAppointment entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Update(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}
