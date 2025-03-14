
using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    internal class UserDAO : IUserDAO
    {

        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;

        public UserDAO(IInspectionAppointmentDAO inspectionAppointmentDAO) 
        {
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;
        }

        bool IServiceDAO<User>.Add(User user)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Users.Add(user);
                return db.SaveChanges() > 0;
            }
        }

        bool IUserDAO.CreateAppointment(InspectionAppointment appointment, UserContext owner, User station, Vehicle vehicle, Window windowToClose)
        {
            var operations = new Dictionary<string, Func<bool>>
            {
                { "add appointment", () => _inspectionAppointmentDAO.Add(appointment) }
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

        bool IServiceDAO<User>.Delete(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var user = db.Users.Find(id);
                if (user != null)
                    db.Users.Remove(user);
                return db.SaveChanges() > 0;
            }
        }

        IEnumerable<User> IServiceDAO<User>.GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.ToList();
            }
        }

        User? IServiceDAO<User>.GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.Find(id);
            }
        }

        IEnumerable<User> IUserDAO.GetInspectorsInStation(int stationId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.StationInspectors
                    .Where(s => s.StationId == stationId && s.IsActive == true)
                    .Select(s => s.Inspector)
                    .ToList();
            }
        }

        User? IUserDAO.GetUserByEmail(string email)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users.FirstOrDefault(u => u.Email == email);
            }
        }

        IEnumerable<User> IUserDAO.GetUserByRole(string role)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.Users
                    .Where(u => u.Role == role)
                    .ToList();
            }
        }

        bool IServiceDAO<User>.Update(User entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.Users.Update(entity);
                return db.SaveChanges() > 0;
            }
        }
    }
}
