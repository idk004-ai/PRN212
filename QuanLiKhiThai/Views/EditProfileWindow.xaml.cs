using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System;
using System.Text;
using System.Windows;

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for EditProfileWindow.xaml
    /// </summary>
    public partial class EditProfileWindow : Window
    {
        private readonly IUserDAO _userDAO;
        private User _currentUser;
        private bool _isLoading = false;

        public EditProfileWindow(IUserDAO userDAO)
        {
            InitializeComponent();
            this._userDAO = userDAO;

            // Register Loaded event for the window
            this.Loaded += EditProfileWindow_Loaded;
        }

        private void EditProfileWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                _isLoading = true;

                // Get current user information from UserContext
                int userId = UserContext.Current.UserId;
                _currentUser = _userDAO.GetById(userId);

                if (_currentUser != null)
                {
                    // Populate the form with user information
                    txtFullName.Text = _currentUser.FullName;
                    txtEmail.Text = _currentUser.Email;
                    txtPhone.Text = _currentUser.Phone;
                    txtAddress.Text = _currentUser.Address;

                    // Clear password fields
                    pwdCurrent.Clear();
                    pwdNew.Clear();
                    pwdConfirm.Clear();
                }
                else
                {
                    MessageBox.Show("Unable to load user information. Please try again later.",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errorMessage = new StringBuilder();
            bool isValid = true;

            // Check required fields
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                errorMessage.AppendLine("• Full name cannot be empty");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                errorMessage.AppendLine("• Phone number cannot be empty");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                errorMessage.AppendLine("• Address cannot be empty");
                isValid = false;
            }

            // Check password if user wants to change it
            if (!string.IsNullOrEmpty(pwdCurrent.Password) ||
                !string.IsNullOrEmpty(pwdNew.Password) ||
                !string.IsNullOrEmpty(pwdConfirm.Password))
            {
                // All three password fields must be filled
                if (string.IsNullOrEmpty(pwdCurrent.Password))
                {
                    errorMessage.AppendLine("• Current password cannot be empty");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(pwdNew.Password))
                {
                    errorMessage.AppendLine("• New password cannot be empty");
                    isValid = false;
                }

                if (string.IsNullOrEmpty(pwdConfirm.Password))
                {
                    errorMessage.AppendLine("• Confirm password cannot be empty");
                    isValid = false;
                }

                // Verify current password
                if (!string.IsNullOrEmpty(pwdCurrent.Password) &&
                    pwdCurrent.Password != _currentUser.Password)
                {
                    errorMessage.AppendLine("• Current password is incorrect");
                    isValid = false;
                }

                // Check if new password and confirm password match
                if (!string.IsNullOrEmpty(pwdNew.Password) &&
                    !string.IsNullOrEmpty(pwdConfirm.Password) &&
                    pwdNew.Password != pwdConfirm.Password)
                {
                    errorMessage.AppendLine("• New password and confirm password do not match");
                    isValid = false;
                }

                // Check new password length
                if (!string.IsNullOrEmpty(pwdNew.Password) && pwdNew.Password.Length < 6)
                {
                    errorMessage.AppendLine("• New password must be at least 6 characters long");
                    isValid = false;
                }
            }

            // Display error messages if any
            if (!isValid)
            {
                MessageBox.Show("Please fix the following errors:\n\n" + errorMessage.ToString(),
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            return isValid;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoading)
                return;

            if (!ValidateInput())
                return;

            try
            {
                // Update personal information
                _currentUser.FullName = txtFullName.Text.Trim();
                _currentUser.Phone = txtPhone.Text.Trim();
                _currentUser.Address = txtAddress.Text.Trim();

                // Update password if user wants to change it
                if (!string.IsNullOrEmpty(pwdNew.Password))
                {
                    _currentUser.Password = pwdNew.Password;
                }

                // Save changes to database
                var operations = new Dictionary<string, Func<bool>>
                {
                    { "update user", () => _userDAO.Update(_currentUser) }
                };

                // Create log entry for profile update
                Log logEntry = new Log
                {
                    UserId = _currentUser.UserId,
                    Action = "Updated personal information",
                    Timestamp = DateTime.Now
                };

                // Process transaction
                string successMessage = "Profile updated successfully!";
                string errorMessage = "Unable to update profile. Please try again later.";

                bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, null, successMessage, errorMessage);

                if (result)
                {
                    // Update UserContext if needed
                    UserContext.Current.FullName = _currentUser.FullName;

                    // Close window after successful update
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isLoading)
                return;

            LoadUserData();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Close window and return to main screen
            this.Close();
        }
    }
}
