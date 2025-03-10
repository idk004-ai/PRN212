using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiKhiThai.Context
{
    internal class UserContext
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
