using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for RegisterVehicle.xaml
    /// </summary>
    public partial class RegisterVehicle : Window
    {
        private readonly IVehicleDAO _vehicleDAO;
        // Regular expression for Vietnamese license plates
        private readonly Regex plateNumberRegex = new Regex(@"^\d{2}[A-Z]-\d{4,5}$|^\d{2}[A-Z]\s\d{4,5}$");

        public RegisterVehicle(IVehicleDAO vehicleDAO)
        {
            InitializeComponent();
            this._vehicleDAO = vehicleDAO;
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
            else
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
                    // Clear the form fields after successful registration
                    ClearFields();
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
            txtBrand.Clear();
            txtModel.Clear();
            txtManufactureYear.Clear();
            txtEngineNumber.Clear();
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
    }
}
