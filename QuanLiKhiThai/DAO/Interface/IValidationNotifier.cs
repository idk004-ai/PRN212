using QuanLiKhiThai.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IValidationNotifier
    {
        bool ProcessValidationResult(ValidationResult result);
        void ShowWarning(string message, string title);
        void ShowError(string message, string title);
        void ShowInformation(string message, string title);
        bool AskForConfirmation(string message, string title);
    }
}
