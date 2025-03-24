using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.ViewModel
{
    public class AssignedVehicleViewModel
    {
        public int RecordId { get; set; }
        public string PlateNumber { get; set; }
        public string InspectorName { get; set; }
        public DateTime? AssignedDate { get; set; }
        public string Result { get; set; }
    }
}
