using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for AssignInspectorWindow.xaml
    /// </summary>
    public partial class AssignInspectorWindow : Window
    {
        private VehicleCheckViewModel _vehicleViewModel;
        private bool _isProcessing = false; // Flag to prevent multiple clicks
        private static readonly object _lockObj = new object(); // ensure thread safety
        private readonly IUserDAO _userDAO;
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly ValidationService _validationService;
        public bool AssignmentSuccess { get; private set; } = false;

        public AssignInspectorWindow(VehicleCheckViewModel vehicleViewModel, IUserDAO userDAO, IVehicleDAO vehicleDAO, IInspectionAppointmentDAO inspectionAppointmentDAO, IInspectionRecordDAO inspectionRecordDAO, ValidationService validationService)
        {
            InitializeComponent();
            this._vehicleViewModel = vehicleViewModel;
            this._userDAO = userDAO;
            this._vehicleDAO = vehicleDAO;
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;
            this._inspectionRecordDAO = inspectionRecordDAO;
            this._validationService = validationService;
            LoadData();
        }

        private void DisableAllAssignButtons()
        {
            foreach (var item in dgInspectors.Items)
            {
                DataGridRow row = (DataGridRow)dgInspectors.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                {
                    var button = DataGridHelper.FindVisualChild<Button>(row);
                    if (button != null)
                    {
                        button.IsEnabled = false;
                    }
                }
            }
        }

        private void EnableAllAssignButtons()
        {
            foreach (var item in dgInspectors.Items)
            {
                DataGridRow row = (DataGridRow)dgInspectors.ItemContainerGenerator.ContainerFromItem(item);
                if (row != null)
                {
                    var button = DataGridHelper.FindVisualChild<Button>(row);
                    if (button != null)
                    {
                        button.IsEnabled = true;
                    }
                }
            }
        }


        private void IsVehicleHaveAnyAssignment()
        {
            Vehicle? vehicle = _vehicleDAO.GetByPlateNumber(_vehicleViewModel.PlateNumber);
            if (vehicle != null)
            {
                var records = _inspectionRecordDAO.GetRecordByVehicle(vehicle.VehicleId);
                bool hasTestRecord = records.Any(r => r.Result == Constants.RESULT_TESTING);
                if (hasTestRecord)
                {
                    MessageBox.Show($"Vehicle {_vehicleViewModel.PlateNumber} has already been assigned to an inspector.",
                        "Already Assigned", MessageBoxButton.OK, MessageBoxImage.Information);
                    DisableAllAssignButtons();
                }
            }
        }

        private void LoadData()
        {
            // TODO: Load inspectors in the station
            List<User> inspectors = _userDAO.GetInspectorsInStation(UserContext.Current.UserId).ToList();
            this.dgInspectors.ItemsSource = inspectors;
            IsVehicleHaveAnyAssignment();
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            lock (_lockObj)
            {
                if (_isProcessing)
                {
                    MessageBox.Show("Assignment in progress. Please wait.",
                        "Processing", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                _isProcessing = true;
            }

            Button? button = sender as Button;

            try
            {
                User? selectedInspector = button?.DataContext as User;

                if (selectedInspector != null)
                {
                    DisableAllAssignButtons();

                    // TODO: Assign inspector to vehicle
                    Vehicle? vehicle = _vehicleDAO.GetByPlateNumber(_vehicleViewModel.PlateNumber);
                    if (vehicle == null)
                    {
                        MessageBox.Show("Vehicle information not found.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        EnableAllAssignButtons();
                        return;
                    }

                    List<InspectionAppointment>? appointments = _inspectionAppointmentDAO.GetByVehicleAndStation(vehicle.VehicleId, UserContext.Current.UserId).ToList();
                    if (appointments == null || appointments.Count == 0)
                    {
                        MessageBox.Show("No inspection appointment found for this vehicle", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        EnableAllAssignButtons();
                        return;
                    }

                    InspectionAppointment appointment = appointments.OrderByDescending(a => a.CreatedAt).First();
                    if (appointment.Status != Constants.STATUS_PENDING)
                    {
                        MessageBox.Show("Data inconsistency: This is an ongoing inspection. Please contact admin to resolve the problem",
                            "Ongoing Inspection", MessageBoxButton.OK, MessageBoxImage.Information);
                        EnableAllAssignButtons();
                        return;
                    }

                    var existingRecord = _inspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId);
                    if (existingRecord != null && existingRecord.Result == Constants.RESULT_TESTING)
                    {
                        MessageBox.Show("Data inconsistency: This is an ongoing inspection. Please contact admin to resolve the problem",
                            "Ongoing Inspection", MessageBoxButton.OK, MessageBoxImage.Information);
                        EnableAllAssignButtons();
                        return;
                    }

                    // TODO5: Create inspection record
                    InspectionRecord newRecord = new InspectionRecord
                    {
                        VehicleId = vehicle.VehicleId,
                        StationId = UserContext.Current.UserId,
                        InspectorId = selectedInspector.UserId,
                        AppointmentId = appointment.AppointmentId,
                        InspectionDate = null, // Not yet inspected
                        Result = Constants.RESULT_TESTING,
                        Co2emission = 0,
                        Hcemission = 0,
                        Comments = "",
                    };

                    // TODO6: Update appointment status to reflect the assignment
                    appointment.Status = Constants.STATUS_ASSIGNED;

                    bool assignSuccess = _inspectionRecordDAO.AssignInspector
                        (
                            newRecord,
                            appointment,
                            selectedInspector,
                            _vehicleViewModel.PlateNumber,
                            UserContext.Current.UserId,
                            UserContext.Current.FullName,
                            this
                        );

                    if (!assignSuccess)
                    {
                        MessageBox.Show("Failed to assign inspector to vehicle", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        EnableAllAssignButtons();
                    }
                    else
                    {
                        AssignmentSuccess = true;
                    }
                }
                else
                {
                    MessageBox.Show("Please select an inspector", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    EnableAllAssignButtons();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                EnableAllAssignButtons();
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
