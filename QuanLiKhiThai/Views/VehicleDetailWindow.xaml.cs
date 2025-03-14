﻿using QuanLiKhiThai.DAO;
using QuanLiKhiThai.DAO.Interface;
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
    /// Interaction logic for VehicleDetailWindow.xaml
    /// </summary>
    public partial class VehicleDetailWindow : Window
    {
        private VehicleCheckViewModel _viewModel;
        private IVehicleDAO _vehicleDAO;
        public VehicleDetailWindow(VehicleCheckViewModel viewModel, IVehicleDAO vehicleDAO)
        {
            InitializeComponent();
            this._viewModel = viewModel;
            this._vehicleDAO = vehicleDAO;
            LoadData();
        }

        private void LoadData()
        {
            var vehicle = _vehicleDAO.GetByPlateNumber(_viewModel.PlateNumber);
            if (vehicle == null)
            {
                MessageBox.Show("Vehicle not found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }

            // TODO: Show Owner info

            this.DataContext = vehicle;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
