using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.ViewModel;
using QuanLiKhiThai.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for VehicleDetailWindow.xaml
    /// </summary>
    public partial class VehicleDetailWindow : Window
    {
        private readonly VehicleCheckViewModel _viewModel;
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;
        private readonly PaginationService _paginationService;
        private readonly ValidationService _validationService;
        private readonly ILogDAO _logDAO;

        // Vehicle data cache
        private Vehicle? _vehicle;

        // Store the next appointment for easy access
        private InspectionAppointment? _nextAppointment;

        // Keys for pagination helpers
        private const string HISTORY_VEHICLES_KEY = "vehicle_history";

        // Default page size
        private const int DEFAULT_PAGE_SIZE = 10;

        public VehicleDetailWindow(
            VehicleCheckViewModel viewModel,
            IVehicleDAO vehicleDAO,
            IInspectionRecordDAO inspectionRecordDAO,
            IInspectionAppointmentDAO inspectionAppointmentDAO,
            PaginationService paginationService,
            ValidationService validationService,
            ILogDAO logDAO)
        {
            InitializeComponent();

            // Store dependencies
            _viewModel = viewModel;
            _vehicleDAO = vehicleDAO;
            _inspectionRecordDAO = inspectionRecordDAO;
            _inspectionAppointmentDAO = inspectionAppointmentDAO;
            _paginationService = paginationService;
            _validationService = validationService;
            _logDAO = logDAO;

            // Register event handlers
            cbStatusFilter.SelectionChanged += Filter_Changed;
            cbTimeRange.SelectionChanged += Filter_Changed;
            cmbHistoryPageSize.SelectionChanged += PageSize_Changed;
            btnHistoryPrevious.Click += BtnHistoryPrevious_Click;
            btnHistoryNext.Click += BtnHistoryNext_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnEditScheduledDate.Click += BtnEditScheduledDate_Click;

            // Load initial data
            LoadData();
            _logDAO = logDAO;
        }

        /// <summary>
        /// Load vehicle data and inspection history
        /// </summary>
        private void LoadData()
        {
            // Load vehicle data
            _vehicle = _vehicleDAO.GetByPlateNumber(_viewModel.PlateNumber);
            if (_vehicle == null)
            {
                MessageBox.Show("Vehicle not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Update UI
            Title = $"Vehicle Details - {_vehicle.PlateNumber}";
            txtVehiclePlate.Text = $"Vehicle: {_vehicle.PlateNumber} ({_vehicle.Brand} {_vehicle.Model})";

            // Set context for binding vehicle information
            this.DataContext = _vehicle;

            // Load scheduled inspection date
            LoadScheduledInspectionDate();

            // Load inspection history
            LoadInspectionHistory();
        }

        /// <summary>
        /// Load the scheduled inspection date for this vehicle
        /// </summary>
        private void LoadScheduledInspectionDate()
        {
            if (_vehicle == null) return;

            // Get pending appointment for this vehicle at this station (each vehicle should have only 1 appointment)
            var pendingAppointments = _inspectionAppointmentDAO
                .GetByVehicleAndStation(_vehicle.VehicleId, UserContext.Current.UserId)
                .Where(a => a.Status == Constants.STATUS_PENDING)
                .OrderBy(a => a.ScheduledDateTime)
                .ToList();

            if (pendingAppointments.Any())
            {
                // Get the next scheduled appointment
                _nextAppointment = pendingAppointments.FirstOrDefault();

                // Update UI to display the scheduled date
                txtScheduledDate.Text = _nextAppointment.ScheduledDateTime.ToString("dd/MM/yyyy HH:mm");

                // Apply styling for overdue dates
                if (_nextAppointment.ScheduledDateTime < DateTime.Now)
                {
                    txtScheduledDate.Foreground = new SolidColorBrush(Colors.Red);
                    txtScheduledDate.FontWeight = FontWeights.Bold;
                }
                else
                {
                    txtScheduledDate.Foreground = new SolidColorBrush(Colors.Black);
                    txtScheduledDate.FontWeight = FontWeights.Normal;
                }
            }
            else
            {
                _nextAppointment = null;
                txtScheduledDate.Text = "No scheduled date";
                txtScheduledDate.Foreground = new SolidColorBrush(Colors.Gray);
                txtScheduledDate.FontWeight = FontWeights.Normal;
            }
        }

        /// <summary>
        /// Load inspection history for the vehicle
        /// </summary>
        private void LoadInspectionHistory()
        {
            // Get inspection records for this vehicle
            var allRecords = _inspectionRecordDAO.GetRecordByVehicle(_vehicle.VehicleId);
            if (allRecords == null)
            {
                allRecords = new List<InspectionRecord>();
            }

            // Filter to only show records from this station
            allRecords = allRecords.Where(r => r.StationId == UserContext.Current.UserId).ToList();

            // Apply filters
            allRecords = ApplyFilters(allRecords);

            // Sort by date (newest first)
            allRecords = allRecords.OrderByDescending(r => r.InspectionDate).ToList();

            // Get page size
            int pageSize = GetSelectedPageSize();

            // Get or create paginator
            var paginator = _paginationService.GetOrCreatePaginator(
                HISTORY_VEHICLES_KEY,
                allRecords,
                pageSize);

            // Update UI
            UpdateHistoryUI(paginator);
        }

        /// <summary>
        /// Apply filters to the inspection records
        /// </summary>
        private List<InspectionRecord> ApplyFilters(List<InspectionRecord> records)
        {
            // Apply status filter if selected
            string statusFilter = GetSelectedStatus();
            if (statusFilter != "All")
            {
                records = records.Where(r => r.Result == statusFilter).ToList();
            }

            // Apply time range filter
            DateTime fromDate = GetFromDate();
            records = records.Where(r => r.InspectionDate == null || r.InspectionDate >= fromDate).ToList();

            return records;
        }

        /// <summary>
        /// Update UI with paginated data
        /// </summary>
        private void UpdateHistoryUI(PaginationHelper<InspectionRecord> paginator)
        {
            // Update data grid
            dgInspectionHistory.ItemsSource = paginator.GetCurrentPage();

            // Update pagination info
            txtHistoryPageInfo.Text = $"Page {paginator.CurrentPage} of {paginator.TotalPages}";
            btnHistoryPrevious.IsEnabled = paginator.HasPreviousPage;
            btnHistoryNext.IsEnabled = paginator.HasNextPage;

            // Update record count
            txtHistoryRecordCount.Text = paginator.TotalItems.ToString();
        }

        /// <summary>
        /// Get selected status from filter dropdown
        /// </summary>
        private string GetSelectedStatus()
        {
            if (cbStatusFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content.ToString();
            }
            return "All";
        }

        /// <summary>
        /// Get from date based on selected time range
        /// </summary>
        private DateTime GetFromDate()
        {
            DateTime now = DateTime.Now;

            if (cbTimeRange.SelectedItem is ComboBoxItem selectedItem)
            {
                string range = selectedItem.Content.ToString();

                return range switch
                {
                    "Last 3 Months" => now.AddMonths(-3),
                    "Last 6 Months" => now.AddMonths(-6),
                    "Last Year" => now.AddYears(-1),
                    _ => DateTime.MinValue // All Time
                };
            }

            return now.AddMonths(-3); // Default to last 3 months
        }

        /// <summary>
        /// Get selected page size
        /// </summary>
        private int GetSelectedPageSize()
        {
            if (cmbHistoryPageSize.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content.ToString(), out int pageSize))
            {
                return pageSize;
            }
            return DEFAULT_PAGE_SIZE;
        }

        #region Event Handlers

        /// <summary>
        /// Handle edit scheduled date button click
        /// </summary>
        private void BtnEditScheduledDate_Click(object sender, RoutedEventArgs e)
        {
            if (_vehicle == null) return;

            if (_nextAppointment == null)
            {
                var defaultDate = DateTime.Now.AddDays(1);

                var newAppointment = new InspectionAppointment
                {
                    VehicleId = _vehicle.VehicleId,
                    StationId = UserContext.Current.UserId,
                    ScheduledDateTime = defaultDate,
                    Status = Constants.STATUS_PENDING,
                    CreatedAt = DateTime.Now
                };

                // Open the date edit dialog
                var dateEditWindow = new StationDateEditWindow(defaultDate);
                dateEditWindow.Owner = this;
                var result = dateEditWindow.ShowDialog();

                if (result == true && dateEditWindow.SelectedDate.HasValue)
                {
                    // Update with selected date
                    newAppointment.ScheduledDateTime = dateEditWindow.SelectedDate.Value;

                    // Save new appointment
                    bool success = _inspectionAppointmentDAO.Add(newAppointment);
                    if (success)
                    {
                        MessageBox.Show("Inspection date scheduled successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        // Refresh data
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to schedule inspection date.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                // Edit existing appointment
                var dateEditWindow = new StationDateEditWindow(_nextAppointment.ScheduledDateTime);
                dateEditWindow.Owner = this;
                var result = dateEditWindow.ShowDialog();

                if (result == true && dateEditWindow.SelectedDate.HasValue)
                {
                    // Update appointment with selected date
                    _nextAppointment.ScheduledDateTime = dateEditWindow.SelectedDate.Value;

                    // Save changes
                    bool success = _inspectionAppointmentDAO.Update(_nextAppointment);
                    if (success)
                    {
                        MessageBox.Show("Inspection date updated successfully.", "Success",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        Log logEntry = new Log
                        {
                            UserId = UserContext.Current.UserId,
                            Action = $"Updated scheduled inspection date for vehicle {_vehicle.PlateNumber} to {_nextAppointment.ScheduledDateTime:dd/MM/yyyy HH:mm}",
                            Timestamp = DateTime.Now
                        };
                        _logDAO.Add(logEntry);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update inspection date.", "Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Handle filters changed (status or time range)
        /// </summary>
        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadInspectionHistory();
        }

        /// <summary>
        /// Handle page size change
        /// </summary>
        private void PageSize_Changed(object sender, SelectionChangedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(HISTORY_VEHICLES_KEY);
            if (paginator == null)
            {
                LoadInspectionHistory();
                return;
            }

            int pageSize = GetSelectedPageSize();
            paginator.SetPageSize(pageSize);
            UpdateHistoryUI(paginator);
        }

        /// <summary>
        /// Handle previous page button click
        /// </summary>
        private void BtnHistoryPrevious_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(HISTORY_VEHICLES_KEY);
            if (paginator != null && paginator.PreviousPage())
            {
                UpdateHistoryUI(paginator);
            }
        }

        /// <summary>
        /// Handle next page button click
        /// </summary>
        private void BtnHistoryNext_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(HISTORY_VEHICLES_KEY);
            if (paginator != null && paginator.NextPage())
            {
                UpdateHistoryUI(paginator);
            }
        }

        /// <summary>
        /// Handle refresh button click
        /// </summary>
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// Handle close button click
        /// </summary>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handle cancel appointment button click
        /// </summary>
        private void BtnCancelAppointment_Click(object sender, RoutedEventArgs e)
        {
            // Get the clicked button
            var button = sender as Button;
            if (button == null) return;

            // Get the data context (InspectionRecord)
            var record = button.DataContext as InspectionRecord;
            if (record == null) return;

            if (record.Appointment == null)
            {
                MessageBox.Show("No appointment found for this record.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            bool isValid = _validationService.ValidateDataConsistencyForAppointmentCancellation(record.AppointmentId);
            if (!isValid)
            {
                return;
            }

            // Open the cancellation email window
            var cancellationWindow = new AppointmentCancellationEmailWindow(
                record.Vehicle,
                record.Appointment);

            // Show as dialog
            cancellationWindow.Owner = this;
            cancellationWindow.ShowDialog();

            // If cancelled, update data
            if (cancellationWindow.CancellationConfirmed)
            {
                string reason = cancellationWindow.CancellationReason;

                // Cancel appointment using DAO
                bool success = _inspectionAppointmentDAO.CancelAppointment(
                    record.Appointment,
                    UserContext.Current,
                    record.Vehicle.PlateNumber,
                    record.Station.FullName,
                    reason,
                    null);

                if (success)
                {
                    // Refresh the inspection history and scheduled date
                    LoadData();
                }
            }
        }

        #endregion
    }
}
