using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Models;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        void LoadDataGridUser()
        {
            List<User> users = UserDAO.GetUsers();
            //this.dataGridUser.ItemsSource = users;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = this.txtFullName.Text;
            string email = this.txtEmail.Text;
            string password = this.txtPassword.Password;
            string confirmPassword = this.txtConfirmPassword.Password;
            string phoneNumber = this.txtPhone.Text;
            string address = this.txtAddress.Text;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!password.Equals(confirmPassword))
            {
                MessageBox.Show("Password and Confirm Password do not match", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User user = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Phone = phoneNumber,
                Address = address,
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
    }
}