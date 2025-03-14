using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System.Windows;
using System.Windows.Controls;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for ScheduleTestWindow.xaml
    /// </summary>
    public partial class ScheduleTestWindow : Window
    {
        private bool _isProcessing = false;
        private DateTime _lastButtonClickTime = DateTime.MinValue;
        private readonly TimeSpan _minimumTimeBetweenClicks = TimeSpan.FromSeconds(1);
        private readonly IUserDAO _userDAO;
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;

        public ScheduleTestWindow(IUserDAO userDAO, IVehicleDAO vehicleDAO, IInspectionAppointmentDAO inspectionAppointmentDAO)
        {
            InitializeComponent();
            this._userDAO = userDAO;
            this._vehicleDAO = vehicleDAO;
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;
            LoadData();
        }

        private void LoadData()
        {
            int ownerId = UserContext.Current.UserId;
            List<Vehicle> vehicles = _vehicleDAO.GetVehicleByOwnerId(ownerId).ToList();

            // Filter out vehicles with pending appointments
            var vehiclesWithoutPendingAppointments = vehicles.Where(v =>
                !_inspectionAppointmentDAO.HavePendingAppointment(v.VehicleId)).ToList();

            this.cbVehicles.ItemsSource = vehiclesWithoutPendingAppointments;
            this.cbVehicles.DisplayMemberPath = "PlateNumber";
            this.cbVehicles.SelectedValuePath = "VehicleId";

            List<User> stations = _userDAO.GetUserByRole(Constants.Station).ToList();
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
            // Check duration between clicks
            if ((DateTime.Now - _lastButtonClickTime) < _minimumTimeBetweenClicks)
            {
                return; // Skip if the duration is too short
            }

            _lastButtonClickTime = DateTime.Now;

            // Check if the previous operation is still processing
            if (_isProcessing)
            {
                ShowMessage("Please wait, your request is being processed.");
                return;
            }

            _isProcessing = true;
            Button scheduleButton = (Button)sender;
            scheduleButton.IsEnabled = false;

            try
            {
                if (cbVehicles.SelectedItem == null)
                {
                    ShowMessage("Please select a vehicle");
                    ResetButtonState(scheduleButton);
                    return;
                }

                if (cbStations.SelectedItem == null)
                {
                    ShowMessage("Please select a station");
                    ResetButtonState(scheduleButton);
                    return;
                }

                if (!dpScheduleDate.SelectedDate.HasValue || dpScheduleDate.SelectedDate.Value < DateTime.Now)
                {
                    ShowMessage("Please select a valid future date");
                    ResetButtonState(scheduleButton);
                    return;
                }

                Vehicle? selectedVehicle = (Vehicle)cbVehicles.SelectedItem;
                User? selectedStation = _userDAO.GetById((int)cbStations.SelectedValue);

                if (selectedVehicle == null || selectedStation == null)
                {
                    ShowMessage("Invalid vehicle or station");
                    ResetButtonState(scheduleButton);
                    return;
                }

                if (!InspectionAppointmentValidation.ValidatingScheduling(selectedVehicle.VehicleId))
                {
                    ResetButtonState(scheduleButton);
                    return;
                }

                // Create a new inspection appointment
                InspectionAppointment iAppointment = new InspectionAppointment
                {
                    VehicleId = selectedVehicle.VehicleId,
                    StationId = selectedStation.UserId,
                    ScheduledDateTime = dpScheduleDate.SelectedDate.Value,
                    Status = Constants.STATUS_PENDING,
                    CreatedAt = DateTime.Now
                };

                _userDAO.CreateAppointment(iAppointment, UserContext.Current, selectedStation, selectedVehicle, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ResetButtonState(scheduleButton);
            }
        }

        private void ResetButtonState(Button button)
        {
            _isProcessing = false;
            button.IsEnabled = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
