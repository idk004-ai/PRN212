using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Views;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class OwnerHome : Window
    {
        private readonly INavigationService _navigationService;

        public OwnerHome(INavigationService navigationService)
        {
            this._navigationService = navigationService;
            InitializeComponent();
        }

        private void HistoryTestResultButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Vehicle History window
            _navigationService.NavigateTo<VehicleHistory>();
        }

        private void RegisterVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Register Vehicle window
            _navigationService.NavigateTo<RegisterVehicle>();
        }

        private void ScheduleTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Schedule Test window
            _navigationService.NavigateTo<ScheduleTestWindow>();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<Login>();
            this.Close();
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<EditProfileWindow>();
        }
    }
}
