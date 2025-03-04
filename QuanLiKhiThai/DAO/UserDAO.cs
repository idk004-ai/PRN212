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

        public static User? GetUserById(int userId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.Find(userId);
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

        internal static User? GetUserByEmail(string email)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        internal static List<User> GetUserByRole(string role)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.Where(u => u.Role == role).ToList();
            }
        }
    }
}
