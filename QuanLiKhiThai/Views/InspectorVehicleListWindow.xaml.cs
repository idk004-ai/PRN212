using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using QuanLiKhiThai.DAO.Interface;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for InspectorVehicleListWindow.xaml
    /// </summary>
    public partial class InspectorVehicleListWindow : Window
    {
        private List<InspectionRecord> _allRecords = new List<InspectionRecord>();
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly INavigationService _navigationService;

        public InspectorVehicleListWindow(IInspectionRecordDAO inspectionRecordDAO, INavigationService navigationService)
        {
            InitializeComponent();

            // Set today's date as default
            dpFilterDate.SelectedDate = DateTime.Today;
            this._inspectionRecordDAO = inspectionRecordDAO;
            this._navigationService = navigationService;

            LoadData();
            _navigationService = navigationService;
        }

        private void LoadData()
        {
            try
            {
                // Get the current user (inspector) ID
                int inspectorId = UserContext.Current.UserId;

                // Get all active inspection records assigned to this inspector
                _allRecords = _inspectionRecordDAO.GetTestingRecordsByInspectorId(inspectorId);

                // Apply date filter if selected
                ApplyDateFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyDateFilter()
        {
            try
            {
                List<InspectionRecord> filteredRecords = _allRecords;

                // Apply date filter if a date is selected
                if (dpFilterDate.SelectedDate.HasValue)
                {
                    DateTime selectedDate = dpFilterDate.SelectedDate.Value.Date;
                    filteredRecords = _allRecords.Where(r =>
                        r.Appointment.ScheduledDateTime.Date == selectedDate).ToList();
                }

                // Update the DataGrid with filtered records
                this.dgAssignedVehicles.ItemsSource = filteredRecords;

                // Update record count
                txtRecordCount.Text = filteredRecords.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying filter: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyDateFilter();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            dpFilterDate.SelectedDate = DateTime.Today;
            // ApplyDateFilter will be called by the SelectedDateChanged event
        }

        private void AllRecordsButton_Click(object sender, RoutedEventArgs e)
        {
            dpFilterDate.SelectedDate = null;
            this.dgAssignedVehicles.ItemsSource = _allRecords;
            txtRecordCount.Text = _allRecords.Count.ToString();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            InspectionRecord? record = button?.CommandParameter as InspectionRecord;

            if (record != null)
            {
                // Open the vehicle inspection details window
                _navigationService.NavigateTo<VehicleInspectionDetailsWindow, InspectionRecord>(record);

                // Refresh data when dialog closes in case changes were made
                LoadData();
            }
        }

        private void dgAssignedVehicles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            InspectionRecord? record = dgAssignedVehicles.SelectedItem as InspectionRecord;
            if (record != null)
            {
                _navigationService.NavigateTo<VehicleInspectionDetailsWindow, InspectionRecord>(record);
                LoadData();
            }
        }

        private void CancelInspectionButton_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
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
                    try
                    {
                        // Cancel inspection
                        _inspectionRecordDAO.CancelInspection(
                            record,
                            UserContext.Current,
                            record.Vehicle.PlateNumber,
                            record.Station.FullName,
                            this);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error cancelling inspection: {ex.Message}",
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
