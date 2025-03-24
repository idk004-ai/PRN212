using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    class InspectionRecordDAO : IInspectionRecordDAO
    {
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;
        private readonly ValidationService _validationService;
        private readonly IValidationNotifier _notifier;

        public InspectionRecordDAO(IInspectionAppointmentDAO inspectionAppointmentDAO, ValidationService validationService, IValidationNotifier validationNotifier)
        {
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;
            this._validationService = validationService;
            this._notifier = validationNotifier;
        }

        public bool Add(InspectionRecord entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        public bool Delete(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var entity = db.InspectionRecords.Find(id);
                if (entity != null)
                {
                    db.InspectionRecords.Remove(entity);
                    return db.SaveChanges() > 0;
                }
                return false;
            }
        }

        public IEnumerable<InspectionRecord> GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Include(ir => ir.Vehicle)
                    .Include(ir => ir.Vehicle.Owner)
                    .Include(ir => ir.Station)
                    .Include(ir => ir.Inspector)
                    .Include(ir => ir.Appointment)
                    .ToList();
            }
        }

        public InspectionRecord? GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return ((IServiceDAO<InspectionRecord>)this)
                    .GetAll()
                    .Where(i => i.RecordId == id)
                    .FirstOrDefault();
            }
        }

        public List<InspectionRecord> GetRecordByVehicle(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return ((IServiceDAO<InspectionRecord>)this)
                    .GetAll()
                    .Where(i => i.VehicleId == vehicleId)
                    .ToList();
            }
        }

        public InspectionRecord? GetRecordByAppointment(int appointmentId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.AppointmentId == appointmentId)
                    .FirstOrDefault();
            }
        }

        public List<InspectionRecord> GetTestingRecordsByInspectorId(int inspectorId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Include(ir => ir.Vehicle)
                    .Include(ir => ir.Vehicle.Owner)
                    .Include(ir => ir.Station)
                    .Include(ir => ir.Appointment)
                    .Where(ir => ir.InspectorId == inspectorId && ir.Result == Constants.RESULT_TESTING)
                    .ToList();
            }
        }

        public bool Update(InspectionRecord entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Update(entity);
                return db.SaveChanges() > 0;
            }
        }

        public bool AssignInspector(InspectionRecord record, InspectionAppointment appointment, User inspector, string vehiclePlateNumber, int stationId, string stationFullName, Window? windowToClose)
        {
            var operations = new Dictionary<string, Func<bool>>
            {
                { "add record", () => ((IServiceDAO<InspectionRecord>)this).Add(record) },
                { "update appointment", () => _inspectionAppointmentDAO.Update(appointment) }
            };
            Log logEntry = new Log
            {
                UserId = stationId,
                Action = $"Assigned inspector {inspector.FullName} to vehicle {vehiclePlateNumber}",
                Timestamp = DateTime.Now
            };
            Notification notification = new Notification
            {
                UserId = inspector.UserId, // Should be sending to the inspector, not back to the station
                Message = $"You have been assigned to inspect vehicle {vehiclePlateNumber} at {stationFullName}",
                SentDate = DateTime.Now,
                IsRead = false
            };
            string successMessage = $"Inspector {inspector.FullName} has been assigned to vehicle {vehiclePlateNumber}.";
            string errorMessage = "Failed to assign inspector to vehicle";
            bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, notification, successMessage, errorMessage);
            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }
            return result;
        }

        public bool RecordInspectionResult(InspectionRecord record, UserContext inspector, string vehiclePlateNumber, string stationFullName, Window? windowToClose)
        {
            var operations = new Dictionary<string, Func<bool>>
            {
                { "update record", () => ((IServiceDAO<InspectionRecord>)this).Update(record) },
                { "update appointment", () => {
                    if (!_validationService.ValidateDataConsistency(record.Appointment.AppointmentId, Constants.STATUS_COMPLETED))
                    {
                        return false;
                    }
                    record.Appointment.Status = Constants.STATUS_COMPLETED;
                    return _inspectionAppointmentDAO.Update(record.Appointment);
                }}
            };
            Log logEntry = new Log
            {
                UserId = inspector.UserId,
                Action = $"Recorded inspection result for vehicle {vehiclePlateNumber} at {stationFullName}",
                Timestamp = DateTime.Now
            };
            Notification notification = new Notification
            {
                UserId = record.Vehicle.OwnerId,
                Message = $"Your vehicle {vehiclePlateNumber} has been inspected at {stationFullName}.",
                SentDate = DateTime.Now,
                IsRead = false
            };
            string successMessage = $"Inspection result for vehicle {vehiclePlateNumber} has been recorded.";
            string errorMessage = "Failed to record inspection result";
            bool result = TransactionHelper.ExecuteTransaction(operations, logEntry, notification, successMessage, errorMessage);
            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }
            return result;
        }

        public bool CancelInspection(
            InspectionRecord inspectionRecord, 
            UserContext inspector, 
            string vehiclePlateNumber, 
            string stationFullName, 
            string cancellationReason, 
            Window? windowToClose,
            bool isSystemCancellation = false)
        {
            if (inspectionRecord == null)
            {
                if (!isSystemCancellation)
                    _notifier.ShowError("Cannot cancel inspection. Record not found.", "Error");
                return false;
            }

            int appointmentId = inspectionRecord.AppointmentId;
            var appointment = _inspectionAppointmentDAO.GetById(appointmentId);
            if (appointment == null)
            {
                if (!isSystemCancellation)
                    _notifier.ShowError("Cannot cancel inspection. Appointment not found.", "Error");
                return false;
            }

            if (!isSystemCancellation)
            {
                bool validateResult = _validationService.ValidateDataConsistencyForRecordCancellation(inspectionRecord.RecordId);
                if (!validateResult) return false;
            }

            var operations = new Dictionary<string, Func<bool>>
            {
                {"update record", () => 
                {
                    inspectionRecord.Result = Constants.RESULT_CANCELLED;
                    inspectionRecord.InspectionDate = DateTime.Now; // set cancellation date
                    inspectionRecord.Comments = cancellationReason;
                    return Update(inspectionRecord);
                }},
                {"update appointment", () =>
                {
                    appointment.Status = Constants.STATUS_PENDING; // reset appointment status to pending
                    return _inspectionAppointmentDAO.Update(appointment);
                }}
            };

            Log logEntry = new Log
            {
                UserId = inspector.UserId,
                Action = isSystemCancellation == false ? $"Cancelled inspection for vehicle {vehiclePlateNumber}" : $"System automatically cancelled inspection for vehicle {vehiclePlateNumber}",
                Timestamp = DateTime.Now
            };

            string successMessage = $"Inspection for vehicle {vehiclePlateNumber} has been cancelled at {stationFullName}.";
            string errorMessage = "Failed to cancel inspection";

            bool result = TransactionHelper.ExecuteTransaction(
                operations,
                logEntry,
                notification: null,
                successMessage,
                errorMessage,
                isSystemCancellation
            );

            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }

            return result;

        }

        public List<InspectionRecord> GetRecordInDateRange(DateTime startDate, DateTime endDate)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Include(ir => ir.Vehicle)
                    .Include(ir => ir.Vehicle.Owner)
                    .Include(ir => ir.Station)
                    .Include(ir => ir.Appointment)
                    .Include(ir => ir.Inspector)
                    .Where(ir => ir.InspectionDate >= startDate && ir.InspectionDate <= endDate)
                    .ToList();
            }
        }
    }
}
