using QuanLiKhiThai.Models;

namespace QuanLiKhiThai.DAO
{
    internal class UserDAO
    {
        public static List<User> GetUsers()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.ToList();
            }
        }

        internal static bool AddUser(User user)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Users.Add(user);
                return db.SaveChanges() > 0;
            }
        }
    }
}
