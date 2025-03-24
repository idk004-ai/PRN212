using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for NeededCheckVehicleList.xaml
    /// </summary>
    public partial class NeededCheckVehicleList : Window
    {
        private readonly INavigationService _navigationService;
        private readonly IVehicleDAO _vehicleDAO;
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;
        private readonly PaginationService _paginationService;

        // Keys for pagination helpers
        private const string PENDING_VEHICLES_KEY = "pending_vehicles";
        private const string ASSIGNED_VEHICLES_KEY = "assigned_vehicles";

        // Default page sizes
        private const int DEFAULT_PAGE_SIZE = 10;

        // Prevent double-clicks on refresh buttons
        private bool _isRefreshing = false;

        public NeededCheckVehicleList(INavigationService navigationService, IVehicleDAO vehicleDAO,
            IInspectionRecordDAO inspectionRecordDAO, PaginationService paginationService, IInspectionAppointmentDAO inspectionAppointmentDAO)
        {
            InitializeComponent();

            // Remove event handlers temporarily to prevent them from firing
            cmbPendingPageSize.SelectionChanged -= PendingPageSize_SelectionChanged;
            cmbAssignedPageSize.SelectionChanged -= AssignedPageSize_SelectionChanged;

            _navigationService = navigationService;
            _vehicleDAO = vehicleDAO;
            _inspectionRecordDAO = inspectionRecordDAO;
            _paginationService = paginationService;
            _inspectionAppointmentDAO = inspectionAppointmentDAO;

            LoadPendingVehicles();
            LoadAssignedVehicles();

            // Restore event handlers
            cmbPendingPageSize.SelectionChanged += PendingPageSize_SelectionChanged;
            cmbAssignedPageSize.SelectionChanged += AssignedPageSize_SelectionChanged;
        }

        private void LoadPendingVehicles()
        {
            // Get list of vehicles with pending appointments and sort by scheduled date
            List<Vehicle> vehicles = _vehicleDAO.GetVehicleWithPendingStatus(UserContext.Current.UserId).ToList();
            var vehicleViewModels = new List<VehicleCheckViewModel>();

            foreach (var vehicle in vehicles)
            {
                var appointments = _inspectionAppointmentDAO.GetByVehicleAndStation(vehicle.VehicleId, UserContext.Current.UserId)
                    .Where(a => a.Status == Constants.STATUS_PENDING)
                    .ToList();

                var viewModel = new VehicleCheckViewModel
                {
                    PlateNumber = vehicle.PlateNumber,
                    EmailOwner = vehicle.Owner.Email,
                    ScheduledDate = appointments.Any() ? appointments.First().ScheduledDateTime : (DateTime?)null
                };

                vehicleViewModels.Add(viewModel);
            }

            vehicleViewModels = vehicleViewModels
                .OrderByDescending(v => v.IsOverdue)
                .ThenBy(v => v.ScheduledDate)
                .ToList();

            int pageSize = GetSelectedPageSize(cmbPendingPageSize);

            var paginator = _paginationService.GetOrCreatePaginator(
                PENDING_VEHICLES_KEY,
                vehicleViewModels,
                pageSize);

            UpdatePendingVehiclesUI(paginator);
        }

        private void LoadAssignedVehicles()
        {
            // Get all inspection records for this station
            List<InspectionRecord> records = _inspectionRecordDAO
                .GetAll()
                .Where(r => r.StationId == UserContext.Current.UserId)
                .OrderByDescending(r => r.RecordId)
                .ToList();

            // Convert to view models
            var assignedVehicles = records.Select(record => new AssignedVehicleViewModel
            {
                RecordId = record.RecordId,
                PlateNumber = record.Vehicle.PlateNumber,
                InspectorName = record.Inspector.FullName,
                AssignedDate = record.Appointment.CreatedAt,
                Result = record.Result
            }).ToList();

            // Get page size from combo box
            int pageSize = GetSelectedPageSize(cmbAssignedPageSize);

            // Get or create paginator
            var paginator = _paginationService.GetOrCreatePaginator(
                ASSIGNED_VEHICLES_KEY,
                assignedVehicles,
                pageSize);

            // Update UI
            UpdateAssignedVehiclesUI(paginator);
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

        private void UpdatePendingVehiclesUI(PaginationHelper<VehicleCheckViewModel> paginator)
        {
            // Update DataGrid
            dataGridVehicle.ItemsSource = paginator.GetCurrentPage();

            // Update pagination info
            txtPendingPageInfo.Text = $"Page {paginator.CurrentPage} of {paginator.TotalPages}";
            btnPendingPrevious.IsEnabled = paginator.HasPreviousPage;
            btnPendingNext.IsEnabled = paginator.HasNextPage;
        }

        private void UpdateAssignedVehiclesUI(PaginationHelper<AssignedVehicleViewModel> paginator)
        {
            // Update DataGrid
            dataGridAssignedVehicles.ItemsSource = paginator.GetCurrentPage();

            // Update pagination info
            txtAssignedPageInfo.Text = $"Page {paginator.CurrentPage} of {paginator.TotalPages}";
            btnAssignedPrevious.IsEnabled = paginator.HasPreviousPage;
            btnAssignedNext.IsEnabled = paginator.HasNextPage;
        }

        private void PendingPrevious_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<VehicleCheckViewModel>(PENDING_VEHICLES_KEY);
            if (paginator != null && paginator.PreviousPage())
            {
                UpdatePendingVehiclesUI(paginator);
            }
        }

        private void PendingNext_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<VehicleCheckViewModel>(PENDING_VEHICLES_KEY);
            if (paginator != null && paginator.NextPage())
            {
                UpdatePendingVehiclesUI(paginator);
            }
        }

        private void AssignedPrevious_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<AssignedVehicleViewModel>(ASSIGNED_VEHICLES_KEY);
            if (paginator != null && paginator.PreviousPage())
            {
                UpdateAssignedVehiclesUI(paginator);
            }
        }

        private void AssignedNext_Click(object sender, RoutedEventArgs e)
        {
            var paginator = _paginationService.GetPaginator<AssignedVehicleViewModel>(ASSIGNED_VEHICLES_KEY);
            if (paginator != null && paginator.NextPage())
            {
                UpdateAssignedVehiclesUI(paginator);
            }
        }

        private void PendingPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_paginationService == null) return;
            var paginator = _paginationService.GetPaginator<VehicleCheckViewModel>(PENDING_VEHICLES_KEY);
            if (paginator == null)
            {
                LoadPendingVehicles();
                return;
            }
            int pageSize = GetSelectedPageSize(cmbPendingPageSize);
            paginator.SetPageSize(pageSize);
            UpdatePendingVehiclesUI(paginator);
        }

        private void AssignedPageSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_paginationService == null) return;
            var paginator = _paginationService.GetPaginator<AssignedVehicleViewModel>(ASSIGNED_VEHICLES_KEY);
            if (paginator == null)
            {
                LoadAssignedVehicles();
                return;
            }
            int pageSize = GetSelectedPageSize(cmbAssignedPageSize);
            paginator.SetPageSize(pageSize);
            UpdateAssignedVehiclesUI(paginator);
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var vehicleViewModel = button?.CommandParameter as VehicleCheckViewModel;

            if (vehicleViewModel != null)
            {
                _navigationService.NavigateTo<VehicleDetailWindow, VehicleCheckViewModel>(vehicleViewModel);
            }
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;
            try
            {
                var vehicleViewModel = button?.CommandParameter as VehicleCheckViewModel;
                if (vehicleViewModel != null)
                {
                    // Store the reference to the vehicle's plate number before opening the window
                    string plateNumber = vehicleViewModel.PlateNumber;

                    var assignWindow = App.GetService<Func<VehicleCheckViewModel, AssignInspectorWindow>>()(vehicleViewModel);

                    assignWindow.Owner = this;
                    assignWindow.ShowDialog();

                    // Check if assignment was successful
                    if (assignWindow.AssignmentSuccess)
                    {
                        // Refresh both tabs data
                        LoadPendingVehicles();
                        LoadAssignedVehicles();
                    }
                }
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handle refresh button click for pending vehicles
        /// </summary>
        private void RefreshPendingButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing) return;

            try
            {
                _isRefreshing = true;
                btnRefreshPending.IsEnabled = false;

                // Show loading cursor
                Cursor = Cursors.Wait;

                // Clear pagination cache and reload data
                _paginationService.RemovePaginator<VehicleCheckViewModel>(PENDING_VEHICLES_KEY);
                LoadPendingVehicles();

                // Briefly indicate successful refresh
                MessageBox.Show("Pending vehicle list refreshed successfully.", "Refresh Complete",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing data: {ex.Message}", "Refresh Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Restore cursor and button state
                Cursor = Cursors.Arrow;
                btnRefreshPending.IsEnabled = true;
                _isRefreshing = false;
            }
        }

        /// <summary>
        /// Handle refresh button click for assigned vehicles
        /// </summary>
        private void RefreshAssignedButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isRefreshing) return;

            try
            {
                _isRefreshing = true;
                btnRefreshAssigned.IsEnabled = false;

                // Show loading cursor
                Cursor = Cursors.Wait;

                // Clear pagination cache and reload data
                _paginationService.RemovePaginator<AssignedVehicleViewModel>(ASSIGNED_VEHICLES_KEY);
                LoadAssignedVehicles();

                // Briefly indicate successful refresh
                MessageBox.Show("Assigned vehicle list refreshed successfully.", "Refresh Complete",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing data: {ex.Message}", "Refresh Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Restore cursor and button state
                Cursor = Cursors.Arrow;
                btnRefreshAssigned.IsEnabled = true;
                _isRefreshing = false;
            }
        }
    }
}
