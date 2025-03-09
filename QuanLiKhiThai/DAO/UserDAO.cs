
using QuanLiKhiThai.Context;
using System.Windows;

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

        public bool CreateAppointment(
            InspectionAppointment appointment,
            UserContext owner,
            User station,
            Vehicle vehicle,
            Window windowToClose = null
            )
        {
            var operations = new Dictionary<string, Func<bool>>
            {
                { "add appointment", () => InspectionAppointmentDAO.AddInspectionAppointment(appointment) }
            };
            Log logEntry = new Log
            {
                UserId = owner.UserId,
                Action = $"Created an appointment for vehicle {vehicle.PlateNumber} at station {station.FullName}",
                Timestamp = DateTime.Now
            };


            Notification notification = new Notification
            {
                UserId = station.UserId,
                Message = $"New inspection appointment scheduled for vehicle {vehicle.PlateNumber} on {appointment.ScheduledDateTime:MM/dd/yyyy}",
                SentDate = DateTime.Now,
                IsRead = false
            };

            string successMessage = "Appointment created successfully";
            string errorMessage = "Failed to create appointment";

            bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, notification, successMessage, errorMessage);
            
            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }

            return result;
        }

        public static List<User> GetInspectorInStation(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .Where(s => s.StationId == stationId && s.IsActive == true)
                    .Select(s => s.Inspector)
                    .ToList();
            }
        }
    }
}
