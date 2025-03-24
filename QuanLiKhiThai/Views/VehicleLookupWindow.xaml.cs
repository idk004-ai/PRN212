using QuanLiKhiThai.DAO.Interface;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for VehicleLookupWindow.xaml
    /// </summary>
    public partial class VehicleLookupWindow : Window
    {
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly INavigationService _navigationService;
        private Vehicle? _currentVehicle;
        private InspectionRecord? _latestInspection;

        public VehicleLookupWindow(IVehicleDAO vehicleDAO, IInspectionRecordDAO inspectionRecordDAO, INavigationService navigationService)
        {
            InitializeComponent();
            _vehicleDAO = vehicleDAO;
            _inspectionRecordDAO = inspectionRecordDAO;
            _navigationService = navigationService;

            // Initialize the window with empty search
            resultsGrid.Visibility = Visibility.Collapsed;
            txtNoResults.Visibility = Visibility.Visible;
            btnReport.Visibility = Visibility.Collapsed;

            // Set focus on search box
            Loaded += (s, e) => txtPlateNumber.Focus();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string plateNumber = txtPlateNumber.Text.Trim();

            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                MessageBox.Show("Please enter a license plate number to search.", "Empty Search", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPlateNumber.Focus();
                return;
            }

            SearchVehicle(plateNumber);
        }

        private void SearchVehicle(string plateNumber)
        {
            try
            {
                // Update search status
                txtSearchStatus.Text = "Searching...";

                // Get vehicle information
                _currentVehicle = _vehicleDAO.GetByPlateNumber(plateNumber);

                if (_currentVehicle == null)
                {
                    // No vehicle found
                    txtSearchStatus.Text = "No vehicle found with this license plate";
                    resultsGrid.Visibility = Visibility.Collapsed;
                    txtNoResults.Visibility = Visibility.Visible;
                    btnReport.Visibility = Visibility.Collapsed;
                    return;
                }

                // Display vehicle information
                DisplayVehicleInfo();

                // Get the latest inspection record
                List<InspectionRecord> records = _inspectionRecordDAO.GetRecordByVehicle(_currentVehicle.VehicleId);
                _latestInspection = records.OrderByDescending(r => r.InspectionDate).FirstOrDefault();

                // Display inspection information
                DisplayInspectionInfo();

                // Show results and hide no results message
                resultsGrid.Visibility = Visibility.Visible;
                txtNoResults.Visibility = Visibility.Collapsed;

                // Update search status
                txtSearchStatus.Text = "Vehicle found";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while searching: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                txtSearchStatus.Text = "Search failed";
            }
        }

        private void DisplayVehicleInfo()
        {
            if (_currentVehicle == null) return;

            txtDisplayPlateNumber.Text = _currentVehicle.PlateNumber;
            txtBrand.Text = _currentVehicle.Brand;
            txtModel.Text = _currentVehicle.Model;
            txtYear.Text = _currentVehicle.ManufactureYear.ToString();
            txtEngineNumber.Text = _currentVehicle.EngineNumber;

            // Owner information
            if (_currentVehicle.Owner != null)
            {
                txtOwnerName.Text = _currentVehicle.Owner.FullName;
            }
            else
            {
                txtOwnerName.Text = "Unknown";
            }
        }

        private void DisplayInspectionInfo()
        {
            if (_latestInspection == null)
            {
                // No inspection records
                txtInspectionStatus.Text = "No inspection records found";
                inspectionStatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF9C4")); // Yellow background
                txtInspectionStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF6F00")); // Orange text

                // Reset inspection details
                txtInspectionDate.Text = "N/A";
                txtStation.Text = "N/A";
                txtInspector.Text = "N/A";
                txtCO2Level.Text = "N/A";
                txtHCLevel.Text = "N/A";
                txtComments.Text = "N/A";

                // Show report button (vehicle without inspection is a violation)
                btnReport.Visibility = Visibility.Visible;
                return;
            }

            // Set inspection status based on result
            if (_latestInspection.Result == Constants.RESULT_PASSED)
            {
                // Check if inspection is still valid (less than 1 year old)
                bool isValid = _latestInspection.InspectionDate.HasValue &&
                              (DateTime.Now - _latestInspection.InspectionDate.Value).TotalDays <= 365;

                if (isValid)
                {
                    txtInspectionStatus.Text = "VALID - Passed inspection";
                    inspectionStatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8F5E9")); // Green background
                    txtInspectionStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32")); // Green text
                    btnReport.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txtInspectionStatus.Text = "EXPIRED - Needs renewal";
                    inspectionStatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF3E0")); // Orange background
                    txtInspectionStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E65100")); // Orange text
                    btnReport.Visibility = Visibility.Visible;
                }
            }
            else if (_latestInspection.Result == Constants.RESULT_FAILED)
            {
                txtInspectionStatus.Text = "FAILED - Emissions test failed";
                inspectionStatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEBEE")); // Red background
                txtInspectionStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828")); // Red text
                btnReport.Visibility = Visibility.Visible;
            }
            else if (_latestInspection.Result == Constants.RESULT_TESTING)
            {
                txtInspectionStatus.Text = "IN PROGRESS - Testing not completed";
                inspectionStatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3F2FD")); // Blue background
                txtInspectionStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1565C0")); // Blue text
                btnReport.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtInspectionStatus.Text = "UNKNOWN - Status cannot be determined";
                inspectionStatusBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5")); // Gray background
                txtInspectionStatus.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#616161")); // Gray text
                btnReport.Visibility = Visibility.Collapsed;
            }

            // Display inspection details
            txtInspectionDate.Text = _latestInspection.InspectionDate?.ToString("dd/MM/yyyy") ?? "N/A";
            txtStation.Text = _latestInspection.Station?.FullName ?? "N/A";
            txtInspector.Text = _latestInspection.Inspector?.FullName ?? "N/A";
            txtCO2Level.Text = $"{_latestInspection.Co2emission} ppm";
            txtHCLevel.Text = $"{_latestInspection.Hcemission} ppm";
            txtComments.Text = string.IsNullOrEmpty(_latestInspection.Comments) ? "No comments" : _latestInspection.Comments;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            //if (_currentVehicle == null)
            //{
            //    MessageBox.Show("Please search for a vehicle first.", "No Vehicle Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            //    return;
            //}

            //// Navigate to violation record window with current vehicle information
            //try
            //{
            //    _navigationService.NavigateTo<ViolationRecordWindow, int>(_currentVehicle.VehicleId);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Unable to open violation reporting: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
        }
    }
}
