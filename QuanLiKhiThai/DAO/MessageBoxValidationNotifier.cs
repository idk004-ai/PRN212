using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    class MessageBoxValidationNotifier : IValidationNotifier
    {
        public bool AskForConfirmation(string message, string title)
        {
            return MessageBox.Show(
                message,
                title,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public void ShowError(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public void ShowInformation(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public void ShowWarning(string message, string title)
        {
            MessageBox.Show(
                message,
                title,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        /// <summary>
        /// Processes a validation result and shows appropriate UI message
        /// </summary>
        /// <param name="result">Validation result to process</param>
        /// <returns>True if validation passes or confirmation is accepted, false otherwise</returns>
        public bool ProcessValidationResult(ValidationResult result)
        {
            if (result == null)
            {
                ShowError("Validation failed with null result", "Validation Error");
                return false;
            }

            if (result.IsValid)
                return true;

            switch (result.ResultType)
            {
                case ValidationResultType.Warning:
                    ShowWarning(result.Message, result.Title);
                    return false;

                case ValidationResultType.Error:
                    ShowError(result.Message, result.Title);
                    return false;

                case ValidationResultType.ConfirmationRequired:
                    return AskForConfirmation(result.Message, result.Title);

                case ValidationResultType.Information:
                    ShowInformation(result.Message, result.Title);
                    return true;

                default:
                    return false;
            }
        }
    }
}
