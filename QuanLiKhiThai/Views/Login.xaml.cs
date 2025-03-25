using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.Views;
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
        private readonly IStationInspectorDAO _stationInspectorDAO;

        public Login(IUserDAO userDAO, INavigationService navigationService, IStationInspectorDAO stationInspectorDAO)
        {
            this._userDAO = userDAO;
            this._navigationService = navigationService;
            this._stationInspectorDAO = stationInspectorDAO;
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

            if (!user.IsEnabled)
            {
                MessageBox.Show("Your account is not verified. Please check mail to verify account",
                    "Unverified Account",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
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
                    StationInspector? stationInspector = _stationInspectorDAO.GetByInspectorId(user.UserId);
                    if (stationInspector == null)
                    {
                        MessageBox.Show("Inspector is not assigned to any station", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    if (stationInspector.IsActive == false)
                    {
                        MessageBox.Show("Inspector is disabled", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    _navigationService.NavigateTo<InspectorVehicleListWindow>();
                    this.Close();
                    break;
                case Constants.Police:
                    _navigationService.NavigateTo<VehicleLookupWindow>();
                    this.Close();
                    break;
                default:
                    MessageBox.Show($"Unsupported role: {user.Role}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void RegisterTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo<MainWindow>();
            this.Close();
        }

        private void VerifyAccount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo<VerificationWindow>();
            this.Close();
        }
    }
}
