using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.ViewModel
{
    public class VehicleCheckViewModel
    {
        public string PlateNumber { get; set; }
        public string EmailOwner { get; set; }

        public VehicleCheckViewModel()
        {
            
        }

        public VehicleCheckViewModel(string plateNumber, string emailOwner)
        {
            PlateNumber = plateNumber;
            EmailOwner = emailOwner;
        }
    }
}
