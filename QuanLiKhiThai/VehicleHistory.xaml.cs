using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for VehicleInfo.xaml
    /// </summary>
    public partial class VehicleHistory : Window
    {

        public VehicleHistory()
        {
            InitializeComponent();
            LoadVehicleInfo();
        }
        private void LoadVehicleInfo()
        {
            // Load vehicle info
            List<Vehicle> vehicles = VehicleDAO.GetVehicleByOwner(UserContext.Current.UserId);

            // Bind data to DataGrid
            List<InspectionRecordViewModel> records = new List<InspectionRecordViewModel>();
            foreach (Vehicle vehicle in vehicles)
            {
                foreach (InspectionRecord record in vehicle.InspectionRecords)
                {
                    records.Add(new InspectionRecordViewModel
                    {
                        PlateNumber = vehicle.PlateNumber,
                        EngineNumber = vehicle.EngineNumber,
                        Result = record.Result,
                        InspectionDate = record.InspectionDate,
                    });
                }
            }

            this.dataGridVehicle.ItemsSource = records;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Xử lý logic sửa thông tin phương tiện
        }

        private void ScheduleInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Xử lý logic lên lịch kiểm định
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
