using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace QuanLiKhiThai.Helper
{
    public class AppointmentStatusToCancelButtonVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                // Only show Cancel button when appointment's status is "Pending" and "Assigned"
                if (status == Constants.STATUS_PENDING || status == Constants.STATUS_ASSIGNED)
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
