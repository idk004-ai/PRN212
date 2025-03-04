using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
        }

        private void HistoryTestResultButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Vehicle History window
            VehicleHistory historyTestResult = new VehicleHistory();
            historyTestResult.Show();
        }

        private void RegisterVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Register Vehicle window
            RegisterVehicle registerVehicle = new RegisterVehicle();
            registerVehicle.Show();
        }

        private void ScheduleTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Redirect to Schedule Test window
            ScheduleTestWindow scheduleTest = new ScheduleTestWindow();
            scheduleTest.Show();
        }
    }
}
