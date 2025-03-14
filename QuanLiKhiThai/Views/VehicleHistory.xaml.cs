using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.ViewModel;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for VehicleInfo.xaml
    /// </summary>
    public partial class VehicleHistory : Window
    {
        private IVehicleDAO _vehicleDAO;

        public VehicleHistory(IVehicleDAO vehicleDAO)
        {
            InitializeComponent();
            this._vehicleDAO = vehicleDAO;
            LoadVehicleInfo();
        }
        private void LoadVehicleInfo()
        {
            // Load vehicle info
            List<Vehicle> vehicles = _vehicleDAO.GetVehicleByOwnerId(UserContext.Current.UserId).ToList();

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
