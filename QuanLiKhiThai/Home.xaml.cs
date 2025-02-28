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
            // Chuyển hướng đến trang History Test Result
            VehicleHistory historyTestResult = new VehicleHistory();
            historyTestResult.Show();
            this.Close();
        }

        private void RegisterVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển hướng đến trang Register Vehicle
            RegisterVehicle registerVehicle = new RegisterVehicle();
            registerVehicle.Show();
            this.Close();
        }

        private void ScheduleTestButton_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển hướng đến trang Schedule a Test
            //ScheduleTest scheduleTest = new ScheduleTest();
            //scheduleTest.Show();
            this.Close();
        }
    }
}
