using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Login logic
            string email = this.txtEmail.Text;
            string password = this.txtPassword.Password;


            User? user = UserDAO.GetUserByEmail(email);
            if (user == null || !password.Equals(user.Password))
            {
                MessageBox.Show("Email or Password is invalid", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save user info to UserContext
            UserContext.Current.UserId = user.UserId;
            UserContext.Current.FullName = user.FullName;
            UserContext.Current.Email = user.Email;
            UserContext.Current.Role = user.Role;


            // redirect base on the user's role
            switch (user.Role)
            {
                case Constants.Owner:
                    Home home = new Home();
                    home.Show();
                    this.Close();
                    break;
                case Constants.Station:
                    NeededCheckVehicleList neededCheckVehicleList = new NeededCheckVehicleList();
                    neededCheckVehicleList.Show();
                    this.Close();
                    break;
                case Constants.Inspector:
                    InspectorVehicleListWindow inspectorVehicleList = new InspectorVehicleListWindow();
                    inspectorVehicleList.Show();
                    this.Close();
                    break;
            }
        }

        private void RegisterTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow registerWindow = new MainWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}
