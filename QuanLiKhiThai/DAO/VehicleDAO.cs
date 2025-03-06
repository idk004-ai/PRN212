using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    internal class VehicleDAO
    {
        public static List<Vehicle> GetVehicleByOwner(int ownerId)
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

        internal static bool AddVehicle(Vehicle vehicle)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Vehicles.Add(vehicle);
                return db.SaveChanges() > 0;
            }
        }

        public static List<Vehicle> GetVehicleWithPendingAppointments(int stationId)
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

        public static Vehicle? GetVehicleByPlateNumber(string plateNumber)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .FirstOrDefault(v => v.PlateNumber == plateNumber);
            }
        }

        internal static Vehicle? GetVehicleById(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Vehicles
                    .Include(v => v.Owner)
                    .Include(v => v.InspectionRecords)
                    .FirstOrDefault(v => v.VehicleId == vehicleId);
            }
        }
    }
}
