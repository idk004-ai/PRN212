using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.DAO.Interface;

namespace QuanLiKhiThai.DAO
{
    internal class VehicleDAO : IVehicleDAO
    {
        bool IServiceDAO<Vehicle>.Add(Vehicle entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        bool IServiceDAO<Vehicle>.Delete(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var vehicle = db.Vehicles.Find(id);
                if (vehicle != null)
                {
                    db.Vehicles.Remove(vehicle);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }

        IEnumerable<Vehicle> IServiceDAO<Vehicle>.GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles.ToList();
            }
        }

        Vehicle? IServiceDAO<Vehicle>.GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles.Find(id);
            } 
        }

        Vehicle? IVehicleDAO.GetByPlateNumber(string plateNumber)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .FirstOrDefault(v => v.PlateNumber == plateNumber);
            }
        }

        IEnumerable<Vehicle> IVehicleDAO.GetVehicleByOwnerId(int ownerId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Where(v => v.OwnerId == ownerId)
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .ToList();
            }
        }

        Vehicle? IVehicleDAO.GetVehicleByStationId(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(a => a.StationId == stationId)
                    .Include(a => a.Vehicle)
                    .Include(a => a.Vehicle.Owner)
                    .Select(a => a.Vehicle)
                    .FirstOrDefault();
            }
        }

        IEnumerable<Vehicle> IVehicleDAO.GetVehicleWithPendingStatus(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Where(a => a.StationId == stationId && a.Status == Constants.STATUS_PENDING)
                    .Include(a => a.Vehicle)
                    .Include(a => a.Vehicle.Owner)
                    .Select(a => a.Vehicle)
                    .ToList();
            }
        }

        bool IServiceDAO<Vehicle>.Update(Vehicle entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Vehicles.Update(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}
