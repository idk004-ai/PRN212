using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for NeededCheckVehicleList.xaml
    /// </summary>
    public partial class NeededCheckVehicleList : Window
    {
        private readonly INavigationService _navigationService;
        private readonly IVehicleDAO _vehicleDAO;
        private List<VehicleCheckViewModel> _vehicleCheckList;

        public NeededCheckVehicleList(INavigationService navigationService, IVehicleDAO vehicleDAO)
        {
            InitializeComponent();
            this._vehicleCheckList = new List<VehicleCheckViewModel>();
            this._navigationService = navigationService;
            this._vehicleDAO = vehicleDAO;
            LoadData();
        }

        private void LoadData()
        {
            // Get list of vehicles with pending appointments
            List<Vehicle> vehicles = _vehicleDAO.GetVehicleWithPendingStatus(UserContext.Current.UserId).ToList();

            // Create ViewModels List - initialize the class field, not a local variable
            _vehicleCheckList = new List<VehicleCheckViewModel>();
            foreach (Vehicle vehicle in vehicles)
            {
                var owner = vehicle.Owner;
                _vehicleCheckList.Add(new VehicleCheckViewModel
                {
                    PlateNumber = vehicle.PlateNumber,
                    EmailOwner = owner.Email
                });
            }

            // Bind data to DataGrid
            this.dataGridVehicle.ItemsSource = _vehicleCheckList;
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicleViewModel = button?.CommandParameter as VehicleCheckViewModel;

            if (vehicleViewModel != null)
            {
                _navigationService.NavigateTo<VehicleDetailWindow, VehicleCheckViewModel>(vehicleViewModel);
            }
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;
            try
            {
                var vehicleViewModel = button?.CommandParameter as VehicleCheckViewModel;
                if (vehicleViewModel != null)
                {
                    // Store the reference to the vehicle's plate number before opening the window
                    string plateNumber = vehicleViewModel.PlateNumber;

                    // Navigate to assignment window
                    var assignWindow = new AssignInspectorWindow(vehicleViewModel,
                                                               App.GetService<IUserDAO>(),
                                                               _vehicleDAO,
                                                               App.GetService<IInspectionAppointmentDAO>(),
                                                               App.GetService<IInspectionRecordDAO>());

                    assignWindow.Owner = this;
                    assignWindow.ShowDialog();

                    // Check if assignment was successful
                    if (assignWindow.AssignmentSuccess)
                    {
                        // _vehicleCheckList is now properly initialized, so this will work correctly
                        var vehicleToRemove = _vehicleCheckList.FirstOrDefault(v => v.PlateNumber == plateNumber);
                        if (vehicleToRemove != null)
                        {
                            _vehicleCheckList.Remove(vehicleToRemove);
                            // Refresh the DataGrid
                            dataGridVehicle.ItemsSource = null;
                            dataGridVehicle.ItemsSource = _vehicleCheckList;
                        }
                    }
                }
            }
            finally
            {
                button.IsEnabled = true;
            }
        }
    }
}
