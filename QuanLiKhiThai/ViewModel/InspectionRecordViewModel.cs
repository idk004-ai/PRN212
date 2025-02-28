using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.ViewModel
{
    internal class InspectionRecordViewModel
    {
        public string PlateNumber { get; set; }
        public string EngineNumber { get; set; }
        public string Result { get; set; }
        public DateTime? InspectionDate { get; set; }

        public InspectionRecordViewModel() { }

        public InspectionRecordViewModel(string PlateNumber, string EngineNumber, string Result, DateTime date)
        {
            this.PlateNumber = PlateNumber;
            this.EngineNumber = EngineNumber;
            this.Result = Result;
            this.InspectionDate = date;
        }
    }
}
