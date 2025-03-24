

namespace QuanLiKhiThai.Helper
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public ValidationResultType ResultType { get; set; }
    }

    public enum ValidationResultType
    {
        Information,
        Warning,
        Error,
        ConfirmationRequired
    }
}
