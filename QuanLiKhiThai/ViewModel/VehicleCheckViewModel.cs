using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.ViewModel
{
    internal class VehicleCheckViewModel
    {
        public string PlateNumber { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int ManufactureYear { get; set; }
        public string EngineNumber { get; set; }
        public DateTime? LastInspectionDate { get; set; }
        public string LastInspectionResult { get; set; }

        public VehicleCheckViewModel()
        {
            
        }

        public VehicleCheckViewModel(string plateNumber, string brand, string model, int manufactureYear, string engineNumber, DateTime? lastInspectionDate, string lastInspectionResult)
        {
            PlateNumber = plateNumber;
            Brand = brand;
            Model = model;
            ManufactureYear = manufactureYear;
            EngineNumber = engineNumber;
            LastInspectionDate = lastInspectionDate;
            LastInspectionResult = lastInspectionResult;
        }
    }
}
