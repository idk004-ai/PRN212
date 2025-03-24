using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System.Collections.ObjectModel;
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
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly ValidationService _validationService;
        private ObservableCollection<InspectionAppointment> _vehiclesUnderInspection;

        public ScheduleTestWindow(
            IUserDAO userDAO,
            IVehicleDAO vehicleDAO,
            IInspectionAppointmentDAO inspectionAppointmentDAO,
            IInspectionRecordDAO inspectionRecordDAO,
            ValidationService validationService)
        {
            InitializeComponent();
            this._userDAO = userDAO;
            this._vehicleDAO = vehicleDAO;
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;
            this._inspectionRecordDAO = inspectionRecordDAO;
            this._validationService = validationService;
            _vehiclesUnderInspection = new ObservableCollection<InspectionAppointment>();

            // Connect the DataGrid to the observable collection
            dgInspectionVehicles.ItemsSource = _vehiclesUnderInspection;

            // Add event handlers
            Loaded += ScheduleTestWindow_Loaded;
            btnRefresh.Click += BtnRefresh_Click;
            cbStations.SelectionChanged += CbStations_SelectionChanged;

            // Load initial data
            LoadData();
        }

        private void ScheduleTestWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Load vehicles under inspection when window loads
            LoadVehiclesUnderInspection();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadVehiclesUnderInspection();
        }

        private void LoadData()
        {
            int ownerId = UserContext.Current.UserId;
            List<Vehicle> vehicles = _vehicleDAO.GetVehicleByOwnerId(ownerId).ToList();

            // Filter out vehicles with pending appointments or in testing status
            var availableVehicles = new List<Vehicle>();

            foreach (var vehicle in vehicles)
            {
                // Check if vehicle has pending appointment
                bool hasPendingAppointment = _inspectionAppointmentDAO.HavePendingAppointment(vehicle.VehicleId);

                // Check if vehicle is currently in testing process
                bool isInTesting = HasOngoingInspection(vehicle.VehicleId);

                if (!hasPendingAppointment && !isInTesting)
                {
                    availableVehicles.Add(vehicle);
                }
            }

            this.cbVehicles.ItemsSource = availableVehicles;
            this.cbVehicles.DisplayMemberPath = "PlateNumber";
            this.cbVehicles.SelectedValuePath = "VehicleId";

            // Load stations
            List<User> stations = _userDAO.GetUserByRole(Constants.Station).ToList();
            var stationItems = stations.Select(s => new
            {
                StationId = s.UserId,
                DisplayName = $"{s.FullName}",
                Station = s // Store the entire User object for later use
            }).ToList();

            this.cbStations.ItemsSource = stationItems;
            this.cbStations.DisplayMemberPath = "DisplayName";
            this.cbStations.SelectedValuePath = "StationId";

            // Hide station details initially
            stationDetailsGrid.Visibility = Visibility.Collapsed;

            // Load vehicles under inspection
            LoadVehiclesUnderInspection();
        }

        /// <summary>
        /// Handle selection change for stations combobox
        /// </summary>
        private void CbStations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbStations.SelectedItem != null)
            {
                // Get the selected station from the ComboBox
                var selectedStation = (dynamic)cbStations.SelectedItem;
                User station = selectedStation.Station;

                if (station != null)
                {
                    // Show the station details section
                    stationDetailsGrid.Visibility = Visibility.Visible;

                    // Update the station details UI
                    txtStationName.Text = station.FullName;
                    txtStationEmail.Text = station.Email;
                    txtStationPhone.Text = station.Phone;
                    txtStationAddress.Text = station.Address;
                }
                else
                {
                    // If no station selected, hide the details
                    stationDetailsGrid.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // No station selected, hide the details
                stationDetailsGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadVehiclesUnderInspection()
        {
            try
            {
                // Get the current user ID (vehicle owner)
                int ownerId = UserContext.Current.UserId;

                // Clear the current list
                _vehiclesUnderInspection.Clear();

                // Get all vehicles owned by the current user
                var userVehicles = _vehicleDAO.GetVehicleByOwnerId(ownerId).ToList();

                if (userVehicles != null && userVehicles.Any())
                {
                    using (var db = new QuanLiKhiThaiContext())
                    {
                        // Load all appointments for all user's vehicles with their related entities
                        foreach (var vehicle in userVehicles)
                        {
                            var appointments = db.InspectionAppointments
                                                    .Include(ia => ia.Vehicle)
                                                    .Include(ia => ia.Station)
                                                    .Where(ia => ia.VehicleId == vehicle.VehicleId)
                                                    .ToList();

                            // Add to our collection
                            foreach (var appointment in appointments)
                            {
                                _vehiclesUnderInspection.Add(appointment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles under inspection: {ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper method to check if a vehicle has an ongoing inspection (Testing status)
        private bool HasOngoingInspection(int vehicleId)
        {
            List<InspectionRecord> records = _inspectionRecordDAO.GetRecordByVehicle(vehicleId);
            return records.Any(r => r.Result == Constants.RESULT_TESTING);
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

                Vehicle? selectedVehicle = (Vehicle)cbVehicles.SelectedItem;
                User? selectedStation = _userDAO.GetById((int)cbStations.SelectedValue);

                if (selectedVehicle == null || selectedStation == null)
                {
                    ShowMessage("Invalid vehicle or station");
                    ResetButtonState(scheduleButton);
                    return;
                }

                if (!_validationService.ValidateScheduling(selectedVehicle.VehicleId, selectedStation.UserId))
                {
                    ResetButtonState(scheduleButton);
                    return;
                }

                // Create a new inspection appointment
                InspectionAppointment iAppointment = new InspectionAppointment
                {
                    VehicleId = selectedVehicle.VehicleId,
                    StationId = selectedStation.UserId,
                    ScheduledDateTime = DateTime.Now.AddDays(1),
                    Status = Constants.STATUS_PENDING,
                    CreatedAt = DateTime.Now
                };

                bool success = _userDAO.CreateAppointment(iAppointment, UserContext.Current, selectedStation, selectedVehicle, windowToClose: null);

                if (success)
                {
                    // Switch to the Vehicles Under Inspection tab
                    LoadVehiclesUnderInspection();

                    // Select the second tab (Vehicles Under Inspection)
                    MainTabControl.SelectedIndex = 1;

                    // Clear the form and reset displayed station details
                    cbVehicles.SelectedIndex = -1;
                    cbStations.SelectedIndex = -1;
                    stationDetailsGrid.Visibility = Visibility.Collapsed;
                }

                ResetButtonState(scheduleButton);
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
