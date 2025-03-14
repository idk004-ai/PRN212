
using QuanLiKhiThai.DAO.Interface;

public class NotificationDAO : INotificationDAO
{
    bool IServiceDAO<Notification>.Add(Notification entity)
    {
        using (var db = new QuanLiKhiThaiContext())
        {
            db.Notifications.Add(entity);
            return db.SaveChanges() > 0;
        }
    }

    bool IServiceDAO<Notification>.Delete(int id)
    {
        using (var db = new QuanLiKhiThaiContext())
        {
            var entity = db.Notifications.Find(id);
            if (entity == null)
            {
                return false;
            }
            db.Notifications.Remove(entity);
            return db.SaveChanges() > 0;
        }
    }

    IEnumerable<Notification> IServiceDAO<Notification>.GetAll()
    {
        using (var db = new QuanLiKhiThaiContext())
        {
            return db.Notifications.ToList();
        }
    }

    Notification? IServiceDAO<Notification>.GetById(int id)
    {
        using (var db = new QuanLiKhiThaiContext())
        {
            return db.Notifications.Find(id);
        }
    }

    bool IServiceDAO<Notification>.Update(Notification entity)
    {
        using (var db = new QuanLiKhiThaiContext())
        {
            db.Update(entity);
            return db.SaveChanges() > 0;
        }
    }
}
