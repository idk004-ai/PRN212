using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for InspectorManagementWindow.xaml
    /// </summary>
    public partial class InspectorManagementWindow : Window
    {
        private readonly IUserDAO _userDAO;
        private readonly IStationInspectorDAO _stationInspectorDAO;
        private readonly int _currentStationId;
        private readonly INavigationService _navigationService;
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;

        // Collection to hold inspector data for the DataGrid
        private ObservableCollection<StationInspectorViewModel> _inspectors;

        // CollectionView for filtering
        private ICollectionView _inspectorsView;

        public InspectorManagementWindow(INavigationService navigationService, IUserDAO userDAO, IStationInspectorDAO stationInspectorDAO, int stationId, IInspectionRecordDAO inspectionRecordDAO, IInspectionAppointmentDAO inspectionAppointmentDAO)
        {
            InitializeComponent();

            // Store dependencies
            this._navigationService = navigationService;
            this._userDAO = userDAO;
            this._stationInspectorDAO = stationInspectorDAO;
            this._currentStationId = stationId;
            this._inspectionRecordDAO = inspectionRecordDAO;
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;

            // Initialize collections
            _inspectors = new ObservableCollection<StationInspectorViewModel>();

            // Set DataContext
            dgInspectors.ItemsSource = _inspectors;
            _inspectorsView = CollectionViewSource.GetDefaultView(_inspectors);

            // Load initial data
            LoadInspectors();

            // Set up event handlers
            txtSearch.TextChanged += TxtSearch_TextChanged;
            cmbStatus.SelectionChanged += CmbStatus_SelectionChanged;
            _inspectionRecordDAO = inspectionRecordDAO;
            _inspectionAppointmentDAO = inspectionAppointmentDAO;
        }

        private void LoadInspectors()
        {
            try
            {
                // Clear existing data
                _inspectors.Clear();

                // Get all station inspectors assigned to this station
                var stationInspectors = _stationInspectorDAO.GetByStationId(_currentStationId);

                // Load each inspector's details
                foreach (var si in stationInspectors)
                {
                    // Get user details
                    var inspector = _userDAO.GetById(si.InspectorId);

                    if (inspector != null)
                    {
                        _inspectors.Add(new StationInspectorViewModel
                        {
                            UserId = inspector.UserId,
                            FullName = inspector.FullName,
                            Email = inspector.Email,
                            Phone = inspector.Phone,
                            Address = inspector.Address,
                            IsActive = si.IsActive ?? false,
                            AssignedDate = si.AssignedDate,
                            Notes = si.Notes
                        });
                    }
                }

                // Update total count
                UpdateTotalCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading inspectors: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotalCount()
        {
            txtTotalInspectors.Text = $"Total Inspectors: {_inspectors.Count}";
        }

        private void ApplyFilters()
        {
            _inspectorsView.Filter = item =>
            {
                var inspector = item as StationInspectorViewModel;
                if (inspector == null) return false;

                // Apply search filter
                bool matchesSearch = string.IsNullOrWhiteSpace(txtSearch.Text) ||
                                    inspector.FullName.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase) ||
                                    inspector.Email.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase);

                // Apply status filter
                bool matchesStatus = true;
                if (cmbStatus.SelectedIndex == 1) // Active
                    matchesStatus = inspector.IsActive;
                else if (cmbStatus.SelectedIndex == 2) // Inactive
                    matchesStatus = !inspector.IsActive;

                return matchesSearch && matchesStatus;
            };
        }

        #region Event Handlers

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is StationInspectorViewModel inspector)
            {
                var parameters = (StationId: _currentStationId, InspectorId: inspector.UserId);
                _navigationService.NavigateTo<AddInspectorWindow, (int StationId, int InspectorId)>(parameters);
            }
        }

        private void dgInspectors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Enable/disable buttons based on selection
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is StationInspectorViewModel inspector)
            {
                // Display inspector details
                MessageBox.Show($"Inspector Details:\n\nID: {inspector.UserId}\nName: {inspector.FullName}\n" +
                                $"Email: {inspector.Email}\nPhone: {inspector.Phone}\n" +
                                $"Address: {inspector.Address}\n" +
                                $"Assigned Date: {inspector.AssignedDate:dd/MM/yyyy}\n" +
                                $"Status: {inspector.Status}\n" +
                                $"Notes: {inspector.Notes}",
                                "Inspector Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnActivate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is StationInspectorViewModel inspector)
            {
                try
                {
                    // Update status in database
                    var stationInspector = _stationInspectorDAO.GetByStationAndInspectorId(_currentStationId, inspector.UserId);
                    if (stationInspector != null)
                    {
                        stationInspector.IsActive = true;
                        _stationInspectorDAO.Update(stationInspector);

                        // Update UI
                        inspector.IsActive = true;

                        // Refresh view to update visibility of buttons
                        CollectionViewSource.GetDefaultView(dgInspectors.ItemsSource).Refresh();

                        MessageBox.Show($"Inspector {inspector.FullName} has been activated.",
                                      "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error activating inspector: {ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void btnDeactivate_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is StationInspectorViewModel inspector)
            {
                // Show confirmation dialog
                var result = MessageBox.Show(
                    $"Are you sure you want to deactivate inspector {inspector.FullName}?\n" +
                    "This will cancel all their pending inspections.",
                    "Confirm Deactivation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Get all testing records for this inspector
                        var testingRecords = _inspectionRecordDAO.GetTestingRecordsByInspectorId(inspector.UserId);

                        // Dictionary to track which appointments we've already seen
                        var processedAppointments = new Dictionary<int, InspectionAppointment>();

                        // Dictionary of operations
                        var operations = new Dictionary<string, Func<bool>>();

                        // First, load all unique appointments once
                        foreach (var record in testingRecords)
                        {
                            if (!processedAppointments.ContainsKey(record.AppointmentId))
                            {
                                var appointment = _inspectionAppointmentDAO.GetById(record.AppointmentId);
                                if (appointment != null)
                                {
                                    processedAppointments.Add(record.AppointmentId, appointment);
                                }
                            }
                        }

                        // Now add operations for records
                        foreach (var record in testingRecords)
                        {
                            operations.Add($"cancel_record_{record.RecordId}", () =>
                            {
                                record.Result = Constants.RESULT_CANCELLED;
                                record.InspectionDate = DateTime.Now;
                                record.Comments = "Inspector deactivated from station";
                                return _inspectionRecordDAO.Update(record);
                            });
                        }

                        // Add operations for all unique appointments
                        foreach (var appointmentEntry in processedAppointments)
                        {
                            var appointment = appointmentEntry.Value;
                            operations.Add($"update_appointment_{appointment.AppointmentId}", () =>
                            {
                                appointment.Status = Constants.STATUS_PENDING;
                                return _inspectionAppointmentDAO.Update(appointment);
                            });
                        }

                        // Add operation to deactivate inspector
                        operations.Add("deactivate_inspector", () =>
                        {
                            var stationInspector = _stationInspectorDAO.GetByStationAndInspectorId(_currentStationId, inspector.UserId);
                            if (stationInspector != null)
                            {
                                stationInspector.IsActive = false;
                                return _stationInspectorDAO.Update(stationInspector);
                            }
                            return false;
                        });

                        // Create log entry
                        Log logEntry = new Log
                        {
                            UserId = UserContext.Current.UserId,
                            Action = $"Deactivated inspector {inspector.FullName} and cancelled {testingRecords.Count} pending inspections",
                            Timestamp = DateTime.Now
                        };

                        // Execute all operations in a single transaction
                        bool success = TransactionHelper.ExecuteTransaction(
                            operations,
                            logEntry,
                            notification: null,
                            $"Inspector {inspector.FullName} has been deactivated and {testingRecords.Count} pending inspections have been cancelled.",
                            "Failed to deactivate inspector and cancel inspections",
                            false
                        );

                        if (success)
                        {
                            // Update UI
                            inspector.IsActive = false;
                            CollectionViewSource.GetDefaultView(dgInspectors.ItemsSource).Refresh();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"Error deactivating inspector: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnAddInspector_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<AddInspectorWindow, int>(_currentStationId);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadInspectors();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
