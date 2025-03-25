using QuanLiKhiThai.Context;
using System.Windows;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IUserDAO : IServiceDAO<User>
    {
        User? GetUserByEmail(string email);
        IEnumerable<User> GetUserByRole(string role);
        IEnumerable<User> GetInspectorsInStation(int stationId);

        /// <summary>
        /// Tạo một cuộc hẹn kiểm định xe và xử lý các thao tác liên quan như ghi log và gửi thông báo.
        /// </summary>
        /// <param name="appointment">Thông tin cuộc hẹn cần được tạo, bao gồm thời gian, địa điểm và trạng thái.</param>
        /// <param name="owner">Thông tin người dùng hiện tại (chủ xe) đang tạo cuộc hẹn.</param>
        /// <param name="station">Thông tin về trạm đăng kiểm được chọn cho cuộc hẹn.</param>
        /// <param name="vehicle">Thông tin phương tiện cần được kiểm định.</param>
        /// <param name="windowToClose">Tham chiếu đến cửa sổ hiện tại để đóng sau khi tạo cuộc hẹn thành công (tùy chọn).</param>
        /// <returns>
        /// <c>true</c> nếu quá trình tạo cuộc hẹn, ghi log và gửi thông báo thành công;
        /// <c>false</c> nếu quá trình thất bại.
        /// </returns>
        /// <remarks>
        /// Phương thức này thực hiện các thao tác sau trong một giao dịch:
        /// <list type="bullet">
        ///   <item>Thêm thông tin cuộc hẹn vào cơ sở dữ liệu</item>
        ///   <item>Ghi log về hoạt động tạo cuộc hẹn</item>
        ///   <item>Tạo thông báo cho trạm đăng kiểm về cuộc hẹn mới</item>
        /// </list>
        /// <para>
        /// Nếu tất cả các thao tác đều thành công, hệ thống hiển thị thông báo thành công và đóng cửa sổ (nếu có).
        /// Nếu bất kỳ thao tác nào thất bại, giao dịch sẽ được hoàn tác và hiển thị thông báo lỗi.
        /// </para>
        /// </remarks>
        /// <example>
        /// Dưới đây là ví dụ về cách gọi phương thức này:
        /// <code>
        /// var appointment = new InspectionAppointment
        /// {
        ///     VehicleId = vehicle.VehicleId,
        ///     StationId = station.UserId,
        ///     ScheduledDateTime = selectedDateTime,
        ///     Status = "Pending"
        /// };
        /// 
        /// bool success = _userDAO.CreateAppointment(
        ///     appointment,
        ///     UserContext.Current,
        ///     selectedStation,
        ///     selectedVehicle,
        ///     this);
        /// </code>
        /// </example>
        bool CreateAppointment(
            InspectionAppointment appointment,
            UserContext owner,
            User station,
            Vehicle vehicle,
            Window? windowToClose = null
            );

        bool VerifyAccount(string email, string token);
    }
}