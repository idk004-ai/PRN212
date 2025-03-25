using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // create a tuple to store the user's information
        private (string FullName, string Email, string Password, string ConfirmPassword, string Phone, string Address, string Role) userInfo;
        private readonly IUserDAO _userDAO;
        private readonly LogsViewManager _logsViewManager;
        private readonly INavigationService _navigationService;
        private readonly EmailService _emailService;

        public MainWindow(IUserDAO userDAO, LogsViewManager logsViewManager, INavigationService navigationService, EmailService emailService)
        {
            InitializeComponent();
            _userDAO = userDAO;
            _logsViewManager = logsViewManager;
            _navigationService = navigationService;
            _emailService = emailService;

            // Set default role
            cmbRole.SelectedIndex = 0;
        }

        private bool HasAnyEmptyField()
        {
            bool hasEmpty = string.IsNullOrEmpty(userInfo.FullName) ||
                string.IsNullOrEmpty(userInfo.Email) ||
                string.IsNullOrEmpty(userInfo.Password) ||
                string.IsNullOrEmpty(userInfo.ConfirmPassword) ||
                string.IsNullOrEmpty(userInfo.Phone) ||
                string.IsNullOrEmpty(userInfo.Address);

            // Check for role-specific required fields
            if (cmbRole.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Tag.ToString();

                if (selectedRole == Constants.Station && string.IsNullOrEmpty(txtStationCode.Text))
                {
                    return true;
                }
                else if (selectedRole == Constants.Police &&
                         (string.IsNullOrEmpty(txtBadgeNumber.Text) ||
                          string.IsNullOrEmpty(txtDepartment.Text)))
                {
                    return true;
                }
            }

            return hasEmpty;
        }

        private void cmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbRole.SelectedItem is ComboBoxItem selectedItem)
            {
                string selectedRole = selectedItem.Tag.ToString();

                // Show/hide additional fields based on the selected role
                panelStationInfo.Visibility = selectedRole == Constants.Station ? Visibility.Visible : Visibility.Collapsed;
                panelPoliceInfo.Visibility = selectedRole == Constants.Police ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedRole = Constants.Owner;
            if (cmbRole.SelectedItem is ComboBoxItem selectedItem)
            {
                selectedRole = selectedItem.Tag.ToString();
            }

            userInfo = (
                FullName: this.txtFullName.Text,
                Email: this.txtEmail.Text,
                Password: this.txtPassword.Password,
                ConfirmPassword: this.txtConfirmPassword.Password,
                Phone: this.txtPhone.Text,
                Address: this.txtAddress.Text,
                Role: selectedRole ?? Constants.Owner
            );

            if (HasAnyEmptyField())
            {
                MessageBox.Show("Please fill in all required fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!userInfo.Password.Equals(userInfo.ConfirmPassword))
            {
                MessageBox.Show("Password and Confirm Password do not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_userDAO.GetUserByEmail(userInfo.Email) != null)
            {
                MessageBox.Show("Email already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create verification token
            string verificationToken = Guid.NewGuid().ToString("N");

            User user = new User
            {
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                Password = userInfo.Password,
                Phone = userInfo.Phone,
                Address = userInfo.Address,
                Role = userInfo.Role,
                IsEnabled = false, 
                VerificationToken = verificationToken,
                TokenExpiry = DateTime.Now.AddHours(24)
            };

            var operations = new Dictionary<string, Func<bool>>
            {
                { "Register new user", () => _userDAO.Add(user) }
            };

            Log logEntry = new Log
            {
                UserId = Constants.SYSTEM_USER_ID, // System id
                Action = $"New {selectedRole} user registered: {user.Email}",
                Timestamp = DateTime.Now,
            };

            bool success = TransactionHelper.ExecuteTransaction(
                operations,
                logEntry,
                notification: null,
                successMessage: $"Registration successful. Please check your email {user.Email} to verify your account.",
                errorMessage: "Registration failed");

            if (success)
            {
                // Send verification email
                await SendVerificationEmail(user, verificationToken);

                // Navigate to login
                _navigationService.NavigateTo<Login>();
                this.Close();
            }
        }




        private async Task SendVerificationEmail(User user, string token)
        {
            try
            {
                string emailSubject = "Account Verification - Emission Control System";
                string emailBody = $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0;'>
        <h2 style='color: #4a6da7;'>Welcome to the Emission Control System</h2>
        <p>Dear {user.FullName},</p>
        <p>Thank you for registering as a <strong>{user.Role}</strong> in our system.</p>
        <p>To verify your account, please use the verification code below:</p>
        
        <div style='background-color: #f8f8f8; padding: 15px; border-left: 4px solid #4a6da7; margin: 20px 0;'>
            <p>Email: <strong>{user.Email}</strong></p>
            <p>Verification Code: <strong style='background: #ffffcc; padding: 5px; font-size: 18px; letter-spacing: 2px;'>{token}</strong></p>
        </div>
        
        <p>To verify your account:</p>
        <ol>
            <li>Open the Emission Control System application</li>
            <li>Click on 'Already have an account? Login'</li>
            <li>Click on the 'Verify Account' link</li>
            <li>Enter your email and verification code in the form</li>
        </ol>
        
        <p>This verification code will expire in 24 hours.</p>
        <p>If you did not create this account, please ignore this email.</p>
        <hr style='border-top: 1px solid #e0e0e0; margin: 20px 0;'>
        <p style='font-size: 12px; color: #666;'>This is an automated message, please do not reply.</p>
    </div>
</body>
</html>";

                var result = await _emailService.SendEmailAsync(
                    user.Email,
                    user.FullName,
                    emailSubject,
                    emailBody,
                    isBodyHtml: true
                );

                if (!result.Success)
                {
                    MessageBox.Show($"Unable to send verification email. Please contact the administrator.\nError: {result.ErrorMessage}",
                        "Email Sending Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while sending verification email: {ex.Message}",
                    "Email Sending Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void LoginTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo<Login>();
            this.Close();
        }

        private void LogsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _logsViewManager.ShowLogsWindow();
        }
    }
}