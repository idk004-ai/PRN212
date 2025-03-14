using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using System.Windows;
using System.Windows.Input;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // create a tuple to store the user's information
        private (string FullName, string Email, string Password, string ConfirmPassword, string Phone, string Address) userInfo;
        private readonly IUserDAO _userDAO;
        private readonly LogsViewManager _logsViewManager;
        private readonly INavigationService _navigationService;


        public MainWindow(IUserDAO userDAO, LogsViewManager logsViewManager, INavigationService navigationService)
        {
            InitializeComponent();
            _userDAO = userDAO;
            _logsViewManager = logsViewManager;
            _navigationService = navigationService;
            LoadDataGridUser();
        }

        void LoadDataGridUser()
        {
            List<User> users = _userDAO.GetAll().ToList();
        }

        private bool HasAnyEmptyField()
        {
            return string.IsNullOrEmpty(userInfo.FullName) ||
                string.IsNullOrEmpty(userInfo.Email) ||
                string.IsNullOrEmpty(userInfo.Password) ||
                string.IsNullOrEmpty(userInfo.ConfirmPassword) ||
                string.IsNullOrEmpty(userInfo.Phone) ||
                string.IsNullOrEmpty(userInfo.Address);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            userInfo = (
                FullName: this.txtFullName.Text,
                Email: this.txtEmail.Text,
                Password: this.txtPassword.Password,
                ConfirmPassword: this.txtConfirmPassword.Password,
                Phone: this.txtPhone.Text,
                Address: this.txtAddress.Text
            );

            if (HasAnyEmptyField())
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            User user = new User
            {
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                Password = userInfo.Password,
                Phone = userInfo.Phone,
                Address = userInfo.Address,
                Role = Constants.Owner
            };

            var operations = new Dictionary<string, Func<bool>>
            {
                { "Register new user", () => _userDAO.Add(user) }
            };

            Log logEntry = new Log
            {
                UserId = 1, // System id or Admin id
                Action = $"New user registered: {user.Email} with role {Constants.Owner}",
                Timestamp = DateTime.Now,
            };

            bool success = TransactionHelper.ExecuteTransaction(
                operations,
                logEntry,
                notification: null,
                successMessage: "Registration successful. Please log in with your new account",
                errorMessage: "Registration failed");

            if (success)
            {
                // Navigate to login
                _navigationService.NavigateTo<Login>();
                this.Close();
            }
        }

        private void LoginTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo<Login>();
            this.Close();
        }

        private void LogsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Mở logs window (sẽ kiểm tra quyền)
            _logsViewManager.ShowLogsWindow();
        }

    }
}