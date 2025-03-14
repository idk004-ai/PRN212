using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
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
        private readonly IUserDAO _userDAO;
        private readonly INavigationService _navigationService;

        public Login(IUserDAO userDAO, INavigationService navigationService)
        {
            this._userDAO = userDAO;
            this._navigationService = navigationService;
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Login logic
            string email = this.txtEmail.Text;
            string password = this.txtPassword.Password;


            User? user = _userDAO.GetUserByEmail(email);
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

            switch (user.Role)
            {
                case Constants.Owner:
                    _navigationService.NavigateTo<OwnerHome>();
                    this.Close();
                    break;
                case Constants.Station:
                    _navigationService.NavigateTo<StationHome>();
                    this.Close();
                    break;
                case Constants.Inspector:
                    _navigationService.NavigateTo<InspectorVehicleListWindow>();
                    this.Close();
                    break;
            }
        }

        private void RegisterTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo<MainWindow>();
            this.Close();
        }
    }
}
