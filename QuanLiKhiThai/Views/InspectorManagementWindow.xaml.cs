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

        // Collection to hold inspector data for the DataGrid
        private ObservableCollection<StationInspectorViewModel> _inspectors;

        // CollectionView for filtering
        private ICollectionView _inspectorsView;

        public InspectorManagementWindow(INavigationService navigationService, IUserDAO userDAO, IStationInspectorDAO stationInspectorDAO, int stationId)
        {
            InitializeComponent();

            // Store dependencies
            this._navigationService = navigationService;
            this._userDAO = userDAO;
            this._stationInspectorDAO = stationInspectorDAO;
            this._currentStationId = stationId;

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

        private void btnDeactivate_Click(object sender, RoutedEventArgs e)
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
                        stationInspector.IsActive = false;
                        _stationInspectorDAO.Update(stationInspector);

                        // Update UI
                        inspector.IsActive = false;

                        // Refresh view to update visibility of buttons
                        CollectionViewSource.GetDefaultView(dgInspectors.ItemsSource).Refresh();

                        MessageBox.Show($"Inspector {inspector.FullName} has been deactivated.",
                                      "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deactivating inspector: {ex.Message}",
                                   "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
