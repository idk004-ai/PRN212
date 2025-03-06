
public class NotificationDAO
{
    internal static bool AddNotification(Notification notification)
    {
        using (var db = new QuanLiKhiThaiContext())
        {
            db.Notifications.Add(notification);
            return db.SaveChanges() > 0;
        }
    }
}
