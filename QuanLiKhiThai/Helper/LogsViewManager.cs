using QuanLiKhiThai.Context;
using System;
using System.Windows;

namespace QuanLiKhiThai.Helper
{
    public class LogsViewManager
    {
        private static LogsViewManager _instance;
        private static LogsMonitorWindow _logsWindow;
        private static bool _autoShow = true;  // Mặc định là mở tự động

        public static LogsViewManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogsViewManager();
                }
                return _instance;
            }
        }

        // Cấu hình có mở tự động khi khởi động không
        public bool AutoShowOnStartup
        {
            get { return _autoShow; }
            set { _autoShow = value; }
        }

        // Kiểm tra xem người dùng hiện tại có quyền mở logs không
        public bool CanOpenLogsWindow()
        {
            // Hiện tại: Cho phép tất cả người dùng
            // Sau này: Chỉ cho phép Admin
            // return UserContext.Current?.Role == Constants.Admin;
            return true;
        }

        // Hiển thị cửa sổ logs
        public void ShowLogsWindow(bool forceShow = false)
        {
            // Nếu không phải mở bắt buộc và người dùng không có quyền => không mở
            if (!forceShow && !CanOpenLogsWindow())
            {
                MessageBox.Show("You don't have permission to access the logs.",
                    "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_logsWindow == null || !_logsWindow.IsLoaded)
                {
                    _logsWindow = new LogsMonitorWindow();
                    _logsWindow.Closed += (s, e) => _logsWindow = null;
                    _logsWindow.Show();
                }
                else
                {
                    _logsWindow.Activate(); // Đưa cửa sổ lên trước nếu đã mở
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening logs window: {ex.Message}");
                MessageBox.Show($"Error opening logs window: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Đóng cửa sổ logs nếu đang mở
        public void CloseLogsWindow()
        {
            if (_logsWindow != null && _logsWindow.IsLoaded)
            {
                _logsWindow.Close();
                _logsWindow = null;
            }
        }
    }
}
