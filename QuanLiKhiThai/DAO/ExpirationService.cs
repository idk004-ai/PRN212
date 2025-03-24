using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using QuanLiKhiThai.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QuanLiKhiThai.DAO
{
    public class ExpirationService
    {
        private readonly IInspectionRecordDAO _inspectionRecordDAO;
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;
        private readonly IValidationLogger _logger;
        private readonly EmailService _emailService;
        private Timer? _expirationTimer;
        private readonly int _checkIntervalMinutes;
        private readonly int _expirationHours;

        public ExpirationService(
            IInspectionRecordDAO inspectionRecordDAO,
            IInspectionAppointmentDAO inspectionAppointmentDAO,
            IValidationLogger logger,
            EmailService emailService)
        {
            _inspectionRecordDAO = inspectionRecordDAO;
            _inspectionAppointmentDAO = inspectionAppointmentDAO;
            _logger = logger;
            _emailService = emailService;

            try
            {
                // Read configuration from ConfigurationHelper with proper validation
                string intervalStr = ConfigurationHelper.GetAppSetting("ExpirationCheckIntervalMinutes");
                _checkIntervalMinutes = string.IsNullOrWhiteSpace(intervalStr) ? 60 : int.Parse(intervalStr);

                string hoursStr = ConfigurationHelper.GetAppSetting("InspectionExpirationHours");
                _expirationHours = string.IsNullOrWhiteSpace(hoursStr) ? 24 : int.Parse(hoursStr);

                _logger.LogValidationMessage($"ExpirationService initialized with interval: {_checkIntervalMinutes} minutes, expiration: {_expirationHours} hours");
            }
            catch (Exception ex)
            {
                _logger.LogValidationError($"Error initializing ExpirationService settings, using defaults. Error: {ex.Message}", null, ex);

                // Use default values if configuration fails
                _checkIntervalMinutes = 60;
                _expirationHours = 24;
            }
        }

        /// <summary>
        /// Bắt đầu dịch vụ kiểm tra hết hạn
        /// </summary>
        public void Start()
        {
            _logger.LogValidationMessage("Starting inspection expiration service");

            // Kiểm tra ngay lập tức sau khi khởi động, sau đó kiểm tra theo định kỳ
            _expirationTimer = new Timer(CheckExpiredInspections, null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(_checkIntervalMinutes));
        }

        /// <summary>
        /// Dừng dịch vụ kiểm tra hết hạn
        /// </summary>
        public void Stop()
        {
            _logger.LogValidationMessage("Stopping inspection expiration service");
            _expirationTimer?.Dispose();
            _expirationTimer = null;
        }

        /// <summary>
        /// Kiểm tra và xử lý các InspectionRecord đã quá hạn
        /// </summary>
        private void CheckExpiredInspections(object state)
        {
            try
            {
                _logger.LogValidationMessage("Checking for expired inspections");

                // Tìm và xử lý các InspectionRecord hết hạn
                var expiredRecords = GetExpiredInspectionRecords();

                if (expiredRecords.Count > 0)
                {
                    _logger.LogValidationMessage($"Found {expiredRecords.Count} expired inspection records");
                    CancelExpiredInspectionsAsync(expiredRecords);
                }
                else
                {
                    _logger.LogValidationMessage("No expired inspection records found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogValidationError("Error checking expired inspections", null, ex);
            }
        }

        /// <summary>
        /// Lấy danh sách InspectionRecord đã quá hạn
        /// </summary>
        private List<InspectionRecord> GetExpiredInspectionRecords()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                // Tính thời điểm hết hạn dựa trên thời gian hiện tại
                var expirationTime = DateTime.Now.AddHours(-_expirationHours);

                // Lấy các record với Result = TESTING và có thời gian hẹn đã quá hạn
                return db.InspectionRecords
                    .Include(ir => ir.Vehicle)
                    .Include(ir => ir.Vehicle.Owner)
                    .Include(ir => ir.Appointment)
                    .Include(ir => ir.Station)
                    .Include(ir => ir.Inspector)
                    .Where(ir => ir.Result == Constants.RESULT_TESTING &&
                                ir.Appointment.Status == Constants.STATUS_ASSIGNED &&
                                ir.InspectionDate < expirationTime)
                    .ToList();
            }
        }

        /// <summary>
        /// Hủy các InspectionRecord đã quá hạn và gửi email thông báo
        /// </summary>
        private async void CancelExpiredInspectionsAsync(List<InspectionRecord> expiredRecords)
        {
            // Tạo một UserContext hệ thống
            var systemUserContext = new SystemUserContext();

            foreach (var record in expiredRecords)
            {
                try
                {
                    string cancellationReason = $"Auto-cancelled: Inspection expired after {_expirationHours} hours from scheduled time.";

                    // Sử dụng phương thức CancelInspection từ InspectionRecordDAO với tham số isSystemCancellation = true
                    bool success = await CancelInspectionAndSendEmailAsync(record, systemUserContext, cancellationReason);

                    if (success)
                    {
                        _logger.LogValidationMessage($"Successfully auto-cancelled expired inspection for vehicle {record.Vehicle.PlateNumber}, appointment ID: {record.AppointmentId}");
                    }
                    else
                    {
                        _logger.LogValidationError($"Failed to auto-cancel expired inspection for vehicle {record.Vehicle.PlateNumber}, appointment ID: {record.AppointmentId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogValidationError($"Error cancelling expired inspection ID {record.RecordId}", null, ex);
                }
            }
        }

        /// <summary>
        /// Hủy inspection và gửi email thông báo
        /// </summary>
        private async Task<bool> CancelInspectionAndSendEmailAsync(InspectionRecord record, SystemUserContext systemUser, string cancellationReason)
        {
            // Hủy InspectionRecord và đặt lại trạng thái Appointment
            bool cancelled = CancelInspection(record, systemUser, cancellationReason);

            if (!cancelled)
            {
                return false;
            }

            // Gửi email thông báo cho inspector
            await SendExpirationNotificationEmailsAsync(record, cancellationReason);

            return true;
        }


        /// <summary>
        /// Hủy InspectionRecord và đặt lại trạng thái Appointment
        /// </summary>
        private bool CancelInspection(InspectionRecord record, SystemUserContext systemUser, string cancellationReason)
        {
            try
            {
                return _inspectionRecordDAO.CancelInspection(
                        record,
                        systemUser,
                        record.Vehicle.PlateNumber,
                        record.Station.FullName,
                        cancellationReason,
                        windowToClose: null,
                        true);
            }
            catch (Exception ex)
            {
                _logger.LogValidationError($"Error in CancelInspection: {ex.Message}", null, ex);
                return false;
            }
        }

        /// <summary>
        /// Gửi email thông báo cho inspector
        /// </summary>
        private async Task SendExpirationNotificationEmailsAsync(InspectionRecord record, string cancellationReason)
        {
            try
            {
                // Gửi email cho inspector
                if (!string.IsNullOrEmpty(record.Inspector.Email))
                {
                    string inspectorSubject = $"Inspection Cancelled: Vehicle {record.Vehicle.PlateNumber}";
                    string inspectorBody = $@"
Dear {record.Inspector.FullName},

The inspection for vehicle {record.Vehicle.PlateNumber} ({record.Vehicle.Brand} {record.Vehicle.Model}) has been automatically cancelled by the system.

Reason: {cancellationReason}

Inspection Details:
- Vehicle: {record.Vehicle.PlateNumber}
- Owner: {record.Vehicle.Owner.FullName}
- Scheduled Date: {record.Appointment.ScheduledDateTime:dd/MM/yyyy HH:mm}
- Station: {record.Station.FullName}

The appointment has been reset to Pending status and will need to be reassigned.

This is an automated message. Please do not reply.

Regards,
Vehicle Inspection System
                    ";

                    await _emailService.SendEmailAsync(
                        record.Inspector.Email,
                        record.Inspector.FullName,
                        inspectorSubject,
                        inspectorBody);
                }
            }
            catch (Exception ex)
            {
                _logger.LogValidationError($"Error sending expiration notification emails: {ex.Message}", null, ex);
            }
        }
    }

    /// <summary>
    /// UserContext đại diện cho hệ thống tự động
    /// </summary>
    public class SystemUserContext : UserContext
    {
        public SystemUserContext()
        {
            // Thiết lập các giá trị mặc định cho UserContext hệ thống
            UserId = Constants.SYSTEM_USER_ID; // ID của người dùng hệ thống
            FullName = "System";
            Email = "system@vehicleinspection.com";
            Role = "System";
        }
    }
}

