namespace QuanLiKhiThai.Context
{
    public class UserContext
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        private static UserContext? _current;
        public static UserContext Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new UserContext();
                }
                return _current;
            }
        }

        private UserContext() { }


    }
}
