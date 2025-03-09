using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
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
    /// Interaction logic for AssignInspectorWindow.xaml
    /// </summary>
    public partial class AssignInspectorWindow : Window
    {
        private VehicleCheckViewModel _vehicleViewModel;
        private bool _isProcessing = false; // Flag to prevent multiple clicks
        private static readonly object _lockObj = new object(); // ensure thread safety

        public AssignInspectorWindow(VehicleCheckViewModel vehicleViewModel)
        {
            InitializeComponent();
            this._vehicleViewModel = vehicleViewModel;
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
            Vehicle? vehicle = VehicleDAO.GetVehicleByPlateNumber(_vehicleViewModel.PlateNumber);
            if (vehicle != null)
            {
                var existingRecords = InspectionRecordDAO.GetCurrentRecordByVehicleId(vehicle.VehicleId);
                if (existingRecords != null)
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
            List<User> inspectors = UserDAO.GetInspectorInStation(UserContext.Current.UserId);
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

            Button button = sender as Button;

            try
            {
                User? selectedInspector = button?.DataContext as User;

                if (selectedInspector != null)
                {
                    DisableAllAssignButtons();

                    // TODO: Assign inspector to vehicle
                    Vehicle? vehicle = VehicleDAO.GetVehicleByPlateNumber(_vehicleViewModel.PlateNumber);
                    if (vehicle == null)
                    {
                        MessageBox.Show("Vehicle information not found.",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        EnableAllAssignButtons();
                        return;
                    }

                    List<InspectionAppointment>? appointments = InspectionAppointmentDAO.GetAppointmentByVehicleAndStation(vehicle.VehicleId, UserContext.Current.UserId);
                    if (appointments == null || appointments.Count == 0)
                    {
                        MessageBox.Show("No inspection appointment found for this vehicle", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        EnableAllAssignButtons();
                        return;
                    }

                    // TODO3: Validate assignment
                    if (!InspectionAppointmentValidation.ValidateAssignment(appointments, vehicle.VehicleId))
                    {
                        EnableAllAssignButtons();
                        return;
                    }

                    InspectionAppointment appointment = InspectionAppointmentDAO.GetLastAppointment(appointments);

                    // TODO4: Check whether any record for this appointment
                    if (InspectionRecordDAO.GetRecordByAppointment(appointment.AppointmentId) != null)
                    {
                        MessageBox.Show("An inspector has already been assigned to this appointment.",
                            "Already Assigned", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
                        return;
                    }

                    // TODO5: Create inspection record
                    InspectionRecord newRecord = new InspectionRecord
                    {
                        VehicleId = vehicle.VehicleId,
                        StationId = UserContext.Current.UserId,
                        InspectorId = selectedInspector.UserId,
                        AppointmentId = appointment.AppointmentId,
                        InspectionDate = appointment.ScheduledDateTime,
                        Result = Constants.RESULT_TESTING,
                        Co2emission = 0,
                        Hcemission = 0,
                        Comments = "",
                    };

                    // TODO6: Update appointment status to reflect the assignment
                    appointment.Status = Constants.STATUS_ASSIGNED;

                    InspectionAppointmentDAO stationOperations = new InspectionAppointmentDAO();
                    bool assignSuccess = stationOperations.AssignInspector
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
