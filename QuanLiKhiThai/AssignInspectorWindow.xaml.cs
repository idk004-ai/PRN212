using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Models;
using QuanLiKhiThai.ViewModel;
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
    /// Interaction logic for AssignInspectorWindow.xaml
    /// </summary>
    public partial class AssignInspectorWindow : Window
    {
        private VehicleCheckViewModel _vehicleViewModel;
        public AssignInspectorWindow(VehicleCheckViewModel vehicleViewModel)
        {
            InitializeComponent();
            this._vehicleViewModel = vehicleViewModel;
            LoadData();
        }

        private void LoadData()
        {
            List<User> inspectors = UserDAO.GetUserByRole(Constants.Inspector);
            this.comboBoxInspectors.ItemsSource = inspectors;
            this.comboBoxInspectors.DisplayMemberPath = "FullName";
            this.comboBoxInspectors.SelectedValuePath = "Id";
        }

        private void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.comboBoxInspectors.SelectedItem is User selectedInspector)
            {
                // TODO: Assign inspector to vehicle


                MessageBox.Show($"Inspector {selectedInspector.FullName} has been assigned to vehicle {_vehicleViewModel.PlateNumber}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select an inspector", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
