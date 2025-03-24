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

namespace QuanLiKhiThai.Views
{
    /// <summary>
    /// Interaction logic for CancellationReasonWindow.xaml
    /// </summary>
    public partial class CancellationReasonWindow : Window
    {
        public string CancellationReason { get; private set; } = string.Empty;

        public CancellationReasonWindow(string vehiclePlateNumber)
        {
            InitializeComponent();
            Title = $"Cancel Inspection - {vehiclePlateNumber}";
            lblVehicleInfo.Text = $"Vehicle: {vehiclePlateNumber}";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate reason
            if (string.IsNullOrWhiteSpace(txtCancellationReason.Text))
            {
                MessageBox.Show("Please enter a reason for cancellation.",
                    "Reason Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CancellationReason = txtCancellationReason.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogResult = false;
            }
            catch (InvalidOperationException)
            {
            }
            finally
            {
                this.Close();
            }
        }
    }
}
