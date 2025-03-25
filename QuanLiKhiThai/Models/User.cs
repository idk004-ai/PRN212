using System;
using System.Collections.Generic;


public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public string? VerificationToken { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public virtual ICollection<InspectionAppointment> InspectionAppointments { get; set; } = new List<InspectionAppointment>();

    public virtual ICollection<InspectionRecord> InspectionRecordInspectors { get; set; } = new List<InspectionRecord>();

    public virtual ICollection<InspectionRecord> InspectionRecordStations { get; set; } = new List<InspectionRecord>();

    public virtual ICollection<Log> Logs { get; set; } = new List<Log>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<StationInspector> StationInspectorInspectors { get; set; } = new List<StationInspector>();

    public virtual ICollection<StationInspector> StationInspectorStations { get; set; } = new List<StationInspector>();

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<ViolationRecord> ViolationRecords { get; set; } = new List<ViolationRecord>();
}
