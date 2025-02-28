using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Models;
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


        void LoadDataGridUser()
        {
            List<User> users = UserDAO.GetUsers();
        }

        public MainWindow()
        {
            InitializeComponent();
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

            if (UserDAO.GetUserByEmail(userInfo.Email) != null)
            {
                MessageBox.Show("Email already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = new User
            {
                FullName = userInfo.FullName,
                Email = userInfo.Email,
                Password = userInfo.Password,
                Phone = userInfo.ConfirmPassword,
                Address = userInfo.Address,
                Role = Constants.Owner
            };

            if (UserDAO.AddUser(user))
            {
                MessageBox.Show("Register successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Set current user
                UserContext.Current.UserId = user.UserId;
                UserContext.Current.FullName = user.FullName;
                UserContext.Current.Email = user.Email;
                UserContext.Current.Role = user.Role;

                // Open register vehicle window
                RegisterVehicle registerVehicle = new RegisterVehicle();
                registerVehicle.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Register failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoginTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
    }
}