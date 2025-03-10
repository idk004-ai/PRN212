using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLiKhiThai
{
    public partial class LogsMonitorWindow : Window
    {
        private readonly LogMonitorHelper _logMonitor;

        public LogsMonitorWindow()
        {
            _logMonitor = LogMonitorHelper.Instance;
            
            InitializeComponent();

            this.Loaded += async (s, e) =>
            {
                this.DataContext = _logMonitor;

                dpFromDate.SelectedDateChanged -= dpFromDate_SelectedDateChanged;
                dpFromDate.SelectedDate = DateTime.Now.AddDays(-1);
                dpFromDate.SelectedDateChanged += dpFromDate_SelectedDateChanged;

                // refresh 1 logs 1 time
                _logMonitor.SetTimeFilter(dpFromDate.SelectedDate.Value);
                await _logMonitor.RefreshLogsAsync(clearExistingLogs: true);

                if (!_logMonitor.IsMonitoring)
                {
                    _logMonitor.StartMonitoring();
                    btnToggleMonitoring.Content = "Stop Monitoring";
                }
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Dừng service khi đóng cửa sổ
            _logMonitor.StopMonitoring();
        }

        private void dpFromDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_logMonitor != null && dpFromDate.SelectedDate.HasValue)
            {
                _logMonitor.SetTimeFilter(dpFromDate.SelectedDate.Value);
                _logMonitor.RefreshLogsAsync();
            }
        }

        private void cmbRefreshInterval_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_logMonitor == null) return;

            ComboBoxItem selectedItem = cmbRefreshInterval.SelectedItem as ComboBoxItem;
            if (selectedItem != null && int.TryParse(selectedItem.Content.ToString(), out int interval))
            {
                _logMonitor.RefreshIntervalSeconds = interval;
            }
        }

        private void btnToggleMonitoring_Click(object sender, RoutedEventArgs e)
        {
            if (_logMonitor == null) return;
            if (_logMonitor.IsMonitoring)
            {
                _logMonitor.StopMonitoring();
                btnToggleMonitoring.Content = "Start Monitoring";
            }
            else
            {
                _logMonitor.StartMonitoring();
                btnToggleMonitoring.Content = "Stop Monitoring";
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PerformSearch();
            }
        }

        private async void PerformSearch()
        {
            if (_logMonitor == null) return;
            string searchTerm = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                await _logMonitor.RefreshLogsAsync(clearExistingLogs: true);
                return;
            }

            var logs = await Task.Run(() => LogDAO.SearchLogs(searchTerm));
            if (_logMonitor.RecentLogs != null)
            {
                _logMonitor.RecentLogs.Clear();

                foreach (var log in logs)
                {
                    _logMonitor.RecentLogs.Add(log);
                }
            }
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_logMonitor == null) return;
            await _logMonitor.RefreshLogsAsync();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (_logMonitor == null) return;
            _logMonitor.ClearLogs();
            txtSearch.Clear();
        }
    }
}
