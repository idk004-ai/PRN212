using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Context;
using QuanLiKhiThai.DAO.Interface;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    class InspectionRecordDAO : IInspectionRecordDAO
    {
        private readonly IInspectionAppointmentDAO _inspectionAppointmentDAO;

        public InspectionRecordDAO(IInspectionAppointmentDAO inspectionAppointmentDAO)
        {
            this._inspectionAppointmentDAO = inspectionAppointmentDAO;
        }

        bool IServiceDAO<InspectionRecord>.Add(InspectionRecord entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Add(entity);
                return db.SaveChanges() > 0;
            }
        }

        bool IServiceDAO<InspectionRecord>.Delete(int id)
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

        IEnumerable<InspectionRecord> IServiceDAO<InspectionRecord>.GetAll()
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords.ToList();
            }
        }

        InspectionRecord? IServiceDAO<InspectionRecord>.GetById(int id)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords.Find(id);
            }
        }

        InspectionRecord? IInspectionRecordDAO.GetCurrentRecordByVehicleId(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Include(r => r.Inspector)
                    .Include(r => r.Appointment)
                    .Where(r => r.VehicleId == vehicleId && r.Result == Constants.RESULT_TESTING)
                    .OrderByDescending(r => r.InspectionDate)
                    .FirstOrDefault();
            }
        }

        InspectionRecord? IInspectionRecordDAO.GetLastRecordByVehicleId(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.VehicleId == vehicleId)
                    .OrderByDescending(i => i.InspectionDate)
                    .FirstOrDefault();
            }
        }

        InspectionRecord? IInspectionRecordDAO.GetRecordByAppointment(int appointmentId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.AppointmentId == appointmentId)
                    .FirstOrDefault();
            }
        }

        List<InspectionRecord> IInspectionRecordDAO.GetTestingRecordsByInspectorId(int inspectorId)
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

        bool IServiceDAO<InspectionRecord>.Update(InspectionRecord entity)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Update(entity);
                return db.SaveChanges() > 0;
            }
        }

        bool IInspectionRecordDAO.AssignInspector(InspectionRecord record, InspectionAppointment appointment, User inspector, string vehiclePlateNumber, int stationId, string stationFullName, Window windowToClose)
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

        bool IInspectionRecordDAO.RecordInspectionResult(InspectionRecord record, UserContext inspector, string vehiclePlateNumber, string stationFullName, Window windowToClose)
        {
            var operations = new Dictionary<string, Func<bool>>
            {
                { "update record", () => ((IServiceDAO<InspectionRecord>)this).Update(record) },
                { "update appointment", () => {
                    if (!InspectionAppointmentValidation.ValidateDataConsistency(record.Appointment, Constants.STATUS_COMPLETED))
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

        bool IInspectionRecordDAO.CancelInspection(InspectionRecord inspectionRecord, UserContext inspector, string vehiclePlateNumber, string stationFullName, Window windowToClose)
        {
            if (inspectionRecord == null)
            {
                MessageBox.Show("Cannot cancel inspection. No record found for this vehicle.",
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            int appointmentId = inspectionRecord.AppointmentId;
            var appointment = _inspectionAppointmentDAO.GetById(appointmentId);
            if (appointment == null)
            {
                MessageBox.Show("Cannot cancel inspection. No appointment found for this record.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var operations = new Dictionary<string, Func<bool>>
            {
                {"delete record", () => ((IServiceDAO<InspectionRecord>)this).Delete(inspectionRecord.RecordId) },
                {"update appointment", () =>
                {
                    var updatedAppointment = _inspectionAppointmentDAO.GetById(appointmentId);
                    if (updatedAppointment == null) return false;
                    if (!InspectionAppointmentValidation.ValidateDataConsistency(updatedAppointment, Constants.STATUS_CANCELLED)) return false;
                    updatedAppointment.Status = Constants.STATUS_CANCELLED;
                    return _inspectionAppointmentDAO.Update(updatedAppointment);
                } }
            };

            Log logEntry = new Log
            {
                UserId = inspector.UserId,
                Action = $"Cancelled inspection for vehicle {vehiclePlateNumber}",
                Timestamp = DateTime.Now
            };


            Notification notification = new Notification
            {
                UserId = inspector.UserId,
                Message = $"Inspector {inspector.FullName} has cancelled the inspection for vehicle {vehiclePlateNumber}",
                SentDate = DateTime.Now,
                IsRead = false
            };

            string successMessage = $"Inspection for vehicle {vehiclePlateNumber} has been cancelled at {stationFullName}.";
            string errorMessage = "Failed to cancel inspection";

            bool result = TransactionHelper.ExecuteTransaction(
                operations,
                logEntry,
                notification,
                successMessage,
                errorMessage
            );

            if (result && windowToClose != null)
            {
                windowToClose.Close();
            }

            return result;

        }

        List<InspectionRecord> IInspectionRecordDAO.GetRecordInDateRange(DateTime startDate, DateTime endDate)
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
