﻿using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Models;
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
    }
}
