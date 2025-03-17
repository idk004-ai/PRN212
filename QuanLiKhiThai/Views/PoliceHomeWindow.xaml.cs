using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System.Windows;

namespace QuanLiKhiThai.Views
{
    public partial class PoliceHomeWindow : Window
    {
        private readonly INavigationService _navigationService;

        public PoliceHomeWindow(INavigationService navigationService)
        {
            InitializeComponent();
            _navigationService = navigationService;

            // Update user info
            lblWelcome.Content = $"Welcome, {UserContext.Current.FullName}";
        }

        private void btnVehicleLookup_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<VehicleLookupWindow>();
        }

        private void btnViolationRecord_Click(object sender, RoutedEventArgs e)
        {
            //_navigationService.NavigateTo<ViolationRecordWindow>();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Do you want logout?",
                                                     "Confirm",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _navigationService.NavigateTo<Login>();
                this.Close();
            }
        }
    }
}
