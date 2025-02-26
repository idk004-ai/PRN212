using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO;
using QuanLiKhiThai.Models;
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
    /// Interaction logic for RegisterVehicle.xaml
    /// </summary>
    public partial class RegisterVehicle : Window
    {
        public RegisterVehicle()
        {
            InitializeComponent();
        }


        private void RegisterVehicleButton_Click(object sender, RoutedEventArgs e)
        {
            string plateNumber = this.txtPlateNumber.Text;
            string brand = this.txtBrand.Text;
            string model = this.txtModel.Text;
            int manufactureYear = int.Parse(this.txtManufactureYear.Text);
            string engineNumber = this.txtEngineNumber.Text;

            Vehicle vehicle = new Vehicle
            {
                PlateNumber = plateNumber,
                Brand = brand,
                Model = model,
                ManufactureYear = manufactureYear,
                EngineNumber = engineNumber,
                OwnerId = UserContext.Current.UserId
            };

            if (VehicleDAO.AddVehicle(vehicle))
            {
                MessageBox.Show("Vehicle registered successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            else
            {
                MessageBox.Show("Vehicle registration failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
