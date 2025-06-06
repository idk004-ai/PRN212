﻿using QuanLiKhiThai.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLiKhiThai.DAO.Interface
{
    public interface IInspectionRecordDAO : IServiceDAO<InspectionRecord>      
    {
        List<InspectionRecord> GetRecordByVehicle(int vehicleId);
        InspectionRecord? GetRecordByAppointment(int appointmentId);
        List<InspectionRecord> GetTestingRecordsByInspectorId(int inspectorId);
        public bool AssignInspector(
            InspectionRecord record,
            InspectionAppointment appointment,
            User inspector,
            string vehiclePlateNumber,
            int stationId,
            string stationFullName,
            Window? windowToClose = null
        );
        public bool RecordInspectionResult(
            InspectionRecord record,
            UserContext inspector,
            string vehiclePlateNumber,
            string stationFullName,
            Window? windowToClose = null
        );
        public bool CancelInspection(
            InspectionRecord inspectionRecord,
            UserContext inspector,
            string vehiclePlateNumber,
            string stationFullName,
            string cancellationReason,
            Window? windowToClose = null,
            bool isSystemCancellation = false
        );
        List<InspectionRecord> GetRecordInDateRange(DateTime startDate, DateTime endDate);
    }
}
