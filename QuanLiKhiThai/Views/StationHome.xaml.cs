using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using QuanLiKhiThai.Views;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for StationHome.xaml
    /// </summary>
    public partial class StationHome : Window
    {
        private readonly INavigationService _navigationService;

        public StationHome(INavigationService navigationService)
        {
            this._navigationService = navigationService;
            InitializeComponent();
        }

        private void VehicleListButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Vehicle List window (vehicles needing inspection)
            _navigationService.NavigateTo<NeededCheckVehicleList>();
        }

        private void InspectorManagementButton_Click(object sender, RoutedEventArgs e)
        {
            int stationId = UserContext.Current.UserId;
            _navigationService.NavigateTo<InspectorManagementWindow, int>(stationId);
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo<ReportWindow>();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.CloseAllExcept(typeof(LogsMonitorWindow));
            _navigationService.NavigateTo<Login>();
            this.Close();
        }
    }
}
