using QuanLiKhiThai.DAO.Interface;
using System;
using System.Windows;
using System.Windows.Input;

namespace QuanLiKhiThai.Views
{
    public partial class VerificationWindow : Window
    {
        private readonly IUserDAO _userDAO;
        private readonly INavigationService _navigationService;

        public VerificationWindow(IUserDAO userDAO, INavigationService navigationService)
        {
            InitializeComponent();
            _userDAO = userDAO;
            _navigationService = navigationService;
        }

        private async void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string token = txtVerificationCode.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                txtStatus.Text = "Please enter both email and verification code.";
                txtStatus.Foreground = System.Windows.Media.Brushes.Red;
                txtStatus.Visibility = Visibility.Visible;
                return;
            }

            processingOverlay.Visibility = Visibility.Visible;

            try
            {
                // Verify account with a small delay to show processing UI
                await Task.Delay(800); 
                bool verificationResult = _userDAO.VerifyAccount(email, token);

                if (verificationResult)
                {
                    processingOverlay.Visibility = Visibility.Collapsed;
                    successOverlay.Visibility = Visibility.Visible;
                }
                else
                {
                    processingOverlay.Visibility = Visibility.Collapsed;
                    errorOverlay.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                processingOverlay.Visibility = Visibility.Collapsed;
                txtErrorMessage.Text = $"Error: {ex.Message}";
                errorOverlay.Visibility = Visibility.Visible;
            }
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Login)
                {
                    window.Close();
                }
            }
            _navigationService.NavigateTo<Login>();
            this.Close();
        }

        private void TryAgain_Click(object sender, RoutedEventArgs e)
        {
            errorOverlay.Visibility = Visibility.Collapsed;
        }

        private void LoginLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Login)
                {
                    window.Close();
                }
            }
            _navigationService.NavigateTo<Login>();
            this.Close();
        }
    }
}
