using System.Windows;

namespace QuanLiKhiThai.Views
{
    public partial class StationDateEditWindow : Window
    {
        public DateTime? SelectedDate { get; private set; }

        public StationDateEditWindow(DateTime initialDate)
        {
            InitializeComponent();

            datePicker.SelectedDate = initialDate.Date;

            for (int i = 0; i < 24; i++)
            {
                hourComboBox.Items.Add(i.ToString("00"));
            }
            hourComboBox.SelectedIndex = initialDate.Hour;

            for (int i = 0; i < 60; i += 5)
            {
                minuteComboBox.Items.Add(i.ToString("00"));
            }
            minuteComboBox.SelectedIndex = initialDate.Minute / 5;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (datePicker.SelectedDate.HasValue)
            {
                DateTime date = datePicker.SelectedDate.Value;
                int hour = int.Parse((string)hourComboBox.SelectedItem);
                int minute = int.Parse((string)minuteComboBox.SelectedItem);

                SelectedDate = new DateTime(
                    date.Year, date.Month, date.Day,
                    hour, minute, 0
                );

                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a valid date.", "Invalid Date",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.Close();
            }
        }
    }
}
