using Microsoft.EntityFrameworkCore;
using QuanLiKhiThai.Context;
using System.Windows;

namespace QuanLiKhiThai.DAO
{
    class InspectionRecordDAO
    {
        public static bool AddInspectionRecord(InspectionRecord inspectionRecord)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Add(inspectionRecord);
                return db.SaveChanges() > 0;
            }
        }

        internal static InspectionRecord? GetLastRecordByVehicleId(int vehicleId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.VehicleId == vehicleId)
                    .OrderByDescending(i => i.InspectionDate)
                    .FirstOrDefault();
            }
        }

        public static InspectionRecord? GetCurrentRecordByVehicleId(int vehicleId)
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


        internal static InspectionRecord? GetRecordByAppointment(int appointmentId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                return db.InspectionRecords
                    .Where(i => i.AppointmentId == appointmentId)
                    .FirstOrDefault();
            }
        }

        public static List<InspectionRecord> GetTestingRecordsByInspectorId(int inspectorId)
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

        public bool UpdateRecord(InspectionRecord record)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                db.InspectionRecords.Update(record);
                return db.SaveChanges() > 0;
            }
        }

        public bool RecordInspectionResult(
                InspectionRecord record,
                UserContext inspector,
                string vehiclePlateNumber,
                string stationFullName,
                Window windowToClose = null
            )
        {
            var operations = new Dictionary<string, Func<bool>>
            {
                { "update record", () => UpdateRecord(record) },
                { "update appointment", () => { 
                    if (!InspectionAppointmentValidation.ValidateDataConsistency(record.Appointment, Constants.STATUS_COMPLETED))
                    {
                        return false;
                    }
                    record.Appointment.Status = Constants.STATUS_COMPLETED;
                    return InspectionAppointmentDAO.UpdateAppointment(record.Appointment);
                }}
            };

            Log logEntry = new Log
            {
                UserId = inspector.UserId,
                Action = $"Recorded inspection result for vehicle {vehiclePlateNumber}",
                Timestamp = DateTime.Now
            };

            Notification notification = new Notification
            {
                UserId = inspector.UserId,
                Message = $"You have recorded inspection result for vehicle {vehiclePlateNumber} at {stationFullName}",
                SentDate = DateTime.Now,
                IsRead = false
            };

            string successMessage = $"Inspection result for vehicle {vehiclePlateNumber} has been recorded at {stationFullName}.";
            string errorMessage = "Failed to record inspection result";

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

        public bool DeleteRecord(int recordId)
        {
            using (var db = new QuanLiKhiThaiContext())
            {
                var record = db.InspectionRecords.Find(recordId);
                if (record == null)
                {
                    return false;
                }
                db.InspectionRecords.Remove(record);
                return db.SaveChanges() > 0;
            }
        }

        public bool CancelInspection(
                InspectionRecord inspectionRecord,
                UserContext inspector,
                string vehiclePlateNumber,
                string stationFullName,
                Window windowToClose = null
            )
        {
            if (inspectionRecord == null)
            {
                MessageBox.Show("Cannot cancel inspection. No record found for this vehicle.",
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            int appointmentId = inspectionRecord.AppointmentId;
            var appointment = InspectionAppointmentDAO.GetAppointmentById(appointmentId);
            if (appointment == null) {
                MessageBox.Show("Cannot cancel inspection. No appointment found for this record.", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            var operations = new Dictionary<string, Func<bool>>
            {
                {"delete record", () => DeleteRecord(inspectionRecord.RecordId) },
                {"update appointment", () =>
                {
                    var updatedAppointment = InspectionAppointmentDAO.GetAppointmentById(appointmentId);
                    if (updatedAppointment == null) return false;
                    if (!InspectionAppointmentValidation.ValidateDataConsistency(updatedAppointment, Constants.STATUS_CANCELLED)) return false;
                    updatedAppointment.Status = Constants.STATUS_CANCELLED;
                    return InspectionAppointmentDAO.UpdateAppointment(updatedAppointment);
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
    }
}
