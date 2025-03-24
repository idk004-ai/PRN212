using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Views;
using System.Windows;
using System.Windows.Media;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for VehicleInspectionDetailsWindow.xaml
    /// </summary>
    public partial class VehicleInspectionDetailsWindow : Window
    {
        private InspectionRecord _record;
        private Random _random = new Random(); // For simulating measurements
        private bool co2Pass = false;
        private bool hcPass = false;
        private bool _isViewMode = false;

        private readonly IInspectionRecordDAO _inspectionRecordDAO;

        public VehicleInspectionDetailsWindow(InspectionRecord record, bool isViewMode, IInspectionRecordDAO inspectionRecordDAO)
        {
            InitializeComponent();
            _record = record;
            _isViewMode = isViewMode;
            this._inspectionRecordDAO = inspectionRecordDAO;
            LoadVehicleData();
            LoadExistingResults();

            ConfigureUIBasedOnMode();
        }

        private void ConfigureUIBasedOnMode()
        {
            if (_isViewMode || _record.Result == Constants.RESULT_PASSED || _record.Result == Constants.RESULT_FAILED || _record.Result == Constants.RESULT_CANCELLED)
            {
                btnCancelInspection.Visibility = Visibility.Collapsed;
                btnSaveResults.Visibility = Visibility.Collapsed;

                DisableControls();
            }
        }

        private void LoadVehicleData()
        {
            // Set window title
            this.Title = $"Inspection Details - {_record.Vehicle.PlateNumber}";

            // Load vehicle information
            txtHeaderVehicleInfo.Text = $"Inspection Details - {_record.Vehicle.PlateNumber}";
            txtPlateNumber.Text = _record.Vehicle.PlateNumber;
            txtBrand.Text = _record.Vehicle.Brand;
            txtModel.Text = _record.Vehicle.Model;
            txtYear.Text = _record.Vehicle.ManufactureYear.ToString();
            txtEngineNumber.Text = _record.Vehicle.EngineNumber;

            // Load owner information
            txtOwnerName.Text = _record.Vehicle.Owner.FullName;
            txtOwnerEmail.Text = _record.Vehicle.Owner.Email;
            txtOwnerPhone.Text = _record.Vehicle.Owner.Phone;

            // Load appointment and station information
            txtScheduledDate.Text = _record.Appointment.ScheduledDateTime.ToString("dd/MM/yyyy HH:mm");
            txtStationName.Text = _record.Station.FullName;
        }

        private void LoadExistingResults()
        {
            // If there are existing measurement values, load them
            if (_record.Co2emission > 0)
            {
                txtCO2Emission.Text = _record.Co2emission.ToString();
                UpdateCO2VisualFeedback(_record.Co2emission);
            }

            if (_record.Hcemission > 0)
            {
                txtHCEmission.Text = _record.Hcemission.ToString();
                UpdateHCVisualFeedback(_record.Hcemission);
            }

            if (!string.IsNullOrEmpty(_record.Comments))
            {
                txtComments.Text = _record.Comments;
            }
        }

        private void DisableControls()
        {
            // Disable all input controls if inspection is completed
            txtCO2Emission.IsEnabled = false;
            txtHCEmission.IsEnabled = false;
            txtComments.IsEnabled = false;
            btnMeasureCO2.IsEnabled = false;
            btnMeasureHC.IsEnabled = false;
        }

        private void UpdateCO2VisualFeedback(decimal co2Value)
        {
            // Visual feedback based on value
            if (co2Value <= 120)
            {
                txtCO2Emission.Background = Brushes.LightGreen;
                co2Pass = true;
            }
            else
            {
                txtCO2Emission.Background = Brushes.LightPink;
            }
        }

        private void UpdateHCVisualFeedback(decimal hcValue)
        {
            // Visual feedback based on value
            if (hcValue <= 100)
            {
                txtHCEmission.Background = Brushes.LightGreen;
                hcPass = true;
            }
            else
            {
                txtHCEmission.Background = Brushes.LightPink;
            }
        }

        private void btnMeasureCO2_Click(object sender, RoutedEventArgs e)
        {
            // Simulate CO2 emission measurement
            // In a real application, this would interface with measurement equipment
            decimal co2Value = Math.Round((decimal)(_random.NextDouble() * 150), 2);
            txtCO2Emission.Text = co2Value.ToString();

            // Update visual feedback
            UpdateCO2VisualFeedback(co2Value);
        }

        private void btnMeasureHC_Click(object sender, RoutedEventArgs e)
        {
            // Simulate HC emission measurement
            decimal hcValue = Math.Round((decimal)(_random.NextDouble() * 150), 2);
            txtHCEmission.Text = hcValue.ToString();

            // Update visual feedback
            UpdateHCVisualFeedback(hcValue);
        }

        private void SaveResults_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(txtCO2Emission.Text) || string.IsNullOrEmpty(txtHCEmission.Text))
                {
                    MessageBox.Show("Please enter or measure both CO₂ and HC emission values.",
                        "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Parse values
                if (!decimal.TryParse(txtCO2Emission.Text, out decimal co2Value) ||
                    !decimal.TryParse(txtHCEmission.Text, out decimal hcValue))
                {
                    MessageBox.Show("Invalid emission values. Please enter numeric values only.",
                        "Invalid Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Determine result
                string result = Constants.RESULT_TESTING;
                if (co2Pass && hcPass)
                {
                    result = Constants.RESULT_PASSED;
                }
                else 
                {
                    result = Constants.RESULT_FAILED;
                }

                // Update record
                _record.Co2emission = co2Value;
                _record.Hcemission = hcValue;
                _record.Comments = txtComments.Text;
                _record.Result = result;
                _record.InspectionDate = DateTime.Now;

                // If result is final (PASS or FAIL), update appointment status
                bool needsAppointmentUpdate = false;
                if ((result == Constants.RESULT_PASSED || result == Constants.RESULT_FAILED) &&
                    _record.Appointment.Status != Constants.STATUS_COMPLETED)
                {
                    _record.Appointment.Status = Constants.STATUS_COMPLETED;
                    needsAppointmentUpdate = true;
                }

                // Save to database using transaction
                _inspectionRecordDAO.RecordInspectionResult(
                    _record, 
                    UserContext.Current,
                    _record.Vehicle.PlateNumber, 
                    _record.Station.FullName, 
                    this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving results: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelInspection_Click(object sender, RoutedEventArgs e)
        {
            // Ask for confirmation
            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to cancel the inspection for vehicle {_record.Vehicle.PlateNumber}?\n\n" +
                "This action cannot be undone.",
                "Confirm Cancellation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var reasonWindow = new CancellationReasonWindow(_record.Vehicle.PlateNumber);
                    reasonWindow.Owner = this;

                    bool? dialogResult = reasonWindow.ShowDialog();

                    if (dialogResult == true)
                    {
                        string cancellationReason = reasonWindow.CancellationReason;

                        _inspectionRecordDAO.CancelInspection(
                            _record,
                            UserContext.Current,
                            _record.Vehicle.PlateNumber,
                            _record.Station.FullName,
                            cancellationReason,
                            this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cancelling inspection: {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
