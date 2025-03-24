using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for AddInspectorWindow.xaml
    /// </summary>
    public partial class AddInspectorWindow : Window
    {
        private readonly IUserDAO _userDAO;
        private readonly IStationInspectorDAO _stationInspectorDAO;
        private readonly int _stationId;
        private User? _existingUser;
        private StationInspector? _existingStationInspector;
        private bool _isEditMode = false;

        // Flag to determine if changes have been saved
        public bool IsSaved { get; private set; } = false;

        /// <summary>
        /// Constructor for creating a new inspector
        /// </summary>
        public AddInspectorWindow(IUserDAO userDAO, IStationInspectorDAO stationInspectorDAO, int stationId)
        {
            InitializeComponent();

            _userDAO = userDAO;
            _stationInspectorDAO = stationInspectorDAO;
            _stationId = stationId;

            // Set default date to today
            dpAssignedDate.SelectedDate = DateTime.Today;

            Title = "Add New Inspector";
            btnSave.Content = "Create Inspector";
        }

        /// <summary>
        /// Constructor for editing an existing inspector
        /// </summary>
        public AddInspectorWindow(IUserDAO userDAO, IStationInspectorDAO stationInspectorDAO, int stationId, int inspectorId)
        {
            InitializeComponent();

            _userDAO = userDAO;
            _stationInspectorDAO = stationInspectorDAO;
            _stationId = stationId;
            _isEditMode = true;

            // Load existing user and station inspector data
            _existingUser = _userDAO.GetById(inspectorId);
            _existingStationInspector = _stationInspectorDAO.GetByStationAndInspectorId(stationId, inspectorId);

            if (_existingUser != null && _existingStationInspector != null)
            {
                // Populate fields with existing data
                txtFullName.Text = _existingUser.FullName;
                txtEmail.Text = _existingUser.Email;
                txtPhone.Text = _existingUser.Phone;
                txtAddress.Text = _existingUser.Address;
                dpAssignedDate.SelectedDate = _existingStationInspector.AssignedDate ?? DateTime.Today;
                txtNotes.Text = _existingStationInspector.Notes;

                // In edit mode, password fields should be hidden or optional
                passwordSection.Visibility = Visibility.Collapsed;

                // Update window title and button text
                Title = "Edit Inspector";
                btnSave.Content = "Update Inspector";
            }
            else
            {
                MessageBox.Show("Inspector data could not be loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        #region Event Handlers

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
                UpdateExistingInspector();
            else
                CreateNewInspector();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogResult = false;
            }
            catch (InvalidOperationException)
            {
            }
            finally
            {
                this.Close();
            }
        }


        #endregion

        #region Private Methods

        private void UpdateExistingInspector()
        {
            // Validate user input
            if (!ValidateUserInputForEdit())
                return;

            // Validate assignment details
            if (!ValidateAssignmentDetails())
                return;

            // Update existing user and stationinspector
            if (_existingUser != null && _existingStationInspector != null)
            {
                _existingUser.FullName = txtFullName.Text.Trim();
                _existingUser.Email = txtEmail.Text.Trim();
                _existingUser.Phone = txtPhone.Text.Trim();
                _existingUser.Address = txtAddress.Text.Trim();

                _existingStationInspector.AssignedDate = dpAssignedDate.SelectedDate;
                _existingStationInspector.Notes = txtNotes.Text?.Trim();

                var operations = new Dictionary<string, Func<bool>>
                {
                    { "Update inspector user", () => _userDAO.Update(_existingUser) },
                    { "Update station inspector", () => _stationInspectorDAO.Update(_existingStationInspector) }
                };

                Log logEntry = new Log
                {
                    UserId = UserContext.Current?.UserId ?? Constants.SYSTEM_USER_ID,
                    Action = $"Updated inspector '{_existingUser.FullName}' (ID: {_existingUser.UserId}) information",
                    Timestamp = DateTime.Now
                };

                Notification notification = new Notification
                {
                    UserId = _stationId,
                    Message = $"Inspector '{_existingUser.FullName}' information has been updated",
                    SentDate = DateTime.Now,
                    IsRead = false
                };

                string successMessage = "Inspector information has been successfully updated.";
                string errorMessage = "Failed to update inspector information";

                bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, notification, successMessage, errorMessage);

                if (result)
                {
                    IsSaved = true;
                    try
                    {
                        this.DialogResult = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Ignore if window was not opened as a dialog
                    }
                    this.Close();
                }
            }
        }

        private void CreateNewInspector()
        {
            // Validate user input
            if (!ValidateUserInput())
                return;

            // Validate assignment details
            if (!ValidateAssignmentDetails())
                return;

            // Create new user with Inspector role
            var newUser = new User
            {
                FullName = txtFullName.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Password = txtPassword.Password, // In a real application, hash this password
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Role = Constants.Inspector
            };


            var assignedDate = dpAssignedDate.SelectedDate;
            var notes = txtNotes.Text?.Trim();
            var stationId = _stationId;


            var operations = new Dictionary<string, Func<bool>>
            {
                { "Create inspector user", () =>
                {
                    bool success = _userDAO.Add(newUser);
                    if (!success) return false;

                    var createdUser = _userDAO.GetUserByEmail(newUser.Email);
                    if (createdUser == null) return false;

                    // Create StationInspector
                    var stationInspector = new StationInspector
                    {
                        StationId = stationId,
                        InspectorId = createdUser.UserId,
                        AssignedDate = assignedDate,
                        IsActive = true,
                        Notes = notes
                    };
                    return _stationInspectorDAO.Add(stationInspector);
                }}
            };

            Log logEntry = new Log
            {
                UserId = UserContext.Current?.UserId ?? Constants.SYSTEM_USER_ID,
                Action = $"Created new inspector '{newUser.FullName}' and assigned to station ID {_stationId}",
                Timestamp = DateTime.Now
            };

            Notification notification = new Notification
            {
                UserId = _stationId, // ID của station sẽ nhận thông báo
                Message = $"New inspector '{newUser.FullName}' has been assigned to your station",
                SentDate = DateTime.Now,
                IsRead = false
            };

            string successMessage = $"New inspector '{newUser.FullName}' has been successfully created and assigned to this station.";
            string errorMessage = "Failed to create inspector";

            bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, notification, successMessage, errorMessage);

            if (result)
            {
                IsSaved = true;
                try
                {
                    this.DialogResult = true;
                }
                catch (InvalidOperationException)
                {
                    // do not do anything
                }
                this.Close();
            }
        }

        private bool ValidateUserInput()
        {
            // Check for empty fields
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter a full name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter an email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Please enter a password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters long.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Password and confirm password do not match.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtConfirmPassword.Focus();
                return false;
            }

            // Validate email format
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Check if email is already in use
            var existingUser = _userDAO.GetUserByEmail(txtEmail.Text.Trim());
            if (existingUser != null)
            {
                MessageBox.Show("This email address is already in use. Please use a different email.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please enter a phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateUserInputForEdit()
        {
            // Check for empty fields
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Please enter a full name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Please enter an email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Validate email format
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            // Check if email is already in use (but not by this user)
            var existingUser = _userDAO.GetUserByEmail(txtEmail.Text.Trim());
            if (existingUser != null && existingUser.UserId != _existingUser?.UserId)
            {
                MessageBox.Show("This email address is already in use by another user. Please use a different email.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Please enter a phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhone.Focus();
                return false;
            }

            return true;
        }

        private bool ValidateAssignmentDetails()
        {
            if (!dpAssignedDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Please select an assigned date.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpAssignedDate.Focus();
                return false;
            }

            if (dpAssignedDate.SelectedDate.Value > DateTime.Now)
            {
                if (MessageBox.Show("The assigned date is in the future. Are you sure you want to continue?",
                    "Confirm Date", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    dpAssignedDate.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            // Email validation using regex
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        #endregion
    }
}
