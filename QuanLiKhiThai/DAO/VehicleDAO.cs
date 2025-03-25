using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.DAO.Interface;

namespace QuanLiKhiThai.DAO
{
    internal class VehicleDAO : IVehicleDAO
    {
        public bool Add(Vehicle entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var vehicle = db.Vehicles.Find(id);
                if (vehicle != null)
                {
                    vehicle.IsDeleted = true;
                    return Update(vehicle);
                }
                return false;
            }
        }

        public IEnumerable<Vehicle> GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Where(v => !v.IsDeleted)
                    .ToList();
            }
        }

        public Vehicle? GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Where(v => !v.IsDeleted)
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .FirstOrDefault(v => v.VehicleId == id);
            } 
        }

        public Vehicle? GetByPlateNumber(string plateNumber)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Where(v => !v.IsDeleted)
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .FirstOrDefault(v => v.PlateNumber == plateNumber);
            }
        }

        public IEnumerable<Vehicle> GetVehicleByOwnerId(int ownerId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Where(v => v.OwnerId == ownerId && !v.IsDeleted)
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .ToList();
            }
        }

        public IEnumerable<Vehicle> GetVehicleWithPendingStatus(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionAppointments
                    .Include(a => a.Vehicle)
                    .Where(a => a.StationId == stationId && a.Status == Constants.STATUS_PENDING && !a.Vehicle.IsDeleted)
                    .Include(a => a.Vehicle.Owner)
                    .Select(a => a.Vehicle)
                    .ToList();
            }
        }

        public bool Update(Vehicle entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Vehicles.Update(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}
