using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Models;
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
    /// Interaction logic for ScheduleTestWindow.xaml
    /// </summary>
    public partial class ScheduleTestWindow : Window
    {
        public ScheduleTestWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            int ownerId = UserContext.Current.UserId;
            List<Vehicle> vehicles = VehicleDAO.GetVehicleByOwner(ownerId);
            this.cbVehicles.ItemsSource = vehicles;
            this.cbVehicles.DisplayMemberPath = "PlateNumber";
            this.cbVehicles.SelectedValuePath = "VehicleId";

            List<User> stations = UserDAO.GetUserByRole(Constants.Station);
            var stationItems = stations.Select(s => new
            {
                StationId = s.UserId,
                DisplayName = $"{s.FullName} - Address: {s.Address}"
            }).ToList();
            this.cbStations.ItemsSource = stationItems;
            this.cbStations.DisplayMemberPath = "DisplayName";
            this.cbStations.SelectedValuePath = "StationId";

            this.dpScheduleDate.SelectedDate = DateTime.Now.AddDays(1);
        }

        private void ShowMessage(string message)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            if (cbVehicles.SelectedItem == null)
            {
                ShowMessage("Please select a vehicle");
                return;
            }

            if (cbStations.SelectedItem == null)
            {
                ShowMessage("Please select a station");
                return;
            }

            if (!dpScheduleDate.SelectedDate.HasValue || dpScheduleDate.SelectedDate.Value < DateTime.Now)
            {
                ShowMessage("Please select a valid future date");
                return;
            }

            Vehicle? selectedVehicle = (Vehicle)cbVehicles.SelectedItem;
            User? selectedStation = UserDAO.GetUserById((int)cbStations.SelectedValue);

            if (selectedVehicle == null || selectedStation == null)
            {
                ShowMessage("Invalid vehicle or station");
                return;
            }

            InspectionAppointment iAppointment = new InspectionAppointment
            {
                VehicleId = selectedVehicle.VehicleId,
                StationId = selectedStation.UserId,
                ScheduledDateTime = dpScheduleDate.SelectedDate.Value,
                Status = Constants.STATUS_PENDING,
                CreatedAt = DateTime.Now
            };

            bool addSuccess = InspectionAppointmentDAO.AddInspectionAppointment(iAppointment);
            if (addSuccess)
            {
                MessageBox.Show("Schedule test successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Schedule test failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
