using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Models;
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
    /// Interaction logic for NeededCheckVehicleList.xaml
    /// </summary>
    public partial class NeededCheckVehicleList : Window
    {
        public NeededCheckVehicleList()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            // Load data from database
            List<Vehicle> vehicles = VehicleDAO.GetVehicleNeedingInspection(UserContext.Current.UserId);

            // Create ViewModels List
            List<VehicleCheckViewModel> vehicleCheckList = new List<VehicleCheckViewModel>();
            foreach (Vehicle vehicle in vehicles)
            {
                var lastInspection = vehicle.InspectionRecords.OrderByDescending(ir => ir.InspectionDate).FirstOrDefault();
                var owner = vehicle.Owner;
                vehicleCheckList.Add(new VehicleCheckViewModel
                {
                    PlateNumber = vehicle.PlateNumber,
                    EmailOwner = owner.Email,
                    LastInspectionDate = lastInspection?.InspectionDate
                });
            }

            // Bind data to DataGrid
            this.dataGridVehicle.ItemsSource = vehicleCheckList;
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicleViewModel = button?.CommandParameter as VehicleCheckViewModel;

            if (vehicleViewModel != null)
            {
                VehicleDetailWindow vehicleDetailWindow = new VehicleDetailWindow(vehicleViewModel);
                vehicleDetailWindow.Show();
            }
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicleViewModel = button?.CommandParameter as VehicleCheckViewModel;
            if (vehicleViewModel != null)
            {
                AssignInspectorWindow assignInspectorWindow = new AssignInspectorWindow(vehicleViewModel);
            }
        }
    }
}
