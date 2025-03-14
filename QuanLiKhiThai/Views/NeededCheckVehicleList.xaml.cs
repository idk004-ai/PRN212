using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.ViewModel;
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

        public NeededCheckVehicleList(INavigationService navigationService, IVehicleDAO vehicleDAO)
        {
            InitializeComponent();
            this._navigationService = navigationService;
            this._vehicleDAO = vehicleDAO;
            LoadData();
        }

        private void LoadData()
        {
            // TODO1: Get list of vehicles with pending appointments
            List<Vehicle> vehicles = _vehicleDAO.GetVehicleWithPendingStatus(UserContext.Current.UserId).ToList();

            // Create ViewModels List
            List<VehicleCheckViewModel> vehicleCheckList = new List<VehicleCheckViewModel>();
            foreach (Vehicle vehicle in vehicles)
            {
                var owner = vehicle.Owner;
                vehicleCheckList.Add(new VehicleCheckViewModel
                {
                    PlateNumber = vehicle.PlateNumber,
                    EmailOwner = owner.Email
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
                    _navigationService.NavigateTo<AssignInspectorWindow, VehicleCheckViewModel>(vehicleViewModel);
                    LoadData();
                }

            }
            finally
            {
                button.IsEnabled = true;
            }
        }
    }
}
