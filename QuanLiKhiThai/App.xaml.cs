using QuanLiKhiThai.Helper;
using System.Configuration;
using System.Data;
using System.Windows;

namespace QuanLiKhiThai
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Dispatcher.BeginInvoke(new Action(() => {
                try
                {
                    if (LogsViewManager.Instance.AutoShowOnStartup)
                    {
                        LogsViewManager.Instance.ShowLogsWindow(forceShow: true);
                    }
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi nhưng đừng làm gián đoạn khởi động ứng dụng
                    System.Diagnostics.Debug.WriteLine($"Error showing logs window: {ex.Message}");
                }
            }));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            LogsViewManager.Instance.CloseLogsWindow();
            base.OnExit(e);
        }
    }

}
