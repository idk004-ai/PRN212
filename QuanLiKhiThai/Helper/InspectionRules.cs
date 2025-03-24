using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.Helper
{
    /// <summary>
    /// Configuration for inspection rules
    /// </summary>
    public class InspectionRules
    {
        public int EarlyInspectionAllowanceDays { get; set; } = 30;
        public int VehicleAgeThresholdYears { get; set; } = 10;
        public int NewVehicleInspectionIntervalMonths { get; set; } = 24;
        public int OldVehicleInspectionIntervalMonths { get; set; } = 12;
    }
}
