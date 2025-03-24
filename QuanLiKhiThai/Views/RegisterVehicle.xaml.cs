using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for RegisterVehicle.xaml
    /// </summary>
    public partial class RegisterVehicle : Window
    {
        private readonly IVehicleDAO _vehicleDAO;
        // Regular expression for Vietnamese license plates
        private readonly Regex plateNumberRegex = new Regex(@"^\d{2}[A-Z]-\d{4,5}$|^\d{2}[A-Z]\s\d{4,5}$");
        private ObservableCollection<Vehicle> _userVehicles;
        private Vehicle? _selectedVehicle; // For edit mode
        private bool _isEditMode = false;

        public RegisterVehicle(IVehicleDAO vehicleDAO)
        {
            InitializeComponent();
            this._vehicleDAO = vehicleDAO;
            _userVehicles = new ObservableCollection<Vehicle>();
            dgVehicles.ItemsSource = _userVehicles;

            // Add event handlers
            Loaded += RegisterVehicle_Loaded;
            btnRefresh.Click += RefreshButton_Click;

            // Default tab is Register
            MainTabControl.SelectedIndex = 0;
        }

        private void RegisterVehicle_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserVehicles();
        }

        private void LoadUserVehicles()
        {
            try
            {
                _userVehicles.Clear();

                // Get all vehicles owned by the current user
                int ownerId = UserContext.Current.UserId;
                var vehicles = _vehicleDAO.GetVehicleByOwnerId(ownerId).ToList();

                foreach (var vehicle in vehicles)
                {
                    _userVehicles.Add(vehicle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading vehicles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserVehicles();
        }

        private bool ValidateFields()
        {
            // Reset field styles
            ResetFieldStyles();

            bool isValid = true;
            StringBuilder errorMessage = new StringBuilder("Please correct the following errors:\n\n");

            // Validate Plate Number
            if (string.IsNullOrWhiteSpace(txtPlateNumber.Text))
            {
                errorMessage.AppendLine("• Plate Number cannot be empty");
                HighlightField(txtPlateNumber);
                isValid = false;
            }
            else if (!plateNumberRegex.IsMatch(txtPlateNumber.Text))
            {
                errorMessage.AppendLine("• Plate Number format should be like: 43A-12345 or 43A 12345");
                HighlightField(txtPlateNumber);
                isValid = false;
            }
            else if (!_isEditMode) // Only check for duplicates in add mode
            {
                // Check if plate number already exists
                var existingVehicle = _vehicleDAO.GetByPlateNumber(txtPlateNumber.Text);
                if (existingVehicle != null)
                {
                    errorMessage.AppendLine("• This Plate Number is already registered");
                    HighlightField(txtPlateNumber);
                    isValid = false;
                }
            }

            // Validate Brand
            if (string.IsNullOrWhiteSpace(txtBrand.Text))
            {
                errorMessage.AppendLine("• Brand cannot be empty");
                HighlightField(txtBrand);
                isValid = false;
            }
            else if (txtBrand.Text.Length > 50)
            {
                errorMessage.AppendLine("• Brand name is too long (maximum 50 characters)");
                HighlightField(txtBrand);
                isValid = false;
            }

            // Validate Model
            if (string.IsNullOrWhiteSpace(txtModel.Text))
            {
                errorMessage.AppendLine("• Model cannot be empty");
                HighlightField(txtModel);
                isValid = false;
            }
            else if (txtModel.Text.Length > 50)
            {
                errorMessage.AppendLine("• Model name is too long (maximum 50 characters)");
                HighlightField(txtModel);
                isValid = false;
            }

            // Validate Manufacture Year
            if (string.IsNullOrWhiteSpace(txtManufactureYear.Text))
            {
                errorMessage.AppendLine("• Manufacture Year cannot be empty");
                HighlightField(txtManufactureYear);
                isValid = false;
            }
            else if (!int.TryParse(txtManufactureYear.Text, out int year))
            {
                errorMessage.AppendLine("• Manufacture Year must be a number");
                HighlightField(txtManufactureYear);
                isValid = false;
            }
            else if (year < 1900 || year > DateTime.Now.Year)
            {
                errorMessage.AppendLine($"• Manufacture Year must be between 1900 and {DateTime.Now.Year}");
                HighlightField(txtManufactureYear);
                isValid = false;
            }

            // Validate Engine Number
            if (string.IsNullOrWhiteSpace(txtEngineNumber.Text))
            {
                errorMessage.AppendLine("• Engine Number cannot be empty");
                HighlightField(txtEngineNumber);
                isValid = false;
            }
            else if (txtEngineNumber.Text.Length > 20)
            {
                errorMessage.AppendLine("• Engine Number is too long (maximum 20 characters)");
                HighlightField(txtEngineNumber);
                isValid = false;
            }

            if (!isValid)
            {
                MessageBox.Show(errorMessage.ToString(), "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return isValid;
        }

        private void HighlightField(TextBox textBox)
        {
            textBox.BorderBrush = Brushes.Red;
            textBox.BorderThickness = new Thickness(2);
            textBox.Background = new SolidColorBrush(Color.FromArgb(20, 255, 0, 0));
        }

        private void ResetFieldStyles()
        {
            txtPlateNumber.BorderBrush = SystemColors.ControlDarkBrush;
            txtBrand.BorderBrush = SystemColors.ControlDarkBrush;
            txtModel.BorderBrush = SystemColors.ControlDarkBrush;
            txtManufactureYear.BorderBrush = SystemColors.ControlDarkBrush;
            txtEngineNumber.BorderBrush = SystemColors.ControlDarkBrush;

            txtPlateNumber.BorderThickness = new Thickness(1);
            txtBrand.BorderThickness = new Thickness(1);
            txtModel.BorderThickness = new Thickness(1);
            txtManufactureYear.BorderThickness = new Thickness(1);
            txtEngineNumber.BorderThickness = new Thickness(1);

            txtPlateNumber.Background = SystemColors.WindowBrush;
            txtBrand.Background = SystemColors.WindowBrush;
            txtModel.Background = SystemColors.WindowBrush;
            txtManufactureYear.Background = SystemColors.WindowBrush;
            txtEngineNumber.Background = SystemColors.WindowBrush;
        }

        private void RegisterVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
            {
                return;
            }

            string plateNumber = this.txtPlateNumber.Text.Trim();
            string brand = this.txtBrand.Text.Trim();
            string model = this.txtModel.Text.Trim();
            int manufactureYear = int.Parse(this.txtManufactureYear.Text);
            string engineNumber = this.txtEngineNumber.Text.Trim();

            try
            {
                if (_isEditMode)
                {
                    // Update existing vehicle
                    _selectedVehicle.Brand = brand;
                    _selectedVehicle.Model = model;
                    _selectedVehicle.ManufactureYear = manufactureYear;
                    _selectedVehicle.EngineNumber = engineNumber;

                    var operations = new Dictionary<string, Func<bool>>
                    {
                        { "Update vehicle", () => _vehicleDAO.Update(_selectedVehicle) }
                    };

                    Log logEntry = new Log
                    {
                        UserId = UserContext.Current.UserId,
                        Action = $"Updated vehicle with plate number {plateNumber}",
                        Timestamp = DateTime.Now
                    };

                    string successMessage = "Vehicle updated successfully!";
                    string errorMessage = "Failed to update vehicle!";

                    bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, null, successMessage, errorMessage);

                    if (result)
                    {
                        // Switch to the vehicles list tab
                        MainTabControl.SelectedIndex = 1;

                        // Refresh data
                        LoadUserVehicles();

                        // Reset form for future use
                        ResetForm();
                    }
                }
                else
                {
                    // Register new vehicle
                    Vehicle vehicle = new Vehicle
                    {
                        PlateNumber = plateNumber,
                        Brand = brand,
                        Model = model,
                        ManufactureYear = manufactureYear,
                        EngineNumber = engineNumber,
                        OwnerId = UserContext.Current.UserId
                    };

                    var operations = new Dictionary<string, Func<bool>>
                    {
                        { "Register new vehicle", () => _vehicleDAO.Add(vehicle) }
                    };

                    Log logEntry = new Log
                    {
                        UserId = UserContext.Current.UserId,
                        Action = $"Register new vehicle with plate number {plateNumber}",
                        Timestamp = DateTime.Now
                    };

                    string successMessage = "Vehicle registered successfully!";
                    string errorMessage = "Failed to register vehicle!";

                    bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, null, successMessage, errorMessage);

                    if (result)
                    {
                        // Switch to the vehicles list tab
                        MainTabControl.SelectedIndex = 1;

                        // Refresh data
                        LoadUserVehicles();

                        // Clear the form fields
                        ClearFields();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFields()
        {
            txtPlateNumber.Clear();
            txtPlateNumber.IsEnabled = true;
            txtBrand.Clear();
            txtModel.Clear();
            txtManufactureYear.Clear();
            txtEngineNumber.Clear();
            _isEditMode = false;
            _selectedVehicle = null;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            // Clear all fields and reset to Add mode
            ClearFields();

            // Reset button text
            Button registerBtn = this.FindName("RegisterVehicleButton_Click") as Button;
            if (registerBtn != null)
                registerBtn.Content = "Register";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Add input validation for Manufacture Year to only accept numbers
        private void TxtManufactureYear_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text[0]);
        }

        private void EditVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            _selectedVehicle = (Vehicle)button.CommandParameter;

            if (_selectedVehicle != null)
            {
                // Switch to edit mode
                _isEditMode = true;

                // Populate form with vehicle data
                txtPlateNumber.Text = _selectedVehicle.PlateNumber;
                txtPlateNumber.IsEnabled = false;  // Don't allow changing plate number
                txtBrand.Text = _selectedVehicle.Brand;
                txtModel.Text = _selectedVehicle.Model;
                txtManufactureYear.Text = _selectedVehicle.ManufactureYear.ToString();
                txtEngineNumber.Text = _selectedVehicle.EngineNumber;

                // Switch to register tab to edit
                MainTabControl.SelectedIndex = 0;

                // Change button text to indicate editing
                // Since we can't directly access the button from XAML by name,
                // we'll rely on the logic in RegisterVehicleButton_Click to handle
                // both add and edit cases.
            }
        }

        private void DeleteVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            Vehicle selectedVehicle = (Vehicle)((Button)sender).CommandParameter;

            if (selectedVehicle != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the vehicle with plate number {selectedVehicle.PlateNumber}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var operations = new Dictionary<string, Func<bool>>
                        {
                            { "Delete vehicle", () => _vehicleDAO.Delete(selectedVehicle.VehicleId) }
                        };

                        Log logEntry = new Log
                        {
                            UserId = UserContext.Current.UserId,
                            Action = $"Deleted vehicle with plate number {selectedVehicle.PlateNumber}",
                            Timestamp = DateTime.Now
                        };

                        string successMessage = "Vehicle deleted successfully!";
                        string errorMessage = "Failed to delete vehicle. It might be referenced by other records.";

                        bool deleteResult = TransactionHelper.ExecuteTransaction(operations, logEntry, null, successMessage, errorMessage);

                        if (deleteResult)
                        {
                            // Refresh the list
                            LoadUserVehicles();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting vehicle: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
