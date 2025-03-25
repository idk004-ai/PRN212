using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.Views;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for InspectorVehicleListWindow.xaml
    /// </summary>
    public partial class InspectorVehicleListWindow : Window
    {
        // Data Access Objects and Services
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly INavigationService _navigationService;
        private readonly PaginationService _paginationService;

        // Keys for pagination helpers
        private const string ASSIGNED_VEHICLES_KEY = "assigned_vehicles";
        private const string HISTORY_VEHICLES_KEY = "history_vehicles";
        private const string ALL_HISTORY_VEHICLES_KEY = "All";
        // Default page size
        private const int DEFAULT_PAGE_SIZE = 10;

        public InspectorVehicleListWindow(
            IInspectionRecordDAO inspectionRecordDAO,
            INavigationService navigationService,
            PaginationService paginationService)
        {
            InitializeComponent();

            // Remove event handlers temporarily to prevent them from firing
            cmbAssignedPageSize.SelectionChanged -= AssignedPageSize_SelectionChanged;
            cmbHistoryPageSize.SelectionChanged -= HistoryPageSize_SelectionChanged;
            cbHistoryStatus.SelectionChanged -= cbHistoryStatus_SelectionChanged;

            this._inspectionRecordDAO = inspectionRecordDAO;
            this._navigationService = navigationService;
            this._paginationService = paginationService;

            // Load initial data
            LoadAssignedVehicles();
            LoadHistoryRecords();

            // Restore event handlers
            cmbAssignedPageSize.SelectionChanged += AssignedPageSize_SelectionChanged;
            cmbHistoryPageSize.SelectionChanged += HistoryPageSize_SelectionChanged;
            cbHistoryStatus.SelectionChanged += cbHistoryStatus_SelectionChanged;
        }

        #region Data Loading

        private void LoadAssignedVehicles()
        {
            if (_inspectionRecordDAO == null) return;

            // Get the current user (inspector) ID
            int inspectorId = UserContext.Current.UserId;

            // Get assigned (testing) vehicles
            var assignedRecords = _inspectionRecordDAO.GetTestingRecordsByInspectorId(inspectorId);

            // Get page size from combo box
            int pageSize = GetSelectedPageSize(cmbAssignedPageSize);

            // Get or create paginator
            var paginator = _paginationService.GetOrCreatePaginator(
                ASSIGNED_VEHICLES_KEY,
                assignedRecords,
                pageSize);

            // Update UI
            UpdateAssignedVehiclesUI(paginator);
        }

        private void LoadHistoryRecords()
        {
            if (_inspectionRecordDAO == null) return;

            // Get the current user (inspector) ID
            int inspectorId = UserContext.Current.UserId;

            // Get history records
            var allHistoryRecords = _inspectionRecordDAO.GetAll()
                .Where(r => r.InspectorId == inspectorId && r.Result != Constants.RESULT_TESTING)
                .ToList();

            // Filter by status if selected
            string selectedResult = GetSelectedStatus();
            if (selectedResult != ALL_HISTORY_VEHICLES_KEY)
            {
                allHistoryRecords = allHistoryRecords.Where(r => r.Result == selectedResult).ToList();
            }

            // Get page size from combo box
            int pageSize = GetSelectedPageSize(cmbHistoryPageSize);

            // Get or create paginator
            var paginator = _paginationService.GetOrCreatePaginator(
                HISTORY_VEHICLES_KEY,
                allHistoryRecords,
                pageSize);

            // Update UI
            UpdateHistoryVehiclesUI(paginator);
        }

        private int GetSelectedPageSize(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem selectedItem &&
                int.TryParse(selectedItem.Content.ToString(), out int pageSize))
            {
                return pageSize;
            }
            return DEFAULT_PAGE_SIZE;
        }

        private string GetSelectedStatus()
        {
            if (cbHistoryStatus.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content.ToString();
            }
            return ALL_HISTORY_VEHICLES_KEY;
        }

        #endregion

        #region UI Updates

        private void UpdateAssignedVehiclesUI(PaginationHelper<InspectionRecord> paginator)
        {
            // Update DataGrid
            dgAssignedVehicles.ItemsSource = paginator.GetCurrentPage();

            // Update pagination info
            txtAssignedPageInfo.Text = $"Page {paginator.CurrentPage} of {paginator.TotalPages}";
            btnAssignedPrevious.IsEnabled = paginator.HasPreviousPage;
            btnAssignedNext.IsEnabled = paginator.HasNextPage;

            // Update record count
            txtAssignedRecordCount.Text = paginator.TotalItems.ToString();
        }

        private void UpdateHistoryVehiclesUI(PaginationHelper<InspectionRecord> paginator)
        {
            // Update DataGrid
            dgInspectionHistory.ItemsSource = paginator.GetCurrentPage();

            // Update pagination info
            txtHistoryPageInfo.Text = $"Page {paginator.CurrentPage} of {paginator.TotalPages}";
            btnHistoryPrevious.IsEnabled = paginator.HasPreviousPage;
            btnHistoryNext.IsEnabled = paginator.HasNextPage;

            // Update record count
            txtHistoryRecordCount.Text = paginator.TotalItems.ToString();
        }

        #endregion

        #region Pagination Event Handlers

        private void AssignedPrevious_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(ASSIGNED_VEHICLES_KEY);
            if (paginator != null && paginator.PreviousPage())
            {
                UpdateAssignedVehiclesUI(paginator);
            }
        }

        private void AssignedNext_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(ASSIGNED_VEHICLES_KEY);
            if (paginator != null && paginator.NextPage())
            {
                UpdateAssignedVehiclesUI(paginator);
            }
        }

        private void AssignedPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_paginationService == null) return;

            var paginator = _paginationService.GetPaginator<InspectionRecord>(ASSIGNED_VEHICLES_KEY);
            if (paginator == null)
            {
                LoadAssignedVehicles();
                return;
            }

            int pageSize = GetSelectedPageSize(cmbAssignedPageSize);
            paginator.SetPageSize(pageSize);
            UpdateAssignedVehiclesUI(paginator);
        }

        private void HistoryPrevious_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(HISTORY_VEHICLES_KEY);
            if (paginator != null && paginator.PreviousPage())
            {
                UpdateHistoryVehiclesUI(paginator);
            }
        }

        private void HistoryNext_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<InspectionRecord>(HISTORY_VEHICLES_KEY);
            if (paginator != null && paginator.NextPage())
            {
                UpdateHistoryVehiclesUI(paginator);
            }
        }

        private void HistoryPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_paginationService == null) return;

            var paginator = _paginationService.GetPaginator<InspectionRecord>(HISTORY_VEHICLES_KEY);
            if (paginator == null)
            {
                LoadHistoryRecords();
                return;
            }

            int pageSize = GetSelectedPageSize(cmbHistoryPageSize);
            paginator.SetPageSize(pageSize);
            UpdateHistoryVehiclesUI(paginator);
        }

        private void cbHistoryStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // When status filter changes, reload the history records with the new filter
            LoadHistoryRecords();
        }

        #endregion

        #region Button Click Handlers

        private void StartInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            InspectionRecord? record = button?.CommandParameter as InspectionRecord;

            if (record != null)
            {
                // Navigate to inspection form
                _navigationService.NavigateTo<VehicleInspectionDetailsWindow, (InspectionRecord, bool)>((record, false));

                // Refresh data when returning
                LoadAssignedVehicles();
                LoadHistoryRecords();
            }
        }

        private void CancelInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            button.IsEnabled = false;

            try
            {
                InspectionRecord? record = button?.CommandParameter as InspectionRecord;

                if (record != null)
                {
                    // Ask for confirmation
                    MessageBoxResult result = MessageBox.Show(
                        $"Are you sure you want to cancel the inspection for vehicle {record.Vehicle.PlateNumber}?\n\n" +
                        "This action cannot be undone.",
                        "Confirm Cancellation",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Show dialog to get cancellation reason
                        var reasonWindow = new CancellationReasonWindow(record.Vehicle.PlateNumber);
                        reasonWindow.Owner = this; // Set owner for proper modal behavior

                        bool? dialogResult = reasonWindow.ShowDialog();

                        if (dialogResult == true)
                        {
                            string cancellationReason = reasonWindow.CancellationReason;

                            // Cancel inspection with reason
                            bool success = _inspectionRecordDAO.CancelInspection(
                                record,
                                UserContext.Current,
                                record.Vehicle.PlateNumber,
                                record.Station.FullName,
                                cancellationReason,
                                null // Don't close this window
                                );

                            if (success)
                            {
                                // Refresh data
                                LoadAssignedVehicles();
                                LoadHistoryRecords();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling inspection: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }


        private void ViewHistoryDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            InspectionRecord? record = button?.CommandParameter as InspectionRecord;

            if (record != null)
            {
                // Open the vehicle inspection details window in read-only mode
                _navigationService.NavigateTo<VehicleInspectionDetailsWindow, (InspectionRecord, bool)>((record, true));
            }
        }

        private void dgAssignedVehicles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InspectionRecord? record = dgAssignedVehicles.SelectedItem as InspectionRecord;
            if (record != null)
            {
                _navigationService.NavigateTo<VehicleInspectionDetailsWindow, (InspectionRecord, bool)>((record, false));
                LoadAssignedVehicles();
                LoadHistoryRecords();
            }
        }

        private void dgInspectionHistory_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InspectionRecord? record = dgInspectionHistory.SelectedItem as InspectionRecord;
            if (record != null && record.Result != Constants.RESULT_CANCELLED)
            {
                _navigationService.NavigateTo<VehicleInspectionDetailsWindow, (InspectionRecord, bool)>((record, true));
            }
            else if (record != null && record.Result == Constants.RESULT_CANCELLED)
            {
                MessageBox.Show("This inspection has been cancelled and cannot be viewed.",
                    "Inspection Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadAssignedVehicles();
            LoadHistoryRecords();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.CloseAllExcept(typeof(LogsMonitorWindow));
            _navigationService.NavigateTo<Login>();
            this.Close();
        }

        #endregion
    }
}
