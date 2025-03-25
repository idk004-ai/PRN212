using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for ViolationRecordWindow.xaml
    /// </summary>
    public partial class ViolationRecordWindow : Window
    {
        private readonly IViolationRecordDAO _violationRecordDAO;
        private Vehicle _selectedVehicle;

        public ViolationRecordWindow(Vehicle vehicle, IViolationRecordDAO violationRecordDAO)
        {
            InitializeComponent();
            _violationRecordDAO = violationRecordDAO;
            _selectedVehicle = vehicle;

            // Set default date to today
            dpViolationDate.SelectedDate = DateTime.Now;

            // Load vehicle and officer information
            LoadVehicleInfo();
            LoadOfficerInfo();

            // Wire up event handlers
            btnSave.Click += BtnSave_Click;
            btnBack.Click += BtnBack_Click;
            btnPrint.Click += BtnPrint_Click;
        }

        private void LoadVehicleInfo()
        {
            if (_selectedVehicle == null) return;

            txtPlateNumber.Text = $"License: {_selectedVehicle.PlateNumber}";
            txtOwner.Text = _selectedVehicle.Owner?.FullName ?? "Unknown";
            txtBrand.Text = _selectedVehicle.Brand;
            txtModel.Text = _selectedVehicle.Model;
            txtYear.Text = _selectedVehicle.ManufactureYear.ToString();
            txtEngineNumber.Text = _selectedVehicle.EngineNumber;

            // Get the latest inspection record (if any)
            var latestInspection = _selectedVehicle.InspectionRecords?
                .OrderByDescending(ir => ir.InspectionDate)
                .FirstOrDefault();

            if (latestInspection != null)
            {
                txtLastInspection.Text = latestInspection.InspectionDate?.ToString("MM/dd/yyyy") ?? "N/A";
                txtInspectionResult.Text = latestInspection.Result;

                // If inspection failed, check that violation type automatically
                if (latestInspection.Result == "Failed")
                {
                    chkExcessEmission.IsChecked = true;
                }
            }
            else
            {
                txtLastInspection.Text = "No inspection record";
                txtInspectionResult.Text = "N/A";
                chkNoInspection.IsChecked = true;
            }
        }

        private void LoadOfficerInfo()
        {
            if (UserContext.Current != null)
            {
                txtOfficerInfo.Text = $"Officer: {UserContext.Current.FullName}";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                var violationRecord = new ViolationRecord
                {
                    VehicleId = _selectedVehicle.VehicleId,
                    OfficerId = UserContext.Current.UserId,
                    IssueDate = dpViolationDate.SelectedDate ?? DateTime.Now,
                    Location = txtLocation.Text,
                    ViolationType = GetSelectedViolationTypes(),
                    FineAmount = decimal.Parse(txtFineAmount.Text),
                    Notes = txtNotes.Text,
                    CreatedAt = DateTime.Now
                };

                if (_violationRecordDAO.Add(violationRecord))
                {
                    MessageBox.Show("Violation record saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Failed to save violation record.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSelectedViolationTypes()
        {
            var selectedTypes = new List<string>();

            if (chkExcessEmission.IsChecked == true)
                selectedTypes.Add("Excess Emission");

            if (chkNoInspection.IsChecked == true)
                selectedTypes.Add("Missing/Expired Inspection");

            if (chkTampering.IsChecked == true)
                selectedTypes.Add("Emission Control Tampering");

            if (chkOther.IsChecked == true)
                selectedTypes.Add("Other Violation");

            return string.Join(", ", selectedTypes);
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter a location.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLocation.Focus();
                return false;
            }

            if (dpViolationDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a violation date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpViolationDate.Focus();
                return false;
            }

            if (!chkExcessEmission.IsChecked.GetValueOrDefault() &&
                !chkNoInspection.IsChecked.GetValueOrDefault() &&
                !chkTampering.IsChecked.GetValueOrDefault() &&
                !chkOther.IsChecked.GetValueOrDefault())
            {
                MessageBox.Show("Please select at least one violation type.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            decimal fineAmount;
            if (!decimal.TryParse(txtFineAmount.Text, out fineAmount) || fineAmount <= 0)
            {
                MessageBox.Show("Please enter a valid fine amount greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFineAmount.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            txtLocation.Clear();
            dpViolationDate.SelectedDate = DateTime.Now;
            chkExcessEmission.IsChecked = false;
            chkNoInspection.IsChecked = false;
            chkTampering.IsChecked = false;
            chkOther.IsChecked = false;
            txtFineAmount.Text = "0.00";
            txtNotes.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Ask user if they want to discard changes
            var result = MessageBox.Show("Are you sure you want to go back? Any unsaved changes will be lost.",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            // This would typically implement printing functionality
            MessageBox.Show("Print functionality will be implemented in a future version.",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
