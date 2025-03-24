

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IValidationLogger
    {
        void LogValidationMessage(string message, int? userId = null);
        void LogValidationError(string message, int? userId = null, Exception? ex = null);
    }
}
